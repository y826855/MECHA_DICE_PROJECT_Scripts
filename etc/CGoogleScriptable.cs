using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Networking;


#if UNITY_EDITOR
using Unity.EditorCoroutines.Editor;

//[CustomEditor(typeof(CGoogleSheet))]
//public class CGoogleScriptable_Editor : Editor
public class CGoogleScriptable : CScriptableSingletone<CGoogleScriptable>
{
    public const string url = "https://docs.google.com/spreadsheets/d/1GXjEbJy5ifMaFPk6X4lC0_gmtZZrlPKmWLO-AhdFCcc/export?format=tsv";

    public override void BeforeLoadInstance()
    {
        //throw new System.NotImplementedException();
    }


    [Header("=================================================")]
    public string sheet_Card = "&gid=0";
    public string range_Card = "&range=A2:Q";

    [Header("=================================================")]
    public string sheet_Disk = "&gid=286912653";
    public string range_Disk = "&range=A2:N";

    [Header("=================================================")]
    public string sheet_Event = "&gid=1434223314";
    public string range_Event = "&range=A2:F21";

    [Header("=================================================")]
    public string sheet_MonsterSkill = "&gid=1434223314";
    public string range_MonsterSkill = "&range=A2:F21";

    [Header("=================================================")]
    public string sheet_Monster = "&gid=1434223314";
    public string range_Monster = "&range=A2:F21";

    [Header("=================================================")]
    public string sheet_Monster_Group = "&gid=1024300534";
    public string range_Monster_Group = "&range=A2:L";

    [Header("=================================================")]
    public string sheet_ManaSkill_Quest = "&gid=1561941412";
    public string range_ManaSkill_Quest = "&range=A2:J6";

    [Header("=================================================")]
    public string sheet_ManaSkill_Area = "&gid=1024300534";
    public string range_ManaSkill_Area = "&range=A2:L";

    [Header("=================================================")]
    public string sheet_ManaSkill_Use = "&gid=1024300534";
    public string range_ManaSkill_Use = "&range=A2:L";

    [Header("=================================================")]
    public string sheet_SceneInfo = "&gid=1401515475";
    public string range_SceneInfo = "&range=A2:G";

    [Header("=================================================")]
    public string sheet_Language = "&gid=1219428217";
    public string range_Language = "&range=A2:C";

    public static EditorCoroutine editorCoroutine;

    //[MenuItem("Assets/Get GoogleData/All")]
    //public static void GetGoogleData_ALL()
    //{
    //    Debug.Log("지금은 작동하지 않습니다. 위에서부터 수동으로 하나씩 받으세요");
    //    //GetBuffTable();
    //    //GetDebuffTable();
    //}


    //====================================================================================//
    [MenuItem("Assets/Get GoogleData/Event")]
    public static void GetEventType() 
    {
        editorCoroutine =
        EditorCoroutineUtility.StartCoroutine(Instance.CoGetEventType(),
        CGoogleScriptable.Instance);
    }
    public IEnumerator CoGetEventType()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_Event, Instance.range_Event);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();


        string data = www.downloadHandler.text;
        tsvData = data;

        CGameManager.Instance.m_Dictionary.m_AllEvents.Clear();
        CGameManager.Instance.m_Dictionary.m_CH1_Events.Clear();


        var lines = tsvData.Split("\n");
        Debug.Log(lines.Length);
        //foreach (var it in lines)
        for (int i = 0; i < lines.Length; i++) 
        {
            var it = lines[i];
            var datas = it.Split("\t");
            int idx = 0;

            Debug.Log(datas[idx]);
            uint id = CSVParseUINT(datas[idx++]);
            //string name = datas[idx++];
            Debug.Log(datas[idx]);
            //uint name = CSVParseUINT(datas[idx++]);
            string name = datas[idx++];
            Debug.Log(datas[idx]);
            int chapter = CSVParseINT(datas[idx++]);
            int maxLog = CSVParseINT(datas[idx++]);
            int canRandom = CSVParseINT(datas[idx++]);

            var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

            List<CUtility.CEventLog> logs = new List<CUtility.CEventLog>();
            int currIdx = i + 1;
            for (i = i+1; i <= currIdx + maxLog; i++) 
            {
                var logLine = lines[i];
                var logDatas = logLine.Split("\t");
                int logIdx = 0;
                CUtility.CEventLog log = new CUtility.CEventLog();

                //TODO : 조건 따로 받아야함
                string term = logDatas[logIdx++];
                var terms = term.Trim().Split(",");
                if (term.Length <= 0) log.m_Term = null;
                else
                { log.m_Term = GetTerms(terms); }
                
                log.m_Speaker = logDatas[logIdx++];

                log.m_Idx = CSVParseINT(logDatas[logIdx++]);
                log.m_Log_ID = CSVParseUINT(logDatas[logIdx++]);
                log.m_Log = texts[log.m_Log_ID].m_Text[0];
                ///log.m_Log = logDatas[logIdx++];

                string connections = logDatas[logIdx++].Trim();
                if (connections != "") CSVParseInt_List(ref log.m_Connection, connections);

                //보상 추가
                string reward = logDatas[logIdx++].Trim();
                if (reward != "") {
                    var rewards = reward.Trim().Split(",");
                    Debug.Log(rewards.Length);
                    for (int rw = 0; rw < rewards.Length; rw += 2) 
                    {
                        var type = CSVParseENUM<CUtility.ERewardable>(rewards[rw]);
                        var num = CSVParseINT(rewards[rw + 1]);
                        log.m_AddRewards.Add(type, num);
                    }
                }
                else log.m_AddRewards = null;

                logs.Add(log);
            } i--;

            var inst = CScriptable_EventLog.CreatePrefab(logs, 
                name, maxLog, chapter);
            inst.m_ID = id;

            Debug.Log(name + id);
            switch (chapter) 
            {//챕터별 입력
                case 1:
                    if (canRandom == 0) //랜덤에서 등장할건지 체크
                        CGameManager.Instance.m_Dictionary.m_CH1_Events.Add(id, inst);
                    break;
            }

            CGameManager.Instance.m_Dictionary.m_AllEvents.Add(id, inst);
        }
    }

    public List<CUtility.SEventTerm> GetTerms(string[] _terms) 
    {
        var terms = new List<CUtility.SEventTerm>();
        for (int i = 0; i < _terms.Length; i += 2)
        {
            var term = CSVParseENUM<CUtility.EEventTerm>(_terms[i]);
            var num = CSVParseINT(_terms[i + 1]);
            terms.Add(new CUtility.SEventTerm(term, num));
        }
        return terms;
    }


    //====================================================================================//

    [MenuItem("Assets/Get GoogleData/DISK")]
    public static void GetDiskType()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetDiskType(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetDiskType()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_Disk, Instance.range_Disk);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var lines = tsvData.Split("\n");
        //var DiceTypeList = CGameManager.Instance.m_Dictionary.m_DiceTypes;
        var AllDisk = CGameManager.Instance.m_Dictionary.m_AllDisk;
        var DiskList_1tear = CGameManager.Instance.m_Dictionary.m_Disk_Tear1;
        var DiskList_2tear = CGameManager.Instance.m_Dictionary.m_Disk_Tear2;
        var DiskList_3tear = CGameManager.Instance.m_Dictionary.m_Disk_Tear3;
        AllDisk.Clear();
        DiskList_1tear.Clear();
        DiskList_2tear.Clear();
        DiskList_3tear.Clear();

        var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

        foreach (var it in lines)
        {
            CUtility.CDisk temp = new CUtility.CDisk();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = texts[temp.m_Name_ID].m_Text[(int)CUtility.ELanguageTag.ENG];
            //temp.m_Name = datas[idx++];
            temp.m_Type = CSVParseENUM<CUtility.ECardType>(datas[idx++]);
            temp.m_Tear = CSVParseINT(datas[idx++]);

            temp.m_Damage.SetDataFromTSV(datas[idx++]);
            temp.m_StatusEff.SetDataFromTSV(datas[idx++]);
            temp.m_Targets.SetDataFromTSV(datas[idx++]);

            temp.m_Defend.SetDataFromTSV(datas[idx++]);
            temp.m_Mana.SetDataFromTSV(datas[idx++]);

            //TODO : 디버프 통합?
            temp.m_Debuff = CSVParseINT(datas[idx++]);
            //temp.m_Burn_Self = CSVParseINT(datas[idx++]);
            //temp.m_Elec_Self = CSVParseINT(datas[idx++]);
            //temp.m_Damage_Self= CSVParseINT(datas[idx++]);
            temp.m_Return = bool.Parse(datas[idx++]);

            var inst = CScriptable_Disk.CreatePrefab(temp);
            
            if (temp.m_Tear <= 1) DiskList_1tear.Add(temp.m_ID, inst);
            else if (temp.m_Tear == 2) DiskList_2tear.Add(temp.m_ID, inst);
            else if (temp.m_Tear == 3) DiskList_3tear.Add(temp.m_ID, inst);
            AllDisk.Add(temp.m_ID, inst);
        }
        Debug.Log("LOAD DISK FROM GOOGLE SHEET");
    }

    //====================================================================================//
    [MenuItem("Assets/Get GoogleData/SKILL_CARD")]
    public static void GetSkillCardType()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetSkillCardType(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetSkillCardType()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_Card, Instance.range_Card);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var lines = tsvData.Split("\n");

        var AllCard = CGameManager.Instance.m_Dictionary.m_AllCard;
        var CardList_1tear = CGameManager.Instance.m_Dictionary.m_Card_Tear1;
        var CardList_2tear = CGameManager.Instance.m_Dictionary.m_Card_Tear2;
        var CardList_3tear = CGameManager.Instance.m_Dictionary.m_Card_Tear3;
        AllCard.Clear();
        CardList_1tear.Clear();
        CardList_2tear.Clear();
        CardList_3tear.Clear();

        var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

        foreach (var it in lines) 
        {
            CUtility.CSkillCard temp = new CUtility.CSkillCard();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            //temp.m_Name = datas[idx++];
            temp.m_Name_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = texts[temp.m_Name_ID].m_Text[0];
            temp.m_CardType = CSVParseENUM<CUtility.ECardType>(datas[idx++]);
            temp.m_GenType = CSVParseENUM<CUtility.EATK_GenType>(datas[idx++]);

            temp.m_Damage = CSVParseINT(datas[idx++]);
            temp.m_Defend = CSVParseINT(datas[idx++]);
            temp.m_StatusEff = CSVParseINT(datas[idx++]);

            temp.m_Targets = CSVParseINT(datas[idx++]);
            temp.m_Discovery = CSVParseINT(datas[idx++]);
            temp.m_Tear = CSVParseINT(datas[idx++]);

            temp.m_Term = CSVParseENUM<CUtility.EDice_Term>(datas[idx++]);
            temp.m_NeedDice = CSVParseINT(datas[idx++]);
            CSVParseEnum_List<CUtility.EDice>(ref temp.m_RequireDice, datas[idx++]);

            temp.m_Desc = datas[idx++];
            string particle = datas[idx++];
            temp.m_Anim = datas[idx++].Trim();

            var inst = CScriptable_CardSkill.CreatePrefab(temp, particle);

            if (temp.m_Tear <= 1) CardList_1tear.Add(temp.m_ID, inst);
            else if (temp.m_Tear == 2) CardList_2tear.Add(temp.m_ID, inst);
            else if (temp.m_Tear == 3) CardList_3tear.Add(temp.m_ID, inst);
            AllCard.Add(temp.m_ID, inst);
        }

        //foreach (var it in lines)
        //{
        //    CUtility.CSkillCard temp = new CUtility.CSkillCard();
        //    var datas = it.Split("\t");

        //    int idx = 0;
        //    Debug.Log(datas.Length);
        //    temp.m_ID = CSVParseUINT(datas[idx++]);
        //    temp.m_Name = datas[idx++];
        //    temp.m_CardType = CSVParseENUM<CUtility.ECardType>(datas[idx++]);
        //    temp.m_GenType = CSVParseENUM<CUtility.EATK_GenType>(datas[idx++]);

        //    temp.m_Damage = CSVParseINT(datas[idx++]);
        //    temp.m_Defend = CSVParseINT(datas[idx++]);
        //    temp.m_StatusEff = CSVParseINT(datas[idx++]);

        //    temp.m_Targets = CSVParseINT(datas[idx++]);
        //    temp.m_Discovery = CSVParseINT(datas[idx++]);
        //    //CSVParseInt_List(ref temp.m_Sockets, datas[idx++]);
        //    temp.m_Tear = CSVParseINT(datas[idx++]);
        //    temp.m_Term = CSVParseENUM<CUtility.EDice_Term>(datas[idx++]);
        //    temp.m_NeedDice = CSVParseINT(datas[idx++]);
        //    CSVParseEnum_List<CUtility.EDice>(ref temp.m_RequireDice, datas[idx++]);

        //    temp.m_Desc = datas[idx++].Trim();
        //    string particle = datas[idx++].Trim();
        //    //temp.m_Particle = datas[idx++].Trim();
        //    temp.m_Anim = datas[idx++].Trim();

        //    var inst = CScriptable_CardSkill.CreatePrefab(temp, particle);

        //    if (temp.m_Tear <= 1) CardList_1tear.Add(temp.m_ID, inst);
        //    else if (temp.m_Tear == 2) CardList_2tear.Add(temp.m_ID, inst);
        //    else if (temp.m_Tear == 3) CardList_3tear.Add(temp.m_ID, inst);
        //    AllCard.Add(temp.m_ID, inst);
        //}
        Debug.Log("LOAD USER SKILL FROM GOOGLE SHEET");
    }

    //====================================================================================//
    [MenuItem("Assets/Get GoogleData/MONSTER_SKILL")]
    public static void GetMonsterSkillType()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetMonsterSkillType(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetMonsterSkillType() 
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_MonsterSkill, Instance.range_MonsterSkill);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();


        string data = www.downloadHandler.text;
        tsvData = data;

        var lines = tsvData.Split("\n");

        var monsterSkills = CGameManager.Instance.m_Dictionary.m_MonsterSkills;
        monsterSkills.Clear();

        foreach (var it in lines)
        {
            CUtility.CMonsterSkill temp = new CUtility.CMonsterSkill();
            var datas = it.Split("\t");

            int idx = 0;
            Debug.Log(datas.Length);
            temp.m_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = datas[idx++];
            temp.m_Type = CSVParseENUM<CUtility.ECardType>(datas[idx++]);
            temp.m_GenType = CSVParseENUM<CUtility.EATK_GenType>(datas[idx++]);

            temp.m_Dmg = CSVParseINT(datas[idx++]);
            temp.m_Def = CSVParseINT(datas[idx++]);
            temp.m_Eff = CSVParseINT(datas[idx++]);

            temp.m_Count = CSVParseINT(datas[idx++]);
            temp.m_Cost = CSVParseINT(datas[idx++]);
            temp.m_SpecialCost = CSVParseINT(datas[idx++]);
            
            temp.m_Particle = datas[idx++].Trim();
            temp.m_Anim = datas[idx++].Trim();
            temp.m_ShoutOutDesc = datas[idx++].Trim();

            Debug.Log(temp.m_ID);

            var inst = CScriptable_MonsterSkill.CreatePrefab(temp);
            monsterSkills.Add(temp.m_ID, inst);
        }

        Debug.Log("LOAD MONSTER SKILL FROM GOOGLE SHEET");
    }

    //====================================================================================//
    [MenuItem("Assets/Get GoogleData/MONSTER")]
    public static void GetMonsterType()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetMonsterType(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetMonsterType()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_Monster, Instance.range_Monster);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var skills = CGameManager.Instance.m_Dictionary.m_MonsterSkills;
        var monsters = CGameManager.Instance.m_Dictionary.m_Monsters;
        monsters.Clear();


        var lines = tsvData.Split("\n");
        foreach (var it in lines)
        {
            CUtility.CMonster temp = new CUtility.CMonster();
            var datas = it.Split("\t");

            int idx = 0;
            Debug.Log(datas.Length);
            temp.m_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = datas[idx++];

            temp.m_HP = CSVParseINT(datas[idx++]);

            List<uint> atks = new List<uint>();
            CSVParseUInt_List(ref atks, datas[idx++]);
            foreach (var atk in atks)
            {
                if (skills.ContainsKey(atk) == true)
                    temp.m_Skill_Atk.Add(skills[atk]);
            }

            List<uint> defs = new List<uint>();
            CSVParseUInt_List(ref defs, datas[idx++]);
            foreach (var def in defs)
            {
                if (skills.ContainsKey(def) == true)
                    temp.m_Skill_Def.Add(skills[def]); 
            }

            List<uint> specials = new List<uint>();
            CSVParseUInt_List(ref specials, datas[idx++]);
            foreach (var spc in specials)
            {
                if (skills.ContainsKey(spc) == true)
                    temp.m_Skill_Special.Add(skills[spc]); 
            }


            var inst = CScriptable_Monster.CreatePrefab(temp);
            monsters.Add(temp.m_ID, inst);
        }

        Debug.Log("LOAD MONSTER FROM GOOGLE SHEET");
    }

    //====================================================================================//

    [MenuItem("Assets/Get GoogleData/MONSTER_GROUP")]
    public static void GetMonsterGroupType()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetMonsterGroupType(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetMonsterGroupType()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_Monster_Group, Instance.range_Monster_Group);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var monsterGroups_t1 = CGameManager.Instance.m_Dictionary.m_MonsterGroups_Tear01;
        var monsterGroups_t2 = CGameManager.Instance.m_Dictionary.m_MonsterGroups_Tear02;
        var monsterGroups_t3 = CGameManager.Instance.m_Dictionary.m_MonsterGroups_Tear03;
        var monsterGroups_t4 = CGameManager.Instance.m_Dictionary.m_MonsterGroups_Tear04;
        var monsterGroups_sp = CGameManager.Instance.m_Dictionary.m_MonsterGroups_Special;
        var monsters = CGameManager.Instance.m_Dictionary.m_Monsters;
        monsterGroups_t1.Clear();
        monsterGroups_t2.Clear();
        monsterGroups_t3.Clear();
        monsterGroups_t4.Clear();
        monsterGroups_sp.Clear();

        var lines = tsvData.Split("\n");
        foreach (var it in lines) 
        {
            CUtility.CMonsterGroup temp = new CUtility.CMonsterGroup();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = temp.m_ID.ToString();
            //temp.m_Name = temp.m_ID;
            //temp.m_Name = datas[idx++];
            temp.m_Chapter = CSVParseINT(datas[idx++]);
            temp.m_Tear = CSVParseINT(datas[idx++]);

            temp.m_StartCost = CSVParseINT(datas[idx++]);
            temp.m_MaxCost = CSVParseINT(datas[idx++]);

            temp.m_Slots = new List<CScriptable_Monster>();
            for (int i = 0; i < 6; i++) 
            {
                temp.m_Slots.Add(null);
                var monsterIDX = CSVParseUINT(datas[idx++]);
                Debug.Log(temp.m_Slots.Count);
                if (monsterIDX > 0 && monsters.ContainsKey(monsterIDX) == true)
                    temp.m_Slots[i] = monsters[monsterIDX];
            }

            temp.m_Reward_Discovery = CSVParseINT(datas[idx++]);

            //티어별 분간해서 넣음
            var inst = CScriptable_MonsterGroup.CreatePrefab(temp);
            switch (inst.m_Data.m_Tear) 
            {
                case 0: monsterGroups_sp.Add(temp.m_ID, inst); break;
                case 1: monsterGroups_t1.Add(temp.m_ID, inst); break;
                case 2: monsterGroups_t2.Add(temp.m_ID, inst); break;
                case 3: monsterGroups_t3.Add(temp.m_ID, inst); break;
                case 4: monsterGroups_t4.Add(temp.m_ID, inst); break;
            }
        }
        Debug.Log("LOAD MONSTER_GROUP FROM GOOGLE SHEET");
    }

    //====================================================================================//
    [MenuItem("Assets/Get GoogleData/ManaSkill_Quest")]
    public static void GetManaSkill_Quest_Type()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetManaSkill_Quest_Type(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetManaSkill_Quest_Type()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_ManaSkill_Quest, Instance.range_ManaSkill_Quest);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var quests = CGameManager.Instance.m_Dictionary.m_Quests;
        quests.Clear();

        var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

        var lines = tsvData.Split("\n");
        foreach (var it in lines)
        {
            CUtility.CQuest temp = new CUtility.CQuest();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            //temp.m_Name = datas[idx++];
            temp.m_Name_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = texts[temp.m_Name_ID].m_Text[0];
            temp.m_Desc_ID = CSVParseUINT(datas[idx++]);
            temp.m_Description = texts[temp.m_Desc_ID].m_Text[0];

            temp.m_Term = CSVParseENUM<CUtility.CQuest.ETerm>(datas[idx++]);
            temp.m_Count = CSVParseINT(datas[idx++]);
            temp.m_Require = CSVParseINT(datas[idx++]);
            
            temp.m_Reward_Num = CSVParseINT(datas[idx++]);
            temp.m_Reward_Type = CSVParseENUM<CUtility.EManaSkillType>(datas[idx++]);
            temp.m_ResetOnEndTurn = bool.Parse(datas[idx++]);
            temp.m_Particle = datas[idx++].Trim();

            var inst = CScriptable_Quest.CreatePrefab(temp);
            quests.Add(temp.m_ID, inst);
        }

        Debug.Log("LOAD MANASKILL_QUEST FROM GOOGLE SHEET");

    }

    [MenuItem("Assets/Get GoogleData/ManaSkill_Area")]
    public static void GetManaSkill_Area_Type()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetManaSkill_Area_Type(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetManaSkill_Area_Type()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_ManaSkill_Area, Instance.range_ManaSkill_Area);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var areas = CGameManager.Instance.m_Dictionary.m_ManaSkills_Area;
        areas.Clear();

        var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

        var lines = tsvData.Split("\n");
        foreach (var it in lines)
        {
            CUtility.CManaSkill_Area temp = new CUtility.CManaSkill_Area();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            //temp.m_Name = datas[idx++];
            temp.m_Name_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = texts[temp.m_Name_ID].m_Text[0];
            temp.m_Desc_ID = CSVParseUINT(datas[idx++]);
            temp.m_Description = texts[temp.m_Desc_ID].m_Text[0];

            temp.m_Cost = CSVParseINT(datas[idx++]); ;
            temp.m_Range = CSVParseFloat(datas[idx++]); 
            temp.m_Max = CSVParseINT(datas[idx++]);

            temp.m_Type = CSVParseENUM<CUtility.EManaSkillType>(datas[idx++].Trim());
            temp.m_Particle = datas[idx++].Trim();

            var inst = CScriptable_ManaSkill_Area.CreatePrefab(temp);
            areas.Add(temp.m_ID, inst);
        }

        Debug.Log("LOAD MANASKILL_AREA FROM GOOGLE SHEET");

    }

    [MenuItem("Assets/Get GoogleData/ManaSkill_Use")]
    public static void GetManaSkill_Use_Type()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetManaSkill_Use_Type(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetManaSkill_Use_Type()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_ManaSkill_Use, Instance.range_ManaSkill_Use);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;

        var Uses = CGameManager.Instance.m_Dictionary.m_ManaSkills_Use;
        Uses.Clear();

        var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

        var lines = tsvData.Split("\n");
        foreach (var it in lines)
        {
            CUtility.CManaSkill temp = new CUtility.CManaSkill();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            //temp.m_Name = datas[idx++];
            temp.m_Name_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = texts[temp.m_Name_ID].m_Text[0];
            temp.m_Desc_ID = CSVParseUINT(datas[idx++]);
            temp.m_Description = texts[temp.m_Desc_ID].m_Text[0];

            temp.m_Cost = CSVParseINT(datas[idx++]);
            temp.m_Type = CSVParseENUM<CUtility.EManaSkillType>(datas[idx++]);
            temp.m_Param = CSVParseINT(datas[idx++]);

            temp.m_Particle = datas[idx++].Trim();

            var inst = CScriptable_ManaSkill.CreatePrefab(temp);
            Uses.Add(temp.m_ID, inst);
        }

        Debug.Log("LOAD MANASKILL_USE FROM GOOGLE SHEET");

    }

    [MenuItem("Assets/Get GoogleData/SceneInfos")]
    public static void GetSceneInfo_Type()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetSceneInfo_Type(),
            CGoogleScriptable.Instance);
    }

    public IEnumerator CoGetSceneInfo_Type()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_SceneInfo, Instance.range_SceneInfo);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();
        
        string data = www.downloadHandler.text;
        tsvData = data;
        var lines = tsvData.Split("\n");

        var SceneInfos = CGameManager.Instance.m_Dictionary.m_SceneInfos;
        SceneInfos.Clear();

        Debug.Log(lines.Length);
        var texts = CGameManager.Instance.m_Dictionary.m_Language.m_Texts;

        foreach (var it in lines)
        {
            CUtility.CSceneInfo temp = new CUtility.CSceneInfo();
            var datas = it.Split("\t");

            int idx = 0;
            temp.m_ID = CSVParseUINT(datas[idx++]);
            Debug.Log(temp.m_ID);
            temp.m_Name_ID = CSVParseUINT(datas[idx++]);
            temp.m_Name = texts[temp.m_Name_ID].m_Text[0];
            temp.m_Type = CSVParseENUM<CUtility.CSceneInfo.ESceneType>(datas[idx++]);
            temp.m_Connection_Scene = CSVParseUINT(datas[idx++]);
            temp.m_Connection_ID = CSVParseUINT(datas[idx++]);

            temp.m_ResellCost = CSVParseINT(datas[idx++]);
            temp.m_RequireDiscovery = CSVParseINT(datas[idx++]);

            var inst = CScriptable_SceneInfo.CreatePrefab(temp);
            SceneInfos.Add(temp.m_ID, inst);
        }
    }

    [MenuItem("Assets/Get GoogleData/LANGUAGE")]
    public static void GetLanguage_Type()
    {
        editorCoroutine =
            EditorCoroutineUtility.StartCoroutine(Instance.CoGetLanguage_Type(),
            CGoogleScriptable.Instance);
    }
    public IEnumerator CoGetLanguage_Type()
    {
        EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

        string tsvData = "";
        var link = string.Format("{0}{1}{2}", url,
            Instance.sheet_Language, Instance.range_Language);
        UnityWebRequest www = UnityWebRequest.Get(link);
        yield return www.SendWebRequest();

        string data = www.downloadHandler.text;
        tsvData = data;
        var lines = tsvData.Split("\n");

        Debug.Log(lines.Length);
        SerializeDictionary<uint, string> eng = new SerializeDictionary<uint, string>();
        SerializeDictionary<uint, string> kor = new SerializeDictionary<uint, string>();


        SerializeDictionary<uint, CUtility.CLanguageTag> get
            = new SerializeDictionary<uint, CUtility.CLanguageTag>();

        foreach (var it in lines)
        {
            var datas = it.Split("\t");
            int idx = 0;

            uint m_ID = CSVParseUINT(datas[idx++]);
            get.Add(m_ID, new CUtility.CLanguageTag());
            get[m_ID].m_Text.Add(datas[idx++]); // eng;
            get[m_ID].m_Text.Add(datas[idx++]); // kor;
        }

        var inst = CScriptable_Language.CreatePrefab(get);
        CGameManager.Instance.m_Dictionary.m_Language = inst;
    }

    //====================================================================================//
    //====================================================================================//
    //====================================================================================//
    //====================================================================================//
    //====================================================================================//


    static public void CSVParseEnum_List<T>(ref List<T> _get, string _data)
    {
        var datas = _data.Trim().Split(",");
        foreach (var it in datas)
        {
            if (it == "") break;
            _get.Add(CSVParseENUM<T>(it, "ANY"));
        }
    }

    static public void CSVParseInt_List(ref List<int> _get, string _data) 
    {
        var datas = _data.Trim().Split(",");
        foreach (var it in datas)
        {
            if (it == "") break;
            _get.Add(CSVParseINT(it));
        }
    }

    static public void CSVParseUInt_List(ref List<uint> _get, string _data)
    {
        var datas = _data.Trim().Split(",");
        foreach (var it in datas)
        {
            if (it == "") break;
            _get.Add(CSVParseUINT(it));
        }
    }

    public string ChangeLineBreak(string s)
    {
        return s.Replace("\\n", "\n");
        //return s; 
    }
    static float CSVParseFloat(string _data)
    {
        if (_data.Length == 0 || (int)_data[0] <= 13) return 0;
        return float.Parse(_data);
    }
    static uint CSVParseUINT(string _data)
    {
        if (_data.Length == 0 || (int)_data[0] <= 13) return 0;
        return uint.Parse(_data);
    }

    static int CSVParseINT(string _data)
    {
        if (_data.Length == 0 || (int)_data[0] <= 13) return 0;
        return int.Parse(_data);
    }

    static public T CSVParseENUM<T>(string _data, string _noneData = "NONE")
    {
        //Debug.Log(datas[idx]);
        //공백 문자 예외처리
        if (_data.Length == 0 || (int)_data[0] <= 13) _data = _noneData;
        return (T)System.Enum.Parse(typeof(T), _data);
    }

    /*
[Header("=================================================")]
public string sheet_skill = "&gid=1362907170";
public string range_skill = "&range=A2:R";
[Header("=================================================")]
public string sheet_BuffType = "&gid=522964400";
public string range_BuffType = "&range=A2:C";
[Header("=================================================")]
public string sheet_Buff = "&gid=1093066770";
public string range_Buff = "&range=A2:C";
[Header("=================================================")]
public string sheet_Debuff = "&gid=436470194";
public string range_Debuff = "&range=A2:C";
[Header("=================================================")]
public string sheet_MonsterSkill = "&gid=227782649";
public string range_MonsterSkill = "&range=A2:J11";
[Header("=================================================")]
public string sheet_Monster_Small = "&gid=1858156075";
public string range_Monster_Small = "&range=A2:K4";
[Header("=================================================")]
public string sheet_Monster_Combination = "&gid=299612060";
public string range_Monster_Combination = "&range=A2:K3";
[Header("=================================================")]
public string sheet_DiceType = "&gid=1904082780";
public string range_DiceType = "&range=A2:K10";
[Header("=================================================")]
public string sheet_Artifact = "&gid=1830012882";
public string range_Artifact = "&range=A2:H5";
[Header("=================================================")]
public string sheet_Event_Combination = "&gid=1167916786";
public string range_Event_Combination = "&range=A2:I5";

public static EditorCoroutine editorCoroutine;

[MenuItem("Assets/Get GoogleData/All")]
public static void GetGoogleData_ALL() 
{
   Debug.Log("지금은 작동하지 않습니다. 위에서부터 수동으로 하나씩 받으세요");
   //GetBuffTable();
   //GetDebuffTable();
}

[MenuItem("Assets/Get GoogleData/Dice")]
public static void GetDiceType()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetDiceType(),
       CGoogleScriptable.Instance);
}

public IEnumerator CoGetDiceType()
{
   string tsvData = "";
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_DiceType, Instance.range_DiceType);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");
   //var DiceTypeList = CGameManager.Instance.m_Dictionary.m_DiceTypes;
   var DiceTypeList_1tear = CGameManager.Instance.m_Dictionary.m_DiceTypes_1Tear;
   var DiceTypeList_2tear = CGameManager.Instance.m_Dictionary.m_DiceTypes_2Tear;
   var DiceTypeList_3tear = CGameManager.Instance.m_Dictionary.m_DiceTypes_3Tear;
   var DiceTypeList_4tear = CGameManager.Instance.m_Dictionary.m_DiceTypes_4Tear;
   DiceTypeList_1tear.Clear();
   DiceTypeList_2tear.Clear();
   DiceTypeList_3tear.Clear();
   DiceTypeList_4tear.Clear();
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);


   //ID Name Desc
   //Tear Particle_Name
   //Eye01 Eye02 Eye03 Eye04 Eye05 Eye06
   foreach (var it in lines)
   {
       CUtility.CDice temp = new CUtility.CDice();
       var datas = it.Split("\t");

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       temp.m_Description = datas[idx++];

       temp.m_Tear = CSVParseINT(datas[idx++]);
       string particleName = datas[idx++];

       for (int i = 0; i < 6; i++) 
       { temp.m_Eyes.Add(CSVParseINT(datas[idx++])); }

       var inst = CScriptable_Dice.CreatePrefab(temp, particleName);
       if (temp.m_ID == 15001) CGameManager.Instance.m_Dictionary.m_DefaultDice = inst;
       else if (temp.m_Tear <= 1) DiceTypeList_1tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 2) DiceTypeList_2tear.Add(temp.m_ID, inst);
       else if(temp.m_Tear == 3) DiceTypeList_3tear.Add(temp.m_ID, inst);
       else if(temp.m_Tear >= 4) DiceTypeList_4tear.Add(temp.m_ID, inst);

       //DiceTypeList.Add(temp.m_ID, inst);
   }
}


[MenuItem("Assets/Get GoogleData/Buff")]
public static void GetBuffTable() 
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetBuffTable(),
       CGoogleScriptable.Instance);
}


//버프 아이콘 데이터
public IEnumerator CoGetBuffType() 
{
   string tsvData = "";
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_BuffType, Instance.range_BuffType);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");
   CGameManager.Instance.m_BuffMgr.m_Buff_Icons.Clear();
   var buffTypeList = CGameManager.Instance.m_BuffMgr.m_Buff_Icons;
   EditorUtility.SetDirty(CGameManager.Instance.m_BuffMgr);

   //ID
   //NAME
   //DESC
   foreach (var it in lines)
   {
       CUtility.CBuffType temp = new CUtility.CBuffType();
       var datas = it.Split("\t");

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       if (temp.m_Name == "") break;
       temp.m_Desc = datas[idx++];

       var inst = CScriptable_BuffType.CreatePrefab(temp);
       buffTypeList.Add(CSVParseENUM<CBuffManager.EBuffIcon>(temp.m_Name), inst);
       Debug.Log(idx);
   }

}

public IEnumerator CoGetBuffTable() 
{
   yield return EditorCoroutineUtility.StartCoroutine(Instance.CoGetBuffType(),
       CGoogleScriptable.Instance);

   string tsvData = "";
   var link = string.Format("{0}{1}{2}", url, 
       Instance.sheet_Buff, Instance.range_Buff);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");
   CGameManager.Instance.m_Dictionary.m_Buffs.Clear();
   var buffList = CGameManager.Instance.m_Dictionary.m_Buffs;
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

   //ID
   //NAME
   //DESC
   foreach (var it in lines) 
   {
       CUtility.CBuff temp = new CUtility.CBuff();
       var datas = it.Split("\t");

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       if (temp.m_Name == "") break;
       temp.m_Desc = datas[idx++];

       var inst = CScriptable_Buff.CreatePrefab(temp);
       buffList.Add(temp.m_ID, inst);
   }
}

[MenuItem("Assets/Get GoogleData/Debuff")]
public static void GetDebuffTable()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetDebuffTable(),
       CGoogleScriptable.Instance);
}



public IEnumerator CoGetDebuffTable() 
{
   string tsvData = "";
   var link = string.Format("{0}{1}{2}", url, 
       Instance.sheet_Debuff, Instance.range_Debuff);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");
   CGameManager.Instance.m_Dictionary.m_Debuffs.Clear();
   var DebuffList = CGameManager.Instance.m_Dictionary.m_Debuffs;
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

   //ID
   //NAME
   //DESC
   foreach (var it in lines)
   {
       CUtility.CBuff temp = new CUtility.CBuff();
       var datas = it.Split("\t");

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       if (temp.m_Name == "") break;
       temp.m_Desc = datas[idx++];

       //var inst = CScriptable_Debuff.CreatePrefab(temp);
       var inst = CScriptable_Buff.CreatePrefab(temp);
       DebuffList.Add(temp.m_ID, inst);
   }
}


[MenuItem("Assets/Get GoogleData/Artifact")]
public static void GetArtifact()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetArtifact(),
       CGoogleScriptable.Instance);
}

public IEnumerator CoGetArtifact()
{
   string tsvData = "";
   //url, sheet, range
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_Artifact, Instance.range_Artifact);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");

   //var DiceTypeList = CGameManager.Instance.m_Dictionary.m_DiceTypes;
   var artifactList_1tear = CGameManager.Instance.m_Dictionary.m_Artifacts_1Tear;
   var artifactList_2tear = CGameManager.Instance.m_Dictionary.m_Artifacts_2Tear;
   var artifactList_3tear = CGameManager.Instance.m_Dictionary.m_Artifacts_3Tear;
   var artifactList_4tear = CGameManager.Instance.m_Dictionary.m_Artifacts_4Tear;
   artifactList_1tear.Clear();
   artifactList_2tear.Clear();
   artifactList_3tear.Clear();
   artifactList_4tear.Clear();


   //ID	Name	
   //Desc	Tear	
   //Buff	Param	
   //Debuff	param

   foreach (var it in lines)
   {
       CUtility.CArtifact temp = new CUtility.CArtifact();

       var datas = it.Split("\t");

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       temp.m_Description = datas[idx++];
       temp.m_Tear = CSVParseINT(datas[idx++]);

       uint buffID = CSVParseUINT(datas[idx++]);
       temp.m_Buff = buffID > 0 ? CGameManager.Instance.m_Dictionary.m_Buffs[buffID] : null;
       var buffParam = datas[idx++].Split(",");
       foreach (var p in buffParam)
       {
           if (p == "") break;
           temp.m_BuffParam.Add(CSVParseINT(p));
       }

       uint debuffID = CSVParseUINT(datas[idx++]);
       temp.m_Debuff = debuffID > 0 ? CGameManager.Instance.m_Dictionary.m_Debuffs[debuffID] : null;
       var debuffParam = datas[idx++].Trim().Split(",");
       foreach (var p in debuffParam)
       {
           if (p == "") break;
           temp.m_DebuffParam.Add(CSVParseINT(p));
       }

       var inst = CScriptable_Artifact.CreatePrefab(temp);
       if (temp.m_Tear <= 1) artifactList_1tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 2) artifactList_2tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 3) artifactList_3tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear >= 4) artifactList_4tear.Add(temp.m_ID, inst);
   }
}

[MenuItem("Assets/Get GoogleData/Skill")]
public static void GetSkillTable()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetSkillTable(),
       CGoogleScriptable.Instance);
}

public IEnumerator CoGetSkillTable()
{
   string tsvData = "";
   //url, sheet, range
   var link = string.Format("{0}{1}{2}", url, 
       Instance.sheet_skill, Instance.range_skill);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");

   //var DiceTypeList = CGameManager.Instance.m_Dictionary.m_DiceTypes;
   var skillList_1tear = CGameManager.Instance.m_Dictionary.m_Skills_1Tear;
   var skillList_2tear = CGameManager.Instance.m_Dictionary.m_Skills_2Tear;
   var skillList_3tear = CGameManager.Instance.m_Dictionary.m_Skills_3Tear;
   var skillList_4tear = CGameManager.Instance.m_Dictionary.m_Skills_4Tear;
   skillList_1tear.Clear();
   skillList_2tear.Clear();
   skillList_3tear.Clear();
   skillList_4tear.Clear();
   //CGameManager.Instance.m_Dictionary.m_Skills.Clear();
   //var skillList = CGameManager.Instance.m_Dictionary.m_Skills;
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

   //ID  Name  Tear  NeedDice ReturnDice
   //Term  CoolTime  RequireDice
   //Type
   //DMG  DMG_Type
   //Def  Def_Type
   //Buff  Param
   //DeBuff  Param
   //Particle Name
   foreach (var it in lines)
   {
       CUtility.CSkill temp = new CUtility.CSkill();

       var datas = it.Split("\t");

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       if (temp.m_Name == "") break;
       Debug.Log(temp.m_Name);

       temp.m_Tear = CSVParseINT(datas[idx++]);
       temp.m_NeedDice = CSVParseINT(datas[idx++]);
       temp.m_ReturnDice = CSVParseINT(datas[idx++]);

       temp.m_Term = CSVParseENUM<CUtility.EDice_Term>(datas[idx++]);
       temp.m_CoolTime = CSVParseINT(datas[idx++]);
       var requires = datas[idx++].Split(",");
       foreach (var eye in requires)
       { temp.m_Require_Dice.Add(CSVParseENUM<CUtility.EDice>(eye, "ANY")); }

       temp.m_Type = CSVParseENUM<CUtility.ESkillType>(datas[idx++], "ATK");

       temp.m_Dmg = CSVParseINT(datas[idx++]);
       temp.m_Dmg_Type = CSVParseENUM<CUtility.ECalcType>(datas[idx++]);

       temp.m_Def = CSVParseINT(datas[idx++]);
       temp.m_Def_Type = CSVParseENUM<CUtility.ECalcType>(datas[idx++]);

       uint buffID = CSVParseUINT(datas[idx++]);
       temp.m_Buff = buffID > 0 ? CGameManager.Instance.m_Dictionary.m_Buffs[buffID] : null;
       var buffParam = datas[idx++].Split(",");
       foreach (var p in buffParam) 
       {
           if (p == "") break;
           temp.m_BuffParam.Add(CSVParseINT(p)); 
       }

       uint debuffID = CSVParseUINT(datas[idx++]);
       temp.m_Debuff = debuffID > 0 ?  CGameManager.Instance.m_Dictionary.m_Debuffs[debuffID] : null;
       var debuffParam = datas[idx++].Split(",");
       foreach (var p in debuffParam)
       {
           if (p == "") break;
           temp.m_DebuffParam.Add(CSVParseINT(p)); 
       }

       string particleName = datas[idx++].TrimEnd();
       var inst = CScriptable_Skill.CreatePrefab(temp, particleName);
       if (temp.m_Tear <= 1) skillList_1tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 2) skillList_2tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 3) skillList_3tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear >= 4) skillList_4tear.Add(temp.m_ID, inst);
       //skillList.Add(temp.m_ID, inst);
   }



}


[MenuItem("Assets/Get GoogleData/MonsterSkill")]
public static void GetMonsterSkillTable()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetMonsterSkillTable(),
       CGoogleScriptable.Instance);
}
public IEnumerator CoGetMonsterSkillTable()
{
   string tsvData = "";
   //url, sheet, range
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_MonsterSkill, Instance.range_MonsterSkill);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n");
   CGameManager.Instance.m_Dictionary.m_MonsterSkills.Clear();
   var monsterSkillList = CGameManager.Instance.m_Dictionary.m_MonsterSkills;
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);
   //
   //ID Name
   //Dmg Def
   //Buff Param
   //Debuff Param
   //Cost
   //ParticleName
   foreach (var it in lines)
   {
       CUtility.CMonsterSkill temp = new CUtility.CMonsterSkill();

       var datas = it.Split("\t");
       //Debug.Log(it);

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       if (temp.m_Name == "") break;
       temp.m_Dmg = CSVParseINT(datas[idx++]);
       temp.m_Def = CSVParseINT(datas[idx++]);

       uint buffID = CSVParseUINT(datas[idx++]);
       temp.m_Buff = buffID > 0 ? CGameManager.Instance.m_Dictionary.m_Buffs[buffID] : null;
       var buffParam = datas[idx++].Split(",");
       foreach (var p in buffParam)
       { if (p != "") temp.m_BuffParam.Add(CSVParseINT(p)); }

       uint debuffID = CSVParseUINT(datas[idx++]);
       temp.m_Debuff = debuffID > 0 ? CGameManager.Instance.m_Dictionary.m_Debuffs[debuffID] : null;
       var debuffParam = datas[idx++].Split(",");
       foreach (var p in debuffParam)
       { if (p != "")  temp.m_DebuffParam.Add(CSVParseINT(p)); }

       temp.m_Cost = CSVParseINT(datas[idx++]);
       string particleName = datas[idx++].TrimEnd();

       var inst = CScriptable_MonsterSkill.CreatePrefab(temp, particleName);
       monsterSkillList.Add(temp.m_ID, inst);
   }
}

[MenuItem("Assets/Get GoogleData/Monster")]
public static void GetMonsterTable()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetMonsterTable(),
       CGoogleScriptable.Instance);
}

public IEnumerator CoGetMonsterTable()
{
   string tsvData = "";
   //url, sheet, range
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_Monster_Small, Instance.range_Monster_Small);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n", System.StringSplitOptions.None);
   CGameManager.Instance.m_Dictionary.m_Monsters.Clear();
   var monsterList = CGameManager.Instance.m_Dictionary.m_Monsters;
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);

   //ID Name
   //Health Artifact
   //Skill1 Skill2 Skill3 Skill4 Skill5 Skill6 Skill7
   foreach (var it in lines)
   {
       CUtility.CMonster temp = new CUtility.CMonster();

       var datas = it.Split("\t", System.StringSplitOptions.None);

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = datas[idx++];
       temp.m_Health = CSVParseINT(datas[idx++]);
       temp.m_Artifact = datas[idx++];

       CScriptable_MonsterSkill skill = null;

       var skill1 = CSVParseUINT(datas[idx++]);
       skill = skill1 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill1] : null;
       if (skill != null) temp.m_Skills.Add(skill);
       Debug.Log(temp.m_Name+skill1);

       var skill2 = CSVParseUINT(datas[idx++]);
       skill = skill2 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill2] : null;
       if (skill != null) temp.m_Skills.Add(skill);
       Debug.Log(temp.m_Name + skill2);

       var skill3 = CSVParseUINT(datas[idx++]);
       skill = skill3 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill3] : null;
       if (skill != null) temp.m_Skills.Add(skill);
       Debug.Log(temp.m_Name + skill3);

       var skill4 = CSVParseUINT(datas[idx++]);
       skill = skill4 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill4] : null;
       if (skill != null) temp.m_Skills.Add(skill);
       Debug.Log(temp.m_Name + skill4);

       var skill5 = CSVParseUINT(datas[idx++]);
       skill = skill5 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill5] : null;
       if (skill != null) temp.m_Skills.Add(skill);
       Debug.Log(temp.m_Name + skill5);

       var skill6 = CSVParseUINT(datas[idx++].Trim());
       Debug.Log(temp.m_Name + skill6);
       skill = skill6 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill6] : null;
       if (skill != null) temp.m_Skills.Add(skill);

       var skill7 = CSVParseUINT(datas[idx++].Trim());
       Debug.Log(temp.m_Name + skill7);
       skill = skill7 > 0 ? CGameManager.Instance.m_Dictionary.m_MonsterSkills[skill7] : null;
       if (skill != null) temp.m_Skills.Add(skill);

       var inst = CScriptable_Monster.CreatePrefab(temp);
       monsterList.Add(temp.m_ID, inst);
       //temp.m_Skills.Add()
   }
}

[MenuItem("Assets/Get GoogleData/Monster_Combination")]
public static void GetMonsterCombinationTable()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetMonsterCombinationTable(),
       CGoogleScriptable.Instance);
}

public IEnumerator CoGetMonsterCombinationTable()
{
   string tsvData = "";
   //url, sheet, range
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_Monster_Combination, Instance.range_Monster_Combination);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   tsvData = data;

   var lines = tsvData.Split("\n", System.StringSplitOptions.None);

   var mc_1tear = CGameManager.Instance.m_Dictionary.m_MonsterCombinations_1Tear;
   var mc_2tear = CGameManager.Instance.m_Dictionary.m_MonsterCombinations_2Tear;
   var mc_3tear = CGameManager.Instance.m_Dictionary.m_MonsterCombinations_3Tear;
   var mc_4tear = CGameManager.Instance.m_Dictionary.m_MonsterCombinations_4Tear;
   mc_1tear.Clear();
   mc_2tear.Clear();
   mc_3tear.Clear();
   mc_4tear.Clear();
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);


   foreach (var it in lines)
   {
       CUtility.CMonster_Combination temp = new CUtility.CMonster_Combination();

       var datas = it.Split("\t", System.StringSplitOptions.None);

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       temp.m_Name = temp.m_ID.ToString();
       temp.m_Group_Tear = CSVParseINT(datas[idx++]);

       CScriptable_Monster monster = null;

       var monster1 = CSVParseUINT(datas[idx++]);
       monster = monster1 > 0 ? CGameManager.Instance.m_Dictionary.m_Monsters[monster1] : null;
       if (monster != null) temp.m_Monsters.Add(monster);

       var monster2 = CSVParseUINT(datas[idx++]);
       monster = monster2 > 0 ? CGameManager.Instance.m_Dictionary.m_Monsters[monster2] : null;
       if (monster != null) temp.m_Monsters.Add(monster);

       var monster3 = CSVParseUINT(datas[idx++]);
       monster = monster3 > 0 ? CGameManager.Instance.m_Dictionary.m_Monsters[monster3] : null;
       if (monster != null) temp.m_Monsters.Add(monster);

       var monster4 = CSVParseUINT(datas[idx++]);
       monster = monster4 > 0 ? CGameManager.Instance.m_Dictionary.m_Monsters[monster4] : null;
       if (monster != null) temp.m_Monsters.Add(monster);

       var monster5 = CSVParseUINT(datas[idx++]);
       monster = monster5 > 0 ? CGameManager.Instance.m_Dictionary.m_Monsters[monster5] : null;
       if (monster != null) temp.m_Monsters.Add(monster);

       temp.m_Cost = CSVParseINT(datas[idx++]);



       //보상 관리해야함
       temp.m_Reword.m_Gold = CSVParseINT(datas[idx++]);
       temp.m_Reword.m_Tokken = CSVParseINT(datas[idx++]);
       var cardReword = datas[idx++].Trim().Split(",");
       temp.m_Reword.m_Skill_Count = CSVParseINT(cardReword[0]);
       temp.m_Reword.m_Skill_Tear_Min = CSVParseINT(cardReword[1]);
       temp.m_Reword.m_Skill_Tear_Max = CSVParseINT(cardReword[2]);

       var inst = CScriptable_MonsterCombination.CreatePrefab(temp);
       //monsterCombList.Add(temp.m_ID, inst);
       if (temp.m_Group_Tear <= 1) mc_1tear.Add(temp.m_ID, inst);
       else if (temp.m_Group_Tear == 2) mc_2tear.Add(temp.m_ID, inst);
       else if (temp.m_Group_Tear == 3) mc_3tear.Add(temp.m_ID, inst);
       else if (temp.m_Group_Tear >= 4) mc_4tear.Add(temp.m_ID, inst);
   }
}

[MenuItem("Assets/Get GoogleData/Event_Combination")]
public static void GetEvent_Combination()
{
   editorCoroutine =
       EditorCoroutineUtility.StartCoroutine(Instance.CoGetEvent_Combination(),
       CGoogleScriptable.Instance);
}

public IEnumerator CoGetEvent_Combination()
{
   string tsvData = "";
   //url, sheet, range
   var link = string.Format("{0}{1}{2}", url,
       Instance.sheet_Event_Combination, Instance.range_Event_Combination);
   UnityWebRequest www = UnityWebRequest.Get(link);
   yield return www.SendWebRequest();

   string data = www.downloadHandler.text;
   //string data = www.downloadHandler.text.Replace("\\n", "\n");
   tsvData = data;

   var lines = tsvData.Split("\n", System.StringSplitOptions.None);

   var ec_1tear = CGameManager.Instance.m_Dictionary.m_EventCombinations_1Tear;
   var ec_2tear = CGameManager.Instance.m_Dictionary.m_EventCombinations_2Tear;
   var ec_3tear = CGameManager.Instance.m_Dictionary.m_EventCombinations_3Tear;
   var ec_4tear = CGameManager.Instance.m_Dictionary.m_EventCombinations_4Tear;
   ec_1tear.Clear();
   ec_2tear.Clear();
   ec_3tear.Clear();
   ec_4tear.Clear();
   EditorUtility.SetDirty(CGameManager.Instance.m_Dictionary);


    //ID Tear NeedDice
    //Texture  Event 
    //Case_1 Case_2 Case_3 Case_4

   foreach (var it in lines)
   {
       CUtility.CEvent temp = new CUtility.CEvent();

       var datas = it.Split("\t", System.StringSplitOptions.None);

       int idx = 0;
       temp.m_ID = CSVParseUINT(datas[idx++]);
       Debug.Log(temp.m_ID);
       Debug.Log(datas.Length);
       temp.m_Name = string.Format("EC_{0}", temp.m_ID.ToString());
       temp.m_Tear = CSVParseINT(datas[idx++]);
       temp.m_NeedDice = CSVParseINT(datas[idx++]);
       temp.m_Texture = datas[idx++];
       Debug.Log(datas[idx]);
       temp.m_Event = ChangeLineBreak(datas[idx++]);

       var choice = SetEventChoice(datas[idx++].TrimEnd());
       if (choice != null) temp.m_Choices.Add(choice);
       choice = SetEventChoice(datas[idx++].TrimEnd());
       if (choice != null) temp.m_Choices.Add(choice);
       choice = SetEventChoice(datas[idx++].TrimEnd());
       if (choice != null) temp.m_Choices.Add(choice);
       choice = SetEventChoice(datas[idx++].TrimEnd());
       if (choice != null) temp.m_Choices.Add(choice);
       //temp.m_Choices.Add()

       var inst = CScriptable_Event.CreatePrefab(temp);
       if (temp.m_Tear <= 1) ec_1tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 2) ec_2tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear == 3) ec_3tear.Add(temp.m_ID, inst);
       else if (temp.m_Tear >= 4) ec_4tear.Add(temp.m_ID, inst);
   }
}



CUtility.CEvent_Choice SetEventChoice(string _eventCase) 
{
   if (_eventCase == "" || _eventCase == "0") return null;

   Debug.Log(_eventCase);
   var events = _eventCase.Split("|", System.StringSplitOptions.None);
   //Debug.Log(_eventCase);
   //var events = _eventCase.Split("|");
   if (events.Length < 1) return null;

   CUtility.CEvent_Choice choice = new CUtility.CEvent_Choice();
   choice.m_ShowDesc = ChangeLineBreak(events[0]);
   Debug.Log(choice.m_ShowDesc);
   choice.m_SelectedDesc = ChangeLineBreak(events[1]);
   var term = events[2].Split(",");
   if (term.Length >= 2)
   {
       choice.m_Range[0] = CSVParseINT(term[0]);
       choice.m_Range[1] = CSVParseINT(term[1]);
   }

   if (events[3] == "0") { }
   else
   {
       var reword = events[3].TrimEnd().Split(",");

       for (int i = 0; i < reword.Length; i += 2)
       {
           int id = CSVParseINT(reword[i]);
           int count = CSVParseINT(reword[i + 1]);
           choice.m_Rewords.Add(new Vector2Int(id, count));
       }
   }

   return choice;
}

public string ChangeLineBreak(string s)
{ 
   return s.Replace("\\n", "\n"); 
   //return s; 
}
static float CSVParseFloat(string _data)
{
   if (_data.Length == 0 || (int)_data[0] <= 13) return 0;
   return float.Parse(_data);
}
static uint CSVParseUINT(string _data)
{
   if (_data.Length == 0 || (int)_data[0] <= 13) return 0;
   return uint.Parse(_data);
}

static int CSVParseINT(string _data)
{
   if (_data.Length == 0 || (int)_data[0] <= 13) return 0;
   return int.Parse(_data);
}



static public T CSVParseENUM<T>(string _data, string _noneData = "NONE")
{
   //Debug.Log(datas[idx]);
   //공백 문자 예외처리
   if (_data.Length == 0 || (int)_data[0] <= 13) _data = _noneData;
   return (T)System.Enum.Parse(typeof(T), _data);
}

public override void BeforeLoadInstance()
{
   //throw new System.NotImplementedException();
}
*/
}

#endif