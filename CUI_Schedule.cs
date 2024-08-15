using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_Schedule : MonoBehaviour
{
    public CUI_WeekQueue m_Pref_Week = null;
    public List<CUI_WeekQueue> m_Weeks = new List<CUI_WeekQueue>();

    [SerializeField] Transform m_UI_Area = null;

    public int m_CurrWeek = 0;
    [SerializeField] CScheduleManager scheduleMgr = null;

    public GameObject m_Btn_EditWeek = null;
    public GameObject m_RewordUI = null;

    bool IsSetted = false;

    private void Start()
    {
        if (scheduleMgr == null) Set();
    }

    public void OnEnable()
    {
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_Schedule_intro);
    }

    public void Set() 
    {
        //if (scheduleMgr != null) return;
        if (IsSetted == true) return;
        IsSetted = true;

        scheduleMgr = CGameManager.Instance.m_ScheduleMgr;
        scheduleMgr.m_UI_Schedule = this;

        DisplayWeeks();
        scheduleMgr.SetMode(CScheduleManager.EModeScheduler.EDIT_SCHEDULE);
    }

    public void Phase_EditWeek() 
    {
        if (scheduleMgr == null) Set();

        //change btn
        var currWeek = m_Weeks[scheduleMgr.m_CurrWeek];
        m_Btn_EditWeek.gameObject.SetActive(true);
        m_Btn_EditWeek.transform.parent = currWeek.transform;
        m_Btn_EditWeek.transform.localPosition = Vector3.zero;
        m_Btn_EditWeek.transform.localScale = Vector3.one;
    }

    public void OnClick_EditWeek() 
    {
        scheduleMgr.SetMode(CScheduleManager.EModeScheduler.EDIT_WEEK);
        m_Btn_EditWeek.gameObject.SetActive(false);
    }


    public void DisplayWeeks() 
    {
        Debug.Log("DISPLAY");

        //TODO : 현재 스테이지 받아야함
        foreach (var it in scheduleMgr.m_Stage01) 
        {
            var inst = Instantiate(m_Pref_Week, m_UI_Area);
            inst.m_WeekQueue = it.m_Week;
            //inst.m_weekReword.onClick.AddListener();
            //inst.ReadyToSet(OpenWeekReword);
            inst.ReadyToSet(() => OpenWeekReward());
            m_Weeks.Add(inst);
        }
    }

    public void UpdateSchedule()
    {
        foreach (var it in m_Weeks) 
        {
            it.UpdateData();
        }
    }

    public void OpenWeekReward() 
    { 
        //주차 완료 보상 열기
        m_RewordUI.gameObject.SetActive(true);
        scheduleMgr.SetMode(CScheduleManager.EModeScheduler.END_WEEK);
    }


    //일간보상 이후 날짜 넘기기
    public void AfterReward() 
    {
        scheduleMgr.MoveToNextWeek();
    }
}
