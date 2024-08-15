using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Febucci.UI;
using DG.Tweening;

public class CNPC : MonoBehaviour
{
    public enum EActs { 
        NONE=-1, 
        IDLE =0, TALKING, GIVE, THANKYOU, YELL,
        ACT01, ACT02, ACT03,
        EndAct
    }
    public Animator m_Anim = null;

    public CanvasGroup m_ChatBox = null;
    public TextAnimator_TMP m_TMPA_Dialogue = null;

    public Febucci.UI.Core.TypewriterCore typewriter;
    

    //Adds and removes listening to callback
    void OnEnable() => typewriter.onMessage.AddListener(OnTypewriterMessage);
    void OnDisable() => typewriter.onMessage.RemoveListener(OnTypewriterMessage);

    //Does stuff based on event
    void OnTypewriterMessage(Febucci.UI.Core.Parsing.EventMarker eventMarker)
    {
        Debug.Log(eventMarker.name);

        //var act = System.Enum.Parse(typeof(EActs), eventMarker.name);

        switch (eventMarker.name)
        {
            case "Talk":
                Debug.Log("Animation Change To Talk");
                m_Anim.SetTrigger("Talk");
                break;
            case "Thanks":
                Debug.Log("Animation Change To Thanks");
                m_Anim.SetTrigger("Thanks");
                break;
            case "Act01":
                Debug.Log("Animation Change To Act01");
                m_Anim.SetTrigger("Act01");
                break;
            case "EndAct":
                Debug.Log("Animation Change To EndAct");
                m_Anim.SetTrigger("EndAct");
                break;
        }
    }

    public void SetMessage(string _text) 
    {
        m_TMPA_Dialogue.SetText(_text);
        typewriter.StartShowingText(true);
    }

    public void SkipScriptSpawn() 
    {
        typewriter.SkipTypewriter();
    }

    public void OnMessageStart() 
    {
        //m_ChatBox.DOFade(1, 0.2f);
    }

    public void EndOfDialogue() 
    {
        //m_ChatBox.DOFade(0, 0.2f);
    }
}
