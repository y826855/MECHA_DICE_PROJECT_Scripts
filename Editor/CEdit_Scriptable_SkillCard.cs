#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CScriptable_CardSkill), true)]
public class CEdit_Scriptable_SkillCard : Editor
{
    CScriptable_CardSkill select = null;

    private void OnEnable()
    {
        select = target as CScriptable_CardSkill;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        EditorUtility.SetDirty(select);

        if (GUILayout.Button("SET_DATA_PROPERTIES"))
        { select.m_Data.SetProperties(); }

        if (GUILayout.Button("SUM_WHOLE_PROPERTIES"))
        { select.SumProperties(); }
    }
}

#endif