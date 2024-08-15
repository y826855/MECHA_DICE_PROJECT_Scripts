using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CTestUI : MonoBehaviour
{
    public Sequence _sec = null;
    public float m_Duration = 1f;

    private void OnEnable()
    {
        InitializeSequence();
    }

    private void InitializeSequence()
    {
        if (_sec != null) _sec.Kill();
        _sec = DOTween.Sequence();
    }

    public void OnMoveLeft()
    {
        InitializeSequence();
        _sec.Append(this.transform.DOMoveX(10, m_Duration));
    }

    public void OnMoveRight()
    {
        InitializeSequence();
        _sec.Append(this.transform.DOMoveX(-10, m_Duration / 2f));
    }
}