using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CHit_Projectile : CHit_Obj
{
    public override void Spawn(Transform _Loc, CMoveable _user, List<CHitable> _target,
        CAttack_Info _atkInfo)
    {
        currTargetIdx = 0;

        m_SpawnLoc = _Loc;
        User = _user;
        m_TargetList = _target;

        m_Info = _atkInfo;
    }

    public override void UseEff()
    {
        //사용 효과
    }

    public override void Anim_Attack()
    {
        //Debug.Log("ANIM ATK");

        switch (m_Info.m_GenType) 
        {
            case CUtility.EATK_GenType.Combo:
                StartCoroutine(CoSpawn_Delay()); break;
            case CUtility.EATK_GenType.Once:
                StartCoroutine(CoSpawn_Delay(_isMul: true)); break;
            case CUtility.EATK_GenType.Rapid:
                Debug.Log("CHECKED TYPE RAPID"); //break;
                StartCoroutine(CoSpawn_Rapid()); break;
        }
    }

    //투사체 연속 소환
    protected override IEnumerator CoSpawn_Rapid() 
    {
        //Debug.Log("RAPID");

        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);
        Debug.Log("RAPID COUNT" + m_Info.m_CurrUseCount);

        while (m_Info.m_CurrUseCount > 0) 
        {
            if (m_Info.m_Spawn_Particle != null)
            {
                var PS = Instantiate(m_Info.m_Spawn_Particle);
                PS.transform.position = m_SpawnLoc.position;
            }

            var inst = Instantiate(m_Info.m_Pref_Projectile, this.transform);
            inst.transform.position = m_SpawnLoc.position;
            inst.Spawn(m_TargetList[currTargetIdx].m_HitPos, m_Info.m_MoveSpeed,
                Arrival, currTargetIdx);

            //소리 재생
            CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_Info.m_Sound);

            yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);
            currTargetIdx++;
            m_Info.m_CurrUseCount--;
        }

        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);
    }

    //딜레이 이후 투사체 소환
    protected override IEnumerator CoSpawn_Delay(bool _isMul = false)
    {
        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);

        if (m_Info.m_Spawn_Particle != null)
        {
            var PS = Instantiate(m_Info.m_Spawn_Particle);
            PS.transform.position = m_SpawnLoc.position;
        }

        var inst = Instantiate(m_Info.m_Pref_Projectile, this.transform);
        inst.transform.position = m_SpawnLoc.position;

        //소리 재생
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_Info.m_Sound);

        if (_isMul == false)
            inst.Spawn(m_TargetList[currTargetIdx].m_HitPos, m_Info.m_MoveSpeed,
                Arrival, currTargetIdx);
        else
            inst.Spawn(m_TargetList[currTargetIdx].m_HitPos, m_Info.m_MoveSpeed,
                Arrival_HitMul, currTargetIdx);

        currTargetIdx++;
        yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);
    }

    //도착시 데미지 들어감. 1회
    public void Arrival(int _idx) 
    {
        Debug.Log("SKILL ARRIVAL " + _idx + " " + m_TargetList.Count);

        Hit_Check(_idx);

        //if (m_Info.m_CardSkill != null)
        ///Hit_To_Target(m_Info.m_CardSkill, _idx);

        if (m_Info.m_Hit_Particle != null)
        {
            var PH = Instantiate(m_Info.m_Hit_Particle);
            PH.transform.position = m_TargetList[_idx].transform.position;
        }
        
        if (_idx >= m_TargetList.Count-1)
            User.DoneSkill();
    }

    //도착시 피격 멀티
    public void Arrival_HitMul(int _idx) 
    {
        StartCoroutine(CoHit_Mul());
    }

    IEnumerator CoHit_Mul() 
    {
        while (m_Info.m_CurrUseCount > 0)
        {
            Hit_Check(m_Info.m_CurrUseCount - 1);
            //if (m_Info.m_CardSkill != null)
            //Hit_To_Target(m_Info.m_CardSkill, m_Info.m_CurrUseCount - 1);

            if (m_Info.m_Hit_Particle != null)
            {
                var PH = Instantiate(m_Info.m_Hit_Particle);
                PH.transform.position = m_TargetList[m_Info.m_CurrUseCount - 1].transform.position;
            }

            m_Info.m_CurrUseCount--;
            yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);
        }
        yield return null;
        User.DoneSkill();
    }

}
