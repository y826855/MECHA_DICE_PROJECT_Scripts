#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(CUI_ShoutOut), true)]
public class CEdit_Testing : Editor
{
    CUI_ShoutOut select = null;

    private void OnEnable()
    {
        select = target as CUI_ShoutOut;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtility.SetDirty(select);

        if (GUILayout.Button("TEST01"))
        { select.TestRewind_01(); }
        if (GUILayout.Button("TEST02"))
        { select.TestRewind_02(); }
        if (GUILayout.Button("TEST03"))
        { select.TestRewind_03(); }
        if (GUILayout.Button("TEST04"))
        { select.TestRewind_04(); }
    }
}

#endif