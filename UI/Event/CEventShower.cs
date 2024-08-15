using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEventShower : MonoBehaviour
{
    public List<CNPC> m_NPCs = new List<CNPC>();
    public SerializeDictionary<uint, CEvent_Unit> m_EventUnits =
        new SerializeDictionary<uint, CEvent_Unit>();
    public CPlayerChar m_Player = null;

    //���¼�
    public void Event_1001(int _answerIdx) 
    {
        switch (_answerIdx)
        {
            case 4: //������
                break;
            case 5: //�׳� ����
                break;
        }
    }

    //���¼� �̺�Ʈ
    public void Event_12001(CUtility.CEventLog _log) 
    {
        switch (_log.m_Idx)
        {
            case 4://�������� //ü��
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 5://���캻�� //ī��
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    //TEST
    public void Event_12002(CUtility.CEventLog _log)
    {
        int answerIdx = _log.m_Idx;
        Debug.Log("���õ�");
        Debug.Log(_log.m_Idx);

        switch (answerIdx) 
        {
            case 4://��Ÿ�� ģ��
                Debug.Log("��Ÿ~");
                StartCoroutine(CoPlayerAttackNPC02(_log.m_Connection[0]));
                break;
            case 5://ġ�Ḧ �Ѵ�
                Debug.Log("ġ��~");
                StartCoroutine(CoPlayerCure(_log.m_Connection[0]));
                break;
            case 6://�׳� ����
                Debug.Log("�� ��");
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    //��� ����
    public void Event_12003(CUtility.CEventLog _log)
    {
        switch (_log.m_Idx)
        {
            case 7://��Ÿ�� ģ��
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 8://ġ�Ḧ �Ѵ�
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            case 9://�׳� ����
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
        }
    }

    public void Event_12006(CUtility.CEventLog _log) 
    {
        Debug.Log("�̺�Ʈ ���� �ض� ����");

        int answerIdx = _log.m_Idx;
        switch (answerIdx)
        {
            //�ǳʶٱ�
            case 11:
            case 12:
                Debug.Log(_log.m_Connection[0]);
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;

                //���� ��
            case 15:
            case 16:
            case 17:
                CGameManager.Instance.m_EventManager.JumpToLog(_log.m_Connection[0]);
                break;
            //���� ��
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

    //����
    public void Event_12007(CUtility.CEventLog _log)
    {
        int answerIdx = _log.m_Idx;

        switch (answerIdx)
        {
            //���� ����
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
            //���� ����
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
        Debug.Log("�÷��̾� ���ݸ��!");
        m_Player.m_Anim.SetTrigger("Event_ATK");
        yield return CUtility.GetSecD1To5s(1.5f);

        //������ �״� ���
        m_NPCs[1].m_Anim.SetTrigger("EndAct");
        yield return CUtility.GetSecD1To5s(0.5f);

        CGameManager.Instance.m_EventManager.JumpToLog(_nextLog);
    }

    IEnumerator CoPlayerCure(int _nextLog)
    {
        yield return CUtility.GetSecD1To5s(0.5f);
        Debug.Log("�÷��̾� ġ����!");
        m_Player.m_Anim.SetTrigger("Event_Heal");
        yield return CUtility.GetSecD1To5s(2.5f);


        CGameManager.Instance.m_EventManager.JumpToLog(_nextLog);
    }
}
