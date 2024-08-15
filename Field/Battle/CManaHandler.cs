using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CManaHandler : MonoBehaviour
{
    public int m_MaxMana = 15;
    public int m_RegenMana = 5;
    [SerializeField] int currMana = 0;

    public TMPro.TextMeshProUGUI m_TMP_Mana = null;
    public CManaSkillManager m_ManaSkillMgr = null;

    [Header("============================")]
    public List<CScriptable_ManaSkill_Area> m_ManaSkills = new List<CScriptable_ManaSkill_Area>();
    public List<CUI_ManaSkill> m_UI_Buttons = new List<CUI_ManaSkill>();
    public CUI_ManaSkill m_Btn_ManaSkill_Pref = null;
    public Transform m_UI_Parent = null;

    public System.Action<int> m_CB_ManaChange = null;

    public int m_CurrMana
    {
        get { return currMana; }
        set 
        { 
            currMana = value;
            if (m_CurrMana > m_MaxMana) m_CurrMana = m_MaxMana;
            SetManaGauge();
        }
    }

    public Image m_Mana_Fill = null;


    //마나 스킬 생성
    //public void Spawn(List<CScriptable_ManaSkill_Area> _manaSkills)
    //{
    //    foreach (var it in _manaSkills) 
    //    {
    //        var inst = Instantiate(it);
    //        var ui = Instantiate(m_Btn_ManaSkill_Pref, m_UI_Parent);
    //        ui.Spawn(inst, UseMana);
    //        ui.m_ManaSkillMgr = m_ManaSkillMgr;
    //    }
    //}

    //마나 게이지 세팅
    void SetManaGauge() 
    {
        if (currMana > 0)
            m_Mana_Fill.fillAmount = 1f - ((float)currMana / (float)m_MaxMana);
        else m_Mana_Fill.fillAmount = 1;

        m_TMP_Mana.text = string.Format("{0}/{1}", currMana, m_MaxMana);

        foreach (var it in m_UI_Buttons) 
        { it.CheckCanUse(currMana); }
    }

    public void ManaRegen() 
    { m_CurrMana += m_RegenMana; }

    public void UseMana(int _mana) 
    { 
        m_CurrMana -= _mana;
        m_CB_ManaChange(_mana);
    }
}
