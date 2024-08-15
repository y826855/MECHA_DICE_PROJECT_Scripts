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

    //�ʱ�ȭ
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

    //����Ʈ Ŭ����, ��밡�� üũ
    public void CanUse(bool toggle) 
    {
        m_CanUse = toggle;
        if (m_CanUse == true) m_QuestClearParticle.Play();
        CheckCanUse();
    }

    //������ ����Ʈ ���
    public void OnClick_Use() 
    {
        m_IsUsed = true;
        var player = CGameManager.Instance.m_TurnManager.m_PlayerChar;
        player.m_SkillMgr.CancelUseCardSkill();
        int num = m_CurrQuest.m_Data.m_Reward_Num;

        //����Ʈ ����.. ��� ����ϰ� �ұ�
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

    //�ֻ��� ��� üũ
    public void UseDice(int _eye)
    {
        Debug.Log("�ֻ��� ��� üũ");
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

    //������ �ִ°� üũ
    public void GiveDmg(int _dmg)
    {
        switch (m_CurrQuest.m_Data.m_Term)
        {
            case CUtility.CQuest.ETerm.DMG:
                m_QuestCount += _dmg; break;
        }
    }

    //��� �ø��°� üũ
    public void GainDef(int _def)
    {
        Debug.Log(_def);

        switch (m_CurrQuest.m_Data.m_Term)
        {
            case CUtility.CQuest.ETerm.DEF:
                m_QuestCount += _def; break;
        }
    }

    //����Ʈ �ʱ�ȭ
    public void EndTurn()
    {
        if (m_CurrQuest.m_Data.m_ResetOnEndTurn == true)
            m_QuestCount = 0;
    }

    //���� ��� üũ
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
            m_CanUse == true && //����Ʈ Ŭ����?
            m_DiceState_CanUse == true && //��� ���� ����?
            m_IsUsed == false; //�̹� ����߳�?

    }
}
