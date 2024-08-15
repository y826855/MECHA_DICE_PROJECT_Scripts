using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Dynamic_Group : MonoBehaviour
{
    public List<CUI_Dynamic> m_Childs = new List<CUI_Dynamic>();

    public bool m_bAppearLeft = false;
    public bool m_bAppearRight = false;
    public float m_AppearDelay = 0.3f;
    public float m_DisappearDelay = 0.1f;

    private void Awake()
    {
        foreach (var it in m_Childs)
        {
            it.m_Func_Appear = DoneAppear;
            it.m_Func_Disappear = DoneDisappear;
        }
    }

    //비활성화 자식들 활성화
    public void SetForceEnable_Child() 
    {
        foreach (var it in m_Childs)
        { it.m_CanEnable = true; }
    }

    //비활성화 자식들 활성화, 이후 동작까지
    public void SetForceEnable() 
    {
        foreach (var it in m_Childs) 
        { it.m_CanEnable = true; }
        SetEnable();
    }

    //자식 활성화. 이미 활성화 된 상태라면 비활성화 후 활성화
    public void SetEnable() 
    {
        StopAllCoroutines();

        if (this.gameObject.activeSelf == false) { WorkEnable(); }
        //이미 생성된게 있다면 지우고 다시 생성
        else StartCoroutine(CoWaitDisappear());
    }

    //활성화 동작. 왼 오 없음
    public void WorkEnable() 
    {
        this.gameObject.SetActive(true);

        CountCallback = 0;
        if (m_bAppearLeft == true) StartCoroutine(CoAppearLeft(true));
        else if (m_bAppearRight == true) StartCoroutine(CoAppearRight(true));
        else
        {
            for (int i = 0; i < m_Childs.Count; i++)
            { m_Childs[i].SetEnable(); }
        }
    }

    //활성화 중의 비활성화 함수는 종료시 본체를 비활성화 시키지 않음
    bool dontDisable = false;
    IEnumerator CoWaitDisappear() 
    {
        dontDisable = true;
        if (m_bAppearLeft == true) yield return StartCoroutine(CoAppearLeft(false));
        else if (m_bAppearRight == true) yield return StartCoroutine(CoAppearRight(false));
        else
        {
            for (int i = 0; i < m_Childs.Count; i++)
            { m_Childs[i].SetDisable(); }
        }
        yield return null;

        WorkEnable();
        dontDisable = false;
    }

    //좌측부터 활성화
    IEnumerator CoAppearLeft(bool _toggle) 
    {
        for (int i = 0; i < m_Childs.Count; i++)
        {
            if (_toggle == true)
            {
                m_Childs[i].SetEnable();
                yield return CUtility.GetSecD1To5s(m_AppearDelay);
            }
            else
            {
                if (m_Childs[i].gameObject.activeSelf == true)
                {
                    m_Childs[i].SetDisable();
                    yield return CUtility.GetSecD1To5s(m_AppearDelay);
                }
            }
        }
    }

    //우측부터 활성화
    IEnumerator CoAppearRight(bool _toggle)
    {
        for (int i = m_Childs.Count - 1; i >= 0; i--)
        {
            if (_toggle == true)
            {
                m_Childs[i].SetEnable();
                yield return CUtility.GetSecD1To5s(m_AppearDelay);
            }
            else 
            {
                if (m_Childs[i].gameObject.activeSelf == true)
                {
                    m_Childs[i].SetDisable();
                    yield return CUtility.GetSecD1To5s(m_AppearDelay);
                }
            }
        }
    }

    //비활성화 시작 함수
    public void SetDisable() 
    {
        if (this.gameObject.activeSelf == false) return;
        StopAllCoroutines();

        if (m_bAppearLeft == true) StartCoroutine(CoAppearLeft(false));
        else if (m_bAppearRight == true) StartCoroutine(CoAppearRight(false));
        else
        {
            for (int i = 0; i < m_Childs.Count; i++)
            { m_Childs[i].SetDisable(); }
        }
    }

    //자식들 다됬는지 체크용 변수
    int CountCallback = 0;

    //활성화 종료 콜백
    public void DoneAppear() 
    { 
        CountCallback++;
        if (CountCallback == m_Childs.Count) CountCallback = 0;
    }

    //비활성화 종료 콜백
    public void DoneDisappear() 
    {
        if (dontDisable == true) return;
        CountCallback++;
        if (CountCallback == m_Childs.Count) 
        {
            CountCallback = 0;
            this.gameObject.SetActive(false);
        }
    }

    public void OnDisable()
    {
        CountCallback = 0;
        StopAllCoroutines();
        for (int i = 0; i < m_Childs.Count; i++)
        { m_Childs[i].gameObject.SetActive(false); }
    }
}
