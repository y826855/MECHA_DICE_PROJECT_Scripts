using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHit_Obj : MonoBehaviour
{
    [SerializeField] protected CMoveable User = null;
    [SerializeField] protected List<CHitable> m_TargetList = null;

    //public Vector3 m_SpawnPos = Vector3.zero;
    public Transform m_SpawnLoc = null;
    public CAttack_Info m_Info = null;

    [SerializeField] protected int currTargetIdx = 0;
    [SerializeField] string m_DefaultATK = "ATK_01";

    public virtual void Spawn(Transform _Loc, CMoveable _user, List<CHitable> _target,
        CAttack_Info _atkInfo)
    {
        currTargetIdx = 0;

        m_SpawnLoc = _Loc;
        User = _user;
        m_TargetList = _target;

        m_Info = _atkInfo;
    }

    public virtual void UseEff()
    {
        //var data = m_Info.m_WeaponSkill.m_Data;
        //User.m_Hitable.GainDef(data.m_StackData[Stack].m_Def);
    }

    //모션과 파티클이 한번만 나감. 히트는 연타일 수도 있음. 방어에 유효
    public void AttackOnce() 
    {
        if (m_Info.m_AnimName == "") User.m_Anim.SetTrigger(m_DefaultATK);
        else User.m_Anim.SetTrigger(m_Info.m_AnimName);
    }

    //모션은 한번이지만 파티클 연사로 나감.
    public void RapidAttack() 
    {
        Debug.Log(m_Info.m_AnimName);
        //딜레이 살짝 주어 연속 공격 느낌
        if (m_Info.m_AnimName == "") User.m_Anim.SetTrigger(m_DefaultATK);
        else User.m_Anim.SetTrigger(m_Info.m_AnimName);
    }

    public void GenSkill(CUtility.EATK_GenType _genType, int _count = 0, CHitable _self = null) 
    {
        m_Info.m_CurrUseCount = _count;
        m_Info.m_GenType = _genType;
        User.m_IsEndAttack = false;

        //타겟 없으면 셀프타겟 채움
        if (m_TargetList.Count == 0 && _self != null)
            for (int i = 0; i < _count; i++)
            { m_TargetList.Add(_self); }

        switch (_genType) 
        {
            case CUtility.EATK_GenType.Combo:
                CheckMoreAttack(); break;
            case CUtility.EATK_GenType.Once:
                AttackOnce();
                break;
            case CUtility.EATK_GenType.Rapid:
                RapidAttack();
                break;
        }
    }

    public void CheckMoreAttack()
    {
        //첫번째 공격
        if (currTargetIdx == 0)
        {
            User.m_Anim.SetBool("EndCombo", false);

            if (m_Info.m_AnimName == "") User.m_Anim.SetTrigger("ATK_01");
            else User.m_Anim.SetTrigger(m_Info.m_AnimName);

            if (m_TargetList.Count == 1)
                User.m_Anim.SetTrigger("LastComboAtk");
        }

        else if (currTargetIdx < m_TargetList.Count)
        {
            
            if (currTargetIdx == m_TargetList.Count - 1)
                User.m_Anim.SetTrigger("LastComboAtk");
            else
                User.m_Anim.SetTrigger("NextCombo");
        }

        else
        {
            User.m_Anim.SetBool("EndCombo", true);
            User.m_Anim.ResetTrigger("NextCombo");
            User.m_Anim.ResetTrigger("LastComboAtk");
            User.DoneAnim();
            //User.DoneAnim();
        }

        //Debug.Log("ANIM" + currTargetIdx.ToString());
        //Debug.Log(currTargetIdx < m_TargetList.Count);
    }



    public virtual void Anim_Attack()
    {
        StartCoroutine(CoSpawn_Delay());
    }

    protected virtual IEnumerator CoSpawn_Delay(bool _isOnce = false)
    {
        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);
    }

    protected virtual IEnumerator CoSpawn_Rapid() 
    {
        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);
    }

    public void Hit_Check(int _idx) 
    {
        if (m_Info.m_CardSkill != null)
            Hit_To_Target(m_Info.m_CardSkill, _idx);
        else if (m_Info.m_MonsterSkill != null)
            Hit_To_Target(m_Info.m_MonsterSkill, _idx);
    }

    //데미지 주기
    public void Hit_To_Target(CScriptable_CardSkill _skill, int _idx)
    {
        //Debug.Log("HIT TO TARGET");

        var data = _skill.m_Data;

        if (_skill.m_Atk_Info.m_SumAll.m_Damage.m_Num > 0)
        {
            m_TargetList[_idx].m_Field_Info.RemoveTarget();
            m_TargetList[_idx].OnHit(_skill);
        }
        else
            m_TargetList[_idx].GainDef(_skill);
    }

    public void Hit_To_Target(CScriptable_MonsterSkill _skill, int _idx)
    {
        //Debug.Log("HIT TO TARGET");

        var data = _skill.m_Data;

        m_TargetList[_idx].m_Field_Info.RemoveTarget();

        if (_skill.m_Data.m_Dmg > 0)
            m_TargetList[_idx].OnHit(_skill);
        else
            m_TargetList[_idx].GainDef(_skill);
    }

    //파괴 예약
    Coroutine coDestroy = null;
    public void DestroySelf() 
    {
        if (coDestroy == null)
            coDestroy = StartCoroutine(CoWaitChildDestroy());
    }

    protected IEnumerator CoWaitChildDestroy() 
    {
        while (this.transform.childCount > 0)
        { yield return CUtility.GetSecD1To5s(0.5f); }
        Destroy(this.gameObject);
    }


    ////즉발 데미지 주기//운명스킬
    //public void Hit_To_Target(CScriptable_DestinySkill _skill, int _idx)
    //{
    //    var data = _skill.m_Data;

    //    m_TargetList[_idx].m_Field_Info.RemoveTarget();

    //    m_TargetList[_idx].OnHit(
    //        data.m_Dmg, _skill.m_Data.m_Buff_Target.m_Buff_Group);
    //}
    //public void Hit_To_Target(CScriptable_WeaponSkill _skill, int _idx)
    //{
    //    var data = _skill.m_Data.m_StackData[Stack];

    //    m_TargetList[_idx].m_Field_Info.RemoveTarget();

    //    m_TargetList[_idx].OnHit(
    //        data.m_Dmg, _skill.m_Data.m_Buff_Target.m_Buff_Group);
    //}
}
