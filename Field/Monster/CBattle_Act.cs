using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using Febucci.UI;


public class CBattle_Act : MonoBehaviour
{
    public Animator m_Anim = null;
    public TextAnimator_TMP m_TMPA_Dialogue = null;
    public CanvasGroup m_Dialogue = null;

    public Febucci.UI.Core.TypewriterCore typewriter;

    //public List<CUtility.CMini_Event> m_Event_Intro = new List<CUtility.CMini_Event>();
    //public List<CUtility.CMini_Event> m_Event_Outro = new List<CUtility.CMini_Event>();

    [Header("=======================================================")]
    public CScriptable_BattleDialogue m_Event_Intros = null;
    public CScriptable_BattleDialogue m_Event_Outros = null;


    //텍스트 끝남 체크
    //public void DoEvent() 
    //{
    //    typewriter.onTextShowed.AddListener(EndOfText);
    //}

    //public void EndOfText() 
    //{
        
    //}

    public void ShowText(string _text) 
    {
        if (m_Dialogue.alpha < 1)
            StartCoroutine(CoActivateDialogue(true));

        m_TMPA_Dialogue.SetText(_text);
        typewriter.StartShowingText(true);
    }

    public void EndLog() 
    {
        if (m_Dialogue.alpha > 0)
            StartCoroutine(CoActivateDialogue(false));
    }

    IEnumerator CoActivateDialogue(bool _toggle) 
    {
        float t = 0;
        float speed = 0.15f;

        Debug.Log("SHOW " + _toggle);

        while (t < 1f) 
        {
            var delta = Time.deltaTime / speed;
            t += delta;

            if (_toggle == true) m_Dialogue.alpha += delta;
            else m_Dialogue.alpha -= delta;

            yield return null;
        }
    }

    //인트로 실행
    public IEnumerator CoDo_IntroEvent() 
    {
        foreach (var it in m_Event_Intros.m_Data.m_Events) 
        {
            ShowText(it.m_script);
            m_Anim.SetTrigger(it.m_Anim);

            yield return CUtility.GetSecD1To5s(it.m_Duration);
            EndLog();
        }

        //foreach (var it in m_Event_Intro) 
        //{
        //    ShowText(it.m_script);
        //    m_Anim.SetTrigger(it.m_Anim);

        //    yield return CUtility.GetSecD1To5s(it.m_Duration);
        //    EndLog();
        //}

        yield return CUtility.GetSecD1To5s(0.3f);
        EndLog();

        yield return null;
    }

    //아웃트로 실행
    public IEnumerator CoDo_OutroEvent() 
    {
        foreach (var it in m_Event_Outros.m_Data.m_Events)
        {
            ShowText(it.m_script);
            m_Anim.SetTrigger(it.m_Anim);

            yield return CUtility.GetSecD1To5s(it.m_Duration);
            EndLog();
        }


        //foreach (var it in m_Event_Outro)
        //{
        //    ShowText(it.m_script);
        //    m_Anim.SetTrigger(it.m_Anim);

        //    yield return CUtility.GetSecD1To5s(it.m_Duration);
        //    EndLog();
        //}

        yield return CUtility.GetSecD1To5s(0.3f);
        EndLog();

        yield return null;
    }
}
