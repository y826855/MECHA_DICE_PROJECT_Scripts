using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CWeapon : MonoBehaviour
{
    public ParticleSystem m_ParticleCharge = null;

    public void Charge(int _idx) 
    {
        m_ParticleCharge.Play();
    }

    public void UnCharge() 
    {
        m_ParticleCharge.Stop();
    }
}
