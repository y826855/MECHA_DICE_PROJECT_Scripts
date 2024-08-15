using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;

[System.Serializable]
public class CWeek 
{
    public List<CScriptable_SceneInfo> m_Week = new List<CScriptable_SceneInfo>();
}

public class CScheduleManager : MonoBehaviour
{
    //public List<CScriptable_SceneInfo> m_Week = new List<CScriptable_SceneInfo>();
    //public List<CUI_Day_Holder> m_DayHolders = new List<CUI_Day_Holder>();


    public int m_CurrWeek = 0;
    public int m_CurrDay = -1;
    public int m_CurrChapter = 0;
    [SerializeField] int m_Debug_DaySkipAdd = 0;
    public CScriptable_SceneInfo m_CurrMap = null;

    public List<int> m_Chapter_Interval = new List<int>();

    [Header("============================================")]
    public List<CWeek> m_Stage01 = new List<CWeek>();

    public enum EModeScheduler { NONE = 0, EDIT_WEEK, EDIT_SCHEDULE, EXECUTE, DONE, REWARD, END_WEEK }
    public EModeScheduler m_CurrMode = EModeScheduler.NONE;

    [SerializeField] GameObject m_UI_Canvas = null;
    public CBlockVision m_VisionCtrl = null;

    [Header("=========SHORTCUTS===========")]
    public CUI_Schedule m_UI_Schedule = null;
    public CUI_Edit_Week m_UI_EditWeek = null;
    //public CUI_Deck_Canvas m_UI_Deck = null;
    public CUI_Deck_Shower m_UI_Deck = null;
    public CUI_WeekQueue m_WeekOnEdit = null;
    public CUI_ToolBar m_UI_ToolBar = null;
    public CUI_Shop m_UI_Shop = null;

    [Header("=========DisplayGroup===========")]
    public CUI_Scene_Reword m_UI_Reword = null;
    public GameObject m_UI_EndOfGame = null;
    public GameObject m_UI_PlayerDie = null;
    public GameObject m_UI_Player_Toolbar = null;

    private void Awake()
    {
        if (CGameManager.Instance.m_ScheduleMgr != null)
        {
            Destroy(this.gameObject);
            return;
        }

        CGameManager.Instance.m_ScheduleMgr = this;
        DontDestroyOnLoad(this);
        if (m_UI_Canvas != null) DontDestroyOnLoad(m_UI_Canvas);
        if (m_VisionCtrl != null) DontDestroyOnLoad(m_VisionCtrl);
    }

    public List<CScriptable_SceneInfo> GetCurrentWeek() 
    { 
        return m_Stage01[m_CurrWeek].m_Week; 
    }

    //스캐줄 변경
    public void ChangeSchedule() 
    {
        if(m_UI_Schedule != null) m_UI_Schedule.UpdateSchedule();
        //m_UI_ToolBar.m_IsHold_Window = false;
        m_UI_ToolBar.Escape();

        SetMode(CScheduleManager.EModeScheduler.EXECUTE);
        MoveToNextDay();
    }

    public void SetMode(EModeScheduler _mode) 
    {
        switch (_mode) 
        {
            case EModeScheduler.EDIT_SCHEDULE:
                m_VisionCtrl.Uncover_Hard();
                //스테이지 창 홀드, 스테이지창 주차 선택 활성화
                m_UI_ToolBar.OpenWindowByIndex((int)CUI_ToolBar.EIndexToolbar.SCHEDULER, true);
                m_UI_Schedule.Phase_EditWeek();
                break;
            case EModeScheduler.EDIT_WEEK:
                //주차 편집창 홀드, 
                m_VisionCtrl.Uncover_Hard();
                m_UI_EditWeek.OnEditMode();
                m_UI_ToolBar.OpenWindowByIndex((int)CUI_ToolBar.EIndexToolbar.SCHEDULE_EDIT, true);
                break;
            case EModeScheduler.EXECUTE:
                //m_VisionCtrl.Uncover();
                //주차 진행
                break;
            case EModeScheduler.DONE:
                //주차 진행 완료
                Debug.Log("주차 완료");
                m_UI_Schedule.m_Weeks[m_CurrWeek].m_weekReword.interactable = true;
                m_UI_ToolBar.OpenWindowByIndex((int)CUI_ToolBar.EIndexToolbar.SCHEDULER, true);
                //m_UI_Schedule.Phase_EditWeek();
                //m_UI_EditWeek.OnEditMode();
                //m_UI_ToolBar.OpenWindowByIndex((int)CUI_ToolBar.EIndexToolbar.SCHEDULER, true);
                break;
            case EModeScheduler.REWARD:
                //보상 선택중
                m_UI_ToolBar.Escape();
                //현재 맵을 기준으로 보상 줘야함
                m_UI_Reword.m_Discovery += CGameManager.Instance.m_PlayerData.m_Discovery
                    + m_CurrDay + m_CurrWeek * 10;
                m_UI_Reword.SetInfo();
                m_UI_Reword.gameObject.SetActive(true);
                break;

            //주차 보상 열기
            case EModeScheduler.END_WEEK:
                //m_UI_ToolBar.Escape();
                break;
        }
        m_CurrMode = _mode;
    }

    //다음 일정으로
    public void MoveToNextDay() 
    {
        m_CurrDay++;
        m_CurrDay += m_Debug_DaySkipAdd;

        m_UI_Schedule.m_Weeks[m_CurrWeek].MovePointerToday(m_CurrDay);
        bool isEndOfWeek = m_WeekOnEdit.MovePointerToday(m_CurrDay);

        //일정 단순 가시화
        int showDay = m_CurrDay + 1;
        if (showDay > m_WeekOnEdit.m_DayHolders.Count) showDay = m_WeekOnEdit.m_DayHolders.Count;
        m_UI_ToolBar.m_TMP_Schedule.text =
            string.Format("{0}/{1}", showDay, m_WeekOnEdit.m_DayHolders.Count);


        if (isEndOfWeek == true)
        {
            m_VisionCtrl.Cover(_soft: true, () =>
            { SetMode(EModeScheduler.DONE); });
        }

        else 
        {
            m_VisionCtrl.Cover(_soft: false, () =>
            {
                m_CurrMap = GetCurrentWeek()[m_CurrDay];
                m_UI_Reword.m_SceneInfo = m_CurrMap;
                OpenSceneByType();
            });
        }
    }

    public void OpenSceneByType() 
    {
        //TODO 특별한 씬 관리 여기서 해야함
        m_UI_Player_Toolbar.SetActive(true);
        CGameManager.Instance.m_Input.ClearEscapeStack();

        if (m_CurrMap.m_Data.m_Connection_Scene != 0)
        {
            SceneOpenByIdx((int)m_CurrMap.m_Data.m_Connection_Scene);
            Debug.Log("CUSTOM SCENE");
            return;
        }

        switch (m_CurrMap.m_Data.m_Type)
        {
            case CUtility.CSceneInfo.ESceneType.BATTLE_LOW:
            case CUtility.CSceneInfo.ESceneType.BATTLE_MID:
            case CUtility.CSceneInfo.ESceneType.BATTLE_HIGH:
            case CUtility.CSceneInfo.ESceneType.BATTLE_ELITE:
            case CUtility.CSceneInfo.ESceneType.BATTLE_BOSS:
                //TODO : 전투 자체에서 처리하는게 더 좋을거 같음
                m_UI_Reword.m_CurrReward.Add(CUtility.ERewardable.CARD, 0);
                m_UI_Reword.m_CurrReward.Add(CUtility.ERewardable.DISK, 0);
                SceneOpenByIdx(1);
                break;

            case CUtility.CSceneInfo.ESceneType.EVENT:
                SceneOpenByIdx(2);
                break;

            case CUtility.CSceneInfo.ESceneType.SHOP:
                SceneOpenByIdx(2);
                break;
        }
    }

    public void SceneOpenByIdx(int _idx) 
    {
        Debug.Log("CHANGE SCENE");
        var idx = m_Chapter_Interval[m_CurrChapter] + _idx;
        SceneManager.LoadSceneAsync(idx, LoadSceneMode.Single).completed += OnSceneLoaded;
    }

    void OnSceneLoaded(AsyncOperation asyncOperation)
    {
        Debug.Log("DONE CHANGE");
        m_VisionCtrl.Uncover();
    }

    IEnumerator CoOpenDay() 
    {
        yield return null;
        SceneManager.LoadSceneAsync("Scene_Ch1_Battle", LoadSceneMode.Single);
        if (m_UI_Canvas.activeSelf == false) m_UI_Canvas.gameObject.SetActive(true);
    }

    public void MoveToNextWeek() 
    {
        m_CurrDay = -1;
        m_CurrWeek++;

        if (m_CurrWeek >= 4) 
        {
            m_UI_EndOfGame.SetActive(true);
            return;
        }

        SetMode(CScheduleManager.EModeScheduler.EDIT_SCHEDULE);
    }


    public void OnClick_Start() 
    {
        CGameManager.Instance.m_DEBUG_GAMEMODE = false;

        m_UI_Deck.CreateDeck();
        m_UI_Schedule.Set();
        m_UI_Shop.Init();
        m_UI_Reword.ResetData();

        var player = CGameManager.Instance.m_PlayerData;
        //player.m_MaxHP = 80;
        //player.AddGold(0);
        //player.InitUserData();
        //player.AddHP(0);

        player.AddGold(0);
        player.AddHP(0);

        m_VisionCtrl.Cover(_soft: true, 
            () => {
                Debug.Log("ONCLICK START");
                m_UI_Canvas.gameObject.SetActive(true);
                SetMode(CScheduleManager.EModeScheduler.EDIT_SCHEDULE);
            });
    }

    public void GoTo_Lobby() 
    {
        Destroy(m_UI_Canvas);
        Destroy(this.gameObject);

        m_UI_Player_Toolbar.SetActive(false);
        m_UI_EndOfGame.gameObject.SetActive(false);
        m_UI_PlayerDie.gameObject.SetActive(false);
        CGameManager.Instance.m_Input.ClearEscapeStack();
        SceneOpenByIdx(0);
    }
    public void QuitGame() 
    {
        Application.Quit();
    }
}
