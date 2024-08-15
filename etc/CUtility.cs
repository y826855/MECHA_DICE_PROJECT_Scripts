using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


public class CUtility
{
    //
    public enum EDice
    {
        ANY = 0, EYE01, EYE02, EYE03, EYE04, EYE05, EYE06
    }

    public enum EDice_Term //조건
    {
        NONE = 0, //없음
        MORE,     //이상
        LESS,     //이하
        SAME,     //같은
        SAME_OVER,     //x이상 같은
        SAME_LESS,     //x이하 같은
        PICK,     //지정된 수
        STR, //연결된 수
        ODD_NUM,  //홀수
        EVEN_NUM, //짝수
        //---
    }

    public static string m_FolderPath = "Assets/Game/";
    public static string m_TimeFormat = "yyyy-MM-dd HH:mm:ss";
    //public static IEnumerator Eof = WaitForEndOfFrame();


    //WaitForSeconds 모음
    public static WaitForSeconds m_WFS_DOT1 = new WaitForSeconds(0.1f);
    public static WaitForSeconds m_WFS_DOT2 = new WaitForSeconds(0.2f);
    public static WaitForSeconds m_WFS_DOT3 = new WaitForSeconds(0.3f);
    public static WaitForSeconds m_WFS_DOT4 = new WaitForSeconds(0.4f);
    public static WaitForSeconds m_WFS_D5 = new WaitForSeconds(0.5f);
    public static WaitForSeconds m_WFS_1 = new WaitForSeconds(1f);
    public static WaitForSeconds m_WFS_1D5 = new WaitForSeconds(1.5f);
    public static WaitForSeconds m_WFS_2 = new WaitForSeconds(2f);
    public static WaitForSeconds m_WFS_2D5 = new WaitForSeconds(2.5f);
    public static WaitForSeconds m_WFS_3 = new WaitForSeconds(3f);
    public static WaitForSeconds m_WFS_4 = new WaitForSeconds(4f);
    public static WaitForSeconds m_WFS_5 = new WaitForSeconds(5f);
    public static WaitForSeconds m_WFS_10 = new WaitForSeconds(10f);
    public static WaitForSeconds m_WFM_1 = new WaitForSeconds(60f);


    ///<summary>
    ///WaitforSeconds 0.5단위로 받아오기
    ///</summary>
    public static WaitForSeconds GetSecD1To5s(float t)
    {
        if (t <= 0.05) return null;
        else if (t <= 0.1f) return m_WFS_DOT1;
        else if (t <= 0.2f) return m_WFS_DOT2;
        else if (t <= 0.3f) return m_WFS_DOT3;
        else if (t <= 0.4f) return m_WFS_DOT4;
        else if (t <= 0.5f) return m_WFS_D5;
        else if (t <= 1f) return m_WFS_1;
        else if (t <= 1.5f) return m_WFS_1D5;
        else if (t <= 2f) return m_WFS_2;
        else if (t <= 2.5f) return m_WFS_2D5;
        else if (t <= 3f) return m_WFS_3;
        else if (t <= 4f) return m_WFS_4;
        else if (t <= 5f) return m_WFS_5;

        else return m_WFS_5;
    }



    [System.Serializable]
    public class InfoGroup
    {
        public TMPro.TextMeshProUGUI m_TMP = null;
        public Image m_Img = null;

        public void SetToggle(bool _toggle)
        {
            //이미 활성, 비활성화 인지 체크함
            if (m_TMP.gameObject.activeSelf == _toggle) return;
            m_TMP.gameObject.SetActive(_toggle);
            m_Img.gameObject.SetActive(_toggle);
        }
    }

    /////////////////////////////////////////////////CSV DATA/////////////////////////////////////////////////
    [System.Serializable]
    public class CData_CSV
    {
        public uint m_ID = 0;
        public uint m_Name_ID = 0;
        public string m_Name = "";

        //데이터 복사용
        public T Clone<T>() where T : CData_CSV
        {
            return this.MemberwiseClone() as T;
        }
    }
    [System.Serializable]
    public class CData_Iconable : CData_CSV
    {
        public string m_Description = "";
        public uint m_Desc_ID = 0;
        public Sprite m_Icon = null;
    }

    //flag 참조 https://dallcom-forever2620.tistory.com/15
    [System.Flags]
    public enum ECardProperty
    {
        NONE = 0,
        DAMAGE = 1 << 1,

        TARGETS = 1 << 2,
        DEFEND = 1 << 3,
        MANA = 1 << 4,
        RETURN = 1 << 5,

        BURN = 1 << 6,
        ELEC = 1 << 7,
        ROCK = 1 << 8,
    }

    [System.Serializable]
    public class CCardProperty
    {
        public ECardProperty m_Granted = ECardProperty.NONE;

        public bool CanAdd(CCardProperty _get)
        {
            var sum = this.m_Granted | _get.m_Granted;

            int i; //반복하면서 1이 몇개인지 판별. 4개를 초과하면 추가 못함
            for (i = 0; sum != 0; i++)
            {
                sum &= (sum - 1);
                if (i >= 4) return false;
            }

            return true;
        }

        public void Add(ECardProperty _get)
        { m_Granted |= _get; }

        public bool CheckPropertyCanText(ECardProperty _tpye)
        {
            return (m_Granted & _tpye) != 0;

        }
    }

    [System.Serializable]
    public class CNum_Bool
    {
        public CNum_Bool()
        {
            m_Num = 0;
            m_IsDA = false;
        }

        public void SetDataFromTSV(string _data)
        {
            var sp = _data.Split(",");
            m_Num = int.Parse(sp[0]);
            m_IsDA = sp.Length > 1;
        }

        public int m_Num = 0;
        public bool m_IsDA = false;

        public bool CheckIsEmpty()
        { return m_Num == 0 && m_IsDA == false; }

        public int Calc(int _avr)
        {
            if (m_IsDA == true) return m_Num += _avr;
            else return m_Num;
        }

        public void AddData(CNum_Bool _get, int _avr = 0)
        {
            m_Num += _get.m_Num;
            if (_get.m_IsDA == true) m_Num += _avr;
            m_IsDA = m_IsDA || _get.m_IsDA;
        }

        public void Clear() { m_IsDA = false; m_Num = 0; }
    }

    [System.Serializable]
    public class CDisk_Calc
    {
        public float m_ATK = 0;
        public float m_DEF = 0;
        public float m_STACK = 0;
        public float m_TARGET = 0;
        public float m_DEBUFF = 1;
    }


    [System.Serializable]
    public class CDisk : CData_CSV
    {
        public ECardType m_Type = ECardType.ATK;
        public int m_Tear = 0;

        public CNum_Bool m_Damage = new CNum_Bool();
        public CNum_Bool m_Targets = new CNum_Bool();
        public CNum_Bool m_StatusEff = new CNum_Bool();

        public CNum_Bool m_Defend = new CNum_Bool();
        public CNum_Bool m_Mana = new CNum_Bool();

        //TODO : 디버프 누적치는 하나로 합쳐짐
        //public int m_Burn_Self = 0;
        //public int m_Elec_Self = 0;
        //public int m_Damage_Self = 0;

        public int m_Debuff = 0;

        public bool m_Return = false;

        [Header("===========================")]
        public CCardProperty m_CurrProperty = new CCardProperty();

        public void SetProperties()
        {
            m_CurrProperty.m_Granted = ECardProperty.NONE;

            //if (m_Damage.CheckIsEmpty() == false || m_Damage_Self > 0)
            //    m_CurrProperty.m_Granted |= ECardProperty.DAMAGE;

            if (m_Targets.CheckIsEmpty() == false)
                m_CurrProperty.m_Granted |= ECardProperty.TARGETS;
            if (m_Defend.CheckIsEmpty() == false)
                m_CurrProperty.m_Granted |= ECardProperty.DEFEND;
            if (m_Mana.CheckIsEmpty() == false)
                m_CurrProperty.m_Granted |= ECardProperty.MANA;

            //if (m_Burn_Self != 0) m_CurrProperty.m_Granted |= ECardProperty.BURN;
            //if (m_Elec_Self != 0) m_CurrProperty.m_Granted |= ECardProperty.ELEC;
            if (m_Return == true) m_CurrProperty.m_Granted |= ECardProperty.RETURN;
        }

        public string BuffData_To_String_Icon(int _avr)
        {
            string res = "";

            if (m_Damage.m_Num > 0)
            {
                res += GetString((int)ETextIcon.Dmg, m_Damage.m_Num);
                if (m_Damage.m_IsDA == true) res = string.Format("{0}[DA]{2}", res, _avr);
            }
            if (m_Targets.m_Num > 0)
            {
                res += GetString((int)ETextIcon.Dmg, m_Targets.m_Num);
                if (m_Targets.m_IsDA == true) res = string.Format("{0}[DA]{2}", res, _avr);
            }
            if (m_StatusEff.m_Num > 0) // ? 상태 이상 어캐 표시하지;;
            {
                res += GetString((int)ETextIcon.Dmg, m_StatusEff.m_Num);
                if (m_StatusEff.m_IsDA == true) res = string.Format("{0}[DA]{2}", res, _avr);
            }

            if (m_Defend.m_Num > 0)
            {
                res += GetString((int)ETextIcon.Dmg, m_Damage.m_Num);
                if (m_Damage.m_IsDA == true) res = string.Format("{0}[DA]{2}", res, _avr);
            }
            if (m_Mana.m_Num > 0)
            {
                res += GetString((int)ETextIcon.Dmg, m_Mana.m_Num);
                if (m_Mana.m_IsDA == true) res = string.Format("{0}[DA]{2}", res, _avr);
            }



            return res;
        }

        public string GetString(int _icon, int _num)
        {
            string res = "";
            var icon = string.Format("<sprite={0}>", _icon);
            res = string.Format("{0}{1}{2}", res, icon, _num);
            return "";
        }

        public void AddData(CDisk _disk, int _avr = 0)
        {
            this.m_Damage.AddData(_disk.m_Damage, _avr);
            this.m_Targets.AddData(_disk.m_Targets, _avr);
            this.m_StatusEff.AddData(_disk.m_StatusEff, _avr);
            this.m_Defend.AddData(_disk.m_Defend, _avr);
            this.m_Mana.AddData(_disk.m_Mana, _avr);

            m_Debuff += _disk.m_Debuff;
            //m_Burn_Self = _disk.m_Burn_Self;
            //m_Elec_Self = _disk.m_Elec_Self;
            //m_Damage_Self += _disk.m_Damage_Self;

            m_Return = m_Return || _disk.m_Return;
        }

        public void AddData(CSkillCard _card)
        {
            this.m_Damage.m_Num += _card.m_Damage;
            this.m_Targets.m_Num += _card.m_Targets;
            this.m_StatusEff.m_Num += _card.m_StatusEff;
            this.m_Defend.m_Num += _card.m_Defend;
        }

        public void Clear()
        {
            m_Damage.Clear();
            m_Targets.Clear();
            m_StatusEff.Clear();

            m_Defend.Clear();
            m_Mana.Clear();

            m_Debuff = 0;
            //m_Burn_Self = 0;
            //m_Elec_Self = 0;
            //m_Damage_Self = 0;
            m_Return = false;
        }
    }

    public enum EATK_GenType
    {
        Combo = 0,
        Once,
        Rapid,
    }
    public enum ECardType
    {
        ATK = 0,
        ATK_ELEC,
        ATK_BURN,
        ATK_ROCK,

        DEF = 30,
        DEF_ELEC,
        DEF_BURN,
        DEF_ROCK,
    }

    static public int MaxSocketCount = 3;
    [System.Serializable]
    public class CSkillCard : CData_CSV
    {
        public EATK_GenType m_GenType = EATK_GenType.Once;
        public ECardType m_CardType = ECardType.ATK;

        public int m_Damage = 0;
        public int m_Defend = 0;
        public int m_StatusEff = 0; //상태이상 누적치

        public int m_Targets = 0;
        public int m_Discovery = 0;

        [Header("==========================")]
        public CCardProperty m_CurrProperty = new CCardProperty();
        [Header("==========================")]
        //public List<int> m_Sockets = new List<int>();

        public int m_Tear = 1;
        public EDice_Term m_Term = EDice_Term.NONE;
        public int m_NeedDice = 0;
        public List<CUtility.EDice> m_RequireDice = new List<CUtility.EDice>();

        public string m_Desc = "";
        //public string m_Particle = "";
        public string m_Anim = "";

        //데이터 읽으면서 추가하자

        public ENumableIcon GetStatusIcon()
        {
            switch (m_CardType)
            {
                case ECardType.ATK_BURN: return ENumableIcon.BURN;
                case ECardType.ATK_ELEC: return ENumableIcon.ELEC;
                case ECardType.ATK_ROCK: return ENumableIcon.ROCK;
            }

            return ENumableIcon.NONE;
        }
        public bool CanAddDisk(CDisk _disk)
        {
            //속성 없으면 속성 추가 안뜨게함
            if (m_CardType == ECardType.ATK && _disk.m_StatusEff.CheckIsEmpty() != true)
                return false;

            if (m_CardType == ECardType.DEF && _disk.m_StatusEff.CheckIsEmpty() != true)
                return false;

            //방어 카드는 방어 디스크만
            if (_disk.m_Type == ECardType.DEF &&
                (m_CardType == ECardType.ATK ||
                m_CardType == ECardType.ATK_BURN ||
                m_CardType == ECardType.ATK_ELEC ||
                m_CardType == ECardType.ATK_ROCK)) return false;

            //공격 카드는 공격 디스크만
            if (_disk.m_Type == ECardType.ATK &&
                (m_CardType == ECardType.DEF ||
                m_CardType == ECardType.DEF_BURN ||
                m_CardType == ECardType.DEF_ELEC ||
                m_CardType == ECardType.DEF_ROCK)) return false;

            //주사위 반환 중복 방지
            if ((m_CurrProperty.m_Granted & ECardProperty.RETURN) != 0
                && (_disk.m_CurrProperty.m_Granted & ECardProperty.RETURN) != 0)
                return false;

            //조건이 4개 안넘는지 체크
            return this.m_CurrProperty.CanAdd(_disk.m_CurrProperty) == true;
        }

        public void SetProperties()
        {
            m_CurrProperty.m_Granted = ECardProperty.NONE;

            if (m_Damage > 0) m_CurrProperty.m_Granted |= ECardProperty.DAMAGE;
            if (m_Targets > 0) m_CurrProperty.m_Granted |= ECardProperty.TARGETS;
            if (m_Defend > 0) m_CurrProperty.m_Granted |= ECardProperty.DEFEND;

            if (m_CardType == ECardType.ATK_ELEC || m_CardType == ECardType.DEF_ELEC)
                m_CurrProperty.m_Granted |= ECardProperty.ELEC;
            if (m_CardType == ECardType.ATK_BURN || m_CardType == ECardType.DEF_BURN)
                m_CurrProperty.m_Granted |= ECardProperty.BURN;
            if (m_CardType == ECardType.ATK_ROCK || m_CardType == ECardType.DEF_ROCK)
                m_CurrProperty.m_Granted |= ECardProperty.ROCK;
        }
    }

    [System.Serializable]
    public class CMonster : CData_CSV
    {
        //public List<CScriptable_MonsterSkill> m_Skills = new List<CScriptable_MonsterSkill>();
        public List<CScriptable_MonsterSkill> m_Skill_Atk = new List<CScriptable_MonsterSkill>();
        public List<CScriptable_MonsterSkill> m_Skill_Def = new List<CScriptable_MonsterSkill>();
        public List<CScriptable_MonsterSkill> m_Skill_Special = new List<CScriptable_MonsterSkill>();


        public int m_HP = 0;
    }

    [System.Serializable]
    public class CMonsterSkill : CData_CSV
    {
        public ECardType m_Type = ECardType.ATK;
        public EATK_GenType m_GenType = EATK_GenType.Once;

        public int m_Dmg = 0;
        public int m_Def = 0;
        public int m_Eff = 0;

        public int m_Count = 0;
        public int m_Cost = 0;
        public int m_SpecialCost = 0;

        public string m_Particle = "";
        public string m_Anim = "";

        public string m_ShoutOutDesc = "";
        //public CAttack_Info m_Atk_Info = null;
    }

    [System.Serializable]
    public enum EWeaponSkillType { NONE, SUB, NORMAL, MAIN }

    [System.Serializable]
    public enum ESkillType { NONE, ATK, DEF, BUFF, DEBUFF, BEAT/*연타*/ }

    [System.Serializable]
    public enum EShowActionEnemy { ATK = 0, DEF, ATK_DEF, BUFF, UNKNOWN }

    //[System.Serializable]
    //public class CDebuff : CData_CSV
    //{ public string m_Desc = ""; }


    static public string GetCalcTypeToString(EDice_CalcType _type)
    {
        switch (_type)
        {
            case EDice_CalcType.NONE: return "";
            case EDice_CalcType.DiceSum: return "<sprite=0>";
            case EDice_CalcType.DiceSub: return "<sprite=0>";
            case EDice_CalcType.DiceAVR: return "<sprite=0>";
            default: return "";
        }
    }



    //사용전 사용후

    [System.Serializable]
    public enum EDice_CalcType { NONE = 0, DiceSum, DiceSub, DiceAVR }

    [System.Serializable]
    public enum EWeaponType { NONE = 0, SWORD, AXE, STAFF }

    [System.Serializable]
    public class CTurnChar_Info
    {
        int m_HP = 10;
        int m_DEF = 0;
    }

    [System.Serializable]
    public enum ETextIcon
    {
        NONE = -1,
        Rock = 0,
        Electric = 1,
        Burn = 2,
        Def = 3,
        Dmg = 4,
        Time = 5,
        Reflect = 6,
    }

    [System.Serializable]
    public enum ENumableIcon
    {
        NONE = 0,

        DAMAGE = 1,
        TARGET,

        DEFEND,
        MANA,
        COUNT,
        RETURN,

        BURN,
        ELEC,
        ROCK,
    }


    static public void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        System.Random random = new System.Random();

        // 리스트 끝부터 시작하여 요소를 무작위로 선택하여 위치를 바꿈
        for (int i = n - 1; i > 0; i--)
        {
            int randomIndex = random.Next(0, i + 1);
            T temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    [System.Serializable]
    public class CDice : CData_Iconable
    {
        public int m_Tear = 0;
        public List<int> m_Eyes = new List<int>();
    }


    [System.Serializable]
    public class CSceneInfo : CData_CSV
    {
        public enum ESceneType
        {
            NONE = 0, BATTLE_LOW, BATTLE_MID, BATTLE_HIGH,
            BATTLE_BOSS, BATTLE_ELITE,
            EVENT, SHOP
        }
        public ESceneType m_Type = ESceneType.NONE;

        //열려야 하는 씬의 idx
        public uint m_Connection_Scene = 0;
        //MonsterGroup 이나 EventID 등이 들어옴
        public uint m_Connection_ID = 0;

        public int m_ResellCost = 30;
        public int m_RequireDiscovery = 0;

        public CReward_Day m_BasicReward = new CReward_Day();

        //public List<uint> m_Expected_Reward = new List<uint>();
        //public List<uint> m_Confirmed_Reward = new List<uint>();
    }

    [System.Serializable]
    public class CMonsterGroup : CData_CSV
    {
        public int m_Chapter = 0;
        public int m_Tear = 0;

        public int m_StartCost = 0;
        public int m_MaxCost = 0;

        public List<CScriptable_Monster> m_Slots = null;
        public int m_Reward_Discovery = 0;
    }

    [System.Serializable]
    public class CMini_Events : CData_CSV
    {
        public List<CMini_Event> m_Events = new List<CMini_Event>();
    }

    //턴 시작 전 사용할 미니 이벤트
    [System.Serializable]
    public class CMini_Event
    {
        public string m_Anim = "";
        public string m_script = "";
        public float m_Duration = 1f;
    }

    [System.Serializable]
    public class CEventLog
    {
        public List<SEventTerm> m_Term;
        public string m_Speaker = "";
        public int m_Idx = -1;

        public uint m_Log_ID = 0;
        public string m_Log = "";

        //연결 시킬 대사문
        public List<int> m_Connection = new List<int>();
        public SerializeDictionary<CUtility.ERewardable, int> m_AddRewards
            = new SerializeDictionary<ERewardable, int>();
    }

    //이벤트 시작 조건. 여러개의 조건 들고있을 수 있게??
    public enum EEventTerm
    {
        NONE = 0,
        LESS1, OVER1,
        LESS2, OVER2,
        LESS3, OVER3,
        GOLD, HP,
        MAX_HP
    }

    public enum ERewardable
    {
        GOLD,
        TOKKEN,
        HP,
        HP_MAX,
        MANA_MAX,

        CARD,
        DISK,
        SCENE,
        MANA_SKILL,
    }

    [System.Serializable]
    public struct SEventTerm
    {
        public EEventTerm m_Need;
        public int m_Count;

        public SEventTerm(EEventTerm _need = EEventTerm.NONE, int _count = 0)
        {
            m_Need = _need;
            m_Count = _count;
        }
    }

    [System.Serializable]
    public enum ELanguageTag { ENG = 0, KOR };
    [System.Serializable]
    public class CLanguageTag
    {
        public List<string> m_Text = new List<string>();
    }

    [System.Serializable]
    public class CReward_Day
    {
        public int m_MaxMana = 0;
        public int m_Max_HP = 0;
        public int m_HP = 0;
        public int m_Gold = 0;
        public int m_Tokken = 0;

        //이벤트 발견
        public List<uint> m_Cards = new List<uint>();
        public bool m_FillCards = true;
        public int m_DiskTear = -1;

        public uint m_Scene = 0;
        public uint m_ManaSkill = 0;

        public void Add(CUtility.ERewardable _reward, int _num) 
        {
            switch (_reward)
            {
                case ERewardable.GOLD: m_Gold += _num; break;
                case ERewardable.TOKKEN: m_Tokken += _num; break;
                case ERewardable.HP: m_HP += _num; break;
                case ERewardable.HP_MAX: m_Max_HP += _num; break;
                case ERewardable.MANA_MAX: m_MaxMana += _num; break;

                case ERewardable.CARD: m_Cards.Add((uint)_num); break;
                case ERewardable.DISK: m_DiskTear = _num; break;
                case ERewardable.MANA_SKILL: m_ManaSkill = (uint)_num; break;
                case ERewardable.SCENE: m_Scene = (uint)_num; break;
            }
        }

        public void Add(CReward_Day _get) 
        {
            m_MaxMana += _get.m_MaxMana;
            m_Max_HP += _get.m_Max_HP;
            m_HP += _get.m_HP;
            m_Gold += _get.m_Gold;
            m_Tokken += _get.m_Tokken;

            m_Cards.AddRange(_get.m_Cards);
            m_DiskTear = m_DiskTear > _get.m_DiskTear ? m_DiskTear : _get.m_DiskTear;
            m_Scene = _get.m_Scene != 0 ? _get.m_Scene : m_Scene;
            m_ManaSkill = _get.m_ManaSkill;
            //if(m_PlaceOfDay == 0)
            //m_PlaceOfDay = _get.m_PlaceOfDay;
        }

        public void Clear() 
        {
            m_MaxMana = 0;
            m_Max_HP = 0;
            m_HP = 0;
            m_Gold = 0;
            m_Tokken = 0;

            m_FillCards = true;
            m_Cards.Clear();
            m_DiskTear = -1;
            m_Scene = 0;
            m_ManaSkill = 0;
        }
    }

    public enum EManaSkill_Kind { AREA, USE, QUEST };

    [System.Serializable]
    public class CManaSkill : CData_Iconable
    {
        public int m_Cost = 0;

        public EManaSkillType m_Type = EManaSkillType.DEF;
        public int m_Param = 0;

        public string m_Particle = "";
    }

    [System.Serializable]
    public class CManaSkill_Area : CData_Iconable
    {
        public int m_Cost = 0;

        public float m_Range = 2f;
        public int m_Max = 10;

        //주사위 합산 방식 이 변수로 필요할듯

        public EManaSkillType m_Type = EManaSkillType.DEF;
        public string m_Particle = "";
    }

    [System.Serializable]
    public class CQuest : CData_Iconable
    {
        public enum ETerm { DMG = 0, DEF, MANA, DICE_LESS, DICE_OVER, DICE_SAME }

        public ETerm m_Term = ETerm.DEF;

        public int m_Count = 0;
        public int m_Require = 0;

        public int m_Reward_Num = 0;
        public EManaSkillType m_Reward_Type = EManaSkillType.DEF;
        public bool m_ResetOnEndTurn = false;
        public string m_Particle = "";
    }

    public enum EManaSkillType 
    {
        DICE_ADD, DICE_SET,

        DEF, HEAL, MANA,
        DMG_RANDOM, DMG_ALL,
        ADD_ROLL,
    }

    [System.Serializable]
    public enum EDiceCalcType { NONE, DICE_SUM, DICE_AVR }

    [System.Serializable]
    public class CBuff 
    {
        public ECardType m_CurrBuffType = ECardType.ATK;

        [SerializeField]int elec = 0;
        public System.Action CB_ChangeElec = null;
        public int m_Elec 
        {
            get { return elec; }
            set 
            {
                elec = value;
                if (elec < 0) elec = 0;
                if (CB_ChangeElec != null) CB_ChangeElec();
            }
        }
        public int m_Elec_Turn = 0;


        [SerializeField] int burn = 0;
        public System.Action CB_ChangeBurn = null;
        public int m_Burn
        {
            get { return burn; }
            set
            {
                burn = value;
                if (burn < 0) burn = 0;
                if (CB_ChangeBurn != null) CB_ChangeBurn();
            }
        }

        public int m_Rock = 0;

        public void Clear() 
        {
            m_Burn = 0;
            m_Elec = 0;
        }
    }

        /////////////////////////////////////////////////CSV DATA/////////////////////////////////////////////////
}



#region EDITOR

//serializableDictionary
[System.Serializable]
public class SerializeDictionary<K, V> : Dictionary<K, V>, ISerializationCallbackReceiver
{
    [SerializeField]
    protected List<K> keys = new List<K>();

    [SerializeField]
    protected List<V> values = new List<V>();

    public virtual void OnBeforeSerialize()
    {
        keys.Clear();
        values.Clear();

        foreach (KeyValuePair<K, V> pair in this)
        {
            keys.Add(pair.Key);
            values.Add(pair.Value);
        }
    }

    public virtual void OnAfterDeserialize()
    {
        this.Clear();

        for (int i = 0, icount = keys.Count; i < icount; ++i)
        { 
            this.Add(keys[i], values[i]); 
            //this.Add(default(K), default(V));
        }
    }
}
//https://everyday-devup.tistory.com/88
////////////////


//단축키들//
#if UNITY_EDITOR

[SelectionBase]
[ExecuteInEditMode]
public class CEdit_SelectParent : EditorWindow 
{
    //% = ctrl
    //& = alt
    //# = shift
    [MenuItem("CustomEdit/Select parent #p")]
    static void SelectParentOfObject()
    {//부모 선택하기 Shift p
        List<Object> parents = new List<Object>();

        foreach (Transform it in Selection.transforms)
        {
            parents.Add(it.parent.gameObject);
            //Debug.Log(it.parent.name);
        }
        Selection.objects = parents.ToArray();
        parents.Clear();
    }

    [MenuItem("CustomEdit/Select child #o")]
    static void SelectChildOfObjectOnly()
    {//자식 선택하기 Shilf o

        List<Object> childs = new List<Object>();

        foreach (Transform it in Selection.transforms)
        {
            if (it.childCount == 0) continue;
            childs.Add(it.GetChild(0).gameObject);
        }

        if (childs.Count == 0) return;
        Selection.objects = childs.ToArray();
        childs.Clear();
    }


}

//read only//
public class ReadOnlyAttribute : PropertyAttribute
{

}
#endif

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ReadOnlyAttribute))]
public class ReadOnlyDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property,
                                            GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, true);
    }

    public override void OnGUI(Rect position,
                               SerializedProperty property,
                               GUIContent label)
    {
        GUI.enabled = false;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = true;
    }
}

#endif
#endregion