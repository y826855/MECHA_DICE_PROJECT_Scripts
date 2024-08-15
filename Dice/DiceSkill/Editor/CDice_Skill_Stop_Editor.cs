using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(CDice_Skill_Stop))]
public class CDice_Skill_Stop_Editor : Editor
{
    CDice_Skill_Stop selected = null;

    private void OnEnable()
    {
        selected = target as CDice_Skill_Stop;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (selected.m_SkillParticle != null) 
        {
            float defaultSize = selected.m_SkillParticle.main.startSize.constant;
            selected.m_SkillDefaultSize = defaultSize;
        }

    }
}
