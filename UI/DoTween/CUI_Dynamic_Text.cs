using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CUI_Dynamic_Text : CUI_Dynamic
{
    public Febucci.UI.TypewriterByCharacter m_TpyeChar = null;
    public Febucci.UI.TypewriterByWord m_TpyeWord = null;

    public void Skip() 
    {
        if (m_TpyeChar != null) m_TpyeChar.SkipTypewriter();
        if (m_TpyeWord != null) m_TpyeWord.SkipTypewriter();
    }
}
