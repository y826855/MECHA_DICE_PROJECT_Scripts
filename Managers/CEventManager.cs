using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;


public class CEventManager : MonoBehaviour
{
    public CScriptable_EventLog m_CurrEvent = null;
    public GameObject m_TotalLogs = null;
    public TextAnimator_TMP m_TMPA_TotalLogs = null;
    public TextAnimator_TMP m_TMPA_SystemLogs = null;
    public Febucci.UI.Core.TypewriterCore m_SystemTypewriter;

    //Adds and removes listening to callback
    void OnEnable() => m_SystemTypewriter.onMessage.AddListener(OnTypewriterMessage);
    void OnDisable() => m_SystemTypewriter.onMessage.RemoveListener(OnTypewriterMessage);
    public int m_LogIdx = 0;

    //Does stuff based on event
    void OnTypewriterMessage(Febucci.UI.Core.Parsing.EventMarker eventMarker)
    {
        switch (eventMarker.name)
        {
            case "something":
                // do something
                break;
        }
    }

    [Header("=======================")]
    public CEventShower m_EventShower = null;
    public CUI_EventCardGroup m_CardGroup = null;
    public Cinemachine.CinemachineVirtualCamera m_GenCamera = null;
    public Cinemachine.CinemachineVirtualCamera m_EventCamera = null;
    public CPlayerChar m_PlayerChar = null;

    [Header("======================EventMap======================")]
    public List<GameObject> m_EventMaps = new List<GameObject>();
    public GameObject m_CurrMap = null;

    public enum EEventState
    {
        BEGIN_EVENT = 0,

        READ_SCRIPT,

        END_EVENT,
        REWARD,
    }
    public EEventState m_CurrState = EEventState.BEGIN_EVENT;
    Coroutine coState = null;

    private void Awake()
    {
        CGameManager.Instance.m_EventManager = this;
    }

    private void Start()
    {
        foreach (var it in m_EventMaps)
            if (it.gameObject.activeSelf == true) it.gameObject.SetActive(false);
        m_CurrMap = m_EventMaps[Random.Range(0, m_EventMaps.Count)];
        m_CurrMap.SetActive(true);

        LoadEvent();
        BeginEvent();

        m_PlayerChar.m_UI_ManaArea.m_Btn.interactable = false;
        m_PlayerChar.m_UI_ManaUse.m_Btn.interactable = false;

        CGameManager.Instance.m_ScheduleMgr.m_UI_Shop.SetRepair();
    }

    public void BeginEvent()
    {
        m_CurrState = EEventState.BEGIN_EVENT;
        coState = StartCoroutine(CoBeginEvent());

        CGameManager.Instance.m_DiceManager.DiceFirstRoll();
        //CGameManager.Instance.m_DiceManager.EnableButton_DiceRoll();
        //CGameManager.Instance.m_DiceManager.m_Btn_DiceRoll.interactable = true;
    }

    IEnumerator CoBeginEvent()
    {
        yield return CUtility.GetSecD1To5s(0.5f);

        //ĳ���� �̵�
        var walker = m_PlayerChar.m_Walker;
        var walkDuration = Mathf.Clamp(Mathf.Floor(walker.MoveTo()), 1, 100);

        //���� ���� ���̰� �ϱ�

        //�̵� ������
        yield return CUtility.GetSecD1To5s(walkDuration);

        //ī�޶� ��ŷ
        m_GenCamera.gameObject.SetActive(false);
        m_EventCamera.gameObject.SetActive(true);
        yield return CUtility.GetSecD1To5s(0.5f);

        //�̺�Ʈ ����
        yield return CUtility.GetSecD1To5s(0.5f);
        ShowLog_ToTarget();
    }



    //�Լ� �����͸� ����� �ش� �̺�Ʈ�� ����. �̺�Ʈ�� ID�� �з���.
    public System.Action<CUtility.CEventLog> m_CB_SeletedAnswer;
    public void LoadEvent() 
    {
        //System.Action<int> temp = (int idx) 
        var map = CGameManager.Instance.m_ScheduleMgr.m_CurrMap;

        if (map.m_Data.m_Connection_ID != 0)
            m_EventShower.m_NPCs = m_EventShower.m_EventUnits[map.m_Data.m_Connection_ID].ActiveEvent();

        else if (map.m_Data.m_Type == CUtility.CSceneInfo.ESceneType.SHOP)
            m_EventShower.m_NPCs = m_EventShower.m_EventUnits[12007].ActiveEvent();

        //TODO : ���� �̺�Ʈ �޾Ƽ� ���� ��Ű�� �ӽ÷� 12002�� �̺�Ʈ ����
        else if (map.m_Data.m_Type == CUtility.CSceneInfo.ESceneType.EVENT) 
        {
            var currChapyer = CGameManager.Instance.m_ScheduleMgr.m_CurrChapter;
            var idx = CGameManager.Instance.m_Dictionary.GetRandomEvent_By_Chapter(currChapyer);
            m_EventShower.m_NPCs = m_EventShower.m_EventUnits[idx].ActiveEvent();
            m_CurrEvent = CGameManager.Instance.m_Dictionary.m_AllEvents[idx];
        }

        if (map.m_Data.m_Connection_ID > 0)
        {
            m_CurrEvent = CGameManager.Instance.m_Dictionary
                .m_AllEvents[map.m_Data.m_Connection_ID];
        }

        m_CB_SeletedAnswer = (CUtility.CEventLog log)
            =>
        {
            m_CardGroup.OnCardSubmited();
            m_EventShower.SendMessage(
                string.Format("Event_{0}", m_CurrEvent.m_ID), log);
        };
    }

    //On Showed
    //��ũ��Ʈ �б� ����
    public void EndOfCurrLog() 
    {
        Debug.Log("END OF SHOW!");

        //m_SystemTypewriter.StopShowingText();
        //m_TMPA_TotalLogs.ResetState();
        //m_TMPA_TotalLogs.SetVisibilityEntireText(false, true);


        var connection = m_CurrEvent.m_Logs[m_LogIdx].m_Connection;
        if (connection.Count == 0) { StartCoroutine(CoShowNextLog()); return; }
        
        //
        if (connection[0] == -1)
        {
            Debug.Log("�̺�Ʈ ����");
            EndEvent();
            return;
        }
        //��� ī�� ����
        else if(m_CurrEvent.m_Logs[connection[0]].m_Speaker == "Answer")
        {
            //ī�������Ŵ
            foreach (var it in connection)
            {
                //Debug.Log(it);
                //Debug.Log(m_CurrEvent.m_Logs.Count);
                Debug.Log("��� ����");
                m_CardGroup.AddAnswer(m_CurrEvent.m_Logs[it]);
            }
        }
        //���� �������� �Ѿ
        else StartCoroutine(CoShowNextLog());
    }

    ///������ �α׷� �̵���Ŵ
    public void JumpToLog(int _idx) 
    {
        Debug.Log(_idx);
        m_LogIdx = _idx;
        ShowLog_ToTarget();
    }

    IEnumerator CoShowNextLog() 
    {
        yield return CUtility.GetSecD1To5s(1f);
        m_LogIdx++;
        ShowLog_ToTarget();
    }

    //��󿡰� ��� �ο���
    public void ShowLog_ToTarget() 
    {
        var log = m_CurrEvent.m_Logs[m_LogIdx];
        string text = m_CurrEvent.m_Logs[m_LogIdx].m_Log;

        switch (log.m_Speaker) 
        {
            case "System":
                m_TMPA_SystemLogs.SetText(text);
                m_SystemTypewriter.StartShowingText(true);
                text = string.Format("{0}\n", text);
                break;
            case "NPC01":
                Debug.Log("NPC01 desc");
                m_EventShower.m_NPCs[0].SetMessage(text);
                text = string.Format("NPC01 : {0}\n", text);
                break;
            case "NPC02":
                Debug.Log("NPC02 desc");
                m_EventShower.m_NPCs[1].SetMessage(text);
                text = string.Format("NPC02 : {0}\n", text);
                break;

            default: break;
        }

        //m_TMPA_TotalLogs.AppendText(text);
        m_TMPA_TotalLogs.SetText(
            m_TMPA_TotalLogs.textFull + text);
    }


    public void EndEvent()
    {
        m_CurrState = EEventState.END_EVENT;
        StopAllCoroutines();
        coState = StartCoroutine(CoEndEvent());
    }

    IEnumerator CoEndEvent()
    {
        yield return CUtility.GetSecD1To5s(1f);
        CGameManager.Instance.m_ScheduleMgr.SetMode(CScheduleManager.EModeScheduler.REWARD);
    }


    public void OnClick_SkipScriptSpawn()
    {
        m_EventShower.SkipNPC_Scripts();
        m_SystemTypewriter.SkipTypewriter();
    }

    public void OnClick_ToggleTotalLog()
    {
        m_TotalLogs.SetActive(!m_TotalLogs.activeSelf);
    }

    public void OnClick_OpenShop() 
    {
        CGameManager.Instance.m_ScheduleMgr.m_UI_Shop.gameObject.SetActive(true);
    }
}
