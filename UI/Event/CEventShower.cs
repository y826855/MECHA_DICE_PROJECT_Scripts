using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventShower : MonoBehaviour
{
    public List<CNPC> m_NPCs = new List<CNPC>();
    public SerializeDictionary<uint, CEvent_Unit> m_EventUnits =
        new SerializeDictionary<uint, CEvent_Unit>();
    public CPlayerChar m_Player = null;

    //마력석
    public void Event_1001(int _answerIdx) 
    {
        switch (_answerIdx)
        {
            case 4: //만진다
                break;
            case 5: //그냥 간다
                break;
        }
    }

    //마력석 이벤트
    public void Event_12001(CUtility.CEventLog _log) 
    {
        switch (_log.m_Idx)
        {
            case 4://만져본다 //체력
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 5://살펴본다 //카드
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    //TEST
    public void Event_12002(CUtility.CEventLog _log)
    {
        int answerIdx = _log.m_Idx;
        Debug.Log("선택됨");
        Debug.Log(_log.m_Idx);

        switch (answerIdx) 
        {
            case 4://막타를 친다
                Debug.Log("막타~");
                StartCoroutine(CoPlayerAttackNPC02(_log.m_Connection[0]));
                break;
            case 5://치료를 한다
                Debug.Log("치료~");
                StartCoroutine(CoPlayerCure(_log.m_Connection[0]));
                break;
            case 6://그냥 간다
                Debug.Log("걍 가");
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    //드론 수리
    public void Event_12003(CUtility.CEventLog _log)
    {
        switch (_log.m_Idx)
        {
            case 7://막타를 친다
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 8://치료를 한다
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 9://그냥 간다
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    public void Event_12006(CUtility.CEventLog _log) 
    {
        Debug.Log("이벤트 실행 해라 ㅋㅋ");

        int answerIdx = _log.m_Idx;
        switch (answerIdx)
        {
            //건너뛰기
            case 11:
            case 12:
                Debug.Log(_log.m_Connection[0]);
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;

                //마법 줌
            case 15:
            case 16:
            case 17:
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            //마법 줌
            case 24:
            case 25:
            case 26:
            case 27:
            case 28:
            case 29:
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    //상점
    public void Event_12007(CUtility.CEventLog _log)
    {
        int answerIdx = _log.m_Idx;

        switch (answerIdx)
        {
            //상점 열기
            case 2:
                CGameManager.Instance.m_ScheduleMgr.m_UI_Shop.gameObject.SetActive(true);
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 3:
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    public void Event_12008(CUtility.CEventLog _log)
    {
        int answerIdx = _log.m_Idx;

        switch (answerIdx)
        {
            //상점 열기
            case 6:
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }

    }

    public void SkipNPC_Scripts() 
    {
        foreach (var it in m_NPCs) it.SkipScriptSpawn();
    }


    IEnumerator CoPlayerAttackNPC02(int _nextLog)
    {
        yield return CUtility.GetSecD1To5s(0.5f);
        Debug.Log("플레이어 공격모션!");
        m_Player.m_Anim.SetTrigger("Event_ATK");
        yield return CUtility.GetSecD1To5s(1.5f);

        //누운사람 죽는 모션
        m_NPCs[1].m_Anim.SetTrigger("EndAct");
        yield return CUtility.GetSecD1To5s(0.5f);

        CGameManager.Instance.m_EventManager.JumpToLog(_nextLog);
    }

    IEnumerator CoPlayerCure(int _nextLog)
    {
        yield return CUtility.GetSecD1To5s(0.5f);
        Debug.Log("플레이어 치료모션!");
        m_Player.m_Anim.SetTrigger("Event_Heal");
        yield return CUtility.GetSecD1To5s(2.5f);


        CGameManager.Instance.m_EventManager.JumpToLog(_nextLog);
    }
}
