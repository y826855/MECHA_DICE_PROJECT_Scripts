using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CManaSkillManager : MonoBehaviour
{
    public CDiceMananger m_DiceMgr = null;
    //public CSelectableArea_New m_ChoiceArea = null;

    [Header("=======================================")]
    public CDice m_SelectedDice = null;
    public CManaHandler m_ManaHandler = null;
    public CUI_ManaSkill m_CurrManaSkill = null;
    public GameObject m_Particle_Pick_ManaSKillArea = null;

    public void Start_PickDice(CUI_ManaSkill _manaSkill) 
    {
        if (m_DiceMgr.m_State != CDiceMananger.EDiceRollState.ROLL_FIRST
            && m_DiceMgr.m_State != CDiceMananger.EDiceRollState.ROLL_STOP_ACTION_DONE) return;

        CGameManager.Instance.m_Input.SetEscape(OnEscape);

        m_DiceMgr.beforeState = m_DiceMgr.m_State;
        m_DiceMgr.m_State = CDiceMananger.EDiceRollState.MANA_SKILL_SELECT;

        m_SelectedDice = null;
        m_CurrManaSkill = _manaSkill;

        m_Particle_Pick_ManaSKillArea.SetActive(true);
        //m_ChoiceArea.m_IsCanEscape = true;
        //m_ChoiceArea.Open_And_FocusIn();
    }
    public void End_PickDice() 
    {
        m_SelectedDice.GiveManaSkill(m_CurrManaSkill.m_Skill);

        //TODO : USE MANA
        m_ManaHandler.UseMana(m_CurrManaSkill.m_Skill.m_Data.m_Cost);

        m_DiceMgr.m_State = m_DiceMgr.beforeState;
        m_CurrManaSkill = null;
        m_Particle_Pick_ManaSKillArea.SetActive(false);
    }

    public void OnEscape() 
    {
        //m_ChoiceArea.m_IsCanEscape = false;
        //m_ChoiceArea.Force_FocusOut();
        //m_DiceMgr.m_State = CDiceMananger.EDiceRollState.ROLL_STOP_ACTION_DONE;
        m_Particle_Pick_ManaSKillArea.SetActive(false);
        m_DiceMgr.m_State = m_DiceMgr.beforeState;
        m_CurrManaSkill = null;
    }
}
