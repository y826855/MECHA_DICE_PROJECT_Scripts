using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Febucci.UI;

public class CUI_PropertyText : MonoBehaviour
{
    public CUtility.ENumableIcon m_Icon = CUtility.ENumableIcon.NONE;
    public Image m_Img_Icon = null;

    [Header("=============================================")]
    public TextMeshProUGUI m_TMP_NumUp = null;
    //public TextAnimator_TMP m_TMPA_NumUp = null;
    public TypewriterByCharacter m_WriterNumUp = null;
    [Header("=============================================")]
    public TextMeshProUGUI m_TMP_NumDw = null;
    //public TextAnimator_TMP m_TMPA_NumDw = null;
    public TypewriterByCharacter m_WriterNumDw = null;

    public Image m_Icon_IsDA = null;
    public Sprite m_Default_Sprite = null;

    public bool m_IsEmpty = true;

    private void OnEnable()
    {
        if (m_WriterNumUp != null) m_WriterNumUp.StartShowingText(true);
        if (m_WriterNumDw != null) m_WriterNumDw.StartShowingText(true);
        //if (m_TMPA_NumUp != null) m_TMPA_NumUp.SetVisibilityEntireText(true);
        //if (m_TMPA_NumDw != null) m_TMPA_NumDw.SetVisibilityEntireText(true);
    }

    //아이콘 텍스트에 데이터 삽입
    public bool InsertData(CUtility.ENumableIcon _icon, string _text, bool _isAnim, bool _isDA , bool _isUpper = true)
    {
        m_IsEmpty = false;
        if (_text == "") return false;

        if (m_Icon == CUtility.ENumableIcon.NONE || m_Icon == _icon)
        {
            if (m_Icon == CUtility.ENumableIcon.NONE)
            {
                m_Icon = _icon;
                m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[(int)_icon];
            }

            if (_isUpper == true) 
            {
                string anim = "normal";
                if (_isAnim == true) { anim = "good"; }
                var text = string.Format("<{0}>{1}</{0}>", anim, _text);
                if (m_TMP_NumUp.text == _text) return true;

                m_TMP_NumUp.text = text;
                if (this.gameObject.activeInHierarchy == true && _isAnim == true)
                    m_WriterNumUp.StartShowingText(true);

                if (_isDA == true) m_Icon_IsDA.enabled = true;
                else m_Icon_IsDA.enabled = false;
            }
            else 
            {
                string anim = "normal";
                if (_isAnim == true) { anim = "bad"; }
                var text = string.Format("<{0}>{1}</{0}>", anim, _text);
                if (m_TMP_NumDw.text == _text) return true;
                m_TMP_NumDw.text = text;
                
                //m_TMPA_NumDw.SetText(text);
                if (this.gameObject.activeInHierarchy == true && _isAnim == true)
                    m_WriterNumDw.StartShowingText(true);
            }
            return true;
        }
        return false;
    }

    //IEnumerator CoPlayTextAnim(TextAnimator_TMP _tmpa) 
    //{
    //    _tmpa.SetVisibilityEntireText(false);
    //    yield return null;
    //    _tmpa.SetVisibilityEntireText(true);
    //}

    public void ResetData() 
    {
        //m_Img_Icon.color = CGameManager.Instance.color_Disable;
        m_IsEmpty = true;
        //if (m_TMPA_NumUp != null) m_TMPA_NumUp.SetText("-");
        //if (m_TMPA_NumDw != null) m_TMPA_NumDw.SetText("-");

        if (m_TMP_NumUp != null) m_TMP_NumUp.text = "-";
        if (m_TMP_NumDw != null) m_TMP_NumDw.text = "-";
    }

    public void InitData()
    {
        m_Icon = CUtility.ENumableIcon.NONE;
        m_Img_Icon.sprite = m_Default_Sprite;
        //m_Img_Icon.color = CGameManager.Instance.color_Disable;
        ResetData();
    }
}
