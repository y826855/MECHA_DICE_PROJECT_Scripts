using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CUI_SmoothAppear : MonoBehaviour
{
    [SerializeField] CanvasGroup m_CanvasGroup = null;
    [SerializeField] float m_AppearDuration = 0.5f;

    private void OnEnable()
    {
        m_CanvasGroup.alpha = 0;
        m_CanvasGroup.DOFade(1, m_AppearDuration);
    }

    public void SetDisable() 
    {
        m_CanvasGroup.DOFade(0, m_AppearDuration).OnComplete(
            () => { this.gameObject.SetActive(false); });
    }
}
