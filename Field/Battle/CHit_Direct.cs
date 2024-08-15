using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHit_Direct : CHit_Obj
{
    public bool m_IsDropToGroud = false;
    public override void Spawn(Transform _pos, CMoveable _user, List<CHitable> _target,
        CAttack_Info _atkInfo)
    {
        currTargetIdx = 0;

        m_SpawnLoc = _pos;
        User = _user;
        m_TargetList = _target;

        m_Info = _atkInfo;
    }

    public override void UseEff()
    {
        //var data = m_Info.m_WeaponSkill.m_Data;
        //User.m_Hitable.GainDef(data.m_StackData[Stack].m_Def);
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
                //Debug.Log("CHECKED TYPE RAPID"); //break;
                StartCoroutine(CoSpawn_Rapid()); break;
        }
    }

    protected override IEnumerator CoSpawn_Delay(bool _isMul = false) 
    {
        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);


        //스폰 파티클 생성
        if (m_Info.m_Spawn_Particle != null)
        {
            var PS = Instantiate(m_Info.m_Spawn_Particle);
            PS.transform.position = m_SpawnLoc.position;
        }

        //메인 파티클 생성
        if (m_Info.m_ATK_Particle != null)
        {
            var PA = Instantiate(m_Info.m_ATK_Particle);
            //PA.transform.position = m_TargetList[currTargetIdx].transform.position;
            PA.transform.position = m_TargetList[currTargetIdx].m_HitPos.position;
            if (m_IsDropToGroud == true)
            {
                var pos = PA.transform.position;
                pos.y = 0; PA.transform.position = pos;
            }

            //소리 재생
            CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_Info.m_Sound);
        }

        //피격 파티클 생성
        if (m_Info.m_Hit_Particle != null)
        {
            var PH = Instantiate(m_Info.m_Hit_Particle);
            //PH.transform.position = m_TargetList[currTargetIdx].transform.position;
            PH.transform.position = m_TargetList[currTargetIdx].m_HitPos.position;
        }

        yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Arrival_Delay);
        //yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);

        while (m_Info.m_CurrUseCount > 0)
        {
            Hit_Check(currTargetIdx);
            currTargetIdx++;
            //if (currTargetIdx == m_TargetList.Count - 1)
            m_Info.m_CurrUseCount--;

            yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);
        }


        User.DoneSkill();
    }

    protected override IEnumerator CoSpawn_Rapid()
    {
        yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);

        while (m_Info.m_CurrUseCount > 0)
        {
            yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Arrival_Delay);

            if (m_Info.m_Spawn_Particle != null)
            {
                var PS = Instantiate(m_Info.m_Spawn_Particle);
                PS.transform.position = m_SpawnLoc.position;
            }

            //소리 재생
            CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_Info.m_Sound);
            if (m_Info.m_ATK_Particle != null)
            {
                var PA = Instantiate(m_Info.m_ATK_Particle);
                PA.transform.position = m_TargetList[currTargetIdx].m_HitPos.position;
                //PA.transform.position = m_TargetList[currTargetIdx].transform.position;
            }

            if (m_Info.m_Hit_Particle != null)
            {
                var PH = Instantiate(m_Info.m_Hit_Particle);
                PH.transform.position = m_TargetList[currTargetIdx].m_HitPos.position;
                //PH.transform.position = m_TargetList[currTargetIdx].transform.position;
            }

            Hit_Check(currTargetIdx);
            yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);

            m_Info.m_CurrUseCount--;
            currTargetIdx++;
        }

        User.DoneSkill();
    }
}
