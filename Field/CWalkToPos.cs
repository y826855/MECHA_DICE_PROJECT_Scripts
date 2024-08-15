using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

public class CWalkToPos : MonoBehaviour
{
    [SerializeField] Animator m_Anim;

    public Transform m_Destination = null;
    public Transform m_StartLoc = null;
    public float m_Speed = 1f;

    public System.Action CB_DoneMove = null;

    public void SetStartPos(Transform _startLoc = null) 
    {
        if (_startLoc != null) m_StartLoc = _startLoc;
        this.transform.position = m_StartLoc.position;
    }

    public float MoveTo() 
    {
        m_Anim.SetBool("Move", true);
        float duration = Vector3.Distance(this.transform.position, m_Destination.position) / m_Speed;
        this.transform.DOMove(m_Destination.position, duration).OnComplete(
            () => { MoveDone(); });

        return duration;
    }

    public void MoveDone() 
    {
        m_Anim.SetBool("Move", false);
        if (CB_DoneMove != null) CB_DoneMove();
    }
}
