using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class COpening_Battle : CBattleBegin
{
    public override IEnumerator CoActBeforeBattle()
    {
        foreach (var it in m_Battle_Intros) 
        {
            //이벤트가 있으면 실행함
            if (it.m_Event_Intros != null)
            {
                yield return StartCoroutine(it.CoDo_IntroEvent());
                yield return CUtility.GetSecD1To5s(0.5f);
            }
        }
    }

    public override IEnumerator CoActAfterBattle()
    {
        foreach (var it in m_Battle_Intros)
        {
            //이벤트가 있으면 실행함
            if (it.m_Event_Outros != null)
            {
                yield return StartCoroutine(it.CoDo_OutroEvent());
                yield return CUtility.GetSecD1To5s(0.5f);
            }
        }
    }
}
