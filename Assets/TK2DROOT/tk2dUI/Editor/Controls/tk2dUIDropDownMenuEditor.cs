using UnityEngine;
using UnityEditor;
using System.Collections;

[CanEditMultipleObjects]
[CustomEditor(typeof(tk2dUIDropDownMenu))]
public class tk2dUIDropDownMenuEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUIUtility.LookLikeInspector();
        base.OnInspectorGUI();
    }

    public void OnSceneGUI()
    {
        bool wasChange=false;
        tk2dUIDropDownMenu dropdownMenu = (tk2dUIDropDownMenu)target;
        tk2dUIDropDownItem dropdownItemTemplate = dropdownMenu.dropDownItemTemplate;

		// Get rescaled transforms
        Matrix4x4 m = dropdownMenu.transform.localToWorldMatrix;
		Vector3 up = m.MultiplyVector(Vector3.up);
		// Vector3 right = m.MultiplyVector(Vector3.right);

        float newDropDownButtonHeight = tk2dUIControlsHelperEditor.DrawLengthHandles("Dropdown Button Height", dropdownMenu.height, dropdownMenu.transform.position+(up*(dropdownMenu.height/2)), -up, Color.red,.15f, .3f, .05f);
        if (newDropDownButtonHeight != dropdownMenu.height)
        {
            Undo.RegisterUndo(dropdownMenu, "Dropdown Button Height Changed");
            dropdownMenu.height = newDropDownButtonHeight;
            wasChange = true;
        }

        if (dropdownItemTemplate != null)
        {
            float yPosDropdownItemTemplate = -dropdownMenu.height;

            if (dropdownItemTemplate.transform.localPosition.y != yPosDropdownItemTemplate)
            {
                dropdownItemTemplate.transform.localPosition = new Vector3(dropdownItemTemplate.transform.localPosition.x, yPosDropdownItemTemplate, dropdownItemTemplate.transform.localPosition.z);
                EditorUtility.SetDirty(dropdownItemTemplate.transform);
            }


            float newDropDownItemTemplateHeight = tk2dUIControlsHelperEditor.DrawLengthHandles("Dropdown Item Template Height", dropdownItemTemplate.height, dropdownMenu.transform.position - (up * (dropdownMenu.height/2)), -up, Color.blue, .15f, .4f, .05f);
            if (newDropDownItemTemplateHeight != dropdownItemTemplate.height)
            {
                Undo.RegisterUndo(dropdownItemTemplate, "Dropdown Template Height Changed");
                dropdownItemTemplate.height = newDropDownItemTemplateHeight;
                EditorUtility.SetDirty(dropdownItemTemplate);
            }
        }

        if (wasChange)
        {
            EditorUtility.SetDirty(dropdownMenu);
        }
    }

}
