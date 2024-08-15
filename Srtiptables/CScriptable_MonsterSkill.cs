using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/MONSTER_SKILL")]
public class CScriptable_MonsterSkill : CScriptable_CSVData<CScriptable_MonsterSkill>
{
    public CUtility.CMonsterSkill m_Data = new CUtility.CMonsterSkill();
    public CAttack_Info m_Atk_Info = null;

#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/MonsterSkill/";
    static public string m_ATK_Info_Path = "Assets/Game/Scriptables/ATK_Info/";

    static public CScriptable_MonsterSkill CreatePrefab(CUtility.CMonsterSkill _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CMonsterSkill>();

        t.m_Atk_Info = LoadAsset(m_ATK_Info_Path, _data.m_Particle) as CAttack_Info;

        return t;
    }
#endif

    public void Spawn(CHitable _user) 
    {
        m_Atk_Info = Instantiate(m_Atk_Info);
        m_Atk_Info.m_CardSkill = null;
        m_Atk_Info.m_MonsterSkill = this;
        m_Atk_Info.m_AnimName = m_Data.m_Anim;
        m_Atk_Info.m_User = _user;
    }
}
