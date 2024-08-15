using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DG.Tweening;
using TMPro;

public class CUI_Eff_Log : CUI_Eff_FadeInOut
{

    public Vector3 m_ScaleSize = Vector3.one;
    public float m_ScaleDurtion = 0.3f;
    public float m_Speed = 300f;
    public float m_Grav = 1.2f;

    [SerializeField] TextMeshProUGUI m_TMP = null;

    Vector3 defaultPos = Vector3.zero;

    private void Awake()
    {
        defaultPos = this.transform.position;
    }
    public override void OnEnable()
    {
        base.OnEnable();
        this.transform.position = defaultPos;
        this.transform.DOPunchScale(m_ScaleSize, m_ScaleDurtion, 1);
        StartCoroutine(CoMove());
    }

    public void SetDefault(Vector3 _startPos, string _log) 
    {
        this.transform.position = Camera.main.WorldToScreenPoint(_startPos);
        m_TMP.text = _log;
    }

    IEnumerator CoMove() 
    {
        float duration = m_FadeDuration;
        float gravity = m_Grav;
        var dir = Random.insideUnitCircle;
        if (dir.y < 0.5) dir.y = 0.5f;

        while (duration > 0) 
        {
            float dt = Time.deltaTime;
            duration -= dt;
            this.transform.position += (Vector3)(dir * m_Speed * dt);
            dir.y -= gravity * dt;
            gravity += gravity * dt;

            yield return null;
        }

        yield return CUtility.m_WFS_D5;
        Destroy(this.gameObject);
        DestroyImmediate(this.gameObject);
    }
}
