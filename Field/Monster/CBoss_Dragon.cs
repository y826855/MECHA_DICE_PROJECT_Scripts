using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CBoss_Dragon : CMonster
{
    [SerializeField] float m_LandingDuration = 1.5f;
    [SerializeField] ParticleSystem m_FireBreath = null;
    CDragon_Effects m_DragonEffects = null;


    public override void OnBattle()
    {
        StartCoroutine(CoSpawnLanding());
        m_DragonEffects = CGameManager.Instance.m_TurnManager.m_Battle_Event as CDragon_Effects;
    }

    IEnumerator CoSpawnLanding() 
    {
        Debug.Log("LANDING");
        m_Anim.SetTrigger("Landing");
        m_Body.DOLocalMove(Vector3.zero, m_LandingDuration);
        yield return CUtility.GetSecD1To5s(m_LandingDuration);

        m_Hitable.OnBattle();
    }

    public void Wave() 
    {
        m_DragonEffects.m_P_Lore.ActLore(2f);
    }

    public void FireBreath() 
    {
        m_FireBreath.Play();
    }
}
