using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Skill_Manager : MonoBehaviour
{
    public CPlayerChar m_Player = null;
    public CDiceMananger m_DiceMgr = null;
    public CEnemyGroup m_EnemyGroup = null;
    public CanvasGroup m_CanvasCards = null;

    [Header("===================UIs===================")]
    public List<CUI_SkillCard> m_SkillCards = new List<CUI_SkillCard>();

    //public CSelectable_TargetEnemy  m_EnemySelectArea = null;
    public TMPro.TextMeshProUGUI m_TMP_Desc = null;
    
    public Transform m_Skill_Indicator_Area = null;

    public void Start()
    {
        CGameManager.Instance.m_DiceManager.CB_SavedDice = CheckCanUseCards;
    }

    public void ToggleCanvasCard(bool _toggle) 
    { 
        m_CanvasCards.interactable = _toggle; 
    }

    public void CancelUseCardSkill()
    {
        foreach (var it in m_SkillCards)
        {
            //if (it.m_IsUseing == true) 
            if (it.m_CardState == CUI_SkillCard.EState.USING)
            { m_EnemyGroup.Escape(); }
            //if (it.m_IsCharged == true)
            if (it.m_CardState == CUI_SkillCard.EState.FOCUS_CHARGED)
            { it.CancelUse(); }
        }
            
    }

    public void CheckCanUseCards() 
    {
        foreach (var it in m_SkillCards)
            it.CheckCanCharge();
    }

    public void DestinySkill_Disable() 
    {
        foreach (var it in m_SkillCards)
            it.m_IsCanUse = false;
    }
    
}
