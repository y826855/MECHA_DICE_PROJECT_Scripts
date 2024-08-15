using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_LogPool : MonoBehaviour
{
    public CUI_HitLog m_HitLog_Pref = null;

    public List<CUI_HitLog> m_HitLogPool = new List<CUI_HitLog>();
    public void SpawnHitLog(Vector3 _pos, string _data) 
    {
        CUI_HitLog log = null;

        //if(m_HitLogPool.Count > 0)
        //    Debug.Log(m_HitLogPool[0].gameObject.activeSelf);


        if (m_HitLogPool.Count == 0 || m_HitLogPool[0].gameObject.activeSelf == true)
        {
            log = Instantiate(m_HitLog_Pref, this.transform);
            m_HitLogPool.Add(log);
        }
        else
        {
            log = m_HitLogPool[0];
            m_HitLogPool.Add(log);
            m_HitLogPool.RemoveAt(0);
        }

        log.transform.position = _pos;
        log.Spawn(_data);
    }

}
