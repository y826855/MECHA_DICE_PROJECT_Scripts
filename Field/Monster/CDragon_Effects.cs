using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CDragon_Effects : CBattleBegin
{
    [Header("===========================================")]
    public GameObject m_Dragon_Shadow = null;
    [SerializeField] Transform m_DragonShadow_Start = null;
    [SerializeField] Transform m_DragonShadow_End = null;
    [SerializeField] float m_MoveDuration = 2f;

    [Header("===========================================")]
    public ParticleSystem m_P_Wind = null;
    public CDragon_Lore m_P_Lore = null;

    public override IEnumerator CoActBeforeBattle()
    {
        Debug.Log("µÂ∑°∞Ô µÓ¿Â ¿Ã∆Â∆Æ");

        //Spawn Shadow
        m_Dragon_Shadow.SetActive(true);
        m_Dragon_Shadow.transform.position = m_DragonShadow_Start.position;
        m_Dragon_Shadow.transform.DOMove(m_DragonShadow_End.position, m_MoveDuration);

        yield return CUtility.GetSecD1To5s(m_MoveDuration);
        m_Dragon_Shadow.SetActive(false);


        m_P_Wind.Play();
    }


    public void UseLore() 
    {
        m_P_Lore.ActLore();
    }
}