using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_Deck : MonoBehaviour
{
    public Transform m_UsedDeck_Parent = null;
    public Transform m_DrawDeck_Parent = null;

    public CPlayerChar m_Player = null;
    public CUI_SkillCard m_Pref_SkillUI = null;

    public List<CUI_SkillCard> m_DrawDeck = new List<CUI_SkillCard>();
    public List<CUI_SkillCard> m_UsedDeck = new List<CUI_SkillCard>();

    public GameObject m_ParentWindow = null;

    private void OnEnable()
    {
        for (int i = 0; i < m_DrawDeck.Count; i++)
        {
            if (i < m_Player.m_DrawDeck.Count)
            {
                m_DrawDeck[i].gameObject.SetActive(true);
                m_DrawDeck[i].SetUIData(m_Player.m_DrawDeck[i]);
            }
            else m_DrawDeck[i].gameObject.SetActive(false);

            if (i < m_Player.m_UsedDeck.Count) 
            {
                m_UsedDeck[i].gameObject.SetActive(true);
                m_UsedDeck[i].SetUIData(m_Player.m_UsedDeck[i]);
            }
            else m_UsedDeck[i].gameObject.SetActive(false);
        }

    }

    private void OnDisable()
    {

    }


    public void CreateDeck() 
    {
        foreach (var it in m_Player.m_DrawDeck) 
        {
            var instDraw = Instantiate(m_Pref_SkillUI, m_DrawDeck_Parent);
            instDraw.SetUIData(it);
            //instDraw.m_IsOnDeck = true;
            instDraw.m_CardState = CUI_SkillCard.EState.DECK;
            //instDraw.m_Btn_ChangeCard.gameObject.SetActive(false);
            m_DrawDeck.Add(instDraw);

            var instUsed = Instantiate(m_Pref_SkillUI, m_UsedDeck_Parent);
            instUsed.SetUIData(it);
            instUsed.m_CardState = CUI_SkillCard.EState.DECK;
            //instUsed.m_IsOnDeck = true;
            //instUsed.m_Btn_ChangeCard.gameObject.SetActive(false);
            m_UsedDeck.Add(instUsed);

        }
    }
}
