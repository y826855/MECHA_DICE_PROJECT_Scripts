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

    //��Ȱ��ȭ �ڽĵ� Ȱ��ȭ
    public void SetForceEnable_Child() 
    {
        foreach (var it in m_Childs)
        { it.m_CanEnable = true; }
    }

    //��Ȱ��ȭ �ڽĵ� Ȱ��ȭ, ���� ���۱���
    public void SetForceEnable() 
    {
        foreach (var it in m_Childs) 
        { it.m_CanEnable = true; }
        SetEnable();
    }

    //�ڽ� Ȱ��ȭ. �̹� Ȱ��ȭ �� ���¶�� ��Ȱ��ȭ �� Ȱ��ȭ
    public void SetEnable() 
    {
        StopAllCoroutines();

        if (this.gameObject.activeSelf == false) { WorkEnable(); }
        //�̹� �����Ȱ� �ִٸ� ����� �ٽ� ����
        else StartCoroutine(CoWaitDisappear());
    }

    //Ȱ��ȭ ����. �� �� ����
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

    //Ȱ��ȭ ���� ��Ȱ��ȭ �Լ��� ����� ��ü�� ��Ȱ��ȭ ��Ű�� ����
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

    //�������� Ȱ��ȭ
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

    //�������� Ȱ��ȭ
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

    //��Ȱ��ȭ ���� �Լ�
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

    //�ڽĵ� �ى���� üũ�� ����
    int CountCallback = 0;

    //Ȱ��ȭ ���� �ݹ�
    public void DoneAppear() 
    { 
        CountCallback++;
        if (CountCallback == m_Childs.Count) CountCallback = 0;
    }

    //��Ȱ��ȭ ���� �ݹ�
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
