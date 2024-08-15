#if UNITY_EDITOR


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CTestUI), true)]
public class CEdit_TestUI : Editor
{
    CTestUI select = null;

    private void OnEnable()
    {
        select = target as CTestUI;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("MOVE R"))
            select.OnMoveRight();
        if (GUILayout.Button("MOVE L"))
            select.OnMoveLeft();
    }
}

#endif