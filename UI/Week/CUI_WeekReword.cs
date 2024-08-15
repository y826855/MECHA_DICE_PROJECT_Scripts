using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_WeekReword : MonoBehaviour
{
    [System.Serializable]
    public enum ERewrdType { NONE= -1, DECK_MAX_UP,TEST01, TEST02 }

    public List<CUI_Info_ManaSkill> m_ManaSkill_Found = new List<CUI_Info_ManaSkill>();
    public CUI_Info_ManaSkill m_ShowCurrEquip = null;
    public RectTransform m_Img_Frame = null;

    public CUI_Info_ManaSkill m_CurrSelection = null;
    public Button m_Btn_Change = null;

    public void OnEnable()
    {
        m_Btn_Change.interactable = true;
        SetRewards();
    }

    //보상 설정
    public void SetRewards() 
    {
        var player = CGameManager.Instance.m_PlayerData;
        var dic = CGameManager.Instance.m_Dictionary;

        OnClick_Skill(m_ManaSkill_Found[0]);

        foreach (var it in m_ManaSkill_Found) 
        {
            uint idx = player.GetRandom_ManaSkill();
            if (dic.m_Quests.ContainsKey(idx) == true) 
            { it.SetData(dic.m_Quests[idx]); }
            else if (dic.m_ManaSkills_Area.ContainsKey(idx)) 
            { it.SetData(dic.m_ManaSkills_Area[idx]); }
            else if (dic.m_ManaSkills_Use.ContainsKey(idx)) 
            { it.SetData(dic.m_ManaSkills_Use[idx]); }
        }
    }

    public void OnClick_Skill(CUI_Info_ManaSkill _info) 
    {
        var player = CGameManager.Instance.m_PlayerData;
        m_CurrSelection = _info;
        switch (_info.m_Type) 
        {
            case CUtility.EManaSkill_Kind.AREA:
                m_ShowCurrEquip.SetData(player.m_AreaSkill); break;
            case CUtility.EManaSkill_Kind.USE:
                m_ShowCurrEquip.SetData(player.m_UseSkill); break;
            case CUtility.EManaSkill_Kind.QUEST:
                m_ShowCurrEquip.SetData(player.m_Quest); break;
        }

    }

    public void ShowCurrEquip() 
    {
        
    }

    //스킵 버튼 누름
    public void OnClick_Skip() 
    {
        this.gameObject.SetActive(false);
        CGameManager.Instance.m_ScheduleMgr.MoveToNextWeek();
    }

    //스킬 변경 함
    public void OnClick_ChangeSkill() 
    {

        var shop = CGameManager.Instance.m_ScheduleMgr.m_UI_Shop;

        switch (m_ShowCurrEquip.m_Type)
        {//상점에도 바뀐 스킬 적용
            case CUtility.EManaSkill_Kind.AREA:
                shop.ChangeArea(m_CurrSelection.m_ID); break;
            case CUtility.EManaSkill_Kind.USE:
                shop.ChangeUse(m_CurrSelection.m_ID); break;
            case CUtility.EManaSkill_Kind.QUEST:
                shop.ChangeQuest(m_CurrSelection.m_ID); ; break;
        }

        m_Btn_Change.interactable = false;
        //this.gameObject.SetActive(false);
        //OnClick_Skip();

        //this.gameObject.SetActive(false);
        //CGameManager.Instance.m_ScheduleMgr.MoveToNextWeek();
        //TODO :변경 해야하면 선택된 놈이랑 바꾸어ㅑ함
        //CGameManager.Instance.m_ScheduleMgr.m_UI_Schedule.AfterReward();
    }
}
