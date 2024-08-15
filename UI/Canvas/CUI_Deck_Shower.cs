using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Deck_Shower : MonoBehaviour
{
    public Ctnr_Card m_Pref_SkillUI = null;

    public List<Ctnr_Card> m_Deck = new List<Ctnr_Card>();
    public Transform m_Deck_Parent = null;

    public CUI_CardInfo_Handler m_CardInfo_Handler = null;

    public enum EShowState { ALL = 0, DRAW = 1, USED = 2 };
    public EShowState m_ShowState = EShowState.ALL;

    public GameObject m_Btn_Group = null;
    public GameObject m_TMP_Remove = null;

    [System.Serializable]
    public enum EShowType { DISPLAY = 0, BATTLE, REMOVE };
    public EShowType m_State_ShowType = EShowType.DISPLAY;

    public System.Action m_CB_CardEvent = null;

    public void OnEnable()
    {
        CGameManager.Instance.m_Input.AddEscape(() => { Escape(); });

        //CreateDeck();
        ShowCard();

        switch (m_State_ShowType) 
        {
            case EShowType.DISPLAY:
                m_Btn_Group.gameObject.SetActive(false);
                m_TMP_Remove.gameObject.SetActive(false);
                break;
            case EShowType.BATTLE:
                m_Btn_Group.gameObject.SetActive(true);
                m_TMP_Remove.gameObject.SetActive(false);
                break;
            case EShowType.REMOVE:
                m_Btn_Group.gameObject.SetActive(false);
                m_TMP_Remove.gameObject.SetActive(true);
                break;
        }
    }

    //카드를 dictionary로 관리해서 정렬필요할듯

    //상태에 따른 카드 보여주기
    public void ShowCard() 
    {
        switch (m_ShowState) 
        {
            case EShowState.ALL:
                foreach (var it in m_Deck)
                { if (it.gameObject.activeSelf == false) it.gameObject.SetActive(true); }
                break;
            case EShowState.DRAW:
                CheckCardState(CScriptable_CardSkill.ECardState.DECK);
                break;
            case EShowState.USED:
                CheckCardState(CScriptable_CardSkill.ECardState.USED);
                break;
        }
    }

    //카드 상태 체크 후 가시화
    void CheckCardState(CScriptable_CardSkill.ECardState _state) 
    {
        foreach (var it in m_Deck)
        {
            if (it.m_UI_Card.m_SkillCard.m_DeckState == _state)
            {
                if (it.gameObject.activeSelf == false)
                    it.gameObject.SetActive(true);
            }
            else
            {
                if (it.gameObject.activeSelf == true)
                    it.gameObject.SetActive(false);
            }
        }
    }

    //Vector3 localScale = Vector3.one * 1.3f;
    Vector3 localScale = Vector3.one;

    //덱 생성
    public void CreateDeck() 
    {
        if (m_Deck.Count > 0) return;

        var playerData = CGameManager.Instance.m_PlayerData;

        Debug.Log("DECK CREATED");
        foreach (var it in playerData.m_Deck)
        {
            it.Spawn(null);

            var instDraw = Instantiate(m_Pref_SkillUI, m_Deck_Parent);
            instDraw.m_UI_Card.SetUIData(it);
            //Debug.Log(it);
            instDraw.m_UI_Card.m_CardState = CUI_SkillCard.EState.DECK;
            //instDraw.m_UI_Card.m_IsOnDeck = true;
            instDraw.m_CB_Submit = OnClick_ShowInfo;
            instDraw.transform.localScale = localScale;
            m_Deck.Add(instDraw);
        }
    }

    //디스크 업데이트
    public void UpdateDiskData(CScriptable_CardSkill _card) 
    {
        foreach (var it in m_Deck) 
        {
            if (it.m_UI_Card.m_SkillCard == _card)
            {
                it.m_UI_Card.ShowDiskData();
                break;
            }
        }

        
    }


    void InstanceCardUI(CScriptable_CardSkill _card) 
    {
        var instDraw = Instantiate(m_Pref_SkillUI, m_Deck_Parent);
        instDraw.m_UI_Card.SetUIData(_card);
        instDraw.m_UI_Card.m_CardState = CUI_SkillCard.EState.DECK;
        //instDraw.m_UI_Card.m_IsOnDeck = true;
        instDraw.m_CB_Submit = OnClick_ShowInfo;
        instDraw.transform.localScale = localScale;
        m_Deck.Add(instDraw);
    }

    //카드 정보 보기
    public void OnClick_ShowInfo(CScriptable_CardSkill _card)
    {
        if (m_CardInfo_Handler.gameObject.activeSelf == true) return;

        m_CardInfo_Handler.gameObject.SetActive(true);
        m_CardInfo_Handler.m_FocusCard = _card;
        m_CardInfo_Handler.SetData(true);

        if(m_State_ShowType == EShowType.REMOVE)
            m_CardInfo_Handler.m_BtnRemove.SetActive(true);
    }

    //TODO : 카드 추가 여기서 해야함 플레이어 카드 추가 벤
    public void AddCard(CScriptable_CardSkill _get) 
    {
        CGameManager.Instance.m_PlayerData.m_Deck.Add(_get);
        InstanceCardUI(_get);
    }

    public void RemoveCard(CScriptable_CardSkill _get) 
    {
        CGameManager.Instance.m_PlayerData.m_Deck.Remove(_get);
        if (m_CB_CardEvent != null) m_CB_CardEvent();

        foreach (var it in m_Deck) 
        {
            if (it.m_UI_Card.m_SkillCard == _get) 
            {
                var target = it;
                m_Deck.Remove(target);
                Destroy(target.m_UI_Card.m_SkillCard);
                Destroy(target.gameObject);
                break;
            }
        }
    }

    //보기 상태 변경
    public void OnClick_ChangeState(int _state) 
    {
        m_ShowState = (EShowState)_state;
        ShowCard();
    }

    private void OnDisable()
    {
        //m_RemoveMode = false;
        if (m_State_ShowType == EShowType.REMOVE)
            m_State_ShowType = EShowType.DISPLAY;

        m_CB_CardEvent = null;
    }

    public void Escape()
    {
        this.gameObject.SetActive(false);
    }
}
