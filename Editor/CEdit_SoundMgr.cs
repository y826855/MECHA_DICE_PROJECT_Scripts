#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CSoundManager))]
public class CEdit_SoundMgr : Editor
{
    CSoundManager select = null;

    private void OnEnable()
    {
        select = target as CSoundManager;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtility.SetDirty(select);

        if (GUILayout.Button("LOAD ALL SOUND"))
        { select.LoadSoundes(); }
    }
}
#endif