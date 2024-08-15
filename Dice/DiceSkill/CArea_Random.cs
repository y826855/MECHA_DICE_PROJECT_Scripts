using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArea_Random : MonoBehaviour
{
    public CHit_Projectile m_Hitter = null;
    public Transform m_SpawnLoc = null;

    public CProjectile m_Proj = null;
    [SerializeField] float duration = 2f;
    [SerializeField] float proj_Duration = 0.5f;

    public void Spawn(int _dmg)
    {
        StartCoroutine(CoSpawnSpreads(_dmg));
    }

    IEnumerator CoSpawnSpreads(int _dmg) 
    {
        var enemyGroup = CGameManager.Instance.m_TurnManager.m_EnemyGroup;
        var enemies = new List<CMonster>();

        foreach (var it in enemyGroup.m_SpawnedMonsters) {
            if (it.m_Hitable.m_IsDead == true) continue;
            enemies.Add(it);
        }


        yield return CUtility.GetSecD1To5s(1f);


        float intervalTime = 2f / _dmg;
        float sumTime = 0;

        for (int i = 0; i < _dmg; i++) 
        {
            if (enemies.Count == 0) break;

            var monster = enemies[Random.Range(0, enemies.Count)];

            var inst = Instantiate(m_Proj, m_SpawnLoc.transform);
            inst.Spawn(monster.transform, proj_Duration, (int dmg) =>
            { monster.m_Hitable.OnHit(1, 0, CUtility.ETextIcon.NONE); }
            , 0);

            //맞고 죽을놈 제거
            if (monster.m_Hitable.m_DEBUG_HP < 1)
            {
                enemies.Remove(monster);
                if (enemies.Count == 0) break;
            }

            sumTime += intervalTime;

            if (sumTime > 0.1)
            {
                yield return CUtility.GetSecD1To5s(sumTime);
                sumTime = 0;
            }
        }

        CGameManager.Instance.m_TurnManager.m_PlayerChar.DoneSkill();
    }

    //투사체 연속 소환
    //protected IEnumerator CoSpawn_Rapid()
    //{
    //    //Debug.Log("RAPID");

    //    yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);
    //    Debug.Log("RAPID COUNT" + m_Info.m_CurrUseCount);

    //    while (m_Info.m_CurrUseCount > 0)
    //    {
    //        if (m_Info.m_Spawn_Particle != null)
    //        {
    //            var PS = Instantiate(m_Info.m_Spawn_Particle);
    //            PS.transform.position = m_SpawnLoc.position;
    //        }

    //        var inst = Instantiate(m_Info.m_Pref_Projectile, this.transform);
    //        inst.transform.position = m_SpawnLoc.position;
    //        inst.Spawn(m_TargetList[currTargetIdx].m_HitPos, m_Info.m_MoveSpeed,
    //            Arrival, currTargetIdx);

    //        yield return CUtility.GetSecD1To5s(m_Info.m_Hit_Delay);
    //        currTargetIdx++;
    //        m_Info.m_CurrUseCount--;
    //    }

    //    yield return CUtility.GetSecD1To5s(m_Info.m_Spawn_Delay);
    //}

    ////도착시 데미지 들어감. 1회
    //public void Arrival(int _idx)
    //{
    //    Hit_Check(_idx);

    //    //if (m_Info.m_CardSkill != null)
    //    ///Hit_To_Target(m_Info.m_CardSkill, _idx);

    //    if (m_Info.m_Hit_Particle != null)
    //    {
    //        var PH = Instantiate(m_Info.m_Hit_Particle);
    //        PH.transform.position = m_TargetList[_idx].transform.position;
    //    }

    //    if (_idx >= m_TargetList.Count - 1)
    //        User.DoneSkill();
    //}
}