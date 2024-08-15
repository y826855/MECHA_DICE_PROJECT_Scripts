using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerData : MonoBehaviour
{
    public List<CScriptable_CardSkill> m_TEST_Deck = new List<CScriptable_CardSkill>();
    public List<CScriptable_CardSkill> m_Deck = new List<CScriptable_CardSkill>();
    public List<CScriptable_ManaSkill_Area> m_ManaSkills = new List<CScriptable_ManaSkill_Area>();
    public List<CScriptable_SceneInfo> m_DaysBag = new List<CScriptable_SceneInfo>();
    
    public List<CScriptable_SceneInfo> m_SubmitedWeek = new List<CScriptable_SceneInfo>();
    public int m_DayOnWeek = 0;

    [SerializeField] int daysBagMax = 7;

    public int m_MaxDeckSize = 20;
    public int m_HP = 0;
    public int m_MaxHP = 0;
    public int m_GOLD = 0;
    public int m_MaxMana = 20;
    public int m_RegenMana = 5;
    public int m_Discovery = 0; //발견력
    public int m_RollMax = 3;


    [Header("==============EQUIP==============")]
    public CScriptable_ManaSkill m_UseSkill = null;
    public CScriptable_ManaSkill_Area m_AreaSkill = null;
    public CScriptable_Quest m_Quest = null;

    [Header("==============SHOP UNLOCKED==============")]
    public List<uint> m_UnFouned_ManaSkills = new List<uint>();
    public List<uint> m_Founded_ManaSkills = new List<uint>();

    //TODO : 언젠가 지워야함
    //게임 시작시 기본덱 받기
    // or 저장된 덱 불러오기

    public void CloneData(CPlayerData _default) 
    {
        this.m_MaxDeckSize = _default.m_MaxDeckSize;
        this.m_HP = _default.m_HP;
        this.m_MaxHP = _default.m_MaxHP;
        this.m_GOLD = _default.m_GOLD;
        this.m_MaxMana = _default.m_MaxMana;
        this.m_RegenMana = _default.m_RegenMana;
        this.m_Discovery = _default.m_Discovery;
        this.m_RollMax = _default.m_RollMax;

        this.m_UseSkill = _default.m_UseSkill;
        this.m_AreaSkill = _default.m_AreaSkill;
        this.m_Quest = _default.m_Quest;

        m_DaysBag.Clear();
        m_DayOnWeek = 0;


        InitUserData();
    }

    public void InitUserData() 
    {
        m_HP = m_MaxHP;

        m_UnFouned_ManaSkills.Clear();
        m_Founded_ManaSkills.Clear();

        var dic = CGameManager.Instance.m_Dictionary;

        m_UnFouned_ManaSkills.AddRange(dic.m_ManaSkills_Area.Keys);
        m_UnFouned_ManaSkills.AddRange(dic.m_ManaSkills_Use.Keys);
        m_UnFouned_ManaSkills.AddRange(dic.m_Quests.Keys);

        //시작 스킬
        m_Founded_ManaSkills.Add(m_UseSkill.m_Data.m_ID);
        m_UnFouned_ManaSkills.Remove(m_UseSkill.m_Data.m_ID);
        m_Founded_ManaSkills.Add(m_AreaSkill.m_Data.m_ID);
        m_UnFouned_ManaSkills.Remove(m_AreaSkill.m_Data.m_ID);
        m_Founded_ManaSkills.Add(m_Quest.m_Data.m_ID);
        m_UnFouned_ManaSkills.Remove(m_Quest.m_Data.m_ID);
    }
    public bool IsDayBagFull()
    { return m_SubmitedWeek.Count >= daysBagMax; }

    //마나스킬 받아오고 삭제함
    public uint GetRandom_ManaSkill()
    {
        //디버그용 초기화
        //if (m_UnFouned_ManaSkills.Count == 0)
        //    InitUserData();

        var random = Random.Range(0, m_UnFouned_ManaSkills.Count);
        var res = m_UnFouned_ManaSkills[random];
        m_UnFouned_ManaSkills.RemoveAt(random);
        m_Founded_ManaSkills.Add(res);

        CGameManager.Instance.m_ScheduleMgr.m_UI_Shop.FoundedManaSkill(res);

        return res;
    }

    public void AddGold(int _calc) 
    {
        m_GOLD += _calc;
        if (m_GOLD < 0) m_GOLD = 0;

        var tmp = CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar.m_TMP_GOLD;
        tmp.text = string.Format("{0}G", m_GOLD);
    }

    public void AddHP(int _calc) 
    {
        m_HP += _calc;
        m_HP = Mathf.Clamp(m_HP, 0, m_MaxHP);

        var tmp = CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar.m_TMP_HP;
        tmp.text = string.Format("{0}/{1}", m_HP, m_MaxHP);
    }

    public void SetHP(int _hp) 
    {
        m_HP = _hp;
        var tmp = CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar.m_TMP_HP;
        tmp.text = string.Format("{0}/{1}", m_HP, m_MaxHP);
    }

    public void AddMaxHP(int _get) 
    {
        m_MaxHP += _get;
        if (_get > 0)
        { m_HP += _get; }
        if (m_HP > m_MaxHP) m_HP = m_MaxHP;

        var tmp = CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar.m_TMP_HP;
        tmp.text = string.Format("{0}/{1}", m_HP, m_MaxHP);
    }

    public void AddMaxMana(int _get) 
    { 
        m_MaxMana += _get; 
    }

    public void TestSpawn() 
    {
        m_Deck.Clear();

        for (int i = 0; i < m_TEST_Deck.Count; i++)
            m_Deck.Add(Instantiate(m_TEST_Deck[i]));

        Debug.Log("PLAYER TEST DECK SPAWNED");
    }
}
