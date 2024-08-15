#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(CDragon_Lore))]
public class CEdit_TestLore : Editor
{
    CDragon_Lore select = null;

    private void OnEnable()
    {
        select = target as CDragon_Lore;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Lore On")) 
        { select.ActLore(); }
    }
}

#endif