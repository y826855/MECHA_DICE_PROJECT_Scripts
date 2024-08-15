using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_Quest : MonoBehaviour
{
    public CScriptable_Quest m_CurrQuest = null;
    public bool m_CanUse = false;
    public bool m_IsUsed = false;

    [Header("======FILL ON EDITOR======")]
    [SerializeField] Button m_Btn = null;
    [SerializeField] Image m_Img = null;
    [SerializeField] TMPro.TextMeshProUGUI m_TMP_Quest = null;
    [SerializeField] int QuestCount = 0;
    [SerializeField] ParticleSystem m_QuestClearParticle = null;
    public int m_QuestCount 
    {
        get { return QuestCount; }
        set 
        {
            if (m_CanUse == true || m_IsUsed == true) return;
            QuestCount = value; 
            if (QuestCount > m_CurrQuest.m_Data.m_Count) QuestCount = m_CurrQuest.m_Data.m_Count;
            m_TMP_Quest.text = string.Format("{0}/{1}", m_QuestCount, m_CurrQuest.m_Data.m_Count);
            CanUse(QuestCount >= m_CurrQuest.m_Data.m_Count);
        }
    }

    private void Start()
    {
        CGameManager.Instance.m_DiceManager.m_CB_ChangeState += DiceStateChange;
        CheckCanUse();
    }

    //초기화
    public void Spawn(CScriptable_Quest _quest)
    {
        m_CurrQuest = _quest;
        m_Img.sprite = _quest.m_Data.m_Icon;
        m_TMP_Quest.text = string.Format("{0}/{1}", m_QuestCount, m_CurrQuest.m_Data.m_Count);
        m_Btn.interactable = false;

        if (CGameManager.Instance.m_TurnManager != null)
        {
            m_CurrQuest.m_AreaATK = CGameManager.Instance.m_TurnManager.m_Area_ATK_ALL;
            m_CurrQuest.m_Area_RandomATK = CGameManager.Instance.m_TurnManager.m_Area_RandomATK;
        }
    }

    //퀘스트 클리어, 사용가능 체크
    public void CanUse(bool toggle) 
    {
        m_CanUse = toggle;
        if (m_CanUse == true) m_QuestClearParticle.Play();
        CheckCanUse();
    }

    //눌러서 퀘스트 사용
    public void OnClick_Use() 
    {
        m_IsUsed = true;
        var player = CGameManager.Instance.m_TurnManager.m_PlayerChar;
        player.m_SkillMgr.CancelUseCardSkill();
        int num = m_CurrQuest.m_Data.m_Reward_Num;

        //퀘스트 보상.. 어떻게 사용하게 할까
        switch (m_CurrQuest.m_Data.m_Reward_Type) 
        {
            case CUtility.EManaSkillType.DEF:
                player.m_Hitable.GainDef(num);
                break;
            case CUtility.EManaSkillType.DMG_RANDOM:
                m_CurrQuest.DoAction();
                //CGameManager.Instance.m_TurnManager.m_Area_RandomATK.Spawn(num);
                break;

        }
        CheckCanUse();
    }

    //주사위 사용 체크
    public void UseDice(int _eye)
    {
        Debug.Log("주사위 사용 체크");
        switch (m_CurrQuest.m_Data.m_Term)
        {
            case CUtility.CQuest.ETerm.DICE_LESS:
                if (m_CurrQuest.m_Data.m_Require >= _eye)
                {
                    Debug.Log(m_CurrQuest.m_Data.m_Require + " " + _eye);
                    m_QuestCount++; 
                }
                break;
            case CUtility.CQuest.ETerm.DICE_SAME:
                if (m_CurrQuest.m_Data.m_Require == _eye)
                { m_QuestCount++; }
                break;
        }
    }

    //데미지 주는거 체크
    public void GiveDmg(int _dmg)
    {
        switch (m_CurrQuest.m_Data.m_Term)
        {
            case CUtility.CQuest.ETerm.DMG:
                m_QuestCount += _dmg; break;
        }
    }

    //방어 올리는거 체크
    public void GainDef(int _def)
    {
        Debug.Log(_def);

        switch (m_CurrQuest.m_Data.m_Term)
        {
            case CUtility.CQuest.ETerm.DEF:
                m_QuestCount += _def; break;
        }
    }

    //퀘스트 초기화
    public void EndTurn()
    {
        if (m_CurrQuest.m_Data.m_ResetOnEndTurn == true)
            m_QuestCount = 0;
    }

    //마나 사용 체크
    public void UseMana(int _mana) 
    {
        if(m_CurrQuest.m_Data.m_Term == CUtility.CQuest.ETerm.MANA
            && _mana > 0)
            m_QuestCount += _mana; 
    }

    bool m_DiceState_CanUse = false;
    public void DiceStateChange(CDiceMananger.EDiceRollState _state)
    {
        m_DiceState_CanUse = false;
        switch (_state)
        {
            case CDiceMananger.EDiceRollState.ROLL_STOP_ACTION_DONE:
                m_DiceState_CanUse = true; break;
        }
        CheckCanUse();
    }

    public void CheckCanUse() 
    {
        m_Btn.interactable = 
            m_CanUse == true && //퀘스트 클리어?
            m_DiceState_CanUse == true && //사용 가능 상태?
            m_IsUsed == false; //이미 사용했나?

    }
}
