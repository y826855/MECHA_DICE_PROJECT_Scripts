using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CUI_Skill_Desc : MonoBehaviour
{
    [SerializeField] Image m_Img_Icon = null;
    [SerializeField] TextMeshProUGUI m_TMP_Desc = null;
    [SerializeField] TextMeshProUGUI m_TMP_Type = null;
    [SerializeField] TextMeshProUGUI m_TMP_Name = null;
    [SerializeField] TextMeshProUGUI m_TMP_SkillType = null;
    [SerializeField] TextMeshProUGUI m_TMP_Term = null;

    //TODO : 조건들어떡하냐 ㅋㅋ

    public void SetData(CUI_ManaSkill _ui) 
    {
        var data = _ui.m_Skill.m_Data;
        m_TMP_Name.text = data.m_Name;
        m_TMP_Desc.text = data.m_Description;
    }

    public void SetData(CUI_Quest _ui)
    {
        var data = _ui.m_CurrQuest.m_Data;
        m_TMP_Name.text = data.m_Name;
        m_TMP_Desc.text = data.m_Description;
    }

    public void SetData(CUI_ManaSkill_Use _ui) 
    {
        var data = _ui.m_Skill.m_Data;
        m_TMP_Name.text = data.m_Name;
        m_TMP_Desc.text = data.m_Description;

    }
}
