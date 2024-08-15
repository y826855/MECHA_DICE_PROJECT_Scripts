using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



//주사위에 적용시키는 범위스킬

public class CUI_ManaSkill : CUI_Showable
{
    public CManaSkillManager m_ManaSkillMgr = null;
    public CScriptable_ManaSkill_Area m_Skill = null;

    [Header("===================")]
    public TMPro.TextMeshProUGUI m_TMP_Cost = null;
    public TMPro.TextMeshProUGUI m_TMP_Sum = null;
    public Image m_Img = null;
    public Button m_Btn = null;

    bool m_DiceState_CanUse = false;

    private void Start()
    {
        CGameManager.Instance.m_DiceManager.m_CB_ChangeState += DiceStateChange;
        CheckCanUse();
    }

    public void Spawn(CScriptable_ManaSkill_Area _skill) 
    {
        m_Skill = _skill;

        if (CGameManager.Instance.m_TurnManager != null)
        {
            m_Skill.m_AreaATK = CGameManager.Instance.m_TurnManager.m_Area_ATK_ALL;
            m_Skill.m_Area_RandomATK = CGameManager.Instance.m_TurnManager.m_Area_RandomATK;
        }

        m_Img.sprite = _skill.m_Data.m_Icon;
        m_TMP_Cost.text = _skill.m_Data.m_Cost.ToString();
        //m_Btn.onClick.AddListener(OnClick_Use);
    }

    public void CheckCanUse(int _mana) 
    {
        m_Btn.interactable = _mana >= m_Skill.m_Data.m_Cost;
    }

    //마나 량 체크
    public bool CanUse_Mana() 
    { return m_ManaSkillMgr.m_ManaHandler.m_CurrMana >= m_Skill.m_Data.m_Cost; }


    //사용 가능 여부 체크
    public void CheckCanUse()
    {
        m_Btn.interactable = CanUse_Mana() == true && m_DiceState_CanUse == true;
    }

    //주사위 상태 변경
    public void DiceStateChange(CDiceMananger.EDiceRollState _state) 
    {
        m_DiceState_CanUse = false;
        switch (_state) 
        {
            case CDiceMananger.EDiceRollState.ROLL_FIRST:
                m_DiceState_CanUse = true; break;
            case CDiceMananger.EDiceRollState.ROLL_STOP_ACTION_DONE:
                m_DiceState_CanUse = true; break;
        }
        CheckCanUse();
    }


    public void OnClick_Use() 
    {
        m_ManaSkillMgr.Start_PickDice(this);
        CGameManager.Instance.m_TurnManager.m_PlayerChar.m_SkillMgr.CancelUseCardSkill();
    }
}


//플레이어 공격중 x
//적 선택중 선택 단계 취소시키고 진행
//주사위 굴리는중에 사용 x
