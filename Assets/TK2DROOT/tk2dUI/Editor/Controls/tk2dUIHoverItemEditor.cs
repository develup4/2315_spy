using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dUIHoverItem))]
public class tk2dUIHoverItemEditor : tk2dUIBaseItemControlEditor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        tk2dUIHoverItem hoverBtn = (tk2dUIHoverItem)target;

        hoverBtn.overStateGO = tk2dUICustomEditorGUILayout.SceneObjectField("Over State GameObject", hoverBtn.overStateGO,target);
        hoverBtn.outStateGO = tk2dUICustomEditorGUILayout.SceneObjectField("Out State GameObject", hoverBtn.outStateGO,target);

        if (GUI.changed)
        {
            EditorUtility.SetDirty(hoverBtn);
        }
    }

}
