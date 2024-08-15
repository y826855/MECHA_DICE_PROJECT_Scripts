using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class CUI_ReadySkill : MonoBehaviour
{
    [Header("===================")]
    public Image m_Icon = null;
    public TextMeshProUGUI m_TMP_Count = null;
    public TextMeshProUGUI m_TMP_Option = null;

    public Sprite m_Icon_Atk = null;
    public Sprite m_Icon_Def = null;

    [Header("===================")]
    public Image m_Frame = null;
    public RectTransform m_Pref_Divider = null;
    public List<RectTransform> m_Dividers = new List<RectTransform>();

    [SerializeField] float Count = 0;
    [SerializeField] float currCount = 0;
    [SerializeField] float dgDuration = 0.2f;
    public bool m_IsGaugeFull = false;

    public System.Action m_CB_ChangeToSpecial = null;

    public void SetData(CScriptable_MonsterSkill _skill) 
    {
        var data = _skill.m_Data;

        string _textIcon = "";
        string _textNum = "";

        if (data.m_Type == CUtility.ECardType.DEF
            || data.m_Type == CUtility.ECardType.DEF_BURN
            || data.m_Type == CUtility.ECardType.DEF_ELEC
            || data.m_Type == CUtility.ECardType.DEF_ROCK)
        {
            _textNum = data.m_Def.ToString();
            m_Icon.sprite = m_Icon_Def; 
        }
        else
        {
            _textNum = data.m_Dmg.ToString();
            m_Icon.sprite = m_Icon_Atk; 
        }
        
        switch (data.m_Type) 
        {
            case CUtility.ECardType.ATK_ELEC:
                _textIcon = string.Format("<sprite={0}>", (int)CUtility.ETextIcon.Electric);
                break;
            case CUtility.ECardType.ATK_BURN:
                _textIcon = string.Format("<sprite={0}>", (int)CUtility.ETextIcon.Burn);
                break;
            case CUtility.ECardType.ATK_ROCK:
                _textIcon = string.Format("<sprite={0}>", (int)CUtility.ETextIcon.Rock);
                break;

            case CUtility.ECardType.DEF_ELEC:
                _textIcon = string.Format("<sprite={0}>", (int)CUtility.ETextIcon.Electric);
                break;
            case CUtility.ECardType.DEF_BURN:
                _textIcon = string.Format("<sprite={0}>", (int)CUtility.ETextIcon.Burn);
                break;
            case CUtility.ECardType.DEF_ROCK:
                _textIcon = string.Format("<sprite={0}>", (int)CUtility.ETextIcon.Rock);
                break;
        }

        m_TMP_Count.text = string.Format("×{0}", data.m_Count);
        m_TMP_Option.text = string.Format("{0}{1}", _textIcon, _textNum);
    }

    public void SetFrameDivider(int _count = 2) 
    {
        Count = _count;
        int need = m_Dividers.Count - _count;
        if (need < 0) 
        {
            for (int i = 0; i < -need; i++) 
            {
                var inst = Instantiate(m_Pref_Divider, m_Frame.rectTransform);
                m_Dividers.Add(inst);
            }
        }
        

        for (int i = 0; i < m_Dividers.Count; i++) 
        {
            if (i < _count - 1)
            {
                m_Dividers[i].gameObject.SetActive(true);
                float division = 0;
                division = (float)(i + 1) / (float)_count;
                m_Dividers[i].localRotation = Quaternion.Euler(0, 0, division * 360f);
            }
            else m_Dividers[i].gameObject.SetActive(false);
        }

        currCount = 0;
        m_Frame.fillAmount = 0;
    }

    //게이지 1증가.
    public void AddFrameCount()
    {//이미 꽉찬상태면 작동안함
        if (m_IsGaugeFull == true) return;

        m_IsGaugeFull = currCount >= Count;
        if (++currCount >= Count) //게이지 풀이면 스페셜 스킬로 변경
        { m_CB_ChangeToSpecial(); }
        m_Frame.fillAmount = currCount / Count;
        GaugeAnim(currCount / Count);
    }

    //게이지 -num 
    public void AddFrameCount(float _num)
    {//이미 꽉찬상태면 작동안함
        if (m_IsGaugeFull == true) return;

        currCount += _num;
        m_IsGaugeFull = currCount >= Count;
        if (currCount >= Count) //게이지 풀이면 스페셜 스킬로 변경
        { m_CB_ChangeToSpecial(); }
        GaugeAnim(currCount / Count);
    }

    //게이지 set
    public void SetFrameCount(int _count) 
    {//바꿀 수치가 같으면 실행 안함
        if (m_IsGaugeFull == true) return;

        currCount = _count;
        m_IsGaugeFull = currCount >= Count;
        if (currCount >= Count) //게이지 풀이면 스페셜 스킬로 변경
        { m_CB_ChangeToSpecial(); return; }

        GaugeAnim(_count == 0 ? 0 : currCount / Count);
    }

    Sequence seq = null;
    public void GaugeAnim(float _calc) 
    {
        seq.Kill();
        seq = DOTween.Sequence();

        seq.Append(DOTween.To(() => m_Frame.fillAmount, x => m_Frame.fillAmount = x, _calc, dgDuration));
    }
}
