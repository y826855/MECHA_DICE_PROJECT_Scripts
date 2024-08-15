using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;


public class CUI_CardAnim : CUI_Dynamic
{
    public enum EHandCardState { NONE, IDLE, READY, CHANGE }

    [SerializeField] EHandCardState currState = EHandCardState.NONE;

    Sequence seq_Idle = null;
    Sequence seq_Ready = null;
    Sequence seq_Change = null;

    public void Anim_Ready() 
    {
        if (currState == EHandCardState.READY) return;
        currState = EHandCardState.READY;
        seq_Ready = DOTween.Sequence();
        seq_Ready.Append(this.transform.DOLocalMove(m_LocalPos_In, m_FadeIn_Time));
    }

    public void Anim_Change() 
    {
        seq_Change = DOTween.Sequence();
        seq_Change.Append(this.transform.DOLocalMove(m_LocalPos_Out, m_FadeIn_Time));
        seq_Change.Append(this.transform.DOLocalMove(Vector3.zero, m_FadeIn_Time));
    }

    public void Anim_Idle() 
    {
        if (currState == EHandCardState.IDLE) return;
        currState = EHandCardState.IDLE;
        seq_Change = DOTween.Sequence();
        seq_Change.Append(this.transform.DOLocalMove(Vector3.zero, m_FadeIn_Time));
    }
}
