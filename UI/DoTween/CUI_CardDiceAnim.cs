using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_CardDiceAnim : CUI_SimpleAnim
{
    public Image m_Child_Img = null;

    public void SetAlpha(float _alpha) 
    {
        var color = m_Image.color; color.a = _alpha;
        m_Image.color = color;
    }

    public void SetIcon(Sprite _sp, bool _isReset = false) 
    {
        m_Image.sprite = _sp;

        if (_isReset == false)
            Act_LocalMove();
        else
            Act_Reset();
    }
}
