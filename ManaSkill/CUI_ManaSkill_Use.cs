using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_ManaSkill_Use : MonoBehaviour
{
    public CManaSkillManager m_ManaSkillMgr = null;
    public CScriptable_ManaSkill m_Skill = null;
    [Header("===================")]
    public Button m_Btn = null;
    [SerializeField] TMPro.TextMeshProUGUI m_TMP_Cost = null;
    [SerializeField] Image m_Img = null;

    private void Start()
    {
        CGameManager.Instance.m_DiceManager.m_CB_ChangeState += DiceStateChange;
        CheckCanUse();
    }

    public void Spawn(CScriptable_ManaSkill _skill)
    {
        m_Skill = _skill;
        if (CGameManager.Instance.m_TurnManager != null)
        {
            m_Skill.m_AreaATK = CGameManager.Instance.m_TurnManager.m_Area_ATK_ALL;
            m_Skill.m_Area_RandomATK = CGameManager.Instance.m_TurnManager.m_Area_RandomATK;
        }

        m_Img.sprite = _skill.m_Data.m_Icon;
        m_TMP_Cost.text = _skill.m_Data.m_Cost.ToString();

    }

    //사용
    public void OnClick_Use() 
    {
        CGameManager.Instance.m_TurnManager.m_PlayerChar.m_SkillMgr.CancelUseCardSkill();
        m_ManaSkillMgr.m_ManaHandler.UseMana(m_Skill.m_Data.m_Cost);

        switch (m_Skill.m_Data.m_Type) 
        {
            case CUtility.EManaSkillType.DICE_ADD:
                Skill_DiceAdd(); break;

            default:
                Debug.Log("OTHERS");
                m_Skill.DoAction(); break;
        }

        CheckCanUse();
    }

    public void Skill_DiceAdd() 
    {
        var diceMgr = CGameManager.Instance.m_DiceManager;
        var dices = diceMgr.m_DiceChoiceArea.m_Dices;

        foreach (var it in dices) 
        {
            var eye = it.m_eye + 1;
            if (eye > 6) eye = 1;
            it.ChangeDiceRot_By_Eye(eye);
        }
    }
    //public void 


    bool m_DiceState_CanUse = false;
    //주사위 상태 변경
    public void DiceStateChange(CDiceMananger.EDiceRollState _state)
    {
        m_DiceState_CanUse = false;
        switch (_state)
        {
            case CDiceMananger.EDiceRollState.ROLL_FIRST:
                //주사위 눈 추가만 아니면 바로 시전 가능
                if(m_Skill.m_Data.m_Type != CUtility.EManaSkillType.DICE_ADD)
                    m_DiceState_CanUse = true; 
                break;
            case CDiceMananger.EDiceRollState.ROLL_STOP_ACTION_DONE:
                m_DiceState_CanUse = true; break;
        }
        CheckCanUse();
    }

    //마나 량 체크
    public bool CanUse_Mana()
    { return m_ManaSkillMgr.m_ManaHandler.m_CurrMana >= m_Skill.m_Data.m_Cost; }
    //{ return true; }

    //사용 가능 여부 체크
    public void CheckCanUse()
    {
        m_Btn.interactable = CanUse_Mana() == true && m_DiceState_CanUse == true;
    }

}
