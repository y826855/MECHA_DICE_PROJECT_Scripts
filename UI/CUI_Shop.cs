using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_Shop : MonoBehaviour
{
    public CUI_Info_ManaSkill pref_Btns = null;

    //card, use, area, quest
    public List<GameObject> m_UI_Windows = new List<GameObject>();
    [SerializeField]int m_CurrWindow_Idx = 0;
    [Header("==========CARD==========")]
    public List<Ctnr_Card> m_Cards = new List<Ctnr_Card>();
    [SerializeField] CUI_CardInfo_Handler m_UI_CardInfo = null;

    [Header("==========USE SKILL==========")]
    public CUI_Info_ManaSkill m_Info_Equip_Use = null;
    public CUI_Info_ManaSkill m_Info_Select_Use = null;
    public List<CUI_Info_ManaSkill> m_Show_Use_Items = new List<CUI_Info_ManaSkill>();
    public Transform m_UseSkills_Loc = null;
    public Button m_Btn_ChangeSkill_Use = null;

    [Header("==========USE AREA==========")]
    public CUI_Info_ManaSkill m_Info_Equip_Area = null;
    public CUI_Info_ManaSkill m_Info_Select_Area = null;
    public List<CUI_Info_ManaSkill> m_Show_Area_Items = new List<CUI_Info_ManaSkill>();
    public Transform m_AreaSkills_Loc = null;
    public Button m_Btn_ChangeSkill_Area = null;

    [Header("==========USE QUEST==========")]
    public CUI_Info_ManaSkill m_Info_Equip_Quest = null;
    public CUI_Info_ManaSkill m_Info_Select_Quest = null;
    public List<CUI_Info_ManaSkill> m_Show_Quest_Items = new List<CUI_Info_ManaSkill>();
    public Transform m_QuestSkills_Loc = null;
    public Button m_Btn_ChangeSkill_Quest = null;

    [Header("====================")]
    public Button m_Btn_Repair = null;
    int m_RepairHP = 0;
    public TMPro.TextMeshProUGUI m_TMP_Repair = null;
    public Button m_Btn_RemoveCard = null;

    bool inited = false;

    private void OnEnable()
    {
        //Init();
        OnClick_Tab(0);
        CGameManager.Instance.m_Input.AddEscape(() => { Escape(); });
    }

    public void SetRepair() 
    {
        m_RepairHP = CGameManager.Instance.m_PlayerData.m_MaxHP / 2;
        m_TMP_Repair.text = string.Format("HP 50%  [{0}] HEAL", m_RepairHP);
        m_Btn_Repair.interactable = true;
        m_Btn_RemoveCard.interactable = true;
    }


    //클릭시 기존 탭 닫고 새탭 열기
    public void OnClick_Tab(int _idx) 
    {
        if (m_CurrWindow_Idx == _idx) return;

        m_UI_Windows[m_CurrWindow_Idx].gameObject.SetActive(false);
        m_CurrWindow_Idx = _idx;
        m_UI_Windows[m_CurrWindow_Idx].gameObject.SetActive(true);
    }

    //카드 랜덤 돌림. 대상 카드 인스턴싱
    public void RandomCards()
    {
        int discovery = CGameManager.Instance.m_PlayerData.m_Discovery;

        var dic = CGameManager.Instance.m_Dictionary;

        foreach (var it in m_Cards)
        { GetRandomCard(it); }
    }

    public void GetRandomCard(Ctnr_Card _card) 
    {
        var dic = CGameManager.Instance.m_Dictionary;
        int discovery = CGameManager.Instance.m_PlayerData.m_Discovery;

        var card = dic.GetCard_By_Discovery(discovery);
        card = Instantiate(card);
        _card.transform.parent.gameObject.SetActive(true);
        _card.m_UI_Card.SetUIData(card);
        _card.m_CB_Submit = OnClick_ShowInfo;


        //발견력 남으면 디스크 추가
        //var remain = discovery - card.m_Data.m_Discovery;
        //discovery = discovery - card.m_Data.m_Discovery;
        AddDisk_By_Discovery(ref card, discovery);

        //골드 표기
        _card.m_TMP.text = string.Format("{0}G", card.GetGoldCost());
    }

    //남는 발견력으로 디스크 추가하기
    public void AddDisk_By_Discovery(ref CScriptable_CardSkill _card, int _disco)
    {
        var dic = CGameManager.Instance.m_Dictionary;

        //발견력이 동날때까지 발견함
        while (_disco >= 20 && _card.m_Disks.Count < 3)
        {
            var diskTear = dic.m_RandomHelper.DiskTearRandom(_disco);
            var disks = dic.GetDisks_By_Tear(_card, diskTear);
            var disk = Instantiate(disks[Random.Range(0, disks.Count)]);
            _card.AddDisk(disk);

            _disco -= diskTear * 20;
        }
    }

    //클릭시 정보창 열기. 선택 버튼도 활성화
    public void OnClick_ShowInfo(CScriptable_CardSkill _card)
    {
        foreach (var it in m_Cards)
        {
            if (it.m_UI_Card.m_SkillCard == _card)
            {
                m_UI_CardInfo.m_FocusCard = _card;
                m_UI_CardInfo.SetData(_instanced: true);
                m_UI_CardInfo.OpenBuyMode();
                break;
            }
        }
    }


    public void Init()
    {
        if (inited == true) return;
        inited = true;

        var player = CGameManager.Instance.m_PlayerData;
        var dic = CGameManager.Instance.m_Dictionary;
        //var all_Use = CGameManager.Instance.m_Dictionary.m_ManaSkills_Use;

        RandomCards();

        for (int i = 0; i < m_AreaSkills_Loc.childCount; i++) 
        { Destroy(m_AreaSkills_Loc.GetChild(0).gameObject); }
        for (int i = 0; i < m_UseSkills_Loc.childCount; i++)
        { Destroy(m_UseSkills_Loc.GetChild(0).gameObject); }
        for (int i = 0; i < m_QuestSkills_Loc.childCount; i++)
        { Destroy(m_QuestSkills_Loc.GetChild(0).gameObject); }

        foreach (var it in player.m_Founded_ManaSkills) 
        { FoundedManaSkill(it); }

        return;

        #region Dont Use
        /*
        foreach (var it in player.m_Founded_ManaSkills)
        { 
            if (dic.m_ManaSkills_Area.ContainsKey(it) == true)
            {//발견된 마나스킬 광역 데이터
                var inst = Instantiate(pref_Btns, m_AreaSkills_Loc);
                var get = dic.m_ManaSkills_Area[it];
                inst.transform.SetSiblingIndex(0); 
                inst.SetData(get);//데이터 뿌리기 and 버튼에 이벤트 달기
                inst.m_Btn.onClick.AddListener(() => { OnClick_Area(get); });
                if (inst.m_ID == player.m_AreaSkill.m_Data.m_ID)
                {//장착중인 놈은 버튼 잠금
                    m_Info_Equip_Area.SetData(player.m_AreaSkill);
                    inst.m_Btn.interactable = false;
                }
            }
            else if (dic.m_ManaSkills_Use.ContainsKey(it) == true)
            {//발견된 마나스킬 사용 데이터
                var inst = Instantiate(pref_Btns, m_UseSkills_Loc);
                var get = dic.m_ManaSkills_Use[it];
                inst.transform.SetSiblingIndex(0);
                inst.SetData(get);//데이터 뿌리기 and 버튼에 이벤트 달기
                inst.m_Btn.onClick.AddListener(() => { OnClick_Use(get); });
                if (inst.m_ID == player.m_UseSkill.m_Data.m_ID)
                {//장착중인 놈은 버튼 잠금
                    m_Info_Equip_Use.SetData(player.m_UseSkill);
                    inst.m_Btn.interactable = false;
                }
            }
            else if (dic.m_Quests.ContainsKey(it) == true)
            {//발견된 퀘스트데이터
                var inst = Instantiate(pref_Btns, m_QuestSkills_Loc);
                var get = dic.m_Quests[it];
                inst.transform.SetSiblingIndex(0);
                inst.SetData(get);//데이터 뿌리기 and 버튼에 이벤트 달기
                inst.m_Btn.onClick.AddListener(() => { OnClick_Quest(get); });
                if (inst.m_ID == player.m_Quest.m_Data.m_ID)
                {//장착중인 놈은 버튼 잠금
                    m_Info_Equip_Quest.SetData(player.m_Quest);
                    inst.m_Btn.interactable = false;
                }
            }
        }

        return;

        var all_Use = CGameManager.Instance.m_Dictionary.m_ManaSkills_Use;
        //일단 데이터 모두 보이게 함
        foreach (var it in all_Use)
        {
            var inst = Instantiate(pref_Btns, m_UseSkills_Loc);
            inst.transform.SetSiblingIndex(0);
            inst.SetData(it.Value);
            inst.m_Btn.onClick.AddListener(()=> { OnClick_Use(it.Value); });
        }
        m_Info_Equip_Use.SetData(player.m_UseSkill);
        m_Info_Select_Use.DataClear();
        //============================================================//
        var all_Area = CGameManager.Instance.m_Dictionary.m_ManaSkills_Area;
        foreach (var it in all_Area)
        {
            var inst = Instantiate(pref_Btns, m_AreaSkills_Loc);
            inst.transform.SetSiblingIndex(0);
            inst.SetData(it.Value);
            inst.m_Btn.onClick.AddListener(() => { OnClick_Area(it.Value); });
        }
        m_Info_Equip_Area.SetData(player.m_AreaSkill);
        m_Info_Select_Area.DataClear();
        //============================================================//
        var all_Quest = CGameManager.Instance.m_Dictionary.m_Quests;
        foreach (var it in all_Quest)
        {
            var inst = Instantiate(pref_Btns, m_QuestSkills_Loc);
            inst.transform.SetSiblingIndex(0);
            inst.SetData(it.Value);
            inst.m_Btn.onClick.AddListener(() => { OnClick_Quest(it.Value); });
        }
        m_Info_Equip_Quest.SetData(player.m_Quest);
        m_Info_Select_Quest.DataClear();
        */
        #endregion
    }

    /*
    public void ChangeCards() 
    {
        int dicov = CGameManager.Instance.m_PlayerData.m_Discovery;

        foreach (var it in m_Cards) 
        {
            var card = CGameManager.Instance.m_Dictionary.GetCard_By_Discovery(dicov);
            card = Instantiate(card);
            it.m_UI_Card.SetUIData(card);
        }
    }
    */
    public void FoundedManaSkill(uint _idx) 
    {
        var player = CGameManager.Instance.m_PlayerData;
        var dic = CGameManager.Instance.m_Dictionary;

        if (dic.m_ManaSkills_Area.ContainsKey(_idx) == true)
        {//발견된 마나스킬 광역 데이터
            var inst = Instantiate(pref_Btns, m_AreaSkills_Loc);
            var get = dic.m_ManaSkills_Area[_idx];
            inst.transform.SetSiblingIndex(0);
            inst.SetData(get);//데이터 뿌리기 and 버튼에 이벤트 달기
            inst.m_Btn.onClick.AddListener(() => { OnClick_Area(get); });
            m_Show_Area_Items.Add(inst);
            if (inst.m_ID == player.m_AreaSkill.m_Data.m_ID)
            {//장착중인 놈은 버튼 잠금
                m_Info_Equip_Area.SetData(player.m_AreaSkill);
                inst.m_Btn.interactable = false;
            }
        }
        else if (dic.m_ManaSkills_Use.ContainsKey(_idx) == true)
        {//발견된 마나스킬 사용 데이터
            var inst = Instantiate(pref_Btns, m_UseSkills_Loc);
            var get = dic.m_ManaSkills_Use[_idx];
            inst.transform.SetSiblingIndex(0);
            inst.SetData(get);//데이터 뿌리기 and 버튼에 이벤트 달기
            inst.m_Btn.onClick.AddListener(() => { OnClick_Use(get); });
            m_Show_Use_Items.Add(inst);
            if (inst.m_ID == player.m_UseSkill.m_Data.m_ID)
            {//장착중인 놈은 버튼 잠금
                m_Info_Equip_Use.SetData(player.m_UseSkill);
                inst.m_Btn.interactable = false;
            }
        }
        else if (dic.m_Quests.ContainsKey(_idx) == true)
        {//발견된 퀘스트데이터
            var inst = Instantiate(pref_Btns, m_QuestSkills_Loc);
            var get = dic.m_Quests[_idx];
            inst.transform.SetSiblingIndex(0);
            inst.SetData(get);//데이터 뿌리기 and 버튼에 이벤트 달기
            inst.m_Btn.onClick.AddListener(() => { OnClick_Quest(get); });
            m_Show_Quest_Items.Add(inst);
            if (inst.m_ID == player.m_Quest.m_Data.m_ID)
            {//장착중인 놈은 버튼 잠금
                m_Info_Equip_Quest.SetData(player.m_Quest);
                inst.m_Btn.interactable = false;
            }
        }
    }

    public void OnClick_Use(CScriptable_ManaSkill _get) 
    { 
        m_Info_Select_Use.SetData(_get);
        m_Btn_ChangeSkill_Use.interactable = true;
    }
    public void OnClick_Area(CScriptable_ManaSkill_Area _get)
    { 
        m_Info_Select_Area.SetData(_get);
        m_Btn_ChangeSkill_Area.interactable = true;
    }
    public void OnClick_Quest(CScriptable_Quest _get)
    { 
        m_Info_Select_Quest.SetData(_get);
        m_Btn_ChangeSkill_Quest.interactable = true;
    }

    public void OnClick_Repair() 
    {
        var player = CGameManager.Instance.m_PlayerData;
        if (player.m_GOLD < 150) return;
        m_Btn_Repair.interactable = false;

        player.AddHP(m_RepairHP);
        player.AddGold(-150);
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_StoreBuy);
    }


    public void OnClick_RemoveCard() 
    {
        Debug.Log("REMOVE CARD!!");


        var deck = CGameManager.Instance.m_ScheduleMgr.m_UI_Deck;
        deck.m_State_ShowType = CUI_Deck_Shower.EShowType.REMOVE;
        deck.m_CB_CardEvent = CardRemoveDone;
        deck.gameObject.SetActive(true);
    }

    public void CardRemoveDone() 
    {
        var deck = CGameManager.Instance.m_ScheduleMgr.m_UI_Deck;
        CGameManager.Instance.m_PlayerData.AddGold(-150);

        m_Btn_RemoveCard.interactable = false;
        deck.Escape();
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_StoreBuy);
    }

    /// 마나스킬 장비 변경
    public void OnClick_Change_Quest() 
    { ChangeQuest(m_Info_Select_Quest.m_ID); }
    public void ChangeQuest(uint _idx) 
    {
        if (_idx == 0) return;

        var quest = CGameManager.Instance.m_Dictionary.m_Quests[_idx];
        CGameManager.Instance.m_PlayerData.m_Quest = quest;

        foreach (var it in m_Show_Quest_Items)
        {
            if (it.m_ID == quest.m_Data.m_ID) it.m_Btn.interactable = false;
            else if (it.m_ID == m_Info_Equip_Quest.m_ID) it.m_Btn.interactable = true;
        }

        m_Info_Equip_Quest.SetData(quest);
        m_Btn_ChangeSkill_Quest.interactable = false;
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_StoreBuy);
    }
    public void OnClick_Change_Use()
    { ChangeUse(m_Info_Select_Use.m_ID); }
    public void ChangeUse(uint _idx) 
    {
        if (_idx == 0) return;

        var use = CGameManager.Instance.m_Dictionary.m_ManaSkills_Use[_idx];
        CGameManager.Instance.m_PlayerData.m_UseSkill = use;

        foreach (var it in m_Show_Use_Items)
        {
            if (it.m_ID == use.m_Data.m_ID) it.m_Btn.interactable = false;
            else if (it.m_ID == m_Info_Equip_Use.m_ID) it.m_Btn.interactable = true;
        }

        m_Info_Equip_Use.SetData(use);
        m_Btn_ChangeSkill_Use.interactable = false;
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_StoreBuy);
    }
    public void OnClick_Change_Area()
    { ChangeArea(m_Info_Select_Area.m_ID); }
    public void ChangeArea(uint _idx) 
    {
        if (_idx == 0) return;

        var area = CGameManager.Instance.m_Dictionary.m_ManaSkills_Area[_idx];
        CGameManager.Instance.m_PlayerData.m_AreaSkill = area;

        foreach (var it in m_Show_Area_Items)
        {
            if (it.m_ID == area.m_Data.m_ID) it.m_Btn.interactable = false;
            else if (it.m_ID == m_Info_Equip_Area.m_ID) it.m_Btn.interactable = true;
        }

        m_Info_Equip_Area.SetData(area);
        m_Btn_ChangeSkill_Area.interactable = false;
    }
    /////////////////////////////////
    

    public void OnClick_BuyCard(CScriptable_CardSkill _card) 
    {
        CGameManager.Instance.m_PlayerData.AddGold(-_card.GetGoldCost());

        //카드 리필
        foreach (var it in m_Cards)
        {
            if (it.m_UI_Card.m_SkillCard == _card) 
            { Debug.Log("refill"); GetRandomCard(it); break; }
        }

        Debug.Log("buy card");
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.S_StoreBuy);
    }

    public void Escape()
    {
        this.gameObject.SetActive(false);
    }
}
