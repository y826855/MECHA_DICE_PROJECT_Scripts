using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CUI_Info_ManaSkill : MonoBehaviour
{
    [SerializeField] CanvasGroup m_CanvasGroup = null;
    public Button m_Btn = null;

    [Header("================================================")]
    [SerializeField] Image m_Img_Icon = null;
    [SerializeField] Image m_Img_Bg = null;
    [SerializeField] TextMeshProUGUI m_TMP_Name = null;
    [SerializeField] TextMeshProUGUI m_TMP_Desc = null;
    [SerializeField] TextMeshProUGUI m_TMP_SkillType = null;
    [SerializeField] TextMeshProUGUI m_TMP_Term = null;
    [SerializeField] TextMeshProUGUI m_TMP_Type = null;

    [Header("================================================")]
    [SerializeField] float m_ShowDelay = 1.0f;

    //TODO : 마우스 호버 시간 체크, 위치 변경
    //버튼 콜백 연결
    Coroutine coWaitShowDelay = null;
    public void OnEnter(Transform _loc)
    {
        if (coWaitShowDelay != null) StopCoroutine(coWaitShowDelay);
        coWaitShowDelay = StartCoroutine(CoWaitShowDelay());

        if (_loc == null) return;
        var pos = m_CanvasGroup.transform.position;
        pos.y = _loc.position.y;
        m_CanvasGroup.transform.position = pos;
    }

    IEnumerator CoWaitShowDelay() 
    {
        float delay = m_ShowDelay * 0.8f;
        yield return CUtility.GetSecD1To5s(delay);

        delay = m_ShowDelay - delay;

        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime / delay;
            m_CanvasGroup.alpha = t;
            yield return null;
        }
        m_CanvasGroup.alpha = 1;
        coWaitShowDelay = null;
    }

    public void SetData(CUI_Quest _quest) 
    { SetData(_quest.m_CurrQuest); }

    public void SetData(CUI_ManaSkill _skill)
    { SetData(_skill.m_Skill); }

    public void SetData(CUI_ManaSkill_Use _skill)
    { SetData(_skill.m_Skill); }


    public uint m_ID = 0;
    public CUtility.EManaSkill_Kind m_Type = CUtility.EManaSkill_Kind.AREA;

    public void SetData(CScriptable_Quest _quest)
    {
        if (_quest == null) return;
        var temp = _quest;
        m_ID = temp.m_Data.m_ID;
        m_Type = CUtility.EManaSkill_Kind.QUEST;

        m_Img_Icon.sprite = temp.m_Data.m_Icon;
        if (m_Img_Bg != null) m_Img_Bg.color = CGameManager.Instance.color_QUEST;
        if (m_TMP_Name != null) m_TMP_Name.text = temp.m_Data.m_Name;
        if (m_TMP_Desc != null) m_TMP_Desc.text = temp.m_Data.m_Description;
        if (m_TMP_SkillType != null) m_TMP_SkillType.text = temp.m_Data.m_Reward_Type.ToString();
        if (m_TMP_Term != null) m_TMP_Term.text = QuestTerm_ToString(temp.m_Data);
        if (m_TMP_Type != null) m_TMP_Type.text = "QUEST";
    }

    public void SetData(CScriptable_ManaSkill_Area _skill)
    {
        if (_skill == null) return;
        var temp = _skill;
        m_ID = temp.m_Data.m_ID;
        m_Type = CUtility.EManaSkill_Kind.AREA;

        m_Img_Icon.sprite = temp.m_Data.m_Icon;
        if (m_Img_Bg != null) m_Img_Bg.color = CGameManager.Instance.color_AREA;
        if (m_TMP_Name != null) m_TMP_Name.text = temp.m_Data.m_Name;
        if (m_TMP_Desc != null) m_TMP_Desc.text = temp.m_Data.m_Description;
        if (m_TMP_SkillType != null) m_TMP_SkillType.text = temp.m_Data.m_Type.ToString();
        if (m_TMP_Term != null) m_TMP_Term.text = temp.m_Data.m_Cost.ToString();
        if (m_TMP_Type != null) m_TMP_Type.text = "AREA";
    }

    public void SetData(CScriptable_ManaSkill _skill)
    {
        if (_skill == null) return;
        var temp = _skill;
        m_ID = temp.m_Data.m_ID;
        m_Type = CUtility.EManaSkill_Kind.USE;

        m_Img_Icon.sprite = temp.m_Data.m_Icon;
        if (m_Img_Bg != null) m_Img_Bg.color = CGameManager.Instance.color_USE;
        if (m_TMP_Name != null) m_TMP_Name.text = temp.m_Data.m_Name;
        if (m_TMP_Desc != null) m_TMP_Desc.text = temp.m_Data.m_Description;
        if (m_TMP_SkillType != null) m_TMP_SkillType.text = temp.m_Data.m_Type.ToString();
        if (m_TMP_Term != null) m_TMP_Term.text = temp.m_Data.m_Cost.ToString();
        if (m_TMP_Type != null) m_TMP_Type.text = "USE";
    }

    public void DataClear() 
    {
        m_ID = 0;
        m_Img_Icon.sprite = null;
        if (m_Img_Bg != null) m_Img_Bg.color = Color.white;
        if (m_TMP_Name != null) m_TMP_Name.text = "";
        if (m_TMP_Desc != null) m_TMP_Desc.text = "";
        if (m_TMP_SkillType != null) m_TMP_SkillType.text = "";
        if (m_TMP_Term != null) m_TMP_Term.text = "";
        if (m_TMP_Type != null) m_TMP_Type.text = "";
    }

    public void Escape() 
    {
        if (coWaitShowDelay != null) StopCoroutine(coWaitShowDelay);
        if(m_CanvasGroup != null) m_CanvasGroup.alpha = 0;
    }

    //언어 대응 어캐하징,,,
    string QuestTerm_ToString(CUtility.CQuest _quest) 
    {
        string res = "";

        if (_quest.m_ResetOnEndTurn == true) res += "In one turn, ";
        else res += "In battle, ";

        switch (_quest.m_Term) 
        {
            case CUtility.CQuest.ETerm.DMG:
                res += string.Format("Deal more than [{0}] damage", _quest.m_Count); break;
            case CUtility.CQuest.ETerm.DEF:
                res += string.Format("Get more than [{0}] Defense", _quest.m_Count); break;
            case CUtility.CQuest.ETerm.DICE_SAME:
                res += string.Format("Uses {0} dice equal to [{1}]", _quest.m_Count, _quest.m_Require); break;
            case CUtility.CQuest.ETerm.DICE_LESS:
                res += string.Format("[{0}]Uses the following dice {1} times.", _quest.m_Require, _quest.m_Count); break;
            case CUtility.CQuest.ETerm.DICE_OVER:
                res += string.Format("[{0}] or more dice used {1} times", _quest.m_Require, _quest.m_Count); break;
            case CUtility.CQuest.ETerm.MANA:
                res += string.Format("Uses more than [{0}] mana", _quest.m_Count); break;
        }

        return res;
    }
}
