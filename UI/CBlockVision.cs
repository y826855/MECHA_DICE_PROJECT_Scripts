using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class CBlockVision : MonoBehaviour
{
    public Image m_Img_SoftBlock = null;
    public Image m_Img_HardBlock = null;
    [SerializeField] float m_Duration = 0.5f;
    Sequence seq = null;
    public void Start()
    {
        seq = DOTween.Sequence();
    }
    public void Cover(bool _soft = false, System.Action _cbComplete = null) 
    {
        m_Img_SoftBlock.enabled = true;
        seq.Append(m_Img_SoftBlock.DOFade(1, m_Duration)
            .OnComplete(() => { if (_cbComplete != null) _cbComplete(); }));
        if (_soft == false)
        {
            m_Img_HardBlock.enabled = true;
            seq.Append(m_Img_HardBlock.DOFade(1, m_Duration));
        }
    }

    public void Uncover_Hard(System.Action _cbComplite = null) 
    {
        if (m_Img_HardBlock.enabled == false) return;

        seq.Append(m_Img_HardBlock.DOFade(0, m_Duration)
            .OnComplete(() => { 
                m_Img_HardBlock.enabled = false;
                if (_cbComplite != null) _cbComplite();
            }));
    }

    public void Uncover() 
    {
        //m_Img_HardBlock.enabled = false;
        //m_Img_SoftBlock.enabled = false;

        seq.Append(m_Img_HardBlock.DOFade(0, m_Duration)
            .OnComplete(() => { m_Img_HardBlock.enabled = false; }));
        seq.Append(m_Img_SoftBlock.DOFade(0, m_Duration)
            .OnComplete(() => { m_Img_SoftBlock.enabled = false; }));
    }
}
