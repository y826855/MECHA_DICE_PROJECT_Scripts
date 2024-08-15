using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/ATK_Info")]
public class CAttack_Info : ScriptableObject
{
    //public CHit_Direct m_TestHit = null;
    public CHit_Obj m_HitObj = null;

    //운명스킬 사용 횟수
    public int m_CurrUseCount = 0;
    public CUtility.EATK_GenType m_GenType = CUtility.EATK_GenType.Once;

    [Header("==================================================")]
    public ParticleSystem m_Hit_Particle = null;
    public ParticleSystem m_Spawn_Particle = null;
    public ParticleSystem m_ATK_Particle = null;

    public string m_AnimName = "";

    [Header("==================================================")]
    public CScriptable_CardSkill m_CardSkill = null;
    public CScriptable_MonsterSkill m_MonsterSkill = null;

    public CProjectile m_Pref_Projectile = null;

    public float m_Hit_Delay = 0.2f;
    public float m_Hit_Arrival_Delay = 0.2f;
    public float m_Spawn_Delay = 0f;
    public float m_MoveSpeed = 0f;
    public float m_Duration = 1f;

    public CUtility.CDisk m_SumAll = new CUtility.CDisk();
    public CHitable m_User = null;

    public CSoundManager.ELoaded m_Sound = CSoundManager.ELoaded.NONE;

    //수치 합함
    public void SumData(CScriptable_CardSkill _card) 
    {
        m_SumAll.Clear();
        m_SumAll.AddData(_card.m_CalcedDisk);
        m_SumAll.AddData(_card.m_Data);
    }
}
