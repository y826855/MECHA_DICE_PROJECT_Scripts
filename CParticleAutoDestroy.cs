using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Pool;

public class CParticleAutoDestroy : MonoBehaviour
{
    public bool m_Destroy = true;

    public IObjectPool<GameObject> Pool { get; set; }

    private void OnParticleSystemStopped()
    {
        if (m_Destroy == true) Destroy(this.gameObject);
        else 
        {
            if (Pool != null) Pool.Release(this.gameObject);
            else this.gameObject.SetActive(false);
        }
    }
}
