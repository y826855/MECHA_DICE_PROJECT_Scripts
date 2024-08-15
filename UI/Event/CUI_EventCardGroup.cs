using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_EventCardGroup : MonoBehaviour
{
    public List<CUI_EventCard> m_EventCards = new List<CUI_EventCard>();

    private void Start()
    {
        foreach (var it in m_EventCards)
            it.gameObject.SetActive(false);

        CGameManager.Instance.m_DiceManager.CB_SavedDice = DiceRefresh;
    }

    public void AddAnswer(CUtility.CEventLog _log) 
    {
        foreach (var it in m_EventCards) 
        {
            if (it.gameObject.activeSelf == false) 
            {
                //it.gameObject.SetActive(true);
                it.m_CardAnim.SetEnable();
                it.SetLog(_log);
                break;
            }
        }
    }

    public void DiceRefresh() 
    {
        foreach (var it in m_EventCards) 
        {
            if (it.gameObject.activeSelf == true)
                it.CheckCanUse();
        }
    }

    public void OnCardSubmited() 
    {
        Debug.Log("CARDs DISABLE");
        foreach (var it in m_EventCards)
        {
            if (it.gameObject.activeSelf == true)
                //it.gameObject.SetActive(false);
                it.m_CardAnim.SetDisable();
        }
    }
}
