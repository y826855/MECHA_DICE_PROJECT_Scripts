using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using TMPro;
using Febucci.UI;

public class CUI_ShoutOut : MonoBehaviour
{
    public TextAnimator_TMP m_TMPA = null;
    public TypewriterByCharacter m_TypeWriter = null;

    private void OnEnable()
    {
        //Spawn("TESTING ! !");
    }

    public void Spawn(string _text) 
    {
        m_TMPA.SetText(_text);
        m_TypeWriter.DORewind();
    }

    public void TestRewind_01() 
    {
        m_TMPA.SetText("TEST 01 ! ! !");
    }
    public void TestRewind_02()
    {
        m_TypeWriter.DORewind();
    }
    public void TestRewind_03()
    {
        m_TypeWriter.DOPlayBackwards();
    }
    public void TestRewind_04()
    {
        m_TMPA.SetVisibilityEntireText(false);
        m_TypeWriter.DOPlay();
    }
}
