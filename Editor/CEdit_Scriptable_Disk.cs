#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CScriptable_Disk), true)]
public class CEdit_Scriptable_Disk : Editor
{
    CScriptable_Disk select = null;

    private void OnEnable()
    {
        select = target as CScriptable_Disk;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtility.SetDirty(select);

        if (GUILayout.Button("Dictionary_Add_TestCase"))
        { select.m_Data.SetProperties(); }
    }
}

#endif