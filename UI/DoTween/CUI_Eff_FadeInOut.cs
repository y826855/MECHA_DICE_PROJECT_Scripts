using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;
using TMPro;


[RequireComponent(typeof(CanvasGroup))]
public class CUI_Eff_FadeInOut : MonoBehaviour
{
    //public SpriteRenderer sp = null;

    public CanvasGroup m_Group = null;

    public float m_FadeDuration = 1f;
    public Ease m_Ease = Ease.Unset;

    public bool m_FadeIn = false;
    public bool m_DestroySelf = false;

    public virtual void OnEnable()
    {
        if (m_FadeIn == true)  FadeIn(); 
        else FadeOut();
    }

    public void FadeIn() 
    {
        m_Group.DOFade(1, m_FadeDuration).SetEase(m_Ease).
                OnComplete(FadeOut);
    }

    public void FadeOut() 
    {
        m_Group.DOFade(0, m_FadeDuration).SetEase(m_Ease).
            OnComplete(DestroySelf);
    }

    void DestroySelf() 
    {
        if (m_DestroySelf == true) Destroy(this.gameObject);
        else this.gameObject.SetActive(false);
    }
}
