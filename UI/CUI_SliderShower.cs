using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_SliderShower : MonoBehaviour
{
    public List<GameObject> m_Contents = new List<GameObject>();
    public int m_Idx = 0;

    private void OnEnable()
    {
        CGameManager.Instance.m_Input.AddEscape(() => Escape());
        m_Contents[m_Idx].SetActive(true);
    }

    public void Escape() 
    {
        this.gameObject.SetActive(false);
    }

    private void OnDisable()
    {

    }

    public void OnClick_Left() 
    {
        m_Contents[m_Idx].SetActive(false);
        m_Idx--;
        if (m_Idx < 0) m_Idx = m_Contents.Count - 1;
        m_Contents[m_Idx].SetActive(true);
    }
    public void OnClick_Right() 
    {
        m_Contents[m_Idx].SetActive(false);
        m_Idx++;
        if (m_Idx >= m_Contents.Count) m_Idx = 0;
        m_Contents[m_Idx].SetActive(true);
    }
}
