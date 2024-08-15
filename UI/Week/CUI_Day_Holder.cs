using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Day_Holder : MonoBehaviour
{
    public CanvasGroup m_CanvasGroup = null;
    public CUI_Event_Day m_UI_EventDay = null;

    [Header("===========================")]
    public bool m_IsDone = false;
    public bool m_IsBuyable = false;

    [Header("===========================")]
    public GameObject m_Cover = null;

    [Header("===========================")]
    public UnityEngine.UI.Button m_Btn_BuyOrSell = null;
    public TMPro.TextMeshProUGUI m_TMP_BtnText = null;

    [Header("===========================")]
    public CUI_Edit_Week m_EditWeek = null;


    public void Toggle_Canvas(bool _toggle, /*float _alpha = 1f,*/ bool _coverOn = false)
    {
        //m_CanvasGroup.interactable = _toggle;
        //m_CanvasGroup.blocksRaycasts = _toggle;
        //m_CanvasGroup.alpha = _alpha;

        if (m_UI_EventDay != null)
        {
            m_UI_EventDay.m_Frame.raycastTarget = _toggle;
            m_Cover.transform.SetAsLastSibling();
            m_Cover.SetActive(_coverOn);
        }
    }

    public void Spawn(CUI_Edit_Week _editWeek, bool _isBuyable = false, CUI_Event_Day _eventDay = null) 
    {
        m_EditWeek = _editWeek;
        m_UI_EventDay = _eventDay;
        m_IsBuyable = _isBuyable;
        SetBtn(_isBuyable);
    }

    public void EventDone() 
    {
        m_IsDone = true;
        
        m_Cover.SetActive(true);
    }

    public void SetBtn(bool _isBuyable = false)
    {
        m_Btn_BuyOrSell.gameObject.SetActive(m_IsBuyable);
        if (m_IsBuyable == false) return;
        if (m_UI_EventDay == null) Display_Buy();
        else Display_Sell();
    }

    public void Display_Sell() 
    {
        m_TMP_BtnText.text = string.Format("SELL {0}", m_UI_EventDay.m_SceneInfo.m_Data.m_ResellCost);
    }
    public void Display_Buy() 
    {
        //m_TMP_BtnText.text = string.Format("BUY {0}", m_EditWeek.m_DayBuyCost);
        m_TMP_BtnText.text = "";
    }

    public void OnClick_BuyOrSell() 
    {
        if (m_UI_EventDay == null)
        { 
            //m_EditWeek.BuyDay(this);
            //Display_Sell();
        }
        else 
        { 
            m_EditWeek.m_UserBag.SellDay(this);
            Display_Buy();
        }
    }

    public void OnClick_SwapToWeek() 
    {
        m_Btn_BuyOrSell.interactable = m_UI_EventDay.m_IsInQueue == false;
    }

}
