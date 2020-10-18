using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dUIItem))]
public class tk2dUIItemEditor : Editor
{
    SerializedProperty extraBoundsProp;
    SerializedProperty ignoreBoundsProp;

    void OnEnable() {
        extraBoundsProp = serializedObject.FindProperty("editorExtraBounds");
        ignoreBoundsProp = serializedObject.FindProperty("editorIgnoreBounds");
    }

    GameObject cachedMethodBinding = null;
    List<string> cachedMethods = new List<string>();

    void MethodBinding( string name, GameObject target, ref string methodName ) {
        if (target != cachedMethodBinding) {
            cachedMethods.Clear();

            List<System.Type> addedTypes = new List<System.Type>();

            MonoBehaviour[] behaviours = target.GetComponents<MonoBehaviour>();
            foreach (MonoBehaviour beh in behaviours) {
                System.Type type = beh.GetType();
                if (addedTypes.IndexOf(type) == -1) {
                    System.Reflection.MethodInfo[] methods = type.GetMethods(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
                    foreach (System.Reflection.MethodInfo method in methods) {
                        // Only add variables added by user, i.e. we don't want functions from the base UnityEngine baseclasses or lower
                        string moduleName = method.DeclaringType.Assembly.ManifestModule.Name;
                        if (!moduleName.Contains("UnityEngine") && !moduleName.Contains("mscorlib") &&
                            !method.ContainsGenericParameters && 
                            method.Name != "Start" && method.Name != "Awake" && method.Name != "OnEnable" && method.Name != "OnDisable") {
                            System.Reflection.ParameterInfo[] paramInfo = method.GetParameters();
                            if (paramInfo.Length == 0) {
                                cachedMethods.Add(method.Name);
                            }
                            else if (paramInfo.Length == 1 && paramInfo[0].ParameterType == typeof(tk2dUIItem)) {
                                cachedMethods.Add(method.Name);
                            }
                        }
                    }
                }
            }
        }

        int idx = cachedMethods.IndexOf(methodName);
        GUILayout.BeginHorizontal();
        int nidx = EditorGUILayout.Popup( name, idx, cachedMethods.ToArray() );
        if (nidx != idx) {
            methodName = cachedMethods[nidx];
        }
        if (methodName.Length != 0) {
            if (GUILayout.Button("Clear", EditorStyles.miniButton, GUILayout.ExpandWidth(false))) {
                methodName = "";
                Repaint();
            }
        }
        GUILayout.EndHorizontal();
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        bool changeOccurred = false;
        EditorGUIUtility.LookLikeControls(180);
        tk2dUIItem btn = (tk2dUIItem)target;

        bool newIsChildOfAnotherMenuBtn = EditorGUILayout.Toggle("Child of Another UIItem?", btn.InternalGetIsChildOfAnotherUIItem());

        if (newIsChildOfAnotherMenuBtn != btn.InternalGetIsChildOfAnotherUIItem())
        {
            changeOccurred = true;
            btn.InternalSetIsChildOfAnotherUIItem(newIsChildOfAnotherMenuBtn);
        }

        btn.registerPressFromChildren = EditorGUILayout.Toggle("Register Events From Children", btn.registerPressFromChildren);

        btn.isHoverEnabled = EditorGUILayout.Toggle("Is Hover Events Enabled?", btn.isHoverEnabled);

        GUILayout.Label("Send Message", EditorStyles.boldLabel);
        EditorGUI.indentLevel++;

        GameObject newSendMessageTarget = EditorGUILayout.ObjectField("Target", btn.sendMessageTarget, typeof(GameObject), true, null) as GameObject;
        if (newSendMessageTarget != btn.sendMessageTarget)
        {
            changeOccurred = true;
            btn.sendMessageTarget = newSendMessageTarget;
        }
        if (btn.sendMessageTarget != null && EditorUtility.IsPersistent(btn.sendMessageTarget))
        {
            changeOccurred = true;
            btn.sendMessageTarget = null;
        }
        if (btn.sendMessageTarget != null)
        {
            EditorGUI.indentLevel++;
            MethodBinding( "On Down", btn.sendMessageTarget, ref btn.SendMessageOnDownMethodName );
            MethodBinding( "On Up", btn.sendMessageTarget, ref btn.SendMessageOnUpMethodName );
            MethodBinding( "On Click", btn.sendMessageTarget, ref btn.SendMessageOnClickMethodName );
            MethodBinding( "On Release", btn.sendMessageTarget, ref btn.SendMessageOnReleaseMethodName );
            EditorGUI.indentLevel--;
        }
        EditorGUI.indentLevel--;

        if (btn.collider != null) {
            GUILayout.Label("Collider", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            GUILayout.BeginHorizontal();
            EditorGUILayout.PrefixLabel("Automatic Fit");
            if (GUILayout.Button("Fit", GUILayout.MaxWidth(100))) {
                tk2dUIItemBoundsHelper.FixColliderBounds(btn);
            }
            GUILayout.EndHorizontal();

            ArrayProperty("Extra Bounds", extraBoundsProp);
            ArrayProperty("Ignore Bounds", ignoreBoundsProp);

            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
        if (GUI.changed || changeOccurred)
        {
            EditorUtility.SetDirty(btn);
        }
    }

    public static void ArrayProperty(string name, SerializedProperty prop) {
        SerializedProperty localProp = prop.Copy();
        EditorGUIUtility.LookLikeInspector();
        if ( EditorGUILayout.PropertyField(localProp, new GUIContent(name)) ) {
            EditorGUI.indentLevel++;
            bool expanded = true;
            int depth = localProp.depth;
            while (localProp.NextVisible( expanded ) && depth < localProp.depth) {
                expanded = EditorGUILayout.PropertyField(localProp);
            }
            EditorGUI.indentLevel--;
        }
    }

    //checks through hierarchy to find UIItem at this level or above to be used in inspector field
    public static tk2dUIItem FindAppropriateButtonInHierarchy(GameObject go)
    {
        tk2dUIItem btn = null;

        while (go != null)
        {
            btn = go.GetComponent<tk2dUIItem>();
            if (btn != null)
            {
                break;
            }

            go = go.transform.parent.gameObject;
        }

        return btn;
    }

    //locates tk2dUIManager in scene
    public static tk2dUIManager FindUIManagerInScene()
    {
        return GameObject.FindObjectOfType(typeof(tk2dUIManager)) as tk2dUIManager;
    }

    //creates tk2dUIManager
    [MenuItem("GameObject/Create Other/tk2d/UI Manager", false, 13950)]
    static void CreateUIManager()
    {
        GameObject go = tk2dEditorUtility.CreateGameObjectInScene("tk2dUIManager");
        go.transform.parent = null;
        go.transform.position = Vector3.zero;
        go.AddComponent<tk2dUIManager>();

        Selection.activeGameObject = go;
        Undo.RegisterCreatedObjectUndo(go, "Create tk2dUIManager");
    }
}