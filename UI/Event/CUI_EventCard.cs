using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Febucci.UI;

public class CUI_EventCard : MonoBehaviour
{
    public TextAnimator_TMP m_TMPA_Log = null;
    public CUtility.CEventLog m_CurrLog = null;
    public int m_LogIdx = 0;

    [Header("============================")]
    public CUI_CardAnim m_CardAnim = null;
    public List<Sprite> m_Icon_Dice = new List<Sprite>();

    [Header("============================")]
    public List<CUI_CardDiceAnim> m_Img_Dices = new List<CUI_CardDiceAnim>();
    public List<CDice> m_ReserveDices = new List<CDice>();

    public GameObject m_DiceGroup = null;
    public TMPro.TextMeshProUGUI m_TMP_DiceTerm = null;
    public int m_NeedDiceCount = 0;
    public int m_TermDiceNum = 0;
    //public bool m_Over = false;

    [Header("============================")]
    public GameObject m_GoldGroup = null;
    public TMPro.TextMeshProUGUI m_TMP_Gold = null;
    public int m_TermGold = 0;

    [Header("============================")]
    public GameObject m_HPGroup = null;
    public TMPro.TextMeshProUGUI m_TMP_HP = null;
    public int m_TermHP = 0;

    public enum EState
    {
        IDLE, FOCUS_NONE, NEED_CHARGE, CAN_USE
    };
    public EState m_CardState = EState.IDLE;

    [Header("============================")]
    public bool m_CanSelect = true;

    public void SetLog(CUtility.CEventLog _log) 
    {
        m_CanSelect = false;
        //m_CardState = EState.FOCUS_CHARGED;
        //m_IsCharged = true;

        m_TMPA_Log.SetText(_log.m_Log);
        m_LogIdx = _log.m_Idx;
        m_NeedDiceCount = 0;
        m_CurrLog = _log;

        bool IsOver = false;

        foreach (var it in _log.m_Term) 
        {
            switch (it.m_Need) 
            {
                case CUtility.EEventTerm.LESS1:
                    m_NeedDiceCount = 1; IsOver = false;
                    m_TermDiceNum = it.m_Count;
                    break;
                case CUtility.EEventTerm.LESS2:
                    m_NeedDiceCount = 2; IsOver = false;
                    m_TermDiceNum = it.m_Count;
                    break;
                case CUtility.EEventTerm.LESS3:
                    m_NeedDiceCount = 3; IsOver = false;
                    m_TermDiceNum = it.m_Count;
                    break;

                case CUtility.EEventTerm.OVER1:
                    m_NeedDiceCount = 1; IsOver = true;
                    m_TermDiceNum = it.m_Count;
                    break;
                case CUtility.EEventTerm.OVER2:
                    m_NeedDiceCount = 2; IsOver = true;
                    m_TermDiceNum = it.m_Count;
                    break;
                case CUtility.EEventTerm.OVER3:
                    m_NeedDiceCount = 3; IsOver = true;
                    m_TermDiceNum = it.m_Count;
                    break;


                case CUtility.EEventTerm.HP:
                    m_HPGroup.gameObject.SetActive(true);
                    m_TermHP = it.m_Count;
                    m_TMP_HP.text = string.Format("HP : {0}", m_TermHP);
                    break;
                case CUtility.EEventTerm.GOLD:
                    m_GoldGroup.gameObject.SetActive(true);
                    m_TermGold = it.m_Count;
                    m_TMP_Gold.text = string.Format("GOLD : {0}", m_TermGold);
                    break;
            }
        }

        if (m_NeedDiceCount > 0)
        {
            for (int i = 0; i < m_Img_Dices.Count; i++)
            {
                var it = m_Img_Dices[i];
                it.gameObject.SetActive(i < m_NeedDiceCount);
            }
            m_TMP_DiceTerm.text = string.Format("{0}{1}", IsOver ? "Over" : "Less", m_TermDiceNum);
            m_DiceGroup.gameObject.SetActive(true);
        }
        else m_DiceGroup.gameObject.SetActive(false);
        
        CheckCanUse();
    }

    public void CheckCanUse() 
    {
        var playerData = CGameManager.Instance.m_PlayerData;
        m_ReserveDices.Clear();
        m_CanSelect = true;

        foreach (var it in m_CurrLog.m_Term) 
        {
            switch (it.m_Need) 
            {
                case CUtility.EEventTerm.GOLD:
                    if (playerData.m_GOLD <= it.m_Count) { m_CanSelect = false; } break;
                case CUtility.EEventTerm.HP:
                    if (playerData.m_HP <= it.m_Count) { m_CanSelect = false; } break;
                case CUtility.EEventTerm.MAX_HP:
                    if (playerData.m_MaxHP <= it.m_Count) { m_CanSelect = false; } break;

                case CUtility.EEventTerm.OVER1:
                    if (CheckOver(1, it.m_Count) == false) { m_CanSelect = false; } break;
                case CUtility.EEventTerm.OVER2:            
                    if (CheckOver(2, it.m_Count) == false) { m_CanSelect = false; } break;
                case CUtility.EEventTerm.OVER3:            
                    if (CheckOver(3, it.m_Count) == false) { m_CanSelect = false; } break;

                case CUtility.EEventTerm.LESS1:
                    if (CheckLess(1, it.m_Count) == false) { m_CanSelect = false; } break;
                case CUtility.EEventTerm.LESS2:
                    if (CheckLess(2, it.m_Count) == false) { m_CanSelect = false; } break;
                case CUtility.EEventTerm.LESS3:
                    if (CheckLess(3, it.m_Count) == false) { m_CanSelect = false; } break;
            }
        }

    }

    public bool CheckOver(int _diceCount, int _num) 
    {
        m_CardState = EState.NEED_CHARGE;
        var dices = CGameManager.Instance.m_DiceManager.m_DiceSaveArea.m_Dices;

        int count = 0;
        int sum = 0;

        for (int i = 0; i < dices.Count; i++) 
        {
            var it = dices[i];
            sum += it.m_eye;

            m_ReserveDices.Add(it);
            count++;

            if (count == _diceCount)
            {
                if (sum >= _num) return true;
                else //가장 나중에 추가된 주사위 제거
                {
                    count--;
                    sum -= m_ReserveDices[0].m_eye;
                    m_ReserveDices.RemoveAt(0);
                }
            }
        }

        m_ReserveDices.Clear();
        return false;
    }
    public bool CheckLess(int _diceCount, int _num) 
    {
        m_CardState = EState.NEED_CHARGE;
        var dices = CGameManager.Instance.m_DiceManager.m_DiceSaveArea.m_Dices;

        int count = 0;
        int sum = 0;

        for (int i = 0; i < dices.Count; i++)
        {
            var it = dices[i];
            sum += it.m_eye;

            m_ReserveDices.Add(it);
            count++;

            if (count >= _diceCount)
            { 
                if (sum <= _num) return true;
                else
                {//가장 나중에 추가된 주사위 뺌
                    count--;
                    sum -= m_ReserveDices[0].m_eye;
                    m_ReserveDices.RemoveAt(0);
                }
            }
        }

        m_ReserveDices.Clear();
        return false;
    }

    //주사위 채우기
    public void OnInteraction() 
    {
        if (m_CanSelect == false || m_CardState != EState.NEED_CHARGE) return;
        m_CardState = EState.CAN_USE;

        for (int i = 0; i < m_ReserveDices.Count; i++) 
        {  ChargeDice(m_Img_Dices[i], m_ReserveDices[i].m_eye);  }
        CGameManager.Instance.m_DiceManager.m_DiceSaveArea.DiceUseReady(m_ReserveDices);
    }    

    public void OnButtonSelect()
    {
        Debug.Log("selected");
        CGameManager.Instance.m_Input.CB_OnInteraction = OnInteraction;
        CGameManager.Instance.m_Input.SetEscape(OnButtonDeselect);

        if (m_CardAnim != null) m_CardAnim.Anim_Ready();

        m_CardState = EState.FOCUS_NONE;

        CheckCanUse();
        ShowUseDice();
    }

    public void OnButtonDeselect()
    {
        if (CGameManager.Instance.m_Input.CB_OnInteraction == OnInteraction)
            CGameManager.Instance.m_Input.CB_OnInteraction = null;

        if (m_CardAnim != null) m_CardAnim.Anim_Idle();
        CancelUse();
        HideUseDice();

        //if (m_IsCharged == false) CancelUse();
    }

    public void OnSubmit()
    {
        Debug.Log("Button Submited!");

        //if (m_CanSelect == true && 
        //    (m_CardState == EState.NEED_CHARGE || m_CardState == EState.FOCUS_NONE))
        //{ Submited(); }

        if (m_CardState == EState.CAN_USE) Submited();

        //
        if (m_CanSelect == true && m_CardState == EState.FOCUS_NONE)
            m_CardState = EState.CAN_USE;

        //m_CardState = EState.FOCUS_NONE;
        //m_IsFocusing = true;
    }

    //지문 선택됨
    public void Submited() 
    {
        //주사위 사용
        foreach (var it in m_ReserveDices)
        { it.UseDice(); }
        m_ReserveDices.Clear();

        //선택한 대답 보냄
        CGameManager.Instance.m_EventManager.m_CB_SeletedAnswer(m_CurrLog);

        //보상 있으면 보상 추가
        foreach (var it in m_CurrLog.m_AddRewards)
        { CGameManager.Instance.m_ScheduleMgr.m_UI_Reword.m_CurrReward.Add(it.Key, it.Value); }


        var playerData = CGameManager.Instance.m_PlayerData;

        //골드, 체력, 최대체력 소모
        foreach (var it in m_CurrLog.m_Term)
        {
            switch (it.m_Need)
            {
                case CUtility.EEventTerm.GOLD:
                    playerData.AddGold(-it.m_Count);
                    break;
                case CUtility.EEventTerm.HP:
                    playerData.AddHP(it.m_Count);
                    break;
                case CUtility.EEventTerm.MAX_HP:
                    playerData.AddMaxHP(-it.m_Count);
                    break;
            }
        }

        CancelUse();
    }


    public void CancelUse() 
    {
        foreach (var it in m_Img_Dices)
        { DischargeDice(it); }

        if (m_CardAnim != null) m_CardAnim.Anim_Idle();

        //if (m_IsCharged == true && m_ReserveDices.Count > 0)
        //if (m_CardState == EState.NEED_CHARGE && m_ReserveDices.Count > 0)
        if (m_CardState == EState.CAN_USE && m_ReserveDices.Count > 0)
        {
            Dice_BackToSaveArea();
            CGameManager.Instance.m_DiceManager.m_DiceSaveArea.MoveDiceToPos();
            m_ReserveDices.Clear();
        }

        m_CanSelect = false;
        m_CardState = EState.IDLE;

        //선택된 놈 있으면 제거함
        //CGameManager.Instance.m_Input.m_EventSystem.SetSelectedGameObject(null);
        //m_IsCharged = false;
        //m_IsFocusing = false;

    }

    //주사위 돌려줌
    public void Dice_BackToSaveArea(int _countBack = 0)
    {
        int idx = 0;
        if (_countBack != 0) idx = m_ReserveDices.Count - _countBack;

        for (int i = idx; i < m_ReserveDices.Count; i++)
        {
            var it = m_ReserveDices[i];
            CGameManager.Instance.m_DiceManager.m_DiceSaveArea.GetBackDice(it);
        }
    }


    public void ChargeDice(CUI_CardDiceAnim _icon, int _eye)
    {
        _icon.SetIcon(m_Icon_Dice[_eye]);
        _icon.SetAlpha(1);
    }

    public void DischargeDice(CUI_CardDiceAnim _icon)
    {
        _icon.SetIcon(m_Icon_Dice[0], true);
        _icon.m_Image.color = CGameManager.Instance.color_Disable;
    }

    public void ShowUseDice()
    {
        if (m_ReserveDices.Count == 0 || m_CanSelect == false) return;
        foreach (var it in m_ReserveDices)
        { it.m_DiceUI_Img.color = Color.green; }
    }

    public void HideUseDice()
    {
        if (m_ReserveDices.Count == 0) return;
        foreach (var it in m_ReserveDices)
        { it.m_DiceUI_Img.color = Color.white; }
    }

    private void OnDisable()
    {
        CancelUse();
        m_DiceGroup.gameObject.SetActive(false);
        m_GoldGroup.gameObject.SetActive(false);
        m_HPGroup.gameObject.SetActive(false);
    }
}
