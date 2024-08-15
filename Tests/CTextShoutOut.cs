using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Febucci.UI;

public class CTextShoutOut : MonoBehaviour
{
    public TextAnimator_TMP m_TextAnim = null;
    public TMPro.TextMeshProUGUI m_TMP = null;
    public TypewriterByCharacter m_Typewriter = null;

    [SerializeField] float m_RemoveWait = 0.5f;
    [SerializeField] float m_RemainTime = 1f;

    public void TMPChange(string _text) 
    {
        m_TMP.text = _text;
        if (this.gameObject.activeInHierarchy == true)
            m_Typewriter.StartShowingText(true);

    }

    public void SetText(string _text) 
    {
        m_TextAnim.SetText(_text);
        if(this.gameObject.activeInHierarchy == true)
            m_Typewriter.StartShowingText(true);
    }

    private void Start()
    {
        Debug.Log("SPAWN");
        m_Typewriter.StartShowingText(true);
    }

    //
    public void EndTextSpawn() 
    {
        StartCoroutine(CoDisappear());
    }
    IEnumerator CoDisappear() 
    {
        yield return CUtility.GetSecD1To5s(m_RemainTime);
        m_Typewriter.StartDisappearingText();
    }

    //
    public void RemoveSelf() 
    {
        StartCoroutine(CoRemove());
    }

    IEnumerator CoRemove() 
    {
        yield return CUtility.GetSecD1To5s(m_RemoveWait);
        Destroy(this.gameObject);
    }
}
