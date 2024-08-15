using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO 지우고 스캐줄 매니저에 합류시키는게 좋을듯
public class CUI_ToolBar : MonoBehaviour
{
    public GameObject m_UI_Schedule_Shower = null;
    public GameObject m_UI_Tools = null;
    public GameObject m_UI_Settings = null;

    public enum EIndexToolbar { NONE = -1, SCHEDULER, SCHEDULE_EDIT, STATE_EFF_GRAPH }
    
    public EIndexToolbar m_CurrToolbar = EIndexToolbar.NONE;

    public List<GameObject> m_Tools = new List<GameObject>();
    //public List<GameObject> m_Activation = new List<GameObject>();
    public GameObject m_CurrUI = null;

    //public bool m_IsHold_Window = false;
    //public GameObject m_Btns_MoveWindow = null;

    [Header("===================")]
    public TMPro.TextMeshProUGUI m_TMP_GOLD = null;
    public TMPro.TextMeshProUGUI m_TMP_HP = null;
    public TMPro.TextMeshProUGUI m_TMP_Schedule = null;

    private void Start()
    {
        if(CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar == null)
            CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar = this;
    }

    public void Escape()
    {
        //if (m_IsHold_Window == true) return;
        //
        //foreach (var it in m_Activation)
        //    it.gameObject.SetActive(false);
        //m_Activation.Clear();
        //m_Btns_MoveWindow.SetActive(false);

        if(m_CurrUI != null) m_CurrUI.gameObject.SetActive(false);
    }

    public void OpenWindowByIndex(int _idx, bool _hold) 
    {
        //m_Btns_MoveWindow.SetActive(!_hold);
        ////if (m_IsHold_Window == true) return;
        //m_IsHold_Window = _hold;

        OpenUI(_idx);
    }

    public void OpenWindowByIndex(int _idx) 
    {
        //if (m_IsHold_Window == true) return;
        //m_Btns_MoveWindow.SetActive(true);

        OpenUI(_idx);
    }

    public void OpenUI(int _idx) 
    {
        if (_idx >= m_Tools.Count || m_Tools[_idx] == null) return;
        m_CurrToolbar = (EIndexToolbar)_idx;

        //foreach (var it in m_Activation)
        //{ it.gameObject.SetActive(false); }
        //m_Activation.Clear();
        //
        if (m_CurrUI != null) m_CurrUI.gameObject.SetActive(false);
        m_Tools[_idx].gameObject.SetActive(true);
        m_CurrUI = m_Tools[_idx];
        //m_Activation.Add(m_Tools[_idx]);
    }



    public void OnClick_MoveR() 
    {
        int idx = (int)m_CurrToolbar + 1;
        if (idx >= m_Tools.Count) idx = 0;
        OpenWindowByIndex(idx);
    }
    public void OnClick_MoveL() 
    {
        int idx = (int)m_CurrToolbar -1;
        if (idx < 0) idx = m_Tools.Count - 1;
        OpenWindowByIndex(idx);
    }

}
