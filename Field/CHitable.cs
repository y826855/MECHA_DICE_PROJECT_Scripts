using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHitable : MonoBehaviour
{
    public CMoveable m_Owner = null;

    public int m_MaxHP = 0;
    [SerializeField] int DEBUG_HP = 50;
    public System.Action<int> m_CB_HP_Change = null;
    public int m_DEBUG_HP
    {
        get { return DEBUG_HP; }
        set
        {
            DEBUG_HP = value;
            if (DEBUG_HP > m_MaxHP) DEBUG_HP = m_MaxHP;
            m_Field_Info.SetHP();
            if (m_CB_HP_Change != null) m_CB_HP_Change(DEBUG_HP);
        }
    }
    [SerializeField] int DEBUG_DEF = 0;
    public int m_DEBUG_DEF
    {
        get { return DEBUG_DEF; }
        set
        {
            DEBUG_DEF = value;
            m_Field_Info.SetDEF();
        }
    }

    public bool m_IsDead = false;

    public System.Action m_Dead_CB = null;
    public System.Action m_Hit_CB = null;

    //public CBuff_Ctrl m_Buff_Ctrl = null;
    //float dmg, debuff

    [Header("===================")]
    public CUI_Field_Info m_Field_Info = null;
    public System.Action m_CB_RemoveTargetIcon = null;

    [Header("===================")]
    public CUtility.ECardType m_CurrBuffType = CUtility.ECardType.ATK;
    [SerializeField]int currBuffStack = 0;
    int elecTurn = 0;
    public int m_CurrBuffStack
    {
        get { return currBuffStack; }
        set { 
            currBuffStack = value;


            //switch (m_CurrBuffType) 
            //{
            //    case CUtility.ECardType.ATK_ELEC: m_Field_Info.ChangeElec(currBuffStack); break;
            //    case CUtility.ECardType.ATK_BURN: m_Field_Info.ChangeBurn(currBuffStack); break;
            //}

            if (currBuffStack < 0)
            {
                currBuffStack = 0;
                m_CurrBuffType = CUtility.ECardType.ATK;
            }
            else
                m_Field_Info.ChangeBuff(m_CurrBuffType, currBuffStack);

        }
    }
    


    //public CUtility.CBuff m_Buffs = new CUtility.CBuff();

    public bool m_IsEnemy = false;



    [Header("===================")]
    public Transform m_HitPos = null;
    ParticleSystem m_Hit_Elec = null;
    ParticleSystem m_Hit_Burn = null;
    ParticleSystem m_Hit_Rock = null;

    public void Start()
    {
        if (m_Field_Info != null) m_Field_Info.RefreashData();

        m_DEBUG_HP = m_MaxHP;

        var turnMgr = CGameManager.Instance.m_TurnManager;
        if (turnMgr == null) return;
        m_Hit_Burn = Instantiate(turnMgr.m_Hit_Burn, m_HitPos);
        m_Hit_Elec = Instantiate(turnMgr.m_Hit_Elec, m_HitPos);
        m_Hit_Rock = Instantiate(turnMgr.m_Hit_Rock, m_HitPos);

        m_Field_Info.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        if (m_DEBUG_HP > m_MaxHP) m_MaxHP = m_DEBUG_HP;

        m_Field_Info.SetHP();
        m_Field_Info.SetDEF();
    }

    public void OnBattle() 
    {
        m_Field_Info.gameObject.SetActive(true);
    }

    //==============================================================================//
    public void CheckDebuffChange(CUtility.ECardType _type, CHitable _user, int _num, int _dmg) 
    {
        if (m_CurrBuffType == CUtility.ECardType.ATK) 
        {//첫 디버프 들어옴

            if (_type == CUtility.ECardType.ATK_ELEC) 
            {//감전시 높은 스택으로 바뀜
                m_CurrBuffType = CUtility.ECardType.ATK_ELEC;
                m_CurrBuffStack = _num;
                elecTurn = 3;
            }

            else if (_type == CUtility.ECardType.ATK_BURN)
            {
                m_CurrBuffType = CUtility.ECardType.ATK_BURN;
                m_CurrBuffStack += _num;
            }

            return;
        }

        switch (m_CurrBuffType) 
        {
            case CUtility.ECardType.ATK_ELEC:
                StackElec(_type, _user, _num, _dmg); break;
            case CUtility.ECardType.ATK_BURN:
                StackBurn(_type, _user,_num, _dmg); break;
                //case CUtility.ECardType.ATK_ROCK:
                //    StackRock(_type, _num); break;
        }
    }

    public void StackElec(CUtility.ECardType _type, CHitable _user, int _num, int _dmg) 
    {
        int stack = m_CurrBuffStack + _num;

        switch (_type)
        {
            case CUtility.ECardType.ATK_ELEC:
                if (_dmg > 0) StartCoroutine(Electric_Share(_dmg));
                elecTurn = 3;
                //m_CurrBuffStack += _num;
                //감전 바뀐 스택 수식 적용
                m_CurrBuffStack = m_CurrBuffStack < _num ? _num : m_CurrBuffStack;
                break;
            case CUtility.ECardType.ATK_BURN:
                //OnHit(stack / 2, CUtility.ETextIcon.NONE);
                OnHit(stack, 0, CUtility.ETextIcon.NONE);
                m_CurrBuffStack = 0;
                m_CurrBuffType = CUtility.ECardType.ATK;
                break;
            case CUtility.ECardType.ATK_ROCK:
                m_Hit_Rock.Play();
                //OnHit(stack / 2, CUtility.ETextIcon.NONE);
                OnHit(stack, 0, CUtility.ETextIcon.NONE);
                //TODO : gain mana
                m_CurrBuffStack = 0;
                m_CurrBuffType = CUtility.ECardType.ATK;
                break;
        }
    }

    public void StackBurn(CUtility.ECardType _type, CHitable _user, int _num, int _dmg)
    {
        int stack = m_CurrBuffStack + _num;

        switch (_type)
        {
            case CUtility.ECardType.ATK_BURN:
                m_CurrBuffStack += _num; break;
            case CUtility.ECardType.ATK_ELEC:
                //OnHit(stack / 2, CUtility.ETextIcon.NONE);
                OnHit(stack, 0, CUtility.ETextIcon.NONE);
                m_CurrBuffStack = 0; m_CurrBuffType = CUtility.ECardType.ATK; break;
            case CUtility.ECardType.ATK_ROCK:
                m_Hit_Rock.Play();
                //OnHit(stack / 2, CUtility.ETextIcon.NONE);
                OnHit(stack, 0, CUtility.ETextIcon.NONE);
                _user.GainDef(stack);
                m_CurrBuffStack = 0;  m_CurrBuffType = CUtility.ECardType.ATK; break;
        }
    }

    //방어 속성 받기
    public void CheckBuffChange(CUtility.ECardType _type, int _num) 
    {
        switch (m_CurrBuffType) 
        {
            case CUtility.ECardType.ATK: break;
            case CUtility.ECardType.ATK_ELEC:
                DestackElec(_type, _num); break;
            case CUtility.ECardType.ATK_BURN:
                DestackBurn(_type, _num); break;
        }
    }

    public void DestackElec(CUtility.ECardType _type, int _num) 
    {
        int stack = m_CurrBuffStack + _num;

        switch (_type) 
        {
            case CUtility.ECardType.DEF_ELEC:
                m_CurrBuffStack -= _num; break;
            case CUtility.ECardType.DEF_BURN:
                GainDef(stack / 2); break;
            case CUtility.ECardType.DEF_ROCK:
                m_CurrBuffStack -= _num; GainDef(stack); break;
        }
    }
    public void DestackBurn(CUtility.ECardType _type, int _num) 
    {
        int stack = m_CurrBuffStack + _num;

        switch (_type)
        {
            case CUtility.ECardType.DEF_ELEC:
                GainDef(stack / 2); break;
            case CUtility.ECardType.DEF_BURN:
                m_CurrBuffStack -= _num; break;
            case CUtility.ECardType.DEF_ROCK:
                m_CurrBuffStack -= _num; GainDef(stack); break;
        }
    }

    //==============================================================================//

    public void GainDebuff(CUtility.ECardType _eff, int _num)
    {
        //m_CurrBuffType
        //여기서 속성 전환이 일어난다면?
        CheckDebuffChange(_eff, m_Owner.m_Hitable, _num, 0);
    }

    //속성 체크
    //전환 방정식들
    public void OnHit(CScriptable_CardSkill _skill)
    {
        var data = _skill.m_Atk_Info.m_SumAll;

        if (data.m_StatusEff.m_Num > 0)
            CheckDebuffChange(_skill.m_Data.m_CardType, _skill.m_Atk_Info.m_User,
                data.m_StatusEff.m_Num, data.m_Damage.m_Num);

        OnHit(data.m_Damage.m_Num, 0, CUtility.ETextIcon.NONE);
        if (m_Hit_CB != null) m_Hit_CB();
    }
    //피격
    //public void OnHit(CScriptable_CardSkill _skill)
    //{
    //    var data = _skill.m_SumAll;

    //    Debug.Log("HITTED");

    //    if (_skill.m_Data.m_CardType == CUtility.ECardType.ATK_ELEC)
    //    { //감전이 있으면 감전 기믹 수행
    //        if (m_Buffs.m_Elec > 0) //Electric_Share(data.m_Damage.m_Num);
    //            StartCoroutine(Electric_Share(data.m_Damage.m_Num));
    //        m_Buffs.m_Elec_Turn = 3;
    //        m_Buffs.m_Elec += data.m_StatusEff.m_Num;
    //    }

    //    OnHit(data.m_Damage.m_Num, CUtility.ETextIcon.NONE);
    //    if (m_Hit_CB != null) m_Hit_CB();

    //    switch (_skill.m_Data.m_CardType)
    //    {
    //        case CUtility.ECardType.ATK_BURN:
    //            m_Buffs.m_Burn += data.m_StatusEff.m_Num;
    //            break;
    //        case CUtility.ECardType.ATK_ROCK:
    //            m_Buffs.m_Rock += data.m_StatusEff.m_Num;
    //            break;
    //    }
    //}

    //방어획득
    public void GainDef(CScriptable_CardSkill _skill)
    {
        var data = _skill.m_Atk_Info.m_SumAll;

        if (data.m_StatusEff.m_Num > 0)
            CheckBuffChange(_skill.m_Data.m_CardType, data.m_StatusEff.m_Num);

        GainDef(data.m_Defend.m_Num);
    }

    //방어 획득 //수치만
    public void GainDef(int _num) 
    { 
        m_DEBUG_DEF += _num;
        if (m_IsEnemy == false) //플레이어가 방어도 얻는거 체크 퀘스트
        { (m_Owner as CPlayerChar).GainDef(_num); }
    }

    //피격
    public void OnHit(CScriptable_MonsterSkill _skill) 
    {
        var data = _skill.m_Data;
        //Debug.Log("HITTED");

        if (data.m_Eff > 0)
            CheckDebuffChange(data.m_Type, _skill.m_Atk_Info.m_User, data.m_Eff, data.m_Dmg);

        OnHit(data.m_Dmg, 0, CUtility.ETextIcon.NONE);
        if (m_Hit_CB != null) m_Hit_CB();
    }

    //방어 획득
    public void GainDef(CScriptable_MonsterSkill _skill)
    {
        var data = _skill.m_Data;

        m_DEBUG_DEF += data.m_Def;

        var logPool = CGameManager.Instance.m_TurnManager.m_LogPool;
        string dmgLog = "";
        dmgLog = string.Format("<sprite={0}>{1}", (int)CUtility.ETextIcon.Def, data.m_Def);
        logPool.SpawnHitLog(this.transform.position, dmgLog);
    }

    public void Begin_Turn() 
    {
        if (m_DEBUG_DEF > 0)
            m_DEBUG_DEF = 0;
    }

    //턴종 계산
    public void Calc_TurnEnd()
    {
        switch (m_CurrBuffType) 
        {
            case CUtility.ECardType.ATK_ELEC:
                if (elecTurn-- <= 0) RemoveElec(); break;
            case CUtility.ECardType.ATK_BURN:
                OnHit(m_CurrBuffStack, 0,CUtility.ETextIcon.Burn);
                m_Hit_Burn.Play();
                m_CurrBuffStack /= 2;
                break;
        }
    }

    public void RemoveElec()
    {
        m_CurrBuffStack -= 3; 
    }

    //public void OnHit(float _dmg, string _dmgType = "") 
    //속성 기믹에 의한 피격
    public void OnHit(int _dmg, int _stack, CUtility.ETextIcon _dmgType)
    { 
        int dmg = (int)_dmg;


        var logPool = CGameManager.Instance.m_TurnManager.m_LogPool;
        string dmgLog = "";
        if (_dmgType != CUtility.ETextIcon.NONE)
            //dmgLog = string.Format("<sprite={0}>{1}{2}", (int)_dmgType, _stack, dmg);
            dmgLog = string.Format("<sprite={0}>{1}", (int)_dmgType, dmg);
        else
            dmgLog = string.Format("{0}", dmg);

        //토탈 피격 데미지에 저장함
        m_Field_Info.AddHit(_dmg, _stack, _dmgType);

        logPool.SpawnHitLog(this.transform.position, dmgLog);


        //퀘스트가 방어도 때려도 올라가게 할까?
        if (m_IsEnemy == true) 
        { CGameManager.Instance.m_TurnManager.m_PlayerChar.GiveDmg(_dmg); }

        if (m_DEBUG_DEF > 0)
        {
            int calc = m_DEBUG_DEF - dmg;
            m_DEBUG_DEF = calc;
            dmg = calc > 0 ? 0 : -calc;

            //방어 못하면 피격 모션 나옴
            if (m_DEBUG_DEF <= 0)
            {
                Debug.Log("hit anim");
                m_Owner.m_Anim.SetTrigger("Hit");
            }
        }

        m_DEBUG_HP -= dmg;

        if (m_DEBUG_HP <= 0) OnDie();
    }


    //감전 딜 공유
    ///public void Electric_Share(float _dmg)
    ///
    IEnumerator Electric_Share(int _dmg)
    {
        //particle electric show
        yield return CUtility.m_WFS_DOT2;

        elecTurn = 3;

        if (m_IsEnemy == true)
        {
            var monsters = CGameManager.Instance.m_TurnManager.m_EnemyGroup.m_SpawnedMonsters;
            foreach (var it in monsters)
            {
                //감전 있는지 체크
                if (it.m_Hitable.m_CurrBuffType == CUtility.ECardType.ATK_ELEC &&
                    it.m_Hitable.DEBUG_HP > 0)
                {
                    it.m_Hitable.EffectionElec(_dmg);
                    yield return CUtility.m_WFS_DOT2;
                }
            }
        }
        else
        { EffectionElec(_dmg); }
    }

    //감전 딜
    public void EffectionElec(int _dmg) 
    {
        m_CurrBuffStack -= 1;
        OnHit(_dmg, 0, CUtility.ETextIcon.Electric);
        elecTurn = 3;
        m_Hit_Elec.Play();
    }


    public void OnDie() 
    {
        if (m_IsDead == true) return;
        m_IsDead = true;

        m_Dead_CB();
    }
}
