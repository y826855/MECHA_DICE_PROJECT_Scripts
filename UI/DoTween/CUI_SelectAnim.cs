using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;


public class CUI_SelectAnim : MonoBehaviour
{
    public enum EState { NONE = 0, IDLE, HOVER, EXIT, SELECT, DESELECT }

    [SerializeField] EState currState = EState.NONE;
    [SerializeField] bool m_CanSelect = false;
    public Transform m_Target = null;

    public Vector3 m_Scale = Vector3.one;
    public Vector3 m_Move = Vector3.zero;

    Vector3 m_LocalScale = Vector3.one;
    Vector3 m_LocalMove = Vector3.zero;

    public float m_Duration = 0.1f;

    private void OnEnable()
    { 
        if (m_Target == null) m_Target = this.transform;

        m_LocalScale = this.transform.localScale;
        m_LocalMove = this.transform.localPosition;
    }

    public void On_Idle() 
    {
        currState = EState.IDLE;
    }

    public void On_Hover() 
    {
        currState = EState.HOVER;

        Sequence seq = DOTween.Sequence();

        if (m_Move != Vector3.zero)
            seq.Append(m_Target.DOLocalMove(m_Move, m_Duration));
        if (m_Scale != Vector3.one)
            seq.Append(m_Target.DOScale(m_Scale, m_Duration));

        seq.OnComplete(On_Idle);
    }
    public void On_Exit() 
    {
        currState = EState.EXIT;

        Sequence seq = DOTween.Sequence();

        if (m_Move != Vector3.zero && this.transform.localPosition != m_LocalMove)
            seq.Append(m_Target.DOLocalMove(m_LocalMove, m_Duration));
        if (this.transform.localScale != m_LocalScale)
            seq.Append(m_Target.DOScale(m_LocalScale, m_Duration));

        seq.OnComplete(On_Idle);
    }
    public void On_Select() 
    {
        if (m_CanSelect == false) return;
    }
    public void On_Deselect() 
    {
        if (m_CanSelect == false) return;
    }

}
