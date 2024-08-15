
#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CUI_Field), true)]
public class CEdit_UI_Field : Editor
{
    CUI_Field select = null;

    private void OnEnable()
    {
        select = target as CUI_Field;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("LOOK AT CAMERA")) 
        { select.LookAtCamera(); }
    }
}

#endif