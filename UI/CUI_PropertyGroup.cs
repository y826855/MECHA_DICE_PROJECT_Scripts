using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_PropertyGroup : MonoBehaviour
{
    public RectTransform m_Rect = null;

    public List<CUI_PropertyText> m_Properties = new List<CUI_PropertyText>();

    public bool m_IsEmpty = true;
    public bool m_IsShowEmptyData = false;
    public bool m_IsAnim = false;

    public void SetData(CScriptable_CardSkill _skill) 
    {
        m_IsEmpty = false;

        ClearDataNum();
        var data = _skill.m_Data;

        if (data.m_Damage > 0) SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Damage.ToString(), m_IsAnim, false);
        //if (data.m_Targets > 0) SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Damage.ToString(), m_IsAnim);
        if (data.m_Defend > 0) SetDataNum(CUtility.ENumableIcon.DEFEND, data.m_Defend.ToString(), m_IsAnim, false);
        //if (data.m_Count > 0) SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Damage.ToString(), m_IsAnim);
        if (data.m_StatusEff > 0) SetDataNum(data.GetStatusIcon(), data.m_Damage.ToString(), m_IsAnim, false);
    }

    public void SetData(CScriptable_Disk _disk, int _avr = 0, CUtility.ENumableIcon _statEff = CUtility.ENumableIcon.NONE)
    {
        m_IsEmpty = false;

        ClearDataNum();

        var data = _disk.m_Data;
        if (data.m_Damage.CheckIsEmpty() == false)
            SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Damage.Calc(_avr).ToString(),
                m_IsAnim, data.m_Damage.m_IsDA);
        if (data.m_Targets.CheckIsEmpty() == false)
            SetDataNum(CUtility.ENumableIcon.TARGET, data.m_Targets.Calc(_avr).ToString(), 
                m_IsAnim, data.m_Targets.m_IsDA);
        if (data.m_StatusEff.CheckIsEmpty() == false && _statEff != CUtility.ENumableIcon.NONE)
            SetDataNum(_statEff, data.m_StatusEff.Calc(_avr).ToString(), 
                m_IsAnim, data.m_StatusEff.m_IsDA);

        if (data.m_Defend.CheckIsEmpty() == false)
            SetDataNum(CUtility.ENumableIcon.DEFEND, data.m_Defend.Calc(_avr).ToString(), 
                m_IsAnim, data.m_Defend.m_IsDA);
        if (data.m_Mana.CheckIsEmpty() == false)
            SetDataNum(CUtility.ENumableIcon.MANA, data.m_Mana.Calc(_avr).ToString(),
                m_IsAnim, data.m_Mana.m_IsDA);
        
        if (data.m_Return == true)
            SetDataNum(CUtility.ENumableIcon.RETURN, "v", m_IsAnim, false);

        if (data.m_Debuff > 0) 
        {
            switch (_statEff)
            {
                case CUtility.ENumableIcon.ELEC:
                    SetDataNum(CUtility.ENumableIcon.ELEC, data.m_Debuff.ToString(), m_IsAnim, false, _isUpper: false);
                    break;
                case CUtility.ENumableIcon.BURN:
                    SetDataNum(CUtility.ENumableIcon.BURN, data.m_Debuff.ToString(), m_IsAnim, false, _isUpper: false);
                    break;
                case CUtility.ENumableIcon.ROCK:
                    SetDataNum(CUtility.ENumableIcon.ROCK, data.m_Debuff.ToString(), m_IsAnim, false, _isUpper: false);
                    break;
                case CUtility.ENumableIcon.DEFEND:
                    SetDataNum(CUtility.ENumableIcon.DEFEND, data.m_Debuff.ToString(), m_IsAnim, false, _isUpper: false);
                    break;
                default:
                    SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Debuff.ToString(), m_IsAnim, false, _isUpper: false);
                    break;
            }
        }

        //if (data.m_Burn_Self > 0)
        //    SetDataNum(CUtility.ENumableIcon.BURN, data.m_Burn_Self.ToString(), m_IsAnim, false, _isUpper: false);
        //if (data.m_Elec_Self > 0)
        //    SetDataNum(CUtility.ENumableIcon.ELEC, data.m_Elec_Self.ToString(), m_IsAnim, false, _isUpper: false);
        //if (data.m_Damage_Self > 0)
        //    SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Damage_Self.ToString(), m_IsAnim, false, _isUpper: false);


        MakeEmpty();
    }

    public void MakeEmpty()
    {
        if (m_IsShowEmptyData == true) return;

        foreach (var it in m_Properties)
        { it.gameObject.SetActive(it.m_IsEmpty != true); }
    }

    public void SetDataNum(CUtility.ENumableIcon _icon, string _text, bool _anim, bool _isDA, bool _isUpper = true)
    {
        //for문 돌며 해당하는 데이터에 정보 뿌림
        foreach (var it in m_Properties)
        { if (it.InsertData(_icon, _text, _anim, _isDA, _isUpper) == true) return; }
    }


    public void ClearDataNum()
    {
        m_IsEmpty = true;
        //foreach (var it in m_Properties) it.ResetData();
        foreach (var it in m_Properties) it.InitData();
    }
}
