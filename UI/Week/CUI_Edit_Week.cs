using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Edit_Week : MonoBehaviour
{
    public GameObject m_ParentCanvas = null;

    public CUI_Day_Bag m_DefaultBag = null;
    public CUI_Day_Bag m_UserBag = null;
    public CUI_WeekQueue m_Week = null;

    [Header("================")]
    public bool m_CanEditable = false;
    [Header("================")]

    public CUI_Event_Day m_Pref_Day = null;
    public CUI_Day_Holder m_Pref_Holder = null;

    public List<CScriptable_SceneInfo> m_DaysDefault = new List<CScriptable_SceneInfo>();

    public int m_DayBuyCost = 10;

    [SerializeField] GameObject m_Btn_Submit = null;
    [Header("TEST")]
    [SerializeField] CScriptable_SceneInfo m_TestDay = null;

    public void Start()
    {
        m_DefaultBag.m_EditWeek = this;
        m_UserBag.m_EditWeek = this;
        //OnEditMode();
    }

    public void OnEnable()
    {
        SetUIs();
    }

    public void SetUIs() 
    {
        m_DefaultBag.ReadyToSet(m_Pref_Day, m_Pref_Holder, m_DaysDefault, false);
        m_UserBag.ReadyToSet(m_Pref_Day, m_Pref_Holder, CGameManager.Instance.m_PlayerData.m_DaysBag, true);

        m_Week.m_WeekQueue = CGameManager.Instance.m_ScheduleMgr.GetCurrentWeek();
        m_Week.ReadyToSet();
    }

    public void OnEditMode() 
    {
        SetUIs();
        SetEditMode(true);
    }

    public void SetEditMode(bool _toggle) 
    {
        m_CanEditable = _toggle;
        m_Btn_Submit.gameObject.SetActive(m_CanEditable);

        foreach (var it in m_UserBag.m_Holders)
        { it.Toggle_Canvas(m_CanEditable, !m_CanEditable); }

        foreach (var it in m_DefaultBag.m_Holders)
        { it.Toggle_Canvas(m_CanEditable, !m_CanEditable); }
    }

    public void AddToWeek(CUI_Event_Day _ui) 
    {
        if (m_Week.CheckIsFull() == true) return;

        _ui.m_IsInQueue = true;
        m_Week.AddDay(_ui);
    }

    public void ReturnToBag(CUI_Event_Day _ui) 
    {
        _ui.m_IsInQueue = false;
        m_Week.m_DayHolders[_ui.transform.parent.GetSiblingIndex()].m_UI_EventDay = null;
        _ui.ResetPos();

        //if (m_DefaultBag.m_DayHolders.Count + m_DefaultBag.m_StartIdx > _ui.m_ID)
        //{ m_DefaultBag.GetBack_Day(_ui); }
        //else
        //{ m_UserBag.GetBack_Day(_ui); }
    }

    public void BuyDay(CUI_Day_Holder _holder) 
    {
        var inst = Instantiate(m_Pref_Day, _holder.transform);
        inst.Spawn(m_TestDay, this, _holder);
        _holder.m_UI_EventDay = inst;
        _holder.Toggle_Canvas(m_CanEditable, !m_CanEditable);

        CGameManager.Instance.m_PlayerData.m_DaysBag.Add(m_TestDay);
    }

    public void AddDay() 
    {
        
    }


    public void Submit_Week() 
    {
        if (m_Week.CheckIsFull() == false) return;

        SetEditMode(false);
        //foreach (var it in m_UserBag.m_Holders) 
        //{ it.EventDone(); }
        //foreach (var it in m_DefaultBag.m_Holders)
        //{ it.EventDone(); }
        //m_DefaultBag.m_Holders
        m_Week.DecideWeek();

        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_ScheduleConfirm);
        CGameManager.Instance.m_ScheduleMgr.ChangeSchedule();
    }
}
