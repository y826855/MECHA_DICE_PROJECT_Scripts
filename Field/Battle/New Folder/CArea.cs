using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArea : MonoBehaviour
{
    public ParticleSystem m_Particle = null;
    public float m_SpawnDelay = 0.3f;
    public float m_ArrivalDelay = 0.3f;
    public float m_EndDelay = 0.5f;

    public System.Action m_CB_Arrival = null;
    public System.Action m_CB_End = null;
    public CUtility.ECardType m_Type = CUtility.ECardType.ATK;

    public CSoundManager.ECustom m_Sound = CSoundManager.ECustom.S_DICE_GRAB;

    public void Spawn() 
    {
        this.gameObject.SetActive(true);

        
        StartCoroutine(CoDoAction());
    }

    IEnumerator CoDoAction() 
    {
        yield return CUtility.GetSecD1To5s(m_SpawnDelay);
        m_Particle.Play();
        yield return CUtility.GetSecD1To5s(m_ArrivalDelay);
        if (m_CB_Arrival != null) m_CB_Arrival();
        yield return CUtility.GetSecD1To5s(m_EndDelay);
        if (m_CB_End != null) m_CB_End();
    }

    private void OnParticleSystemStopped()
    {
        //this.gameObject.SetActive(false);
        Destroy(this.gameObject);
    }
}
