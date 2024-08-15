using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using Febucci.UI;

public class CUI_Field_Info : CUI_Field
{
    public CHitable m_Owner = null;
    
    public CUtility.InfoGroup m_HP = new CUtility.InfoGroup();
    public CUtility.InfoGroup m_DEF = new CUtility.InfoGroup();

    [Header("=========================================================")]
    public TextAnimator_TMP m_TMP_Debuff = null;
    //public TextAnimator_TMP m_TMP_Elec = null;
    //public TextAnimator_TMP m_TMP_Burn = null;

    [SerializeField] CUI_SmoothAppear m_SmoothAppear = null;
    //public CUtility.InfoGroup m_Elec = new CUtility.InfoGroup();
    //public CUtility.InfoGroup m_Burn = new CUtility.InfoGroup();
    //public CUtility.InfoGroup m_Curse = new CUtility.InfoGroup();
    //public CUtility.InfoGroup m_Ref = new CUtility.InfoGroup();


    public TMPro.TextMeshProUGUI m_TMP_Targeted = null;
    public TMPro.TextMeshProUGUI m_TMP_Hit_Sum = null;
    public string m_TextIcon_Targeted = "<sprite=0>";

    //public void ChangeElec(int _num) 
    //{
    //    if (this.gameObject.activeSelf == false) return;
    //    
    //    m_TMP_Elec.gameObject.SetActive(_num > 0);
    //    m_TMP_Elec.SetText(string.Format("<sprite={0}>{1}"
    //    , (int)CUtility.ETextIcon.Electric, _num)); 
    //}
    //
    //public void ChangeBurn(int _num)
    //{
    //    if (this.gameObject.activeSelf == false) return;
    //
    //    m_TMP_Burn.gameObject.SetActive(_num > 0);
    //    m_TMP_Burn.SetText(string.Format("<sprite={0}>{1}"
    //        , (int)CUtility.ETextIcon.Burn, _num));
    //}

    public void ChangeBuff(CUtility.ECardType _tpye, int _num) 
    {
        string res = "";
        if (_tpye == CUtility.ECardType.ATK_ELEC)
            res += string.Format("<sprite={0}>{1}", (int)CUtility.ETextIcon.Electric, _num);
        else
            res += string.Format("<sprite={0}>{1}", (int)CUtility.ETextIcon.Burn, _num);

        if (_num == 0)
            res = "";

        m_TMP_Debuff.SetText(res);
    }

    public void RefreashData() 
    {
        //m_HP.m_TMP.text = string.Format("{0}/{1}", m_Owner.m_DEBUG_HP, m_Owner.m_MaxHP);
        //ShowToggleUI(m_DEF, m_Owner.m_DEBUG_DEF);
        SetHP();
        SetDEF();
    }

    public void SetHP() 
    {
        m_HP.SetToggle(m_Owner.m_DEBUG_HP > 0);
        var data = string.Format("{0}/{1}", m_Owner.m_DEBUG_HP, m_Owner.m_MaxHP);
        m_HP.m_TMP.text = data;
    }

    public void SetDEF()
    {
        m_DEF.SetToggle(m_Owner.m_DEBUG_DEF > 0);
        m_DEF.m_TMP.text = m_Owner.m_DEBUG_DEF.ToString();
    }

    public void AddTarget() 
    {
        m_TMP_Targeted.text += m_TextIcon_Targeted;
    }

    public void ClearTarget() 
    {
        m_TMP_Targeted.text = "";
    }

    public void RemoveTarget() 
    {
        if (m_TMP_Targeted.text.Length > 0)
            m_TMP_Targeted.text = m_TMP_Targeted.text.Substring(m_TextIcon_Targeted.Length);
    }

    int dmgSum = 0;
    int stackElec = 0;
    int stackBurn = 0;
    int stackRock = 0;

    public void AddHit(int _dmg, int _stack, CUtility.ETextIcon _dmgType) 
    {
        if (m_TMP_Hit_Sum.gameObject.activeSelf == false)
            m_TMP_Hit_Sum.gameObject.SetActive(true);

        string res = "";

        switch (_dmgType)
        {
            case CUtility.ETextIcon.Electric:
                stackElec += _stack;
                break;
            case CUtility.ETextIcon.Burn:
                stackBurn += _stack;
                break;
            case CUtility.ETextIcon.Rock:
                stackRock += _stack;
                break;
        }

        dmgSum += _dmg;
        if (dmgSum > 0) res += string.Format("<sprite=4>{0}", dmgSum);
        if (stackElec > 0) 
        { res += string.Format("<sprite={0}>{1}", (int)_dmgType, stackElec); }
        if (stackBurn > 0)
        { res += string.Format("<sprite={0}>{1}", (int)_dmgType, stackBurn); }
        if (stackRock > 0)
        { res += string.Format("<sprite={0}>{1}", (int)_dmgType, stackRock); }

        m_TMP_Hit_Sum.text = res;

    }

    public void ClearHit() 
    {
        m_TMP_Hit_Sum.gameObject.SetActive(false);
        m_TMP_Hit_Sum.text = "";

        dmgSum = 0;
        stackElec = 0;
        stackBurn = 0;
        stackRock = 0;
    }
}
