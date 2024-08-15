using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

public class CUI_SimpleAnim : MonoBehaviour
{
    public float m_Duration = 0.3f;

    public Image m_Image = null;

    [Header("==================================")]
    public Vector3 m_MovePos = Vector3.zero;
    public Vector3 m_Scale = Vector3.one;

    Sequence seqAct = null;

    public void OnEnable()
    {
        if (seqAct != null)
        { seqAct.Kill(); }
        seqAct = DOTween.Sequence();
    }
    public void Act_LocalMove() 
    {
        if (seqAct == null) seqAct = DOTween.Sequence();
        seqAct.Append(this.transform.DOLocalMove(m_MovePos, m_Duration));
    }

    public void Act_Alpha() 
    {
        
    }
    public void Act_Scale() 
    {
        if (seqAct == null) seqAct = DOTween.Sequence();
        seqAct.Append(this.transform.DOScale(m_Scale, m_Duration));
    }

    public void Act_Reset() 
    {
        if (seqAct != null)// 기존 시퀀스 중단
        { seqAct.Kill(); }
        if (seqAct == null) return;
        seqAct?.Append(this.transform.DOLocalMove(Vector3.zero, m_Duration));
        seqAct?.Append(this.transform.DOScale(Vector3.one, m_Duration));
    }

    private void OnDisable()
    {
        // GameObject가 비활성화될 때 시퀀스 중단
        if (seqAct != null)
        { seqAct.Kill(); }
        seqAct = null;
    }
}
