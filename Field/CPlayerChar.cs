using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(CHitable))]
public class CPlayerChar : CTurnChar
{
    [Header("=============PLAYER_CHAR=================")]

    //public CScriptable_WeaponSkill m_WeaponSkillTest = null;

    public CWalkToPos m_Walker = null;
    public CMonster m_TargetEnemy = null;
    public Transform m_RHand = null;
    public CWeapon m_Weapon = null;

    public List<CHitable> m_SkillTargets = new List<CHitable>();

    public CUI_Skill_Manager m_SkillMgr = null;
    public CManaHandler m_ManaHandler = null;

    [Header("============================================")]
    //public CUI_Deck m_UI_Deck = null;
    //public CUI_Deck_Canvas m_UI_Deck = null;
    public CUI_Deck_Shower m_UI_Deck = null;

    //public List<Ctnr_Card> m_UsedDeck = new List<Ctnr_Card>();
    //public List<Ctnr_Card> m_Deck = new List<Ctnr_Card>();
    //public List<Ctnr_Card> m_Hand = new List<Ctnr_Card>();

    public List<CScriptable_CardSkill> m_UsedDeck = new List<CScriptable_CardSkill>();
    public List<CScriptable_CardSkill> m_DrawDeck = new List<CScriptable_CardSkill>();
    public List<CScriptable_CardSkill> m_Hand = new List<CScriptable_CardSkill>();

    [Header("============================================")]

    [SerializeField] CPlayerData playerData = null;
    public CUI_Quest m_UI_Quest = null;
    public CUI_ManaSkill m_UI_ManaArea = null;
    public CUI_ManaSkill_Use m_UI_ManaUse = null;

    

    private void Awake()
    {
        playerData = CGameManager.Instance.m_PlayerData;
        
        //m_Deck.Clear();
        ////덱 생성
        //foreach (var it in playerData.m_Deck) 
        //{
        //    var inst = Instantiate(it);
        //    inst.Spawn(m_Hitable); m_Deck.Add(inst);
        //}

        //마나 스킬 생성
        //m_ManaHandler.Spawn(playerData.m_ManaSkills);

        m_ManaHandler.m_MaxMana = playerData.m_MaxMana;
        m_ManaHandler.m_CurrMana = 0;
        m_ManaHandler.m_CurrMana = 20;
        m_ManaHandler.m_RegenMana = playerData.m_RegenMana;
        m_ManaHandler.m_CB_ManaChange = UseMana;

    }

    public void Start()
    {
        //foreach (var it in m_SkillMgr.m_SkillCards)
        //    it.OnClick_ChangeCard();

        //if (m_UI_Deck == null)
        //    m_UI_Deck = CGameManager.Instance.m_ScheduleMgr.m_UI_Deck;

        m_Hitable.m_Dead_CB = OnDie;
        m_Hitable.m_Owner = this;
        m_Hitable.m_MaxHP = playerData.m_MaxHP;
        m_Hitable.m_DEBUG_HP = playerData.m_HP;
        m_Hitable.m_CB_HP_Change = playerData.SetHP;

        if (CGameManager.Instance.m_DEBUG_GAMEMODE == true)
        {
            CGameManager.Instance.m_ScheduleMgr.m_UI_Deck.CreateDeck();
            playerData.InitUserData();
            Debug.Log("test creat");

            foreach (var it in playerData.m_Deck)
                it.m_Atk_Info.m_User = m_Hitable;
        }

        //시작시 카드 사용자 발행함
        foreach (var it in playerData.m_Deck)
            it.m_Atk_Info.m_User = m_Hitable;

        m_UI_Quest.Spawn(playerData.m_Quest);
        m_UI_ManaArea.Spawn(playerData.m_AreaSkill);
        m_UI_ManaUse.Spawn(playerData.m_UseSkill);
    }

    public CHit_Obj m_Hitter = null;

    //public override void Spawn() 
    //{
    //    m_Deck.Clear();

    //    //덱 생성
    //    foreach (var it in playerData.m_Deck)
    //    {
    //        var inst = Instantiate(it);
    //        inst.Spawn(m_Hitable); m_Deck.Add(inst);
    //    }

    //    foreach (var it in m_SkillMgr.m_SkillCards)
    //        it.OnClick_ChangeCard();
    //}
    
    //생성시 덱 초기화..
    public override void Spawn() 
    {
        //m_Deck = CGameManager.Instance.m_PlayerData.m_Deck;
        foreach (var it in CGameManager.Instance.m_PlayerData.m_Deck)
            m_DrawDeck.Add(it);

        if (m_SkillMgr != null)
            foreach (var it in m_SkillMgr.m_SkillCards)
                it.OnClick_ChangeCard(_noCost: true);
    }

    public override void OnBattle()
    {
        m_Hitable.OnBattle();

    }

    public void TestATK()
    {

    }

    public void OnCharge(int _idx) 
    {
        if (m_CanAnim == false) return;
        m_Anim.SetBool("ReadyToUse", true);
        m_Weapon.Charge(_idx);
    }

    public void UnCharge() 
    {
        m_Anim.SetBool("ReadyToUse", false);
        m_Weapon.UnCharge();
    }

    public List<System.Action> m_SkillUseQueue = new List<System.Action>();

    public void ATK_SkillCard(CScriptable_CardSkill _skill) 
    {
        if (m_CanAnim == false) 
        {
            m_SkillUseQueue.Add(() => { ATK_SkillCard(_skill); });
            return;
        }
        m_CanAnim = false;

        m_Hitter = Instantiate(_skill.m_Atk_Info.m_HitObj);
        m_Hitter.Spawn(
            m_RHand,
            this,
            m_SkillTargets,
            _skill.m_Atk_Info);

   
        //스킬 사용 후 사용가능 주사위 다시 서칭
        m_SkillMgr.CheckCanUseCards();

        //몬스터 피격 로그 초기화
        m_SkillMgr.m_EnemyGroup.ResetHitLog();

        int num = _skill.m_Atk_Info.m_SumAll.m_Targets.m_Num;
        m_Hitter.GenSkill(_skill.m_Data.m_GenType, num, m_Hitable);
        //m_Hitter.CheckMoreAttack();
    }

    //사용 스킬 등을 등록함
    public void ATK_Skill(System.Action _cb_Skill) 
    {
        if (m_CanAnim == false)
        {
            m_SkillUseQueue.Add(() => { ATK_Skill(_cb_Skill); });
            return;
        }

        //몬스터 피격 로그 초기화
        m_SkillMgr.m_EnemyGroup.ResetHitLog();

        m_CanAnim = false;
        m_IsEndAttack = false;
        _cb_Skill();

        //m_Anim.SetTrigger(m_DefaultATK);
        //사용 스킬이 여기서 실행되야함
    }

    public override void Attack()
    {
        if (m_Hitter == null) return;
        m_Hitter.Anim_Attack();
    }

    //2번 호출 되어야 다음 공격 이어나감
    public override void EndAttack()
    {
        if (m_Hitter == null)
        {
            //hitter 없이 공격하는 마나 스킬 체크
            if (m_CanAnim == false) 
            { DoneAnim(); }

            return;
        }

        //if (m_IsEndAttack == false) 
        //{ m_IsEndAttack = true; }
        //else  m_Hitter.CheckMoreAttack();

        if (m_Hitter.m_Info.m_GenType == CUtility.EATK_GenType.Combo)
            m_Hitter.CheckMoreAttack();
        else 
        {
            m_Anim.SetBool("EndCombo", true);
            DoneAnim();
        }
    }


    //이 함수는 사용 안하는것 같음...
    //public override void Attack_Done()
    //{
    //    Debug.Log("REMOVE HITTER");
    //
    //    m_Hitter.DestroySelf();
    //    m_Hitter = null;
    //
    //    DoneAnim();
    //}

    public override void DoneAnim()
    {
        Debug.Log("DONE ANIM");
        //m_SkillMgr.m_EnemyGroup.AttackDone();
        AttackDone();
    }

    public override void DoneSkill() 
    {
        Debug.Log("DONE SKILL");
        //m_SkillMgr.m_EnemyGroup.AttackDone();
        AttackDone();
    }


    public bool m_CanAnim = true;
    public void AttackDone() 
    {
        if (m_IsEndAttack == false)
        { 
            m_IsEndAttack = true;
            return; 
        }

        if (m_Hitter != null) m_Hitter.DestroySelf();
        CGameManager.Instance.m_TurnManager.m_EnemyGroup.ClearTargetTextIcon();

        m_CanAnim = true;
        if (m_SkillUseQueue.Count > 0) 
        { m_SkillUseQueue[0](); m_SkillUseQueue.RemoveAt(0); }
    }

    //계시 카드 뽑기
    public CScriptable_CardSkill DrawCard() 
    {
        int rand = Random.Range(0, m_DrawDeck.Count);
        var draw = m_DrawDeck[rand];
        m_DrawDeck.RemoveAt(rand);

        if (m_DrawDeck.Count == 0)
        {//뽑을 카드 없으면 리롤
            CUtility.ShuffleList<CScriptable_CardSkill>(m_UsedDeck);
            foreach (var it in m_UsedDeck) 
            {
                it.m_DeckState = CScriptable_CardSkill.ECardState.DECK;
                m_DrawDeck.Add(it); 
            }
            m_UsedDeck.Clear();
            Debug.Log("섞음");
        }

        //return draw.m_UI_Card.m_SkillCard;
        return draw;
    }

    public void OnDiceResult() 
    {
        
    }

    public override void TurnBegin()
    {
        base.TurnBegin();
        //m_ManaHandler.ManaRegen();
        m_Hitable.Begin_Turn();

        m_CanAnim = true;
    }

    public override void TurnEnd()
    {
        base.TurnEnd();
        m_Hitable.Calc_TurnEnd();
        EndTurn();

        m_CanAnim = false;

        //몬스터 피격 로그 제거
        m_SkillMgr.m_EnemyGroup.ResetHitLog();
    }

    public override void OnDie()
    {
        m_Anim.SetTrigger("Dead");
    }

    public void Anim_Die() 
    {
        CGameManager.Instance.m_ScheduleMgr.m_UI_PlayerDie.SetActive(true);
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.ECustom.Hit_Organic, true);
    }

    //여기 있는 함수들 플레이어가 들고 있어도 될듯??
    //퀘스트를 위해 주사위 사용, 데미지 등의 함수 대행
    public void UseDice(int _eye)
    { m_UI_Quest.UseDice(_eye); }
    public void GiveDmg(int _dmg)
    { m_UI_Quest.GiveDmg(_dmg); }
    public void GainDef(int _def)
    { m_UI_Quest.GainDef(_def); }
    public void UseMana(int _mana)
    { m_UI_Quest.UseMana(_mana); }
    //퀘스트 초기화
    public void EndTurn()
    { m_UI_Quest.EndTurn(); }
    
}
