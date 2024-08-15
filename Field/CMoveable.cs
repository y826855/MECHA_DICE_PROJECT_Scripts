using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMoveable : MonoBehaviour
{
    [Header("==========MOVEABLE=========")]
    public CHitable m_Hitable = null;
    public Vector3 m_TargetPos = Vector3.zero;
    public Transform m_Body = null;

    public bool m_IsArrived = false;
    public float m_MoveSpeed = 5f;

    public Animator m_Anim = null;

    public bool m_IsEndAttack = true;


    public void AnimAttack(string _anim) 
    {
        m_Anim.SetTrigger(_anim);
    }

    public void AddAnimCombo() 
    {
        //int combo = m_Anim.GetInteger("Combo");
        //m_Anim.SetInteger("Combo", ++combo);
        m_Anim.SetTrigger("NextCombo");
    }

    public virtual void DoneAnim() 
    {
        //Debug.Log("DONE ANIM");
        //m_Anim.SetInteger("Combo", 0);
        //m_Anim.SetBool("EndCombo", true);
        //m_IsEndAttack = false;
    }

    public virtual void Spawn() 
    {
        
    }

    public void ArriveTargetPos() 
    {
        
    }

    public void LookAt(Transform _loc) 
    {
        Vector3 locPos = _loc.position;
        locPos.y = m_Body.position.y;
        Debug.Log(locPos.y);
        m_Body.LookAt(locPos, Vector3.up);
    }


    public virtual void OnDie() { }

    public virtual void DoneSkill() { }
    public virtual void Attack() { }
    public virtual void EndAttack() { }
    public virtual void Attack_Done() { }
}
