using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class CCSVDictionary : MonoBehaviour
{
    public CRandomHelper m_RandomHelper = null;
    public CScriptable_Language m_Language = null;


    [Header("==================")]
    public SerializeDictionary<uint, ScriptableObject> m_AllScriptables
        = new SerializeDictionary<uint, ScriptableObject>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_Disk> m_AllDisk
        = new SerializeDictionary<uint, CScriptable_Disk>();

    public SerializeDictionary<uint, CScriptable_Disk> m_Disk_Tear1
        = new SerializeDictionary<uint, CScriptable_Disk>();

    public SerializeDictionary<uint, CScriptable_Disk> m_Disk_Tear2
        = new SerializeDictionary<uint, CScriptable_Disk>();

    public SerializeDictionary<uint, CScriptable_Disk> m_Disk_Tear3
        = new SerializeDictionary<uint, CScriptable_Disk>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_CardSkill> m_AllCard
        = new SerializeDictionary<uint, CScriptable_CardSkill>();

    public SerializeDictionary<uint, CScriptable_CardSkill> m_Card_Tear1
        = new SerializeDictionary<uint, CScriptable_CardSkill>();

    public SerializeDictionary<uint, CScriptable_CardSkill> m_Card_Tear2
        = new SerializeDictionary<uint, CScriptable_CardSkill>();

    public SerializeDictionary<uint, CScriptable_CardSkill> m_Card_Tear3
        = new SerializeDictionary<uint, CScriptable_CardSkill>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_EventLog> m_AllEvents
        = new SerializeDictionary<uint, CScriptable_EventLog>();
    public SerializeDictionary<uint, CScriptable_EventLog> m_CH1_Events
        = new SerializeDictionary<uint, CScriptable_EventLog>();
    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_MonsterSkill> m_MonsterSkills
        = new SerializeDictionary<uint, CScriptable_MonsterSkill>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_Monster> m_Monsters
        = new SerializeDictionary<uint, CScriptable_Monster>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_MonsterGroup> m_MonsterGroups_Tear01
        = new SerializeDictionary<uint, CScriptable_MonsterGroup>();
    public SerializeDictionary<uint, CScriptable_MonsterGroup> m_MonsterGroups_Tear02
        = new SerializeDictionary<uint, CScriptable_MonsterGroup>();
    public SerializeDictionary<uint, CScriptable_MonsterGroup> m_MonsterGroups_Tear03
        = new SerializeDictionary<uint, CScriptable_MonsterGroup>();
    public SerializeDictionary<uint, CScriptable_MonsterGroup> m_MonsterGroups_Tear04
        = new SerializeDictionary<uint, CScriptable_MonsterGroup>();
    public SerializeDictionary<uint, CScriptable_MonsterGroup> m_MonsterGroups_Special
        = new SerializeDictionary<uint, CScriptable_MonsterGroup>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_Quest> m_Quests
        = new SerializeDictionary<uint, CScriptable_Quest>();
    public SerializeDictionary<uint, CScriptable_ManaSkill_Area> m_ManaSkills_Area
        = new SerializeDictionary<uint, CScriptable_ManaSkill_Area>();
    public SerializeDictionary<uint, CScriptable_ManaSkill> m_ManaSkills_Use
        = new SerializeDictionary<uint, CScriptable_ManaSkill>();

    [Header("==================")]
    public SerializeDictionary<uint, CScriptable_SceneInfo> m_SceneInfos
        = new SerializeDictionary<uint, CScriptable_SceneInfo>();


    public CUtility.ELanguageTag m_CurrLanguage = CUtility.ELanguageTag.ENG;

    public void ChangeLanguage(int _tag)
    {
        //생성된 instance들은 귀찮아서 안함
        //

        m_CurrLanguage = (CUtility.ELanguageTag)_tag;

        foreach (var it in m_ManaSkills_Use)
        {
            it.Value.m_Data.m_Name =
                m_Language.m_Texts[it.Value.m_Data.m_Name_ID].m_Text[_tag];
            it.Value.m_Data.m_Description =
                m_Language.m_Texts[it.Value.m_Data.m_Desc_ID].m_Text[_tag];
        }
        foreach (var it in m_ManaSkills_Area)
        {
            it.Value.m_Data.m_Name =
                m_Language.m_Texts[it.Value.m_Data.m_Name_ID].m_Text[_tag];
            it.Value.m_Data.m_Description =
                m_Language.m_Texts[it.Value.m_Data.m_Desc_ID].m_Text[_tag];
        }
        foreach (var it in m_Quests)
        {
            it.Value.m_Data.m_Name =
                m_Language.m_Texts[it.Value.m_Data.m_Name_ID].m_Text[_tag];
            it.Value.m_Data.m_Description =
                m_Language.m_Texts[it.Value.m_Data.m_Desc_ID].m_Text[_tag];
        }
        //-------------------------------//
        foreach (var it in m_SceneInfos)
        {
            it.Value.m_Data.m_Name =
                m_Language.m_Texts[it.Value.m_Data.m_Name_ID].m_Text[_tag];
        }
        //-------------------------------//
        foreach (var it in m_AllCard)
        {
            it.Value.m_Data.m_Name =
                    m_Language.m_Texts[it.Value.m_Data.m_Name_ID].m_Text[_tag];
        }
        foreach (var it in m_AllDisk)
        {
            it.Value.m_Data.m_Name =
                    m_Language.m_Texts[it.Value.m_Data.m_Name_ID].m_Text[_tag];
        }
        //-------------------------------//
        foreach (var it in CGameManager.Instance.m_PlayerData.m_Deck) 
        {
            it.m_Data.m_Name =
                    m_Language.m_Texts[it.m_Data.m_Name_ID].m_Text[_tag];
        }
        //-------------------------------//
        foreach (var it in m_AllEvents)
        {
            foreach (var log in it.Value.m_Logs)
                log.m_Log = m_Language.m_Texts[log.m_Log_ID].m_Text[_tag];
        }
        //-------------------------------//
    }



    //티어에 맞는, 사용가능한 디스크 찾음
    public List<CScriptable_Disk> GetDisks_By_Tear(CScriptable_CardSkill _card, int _tear) 
    {
        SerializeDictionary<uint, CScriptable_Disk> temp = null;

        //TODO 3티어 디스크가 없는 관계로 막아둠
        _tear = Mathf.Clamp(_tear, 1, 3);

        if (_tear == 1) temp = m_Disk_Tear1;
        else if (_tear == 2) temp = m_Disk_Tear2;
        else if (_tear == 3) temp = m_Disk_Tear3;

        List<CScriptable_Disk> disks = new List<CScriptable_Disk>();

        Debug.Log("=================");
        Debug.Log(_card.name);

        foreach (var it in temp) 
        {
            if (_card.m_Data.CanAddDisk(it.Value.m_Data) == true) 
            { disks.Add(it.Value); }
        }

        Debug.Log("=================");

        return disks.Count == 0? null : disks;
    }

    //발견력에 따른 카드 얻기
    public CScriptable_CardSkill GetCard_By_Discovery(int _discovery) 
    {
        var res = m_RandomHelper.CardRandom(_discovery);

        List<uint> keys = new List<uint>();

        //TODO : 3티어 카드 없어서 일단 막음
        res = Mathf.Clamp(res, 1, 2);

        switch (res) 
        {
            case 1: keys.AddRange(m_Card_Tear1.Keys); break;
            case 2: keys.AddRange(m_Card_Tear2.Keys);break;
            case 3: keys.AddRange(m_Card_Tear3.Keys); break;
        }

        Debug.Log(keys.Count);
        Debug.Log(keys[Random.Range(0, keys.Count)]);
        return m_AllCard[keys[Random.Range(0, keys.Count)]];
    }

    public CScriptable_MonsterGroup GetMonsterGroup_By_Tear(int _tear) 
    {
        SerializeDictionary<uint, CScriptable_MonsterGroup> temp = null;
        if (_tear == 1) temp = m_MonsterGroups_Tear01;
        else if (_tear == 2) temp = m_MonsterGroups_Tear02;
        else if (_tear == 3) temp = m_MonsterGroups_Tear03;
        else if (_tear == 4) temp = m_MonsterGroups_Tear04;

        List<uint> rand = new List<uint>();
        rand.AddRange(temp.Keys);

        var res = rand[Random.Range(0, rand.Count)];
        return temp[res];
    }

    public CScriptable_MonsterGroup GetMonsterGroup_By_ID(uint _idx) 
    {
        if (m_MonsterGroups_Tear01.ContainsKey(_idx) == true)
            return m_MonsterGroups_Tear01[_idx];
        else if (m_MonsterGroups_Tear02.ContainsKey(_idx) == true)
            return m_MonsterGroups_Tear02[_idx];
        else if (m_MonsterGroups_Tear03.ContainsKey(_idx) == true)
            return m_MonsterGroups_Tear03[_idx];
        else if (m_MonsterGroups_Tear04.ContainsKey(_idx) == true)
            return m_MonsterGroups_Tear04[_idx];
        else if (m_MonsterGroups_Special.ContainsKey(_idx) == true)
            return m_MonsterGroups_Special[_idx];

        return null;
    }

    public uint GetRandomEvent_By_Chapter(int _chapter) 
    {
        List<uint> keys = new List<uint>();

        switch (_chapter)
        {
            case 0:
                keys.AddRange(m_CH1_Events.Keys);
                break;

            default: break;
        }

        return keys[Random.Range(0, keys.Count)];
    }


    /*
    //public SerializeDictionary<Vector2Int, bool> m_Interactions =
    //    new SerializeDictionary<Vector2Int, bool>();


    //상호작용 저장용
    //[System.Serializable]
    //public class CInteraction 
    //{ public List<bool> m_Acts = new List<bool>(); }
    //public SerializeDictionary<CEventCollection.EChapter, CInteraction> m_Interactions 
    //    = new SerializeDictionary<CEventCollection.EChapter, CInteraction>();

    //public SerializeDictionary<uint, CScriptable_Skill> m_Skills
    //    = new SerializeDictionary<uint, CScriptable_Skill>();
    //
    //public SerializeDictionary<uint, CScriptable_Dice> m_DiceTypes
    //    = new SerializeDictionary<uint, CScriptable_Dice>();

    public SerializeDictionary<uint, CScriptable_MonsterSkill> m_MonsterSkills
        = new SerializeDictionary<uint, CScriptable_MonsterSkill>();

    public SerializeDictionary<uint, CScriptable_Monster> m_Monsters
        = new SerializeDictionary<uint, CScriptable_Monster>();

    //public SerializeDictionary<uint, CScriptable_MonsterCombination> m_MonsterCombinations
    //    = new SerializeDictionary<uint, CScriptable_MonsterCombination>();

    public SerializeDictionary<uint, CScriptable_Buff> m_Buffs
        = new SerializeDictionary<uint, CScriptable_Buff>();

    public SerializeDictionary<uint, CScriptable_Buff> m_Debuffs
        = new SerializeDictionary<uint, CScriptable_Buff>();

    public SerializeDictionary<uint, CScriptable_MonsterCombination> m_MonsterCombinations_1Tear
    = new SerializeDictionary<uint, CScriptable_MonsterCombination>();
    public SerializeDictionary<uint, CScriptable_MonsterCombination> m_MonsterCombinations_2Tear
    = new SerializeDictionary<uint, CScriptable_MonsterCombination>();
    public SerializeDictionary<uint, CScriptable_MonsterCombination> m_MonsterCombinations_3Tear
    = new SerializeDictionary<uint, CScriptable_MonsterCombination>();
    public SerializeDictionary<uint, CScriptable_MonsterCombination> m_MonsterCombinations_4Tear
    = new SerializeDictionary<uint, CScriptable_MonsterCombination>();

    public SerializeDictionary<uint, CScriptable_Event> m_EventCombinations_1Tear
    = new SerializeDictionary<uint, CScriptable_Event>();
    public SerializeDictionary<uint, CScriptable_Event> m_EventCombinations_2Tear
    = new SerializeDictionary<uint, CScriptable_Event>();
    public SerializeDictionary<uint, CScriptable_Event> m_EventCombinations_3Tear
    = new SerializeDictionary<uint, CScriptable_Event>();
    public SerializeDictionary<uint, CScriptable_Event> m_EventCombinations_4Tear
    = new SerializeDictionary<uint, CScriptable_Event>();



    public SerializeDictionary<uint, CScriptable_Artifact> m_Artifacts_1Tear
        = new SerializeDictionary<uint, CScriptable_Artifact>();
    public SerializeDictionary<uint, CScriptable_Artifact> m_Artifacts_2Tear
        = new SerializeDictionary<uint, CScriptable_Artifact>();
    public SerializeDictionary<uint, CScriptable_Artifact> m_Artifacts_3Tear
        = new SerializeDictionary<uint, CScriptable_Artifact>();
    public SerializeDictionary<uint, CScriptable_Artifact> m_Artifacts_4Tear
        = new SerializeDictionary<uint, CScriptable_Artifact>();


    public SerializeDictionary<uint, CScriptable_Skill> m_Skills_1Tear
    = new SerializeDictionary<uint, CScriptable_Skill>();
    public SerializeDictionary<uint, CScriptable_Skill> m_Skills_2Tear
    = new SerializeDictionary<uint, CScriptable_Skill>();
    public SerializeDictionary<uint, CScriptable_Skill> m_Skills_3Tear
    = new SerializeDictionary<uint, CScriptable_Skill>();
    public SerializeDictionary<uint, CScriptable_Skill> m_Skills_4Tear
    = new SerializeDictionary<uint, CScriptable_Skill>();


    public CScriptable_Dice m_DefaultDice = null;
    public SerializeDictionary<uint, CScriptable_Dice> m_DiceTypes_1Tear
    = new SerializeDictionary<uint, CScriptable_Dice>();
    public SerializeDictionary<uint, CScriptable_Dice> m_DiceTypes_2Tear
    = new SerializeDictionary<uint, CScriptable_Dice>();
    public SerializeDictionary<uint, CScriptable_Dice> m_DiceTypes_3Tear
    = new SerializeDictionary<uint, CScriptable_Dice>();
    public SerializeDictionary<uint, CScriptable_Dice> m_DiceTypes_4Tear
    = new SerializeDictionary<uint, CScriptable_Dice>();

    public enum ETears
    {
        TEAR01 = 1,
        TEAR02,
        TEAR03,
        TEAR04,
    };
    static public string t = "";

    public List<uint> GetEventCombinationKey_By_Tear(ETears _minTear, ETears _maxTear = ETears.TEAR01)
    {
        if (_minTear > _maxTear) _maxTear = _minTear;
        List<uint> keys = new List<uint>();

        while (_minTear < _maxTear)
        {
            switch (_minTear)
            {
                case ETears.TEAR01: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
                case ETears.TEAR02: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
                case ETears.TEAR03: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
                case ETears.TEAR04: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
            }
            _minTear += 1;
        }

        switch (_maxTear)
        {
            case ETears.TEAR01: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
            case ETears.TEAR02: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
            case ETears.TEAR03: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
            case ETears.TEAR04: keys.AddRange(m_EventCombinations_1Tear.Keys); break;
        }

        return keys;
    }
    public CScriptable_Event GetEventCombination_By_Key(uint _key)
    {
        if (m_EventCombinations_1Tear.ContainsKey(_key)) { return m_EventCombinations_1Tear[_key]; }
        else if (m_EventCombinations_2Tear.ContainsKey(_key)) { return m_EventCombinations_2Tear[_key]; }
        else if (m_EventCombinations_3Tear.ContainsKey(_key)) { return m_EventCombinations_3Tear[_key]; }
        else if (m_EventCombinations_4Tear.ContainsKey(_key)) { return m_EventCombinations_4Tear[_key]; }

        return null;
    }

    public List<uint> GetMonsterCombinationKey_By_Tear(ETears _minTear, ETears _maxTear = ETears.TEAR01)
    {
        if (_minTear > _maxTear) _maxTear = _minTear;
        List<uint> keys = new List<uint>();

        while (_minTear < _maxTear)
        {
            switch (_minTear)
            {//TODO : 현재 1티어 몬스터만 있어서 1티어만 받음
                case ETears.TEAR01: keys.AddRange(m_MonsterCombinations_1Tear.Keys); break;
                case ETears.TEAR02: keys.AddRange(m_MonsterCombinations_2Tear.Keys); break;
                case ETears.TEAR03: keys.AddRange(m_MonsterCombinations_3Tear.Keys); break;
                case ETears.TEAR04: keys.AddRange(m_MonsterCombinations_3Tear.Keys); break;
            }
            _minTear += 1;

        }

        switch (_maxTear)
        {
            case ETears.TEAR01: keys.AddRange(m_MonsterCombinations_1Tear.Keys); break;
            case ETears.TEAR02: keys.AddRange(m_MonsterCombinations_2Tear.Keys); break;
            case ETears.TEAR03: keys.AddRange(m_MonsterCombinations_3Tear.Keys); break;
            case ETears.TEAR04: keys.AddRange(m_MonsterCombinations_3Tear.Keys); break;
        }

        return keys;
    }
    public CScriptable_MonsterCombination GetMonsterCombination_By_Key(uint _key)
    {
        if (m_MonsterCombinations_1Tear.ContainsKey(_key)) { return m_MonsterCombinations_1Tear[_key]; }
        else if (m_MonsterCombinations_2Tear.ContainsKey(_key)) { return m_MonsterCombinations_2Tear[_key]; }
        else if (m_MonsterCombinations_3Tear.ContainsKey(_key)) { return m_MonsterCombinations_3Tear[_key]; }
        else if (m_MonsterCombinations_4Tear.ContainsKey(_key)) { return m_MonsterCombinations_4Tear[_key]; }

        return null;
    }

    public List<uint> GetArtifactKey_By_Tear(ETears _minTear, ETears _maxTear = ETears.TEAR01)
    {
        if (_minTear > _maxTear) _maxTear = _minTear;
        List<uint> keys = new List<uint>();

        while (_minTear < _maxTear)
        {
            switch (_minTear)
            {
                case ETears.TEAR01: keys.AddRange(m_Artifacts_1Tear.Keys); break;
                case ETears.TEAR02: keys.AddRange(m_Artifacts_2Tear.Keys); break;
                case ETears.TEAR03: keys.AddRange(m_Artifacts_3Tear.Keys); break;
                case ETears.TEAR04: keys.AddRange(m_Artifacts_4Tear.Keys); break;
            }
            _minTear += 1;
        }

        switch (_maxTear)
        {
            case ETears.TEAR01: keys.AddRange(m_Artifacts_1Tear.Keys); break;
            case ETears.TEAR02: keys.AddRange(m_Artifacts_2Tear.Keys); break;
            case ETears.TEAR03: keys.AddRange(m_Artifacts_3Tear.Keys); break;
            case ETears.TEAR04: keys.AddRange(m_Artifacts_4Tear.Keys); break;
        }

        //if(keys.)

        CheckIsPlayerAlreadyHave(ref keys);

        return keys;
    }
    public CScriptable_Artifact GetArtifact_By_Key(uint _key)
    {
        if (m_Artifacts_1Tear.ContainsKey(_key)) { return m_Artifacts_1Tear[_key]; }
        else if (m_Artifacts_2Tear.ContainsKey(_key)) { return m_Artifacts_2Tear[_key]; }
        else if (m_Artifacts_3Tear.ContainsKey(_key)) { return m_Artifacts_3Tear[_key]; }
        else if (m_Artifacts_4Tear.ContainsKey(_key)) { return m_Artifacts_4Tear[_key]; }

        return null;
    }


    
    public void CheckIsPlayerAlreadyHave(ref List<uint> _keys) 
    {
        foreach (var it in CGameManager.Instance.m_PlayerData.m_Artifacts)
            _keys.Remove(it.m_Data.m_ID);
    }


    //태그별로 검색함
    List<uint> GetSkillKeys_By_Type(
        SerializeDictionary<uint, CScriptable_Skill> _dic,
        CUtility.ESkillType _skillType = CUtility.ESkillType.ATK)
    {
        switch (_skillType)
        {
            case CUtility.ESkillType.ATK:
            case CUtility.ESkillType.ATK_ALL:
                var atks = _dic.Where(
                x => x.Value.m_Data.m_Type == CUtility.ESkillType.ATK
                || x.Value.m_Data.m_Type == CUtility.ESkillType.ATK_ALL)
                    .ToDictionary(x => x.Key, x => x.Value).Keys;
                return atks.ToList();

            case CUtility.ESkillType.DEF:
                var defs = _dic.Where(
                x => x.Value.m_Data.m_Type == CUtility.ESkillType.DEF)
                    .ToDictionary(x => x.Key, x => x.Value).Keys;
                return defs.ToList();
            case CUtility.ESkillType.BUFF:
                var bufs = _dic.Where(
                x => x.Value.m_Data.m_Type == CUtility.ESkillType.BUFF)
                        .ToDictionary(x => x.Key, x => x.Value).Keys;
                return bufs.ToList();
            case CUtility.ESkillType.ALL:
                return _dic.Keys.ToList();
        }

        return null;
    }

    public List<uint> GetSkillKey_By_Tear(ETears _minTear, ETears _maxTear = ETears.TEAR01,
        CUtility.ESkillType _skillType = CUtility.ESkillType.ALL)
    {
        if (_minTear > _maxTear) _maxTear = _minTear;
        List<uint> keys = new List<uint>();

        while (_minTear < _maxTear)
        {
            switch (_minTear)
            {
                //case ETears.TEAR01: keys.AddRange(m_Skills_1Tear.Keys); break;
                //case ETears.TEAR02: keys.AddRange(m_Skills_2Tear.Keys); break;
                //case ETears.TEAR03: keys.AddRange(m_Skills_3Tear.Keys); break;
                //case ETears.TEAR04: keys.AddRange(m_Skills_4Tear.Keys); break;
                case ETears.TEAR01:
                    keys.AddRange(GetSkillKeys_By_Type(m_Skills_1Tear, _skillType)); break;
                case ETears.TEAR02:
                    keys.AddRange(GetSkillKeys_By_Type(m_Skills_2Tear, _skillType)); break;
                    //CBS  현재 스킬 2티어까지 구상되어 있어서 2티어만 반환
                case ETears.TEAR03:
                    keys.AddRange(GetSkillKeys_By_Type(m_Skills_2Tear, _skillType)); break;
                    //keys.AddRange(GetSkillKeys_By_Type(m_Skills_3Tear, _skillType)); break;
                case ETears.TEAR04:
                    keys.AddRange(GetSkillKeys_By_Type(m_Skills_2Tear, _skillType)); break;
                    //keys.AddRange(GetSkillKeys_By_Type(m_Skills_4Tear, _skillType)); break;
            }
            _minTear += 1;
        }

        switch (_maxTear)
        {
            case ETears.TEAR01:
                keys.AddRange(GetSkillKeys_By_Type(m_Skills_1Tear, _skillType)); break;
            case ETears.TEAR02:
                keys.AddRange(GetSkillKeys_By_Type(m_Skills_2Tear, _skillType)); break;
            case ETears.TEAR03:
                keys.AddRange(GetSkillKeys_By_Type(m_Skills_2Tear, _skillType)); break;
                //keys.AddRange(GetSkillKeys_By_Type(m_Skills_3Tear, _skillType)); break;
            case ETears.TEAR04:
                keys.AddRange(GetSkillKeys_By_Type(m_Skills_2Tear, _skillType)); break;
                //keys.AddRange(GetSkillKeys_By_Type(m_Skills_4Tear, _skillType)); break;
        }

        return keys;
    }

    public CScriptable_Skill GetSkill_By_Key(uint _key)
    {
        if (m_Skills_1Tear.ContainsKey(_key)) { return m_Skills_1Tear[_key]; }
        else if (m_Skills_2Tear.ContainsKey(_key)) { return m_Skills_2Tear[_key]; }
        else if (m_Skills_3Tear.ContainsKey(_key)) { return m_Skills_3Tear[_key]; }
        else if (m_Skills_4Tear.ContainsKey(_key)) { return m_Skills_4Tear[_key]; }

        return null;
    }

    public List<uint> GetDiceKey_By_Tear(ETears _minTear, ETears _maxTear = ETears.TEAR01)
    {
        if (_minTear > _maxTear) _maxTear = _minTear;
        List<uint> keys = new List<uint>();

        while (_minTear < _maxTear)
        {
            switch (_minTear)
            {
                case ETears.TEAR01: keys.AddRange(m_DiceTypes_1Tear.Keys); break;
                case ETears.TEAR02: keys.AddRange(m_DiceTypes_2Tear.Keys); break;
                case ETears.TEAR03: keys.AddRange(m_DiceTypes_3Tear.Keys); break;
                case ETears.TEAR04: keys.AddRange(m_DiceTypes_4Tear.Keys); break;
            }
            _minTear += 1;
        }

        switch (_maxTear)
        {
            case ETears.TEAR01: keys.AddRange(m_DiceTypes_1Tear.Keys); break;
            case ETears.TEAR02: keys.AddRange(m_DiceTypes_2Tear.Keys); break;
            case ETears.TEAR03: keys.AddRange(m_DiceTypes_3Tear.Keys); break;
            case ETears.TEAR04: keys.AddRange(m_DiceTypes_4Tear.Keys); break;
        }

        return keys;
    }

    public CScriptable_Dice GetDice_By_Key(uint _key)
    {
        if (m_DiceTypes_1Tear.ContainsKey(_key)) { return m_DiceTypes_1Tear[_key]; }
        else if (m_DiceTypes_2Tear.ContainsKey(_key)) { return m_DiceTypes_2Tear[_key]; }
        else if (m_DiceTypes_3Tear.ContainsKey(_key)) { return m_DiceTypes_3Tear[_key]; }
        else if (m_DiceTypes_4Tear.ContainsKey(_key)) { return m_DiceTypes_4Tear[_key]; }

        return null;
    }

    public T GetData_Rewordable_OfType<T>(uint _idx) where T : CScriptable_CSVData<T>
    {
             if (m_Artifacts_1Tear.ContainsKey(_idx)) { return m_Artifacts_1Tear[_idx] as T; }
        else if (m_Artifacts_2Tear.ContainsKey(_idx)) { return m_Artifacts_2Tear[_idx] as T; }
        else if (m_Artifacts_3Tear.ContainsKey(_idx)) { return m_Artifacts_3Tear[_idx] as T; }
        else if (m_Artifacts_4Tear.ContainsKey(_idx)) { return m_Artifacts_4Tear[_idx] as T; }
        else if (m_Skills_1Tear.ContainsKey(_idx)) { return m_Skills_1Tear[_idx] as T; }
        else if (m_Skills_2Tear.ContainsKey(_idx)) { return m_Skills_2Tear[_idx] as T; }
        else if (m_Skills_3Tear.ContainsKey(_idx)) { return m_Skills_3Tear[_idx] as T; }
        else if (m_Skills_4Tear.ContainsKey(_idx)) { return m_Skills_4Tear[_idx] as T; }
        else if (_idx == 15001) { return m_DefaultDice as T; }
        else if (m_DiceTypes_1Tear.ContainsKey(_idx)) { return m_DiceTypes_1Tear[_idx] as T; }
        else if (m_DiceTypes_2Tear.ContainsKey(_idx)) { return m_DiceTypes_2Tear[_idx] as T; }
        else if (m_DiceTypes_3Tear.ContainsKey(_idx)) { return m_DiceTypes_3Tear[_idx] as T; }
        else if (m_DiceTypes_4Tear.ContainsKey(_idx)) { return m_DiceTypes_4Tear[_idx] as T; }

        return null;
    }

    public enum EDataID
    {
        NONE = -1,
    }

        return dataList;
    }
    
    //public T GetDataOfType<T>(uint _idx) where T : ScriptableObject
    public T GetDataOfType<T>(uint _idx) where T : CScriptable_CSVData<T>
    {
        T data = m_AllScriptables[_idx] as T;
        return data;
    }

    //ID를 받아 타입으로 변환
    public System.Type  IDtoType(uint _idx) 
    {
        //if (_idx < ((uint)EDataID.ENEMY)) return typeof(CUtility.CData_Skill);
        //else if (_idx < ((uint)EDataID.DROPTABLE)) return typeof(CUtility.CData_Enemy);
        //else if (_idx < ((uint)EDataID.ITEM)) return typeof(CUtility.CData_Drop);
        //else if (_idx < ((uint)EDataID.WEAPON)) return typeof(CUtility.CData_Item);
        //else return typeof(CUtility.CData_Weapon);

        return null;
    }

    //불러오기
    public void LoadDatas() 
    {
        //JsonUtility.FromJson
        string path = Application.dataPath + "/Data/InteractionData.txt";

        if (File.Exists(path) == true)
        {
            string data = File.ReadAllText(path);
            //m_Interactions = JsonUtility.FromJson
            //    <SerializeDictionary<CEventCollection.EChapter, CInteraction>>(data);
            Debug.Log(data);
        }
    }

    //저장
    public void SaveDatas() 
    {
        string path = Application.dataPath + "/Data";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        //string data = JsonUtility.ToJson(m_Interactions);
        //Debug.Log(data);
        //File.WriteAllText(path + "/InteractionData.txt", data);
    }

    */
}
