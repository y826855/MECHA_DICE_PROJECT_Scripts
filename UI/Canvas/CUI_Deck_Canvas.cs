using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// /////////////// 안쓰는 클래스 CUI_Deck_Shower 으로 대체함
/// </summary>

public class CUI_Deck_Canvas : MonoBehaviour
{
    public CPlayerChar m_Player = null;
    public Ctnr_Card m_Pref_SkillUI = null;

    public List<Ctnr_Card> m_DrawDeck = new List<Ctnr_Card>();
    public List<Ctnr_Card> m_UsedDeck = new List<Ctnr_Card>();

    public Transform m_UsedDeck_Parent = null;
    public Transform m_DrawDeck_Parent = null;

    public CUI_CardInfo_Handler m_CardInfo_Handler = null;

    private void OnEnable()
    {
        if (m_Player == null) return;

        for (int i = 0; i < m_DrawDeck.Count; i++)
        {
            if (i < m_Player.m_DrawDeck.Count)
            {
                m_DrawDeck[i].gameObject.SetActive(true);
                m_DrawDeck[i].m_UI_Card.SetUIData(m_Player.m_DrawDeck[i]);
            }
            else m_DrawDeck[i].gameObject.SetActive(false);

            if (i < m_Player.m_UsedDeck.Count)
            {
                m_UsedDeck[i].gameObject.SetActive(true);
                m_UsedDeck[i].m_UI_Card.SetUIData(m_Player.m_UsedDeck[i]);
            }
            else m_UsedDeck[i].gameObject.SetActive(false);
        }


    }

    public void CreateDeck()
    {
        Vector3 localScale = new Vector3(1.3f, 1.3f, 1.3f);
        foreach (var it in m_Player.m_DrawDeck)
        {
            var instDraw = Instantiate(m_Pref_SkillUI, m_DrawDeck_Parent);
            instDraw.m_UI_Card.SetUIData(it);
            //instDraw.m_UI_Card.m_IsOnDeck = true;
            instDraw.m_UI_Card.m_CardState = CUI_SkillCard.EState.DECK;
            instDraw.m_CB_Submit = OnClick_ShowInfo;
            m_DrawDeck.Add(instDraw);

            var instUsed = Instantiate(m_Pref_SkillUI, m_UsedDeck_Parent);
            instUsed.m_UI_Card.SetUIData(it);
            //instUsed.m_UI_Card.m_IsOnDeck = true;
            instUsed.m_UI_Card.m_CardState = CUI_SkillCard.EState.DECK;
            instUsed.m_CB_Submit = OnClick_ShowInfo;

            m_UsedDeck.Add(instUsed);
        }
    }

    public void UpdateDeck() 
    {
        
    }

    public void OnClick_ShowInfo(CScriptable_CardSkill _card) 
    {
        if (m_CardInfo_Handler.gameObject.activeSelf == true) return;

        m_CardInfo_Handler.gameObject.SetActive(true);
        m_CardInfo_Handler.m_FocusCard = _card;
        m_CardInfo_Handler.SetData();
    }


}
