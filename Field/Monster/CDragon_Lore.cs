using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CDragon_Lore : MonoBehaviour
{
    //레이어 인덱스를 바꾸자 최대 4
    public List<StylizedGrass.GrassBender> m_LoreSphere = new List<StylizedGrass.GrassBender>();
    public StylizedGrass.GrassBender m_Pref_LoreSphere = null;

    public int m_LoreWaveCount = 5;
    public float m_Lore_Duration = 1f;
    float maxDuration = 0;

    public Vector3 m_MaxSize = new Vector3(6, 6, 6);
    public Vector3 m_DefaultSize = new Vector3(5, 5, 5);


    public void SpawnLore() 
    {
        m_LoreWaveCount = Mathf.Clamp(m_LoreWaveCount, 1, 5);

        if (m_LoreSphere.Count >= m_LoreWaveCount) return;

        for (int i = 0; i < m_LoreWaveCount; i++) 
        {
            var inst = Instantiate(m_Pref_LoreSphere, this.gameObject.transform);
            inst.name += i.ToString();
            m_LoreSphere.Add(inst);
        }
    }

    Sequence SeqLore = null;

    public void ActLore(float _duration = 3f) 
    {
        SpawnLore();

        SeqLore = DOTween.Sequence();

        float interval = m_Lore_Duration / Mathf.Clamp(m_LoreWaveCount, 1, 4);
        Debug.Log(interval);
        int maxDuration = Mathf.CeilToInt(_duration / interval);
        int count = m_LoreSphere.Count;

        //sort layer 떔에 끊겨서 보임

        SeqLore.AppendInterval(interval);

        for (int i = 0; i < maxDuration; i++) 
        {
            int idx = i % count;
            SeqLore.InsertCallback(interval * i, () => { LoreLayerCtrl(idx); });
            SeqLore.Insert(interval * i, m_LoreSphere[idx].transform.DOScale(m_MaxSize, m_Lore_Duration)
                .OnComplete(() => { m_LoreSphere[idx].transform.localScale = Vector3.one; }));
        }
        
    }

    [SerializeField] int currIdx = 0;
    public void LoreLayerCtrl(int _idx) 
    {
        //m_LoreSphere[_idx].transform.SetAsFirstSibling();
        int count = m_LoreSphere.Count;
        currIdx = _idx;

        int start = 4 - _idx;
        for (int i = 0; i < count; i++) 
        { 
            m_LoreSphere[i].sortingLayer = (start + i) % count;
            m_LoreSphere[i].UpdateProperties();
        }
        
    }
}
