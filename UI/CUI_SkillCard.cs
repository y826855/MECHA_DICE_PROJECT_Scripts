using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

using System.Linq;
using TMPro;


public class CUI_SkillCard : CUI_Showable, ISelectEvent
{
    public CScriptable_CardSkill m_SkillCard = null;
    public CUI_CardAnim m_CardAnim = null;

    public List<Sprite> m_Icon_Dice = new List<Sprite>();

    public List<Image> m_Img_TermDice = null;
    public List<CUI_CardDiceAnim> m_Img_Dices_Start = new List<CUI_CardDiceAnim>();
    public List<CUI_CardDiceAnim> m_Img_Dices_Add = new List<CUI_CardDiceAnim>();
    [SerializeField] List<Image> m_ColorParts = new List<Image>();

    [Header("===============================================")]
    //-------------------------------------------//
    public TextMeshProUGUI m_TMP_Name = null;
    public TextMeshProUGUI m_TMP_Term = null;
    public TextMeshProUGUI m_TMP_Avr = null;

    [System.Serializable]
    public class CUI_IconText
    {
        public CUtility.ENumableIcon m_Icon = CUtility.ENumableIcon.NONE;
        public Image m_Img_Icon = null;
        public TextMeshProUGUI m_TMP_NumUp = null;
        public TextMeshProUGUI m_TMP_NumDw = null;
    }

    public List<CUI_PropertyText> m_UI_PropertyText_Main = new List<CUI_PropertyText>();
    public CUI_PropertyText m_IconText_Target_Count = null;
    //public List<CUI_PropertyText> m_UI_PropertyText_Disk = new List<CUI_PropertyText>();

    public List<CDice> m_ReserveDices = new List<CDice>();
    public List<CDice> m_ChargedDice = new List<CDice>();
    //-------------------------------------------//
    [Header("===============================================")]
    public Image m_ImgCanUse = null;
    public CUI_Skill_Manager m_SkillMgr = null;
    public CUI_Reload_Disk m_Reload_Disk = null;

    //[Header("===============================================")]
    //-------------------------------------------//
    //public Button m_Btn_ChangeCard = null;
    //-------------------------------------------//

    
    public enum EState { 
        IDLE, DECK, 
        FOCUS_NONE, FOCUS_CHARGED, 
        USING
    };
    public EState m_CardState = EState.IDLE;

    [Header("===============================================")]
    public int m_CurrDiskIdx = 0;
    //public bool m_IsOnDeck = false;
    //public bool m_IsCharged = false;
    //public bool m_IsUseing = false;




    [SerializeField] bool IsCanUse = false;
    public bool m_IsCanUse
    {
        get { return IsCanUse; }
        set
        {
            IsCanUse = value;
            m_ImgCanUse.enabled = IsCanUse;
        }
    }
    //TODO : CANUSE 수정해야함

    public void InitData() 
    {
        foreach (var it in m_UI_PropertyText_Main)
            it.InitData();
        m_IconText_Target_Count.InitData();
    }

    //UI 데이터 세팅함
    public void SetUIData(CScriptable_CardSkill _skill)
    {
        if (m_SkillCard != _skill)
        {//다른 카드 정보 들어오면 새로고침
            InitData();
            m_Reload_Disk.Init(_skill);
            SetColorParts_Color(_skill.m_Data.m_CardType);
        }

        
        m_SkillCard = _skill;
        var skillData = m_SkillCard.m_Data;

        m_TMP_Name.text = skillData.m_Name;
        m_TMP_Term.text = skillData.m_Term.ToString();

        var disableColor = CGameManager.Instance.color_Disable;
        foreach (var it in m_Img_Dices_Start) it.m_Image.color = disableColor;


        //필요 주사위 표기
        for (int i = 0; i < m_Img_Dices_Start.Count; i++)
        {
            if (i < skillData.m_NeedDice)
            {
                m_Img_Dices_Start[i].gameObject.SetActive(true);
                m_Img_Dices_Start[i].m_Image.color = disableColor;
            }
            else m_Img_Dices_Start[i].gameObject.SetActive(false);
        }
        

        for (int i = 0; i < m_Img_TermDice.Count; i++) 
        {
            if (i < skillData.m_RequireDice.Count) 
            {
                m_Img_TermDice[i].gameObject.SetActive(true);
                //m_Img_TermDice[i].color = disableColor;
            }
            else m_Img_TermDice[i].gameObject.SetActive(false);
        }


        if (skillData.m_RequireDice[0] == CUtility.EDice.ANY)
            m_Img_TermDice[0].sprite = m_Icon_Dice[0];

        //조건에 따른 필요 주사위 표기
        else if (skillData.m_RequireDice.Count > 0)
        {
            //Same, Over, Less
            if (skillData.m_RequireDice.Count == 1)
            {
                //Debug.Log(_skill.m_Require_Dice[0]);
                var sp = m_Icon_Dice[(int)skillData.m_RequireDice[0]];
                m_Img_TermDice[0].sprite = sp;
            }
            //Term == Pick
            else// if (_skill.m_Require_Dice[0] != CUtility.EDice.ANY)
            {
                for (int i = 0; i < skillData.m_NeedDice; i++)
                { m_Img_TermDice[i].sprite = m_Icon_Dice[(int)skillData.m_RequireDice[i]]; }
            }
        }

        //디스크 티어 표기
        ShowDiskData();
        SetTextData();
    }

    public void ShowDiskData() 
    {
        //디스크 티어 표기
        var upgradeCount = m_SkillCard.m_Disks.Count;
        Color color = Color.white;
        for (int i = 0; i < m_Img_Dices_Add.Count; i++)
        {
            if (i < m_SkillCard.m_Disks.Count)
            {
                m_Img_Dices_Add[i].gameObject.SetActive(true);
                //switch (skillData.m_Sockets[i])
                switch (m_SkillCard.m_Disks[i].m_Data.m_Tear)
                {
                    case 1: color = CGameManager.Instance.color_Tear1; break;
                    case 2: color = CGameManager.Instance.color_Tear2; break;
                    case 3: color = CGameManager.Instance.color_Tear3; break;
                    default: color = CGameManager.Instance.color_Disable; break;
                }

                if (i < upgradeCount)
                    m_Img_Dices_Add[i].m_Child_Img.color = color;
                else
                    m_Img_Dices_Add[i].m_Child_Img.color = Color.clear;

                //if (i < upgradeCount)
                //{ color.a = CGameManager.Instance.color_Wait.a; }
                //else color.a = CGameManager.Instance.color_Disable.a;
                //m_Img_Dices_Add[i].m_Image.color = color;
            }
            else m_Img_Dices_Add[i].gameObject.SetActive(false);
        }
    }

    //충전 가능 여부 체크
    public void CheckCanCharge()
    {
        if (m_SkillMgr == null) return;
        Debug.Log("CHECK CHARGE");

        //충전이 되어 있다면 추가 충전 확인
        //if (m_IsCharged == true)
        if (m_CardState == EState.FOCUS_CHARGED)
        {
            CheckCanCharge_Over();
            return;
        }

        m_ReserveDices.Clear();
        int needDice = m_SkillCard.m_Data.m_NeedDice;
        var require = m_SkillCard.m_Data.m_RequireDice;

        //주사위를 전부 dictionary에 넣어 조건 체크 쉽게함
        var originDices = m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices;
        Dictionary<int, List<CDice>> dices = new Dictionary<int, List<CDice>>();
        foreach (var it in originDices)
        {
            if (dices.ContainsKey(it.m_eye) == false)
            { dices[it.m_eye] = new List<CDice>(); }
            dices[it.m_eye].Add(it);
        }

        //주사위 부족
        if (m_SkillCard.m_Data.m_NeedDice > originDices.Count)
        {
            Debug.Log("주사위 부족");
            SetTextData();
            m_IsCanUse = false;
            return;
        }

        //조건 체크
        switch (m_SkillCard.m_Data.m_Term)
        {
            case CUtility.EDice_Term.NONE:
                m_IsCanUse = true;
                for (int i = 0; i < needDice; i++) m_ReserveDices.Add(originDices[i]);
                break;
            case CUtility.EDice_Term.MORE:
                CheckMoreLess(_more: true, dices, needDice);
                break;
            case CUtility.EDice_Term.LESS:
                CheckMoreLess(_more: false, dices, needDice);
                break;
            case CUtility.EDice_Term.SAME: 
                CheckSame(dices, needDice);
                break;
            case CUtility.EDice_Term.SAME_OVER: break;
            case CUtility.EDice_Term.SAME_LESS: break;
            case CUtility.EDice_Term.PICK:
                CheckPick(dices, needDice);
                break;
            case CUtility.EDice_Term.STR:
                CheckStr(dices, needDice);
                break;
            case CUtility.EDice_Term.ODD_NUM: break;
            case CUtility.EDice_Term.EVEN_NUM: break;
            default: break;
        }

        foreach (var it in dices.Keys)
            dices[it].Clear();
        dices.Clear();
    }

    //조건체크_이상, 이하
    void CheckMoreLess(bool _more, Dictionary<int, List<CDice>> _dices, int _needDice)
    {
        int req = (int)m_SkillCard.m_Data.m_RequireDice[0];

        List<CDice> res = new List<CDice>();

        foreach (var key in _dices.Keys)
        {
            //over less 동시에 처리하기
            bool check =
                (_more == true && key >= req) ||
                (_more == false && key <= req);

            if (check == true && _dices.ContainsKey(key))
            {
                res.AddRange(_dices[key]);
            }
        }

        //가까운 순으로 정렬
        m_ReserveDices = res.OrderBy(eye => eye.transform.GetSiblingIndex()).Take(_needDice).ToList();
        m_IsCanUse = res.Count >= _needDice;
    }

    //조건체크_사용자지정
    void CheckPick(Dictionary<int, List<CDice>> _dices, int _needDice)
    {
        foreach (var it in m_SkillCard.m_Data.m_RequireDice)
        {
            if (_dices.ContainsKey((int)it) == false)
            {
                m_ReserveDices.Clear();
                m_IsCanUse = false;
                return;
            }

            var target = _dices[(int)it];
            m_ReserveDices.Add(target[0]);

            target.RemoveAt(0);
            if (target.Count == 0) _dices.Remove((int)it);
        }
        m_IsCanUse = true;
    }

    void CheckSame(Dictionary<int, List<CDice>> _dices, int _needDice) 
    {
        if (m_SkillCard.m_Data.m_RequireDice[0] == CUtility.EDice.ANY)
        {//지정되지 않은 same
            for (int idx = 0; idx < 6; idx++) 
            {//낮은 수 부터 체크함
                if (_dices.ContainsKey(idx) == true && _dices[idx].Count >= _needDice) 
                {
                    for (int i = 0; i < _needDice; i++)
                        m_ReserveDices.Add(_dices[idx][i]);
                    m_IsCanUse = true;
                    return;
                }
            }
            m_IsCanUse = false;
        }
        else
        {//지정된 놈의 same
            int idx = (int)m_SkillCard.m_Data.m_RequireDice[0];
            if (_dices.ContainsKey(idx) == true && _dices[idx].Count >= _needDice)
            {
                for (int i = 0; i < _needDice; i++)
                    m_ReserveDices.Add(_dices[idx][i]);
                m_IsCanUse = true;
                return;
            }
            m_IsCanUse = false;
        }
    }

    //조건체크_스트레이트
    void CheckStr(Dictionary<int, List<CDice>> _dices, int _needDice)
    {
        //낮은 수 부터 스트레이트 구성함
        int count = 0;

        for(int i =0; i < 6;i++)
        {
            count = _dices.ContainsKey(i) == true ? count + 1 : 0;

            if (count == _needDice) 
            {
                int start = i - count +1;
                int end = start + _needDice;
                m_IsCanUse = true;

                for (int idx = start; idx < end; idx++) 
                { m_ReserveDices.Add(_dices[idx][0]);  }
                return;
            }
        }
        m_IsCanUse = false;
    }

    //카드 색상 지정
    public void SetColorParts_Color(CUtility.ECardType _type) 
    {
        Color color = Color.white;

        switch (_type) 
        {
            case CUtility.ECardType.ATK:
            case CUtility.ECardType.ATK_BURN:
            case CUtility.ECardType.ATK_ELEC:
            case CUtility.ECardType.ATK_ROCK:
                color = CGameManager.Instance.color_ATK;
                break;
            case CUtility.ECardType.DEF:
            case CUtility.ECardType.DEF_BURN:
            case CUtility.ECardType.DEF_ELEC:
            case CUtility.ECardType.DEF_ROCK:
                color = CGameManager.Instance.color_DEF;
                break;
        }

        foreach (var it in m_ColorParts) 
        { it.color = color; }
    }

    //수치 데이터 수정
    public void SetTextData(bool _isAnim = true)
    {
        ClearDataNum();
        var data = m_SkillCard.m_Data;
        var disk = m_SkillCard.m_CalcedDisk;

        if ((data.m_CurrProperty.m_Granted & CUtility.ECardProperty.DAMAGE) != 0)
            SetDataNum(CUtility.ENumableIcon.DAMAGE,
                (data.m_Damage + disk.m_Damage.m_Num).ToString(),
                !disk.m_Damage.CheckIsEmpty() && _isAnim == true, disk.m_Damage.m_IsDA);
        if ((data.m_CurrProperty.m_Granted & CUtility.ECardProperty.DEFEND) != 0)
            SetDataNum(CUtility.ENumableIcon.DEFEND, 
                (data.m_Defend + disk.m_Defend.m_Num).ToString(), 
                !disk.m_Defend.CheckIsEmpty() && _isAnim == true, disk.m_Defend.m_IsDA);

        CUtility.ENumableIcon icon = CUtility.ENumableIcon.NONE;
        switch (data.m_CardType)
        {
            case CUtility.ECardType.ATK_ELEC: icon = CUtility.ENumableIcon.ELEC; break;
            case CUtility.ECardType.ATK_BURN: icon = CUtility.ENumableIcon.BURN; break;
            case CUtility.ECardType.ATK_ROCK: icon = CUtility.ENumableIcon.ROCK; break;
            case CUtility.ECardType.DEF_ELEC: icon = CUtility.ENumableIcon.ELEC; break;
            case CUtility.ECardType.DEF_BURN: icon = CUtility.ENumableIcon.BURN; break;
            case CUtility.ECardType.DEF_ROCK: icon = CUtility.ENumableIcon.ROCK; break;
        }

        if (icon != CUtility.ENumableIcon.NONE)
            SetDataNum(icon, (data.m_StatusEff + disk.m_StatusEff.m_Num).ToString(), 
                !disk.m_StatusEff.CheckIsEmpty() && _isAnim == true, disk.m_StatusEff.m_IsDA);

        //한방 짜리는 반복 횟수로 표기함
        var targetIconType = CUtility.ENumableIcon.TARGET;
        if (data.m_GenType == CUtility.EATK_GenType.Once)
            targetIconType = CUtility.ENumableIcon.COUNT;

        m_IconText_Target_Count.InsertData(targetIconType,
            (data.m_Targets + disk.m_Targets.m_Num).ToString(),
            disk.m_Targets.m_Num > 0 && _isAnim == true, disk.m_Targets.m_IsDA);

        if ((data.m_CurrProperty.m_Granted & CUtility.ECardProperty.MANA) != 0)
            SetDataNum(CUtility.ENumableIcon.MANA, disk.m_Mana.m_Num.ToString(), _anim: !disk.m_Mana.CheckIsEmpty(), disk.m_Mana.m_IsDA);
        if ((data.m_CurrProperty.m_Granted & CUtility.ECardProperty.RETURN) != 0)
            if (disk.m_Return == true)
                SetDataNum(CUtility.ENumableIcon.RETURN, "v", _anim: true, _isDA: false, true);
            else
                SetDataNum(CUtility.ENumableIcon.RETURN, "x", _anim: false, _isDA: false, true);

        if (disk.m_Debuff <= 0) return;

        switch (data.m_CardType)
        {
            case CUtility.ECardType.DEF_ELEC:
            case CUtility.ECardType.ATK_ELEC:
                SetDataNum(CUtility.ENumableIcon.ELEC, disk.m_Debuff.ToString(), _isAnim == true, false, false);
                break;
            case CUtility.ECardType.DEF_BURN:
            case CUtility.ECardType.ATK_BURN:
                SetDataNum(CUtility.ENumableIcon.BURN, disk.m_Debuff.ToString(), _isAnim == true, false, false);
                break;
            case CUtility.ECardType.DEF_ROCK:
            case CUtility.ECardType.ATK_ROCK:
                SetDataNum(CUtility.ENumableIcon.ROCK, disk.m_Debuff.ToString(), _isAnim == true, false, false);
                break;
            default:
                SetDataNum(CUtility.ENumableIcon.DAMAGE, disk.m_Debuff.ToString(), _isAnim == true, false, false);
                break;
        }

        //if (disk.m_Burn_Self > 0)
        //    SetDataNum(CUtility.ENumableIcon.BURN, disk.m_Burn_Self.ToString(), _isAnim == true, false, false);
        //if (disk.m_Elec_Self > 0)
        //    SetDataNum(CUtility.ENumableIcon.ELEC, disk.m_Elec_Self.ToString(), _isAnim == true, false, false);
        //if (disk.m_Damage_Self > 0)
        //    SetDataNum(CUtility.ENumableIcon.DAMAGE, disk.m_Damage_Self.ToString(), _isAnim == true, false, false);
    }

    //먼저 들어온 순서에 따라 아이콘에 따른 데이터 삽입
    //public void SetDataNum(CUtility.ENumableIcon _icon, int _num, bool _isUpper = true)
    public void SetDataNum(CUtility.ENumableIcon _icon, string _text, bool _anim, bool _isDA, bool _isUpper = true)
    {
        //for문 돌며 해당하는 데이터에 정보 뿌림
        foreach (var it in m_UI_PropertyText_Main)
        { if (it.InsertData(_icon, _text, _anim, _isDA,_isUpper) == true) break; }
    }

    public void ClearDataNum()
    {
        foreach (var it in m_UI_PropertyText_Main) it.ResetData();
    }

    //충전 가능한지 체크
    public void CheckCanCharge_Over() 
    {
        var savedDice = m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices;
        m_ReserveDices.Clear();

        //저장 된 주사위 없다면 반환
        if (savedDice.Count <= 0) return;
        //디스크 모두 차있으면 반환
        if (m_SkillCard.m_Disks.Count <= m_CurrDiskIdx) return;
        
        m_ReserveDices.Add(savedDice[0]);
    }

    //사용 취소 하고 주사위 돌려줌
    public void CancelUse()
    {
        if (m_CardState == EState.DECK) return;
        //if (m_IsOnDeck == true) return;

        Debug.Log("CANCEL");

        m_SkillCard.m_CalcedDisk.Clear();
        m_CardState = EState.IDLE;
        //m_IsCharged = false;
        m_CurrDiskIdx = 0;
        DiceSum = 0;

        CGameManager.Instance.m_TurnManager.m_PlayerChar.UnCharge();

        foreach (var it in m_Img_Dices_Start)
        { DischargeDice(it); }

        foreach (var it in m_Img_Dices_Add)
        { DischargeDice(it); }

        m_ChargedDice.Reverse();
        Dice_BackToSaveArea();
        //foreach (var it in m_ChargedDice)
        //{
        //    it.gameObject.SetActive(true);
        //    //m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices.Add(it);
        //    m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices.Insert(0, it);
        //    it.m_DiceState = CDice.EDiceState.DICE_SAVED;
        //}

        //주사위 돌려주기
        m_SkillMgr.m_DiceMgr.m_DiceSaveArea.MoveDiceToPos();
        m_ChargedDice.Clear();

        //데이터 초기화
        SetUIData(m_SkillCard);
        SetTextData(_isAnim: false);

        //디스크 애니메이션
        m_Reload_Disk.MoveReset();

        //카드 애니메이션
        if(m_CardAnim != null) m_CardAnim.Anim_Idle();
    }

    public void UseSkill()
    {
        m_SkillCard.m_Atk_Info.SumData(m_SkillCard);

        //주사위 사용시 기믹 체크
        foreach (var it in m_ChargedDice) 
        { m_SkillMgr.m_Player.UseDice(it.m_eye); it.UseDice(); }
        
        //디스크의 반환 효과
        if (m_SkillCard.m_Atk_Info.m_SumAll.m_Return == true)
        { Dice_BackToSaveArea(m_CurrDiskIdx); }

        //디버프 체크
        if (m_SkillCard.m_Atk_Info.m_SumAll.m_Debuff > 0) 
        {
            var sum = m_SkillCard.m_Atk_Info.m_SumAll;
            var hitable = m_SkillMgr.m_Player.m_Hitable;

            //enum flag 라서 속성 먼저 체크함
            switch (m_SkillCard.m_Data.m_CardType) 
            {
                case CUtility.ECardType.ATK_ELEC:
                case CUtility.ECardType.ATK_BURN:
                case CUtility.ECardType.ATK_ROCK:
                    hitable.GainDebuff(m_SkillCard.m_Data.m_CardType, sum.m_Debuff);
                    break;
                default:
                    hitable.OnHit(sum.m_Debuff, 0,CUtility.ETextIcon.NONE);
                    break;
            }
        }

        //공방일체 방어력
        if (m_SkillCard.m_Atk_Info.m_SumAll.m_Damage.m_Num > 0 &&
            m_SkillCard.m_Atk_Info.m_SumAll.m_Defend.m_Num > 0)
        { m_SkillMgr.m_Player.m_Hitable.GainDef(
            m_SkillCard.m_Atk_Info.m_SumAll.m_Defend.m_Num); }

        //마나 충전
        if (m_SkillCard.m_Atk_Info.m_SumAll.m_Mana.m_Num > 0) 
        {
            CGameManager.Instance.m_TurnManager.m_PlayerChar.m_ManaHandler.UseMana
                (-m_SkillCard.m_Atk_Info.m_SumAll.m_Mana.m_Num);
        }


        //플레이어에게 공격 지시
        m_SkillMgr.m_Player.ATK_SkillCard(m_SkillCard);
        
        StartCoroutine(CoWait_Reset());
    }

    //사용 후 리셋 대기
    IEnumerator CoWait_Reset() 
    {
        m_ChargedDice.Clear();
        CancelUse();
        yield return CUtility.m_WFS_DOT4;

        //m_IsCharged = false;
        m_CurrDiskIdx = 0;
        //m_IsUseing = false;
        m_CardState = EState.IDLE;

        //m_Btn_ChangeCard.interactable = true;
        OnClick_ChangeCard(_noCost: true);
    }

    //주사위 돌려줌
    public void Dice_BackToSaveArea(int _countBack = 0) 
    {
        int idx = 0;
        if (_countBack != 0) idx = m_ChargedDice.Count - _countBack;

        for (int i = idx; i < m_ChargedDice.Count; i++) 
        {
            var it = m_ChargedDice[i];
            //m_SkillMgr.m_DiceMgr.m_DiceSaveArea.GetBackDice(it);
            CGameManager.Instance.m_DiceManager.m_DiceSaveArea.GetBackDice(it);
        }
    }

    public void EnemySelect()
    {
        m_CardState = EState.USING;
        //m_IsUseing = true;

        m_SkillCard.m_Atk_Info.SumData(m_SkillCard);
        int targetCount = m_SkillCard.m_Data.m_GenType == CUtility.EATK_GenType.Once ?
            1 : m_SkillCard.m_Atk_Info.m_SumAll.m_Targets.m_Num;

        //TODO : remake enemySelect
        var enemyGroup = CGameManager.Instance.m_TurnManager.m_EnemyGroup;
        enemyGroup.OnSelectMode(targetCount, this);
    }

    [SerializeField] int diceSum = 0;
    int DiceSum 
    {
        get { return diceSum; }
        set { 
            diceSum = value;
            if (diceSum == 0 || m_CurrDiskIdx == 0)
                m_TMP_Avr.text = "0";
            else
                m_TMP_Avr.text = (diceSum / m_CurrDiskIdx).ToString();
        } 
    }

    //주사위 충전 상호작용
    public void OnInteraction() 
    {
        Debug.Log("interaction!");

        if (m_IsCanUse == false) return;
        if (m_ReserveDices.Count <= 0) return;
        if (m_CurrDiskIdx != 0 && m_CurrDiskIdx >= m_SkillCard.m_Disks.Count) return;

        //m_IsCharged = true;
        m_CardState = EState.FOCUS_CHARGED;
        CGameManager.Instance.m_TurnManager.m_PlayerChar.OnCharge(m_CurrDiskIdx);

        m_Reload_Disk.MoveUp();
        var saveArea = m_SkillMgr.m_DiceMgr.m_DiceSaveArea;

        if (m_ChargedDice.Count == 0) 
        {
            for (int i = 0; i < m_ReserveDices.Count; i++) 
            { ChargeDice(m_Img_Dices_Start[i], m_ReserveDices[i].m_eye); }

            m_ChargedDice.AddRange(m_ReserveDices.ToArray());
            saveArea.DiceUseReady(m_ReserveDices);
            m_SkillCard.ReadyToUse();
        }
        else
        {
            if (saveArea.m_Dices.Count <= 0) return;
            var dice = saveArea.m_Dices[0];

            ChargeDice(m_Img_Dices_Add[m_CurrDiskIdx], dice.m_eye);

            m_CurrDiskIdx++;
            DiceSum += dice.m_eye;
            m_SkillCard.SumDiskData(DiceSum / m_CurrDiskIdx, m_CurrDiskIdx);

            m_ChargedDice.Add(dice);
            saveArea.DiceUse(0);
        }

        CheckCanCharge();
        ShowUseDice();
        SetTextData();
    }

    public void ChargeDice(CUI_CardDiceAnim _icon, int _eye) 
    {
        _icon.SetIcon(m_Icon_Dice[_eye]);
        _icon.SetAlpha(1);
    }

    public void DischargeDice(CUI_CardDiceAnim _icon) 
    {
        _icon.SetIcon(m_Icon_Dice[0], true);
        _icon.m_Image.color = CGameManager.Instance.color_Disable;
    }

    /// /////////////////////////////////////// /////////////////////////////////////// ////////////////////////////////////
    /// /////////////////////////////////////// /////////////////////////////////////// ////////////////////////////////////
    /// /////////////////////////////////////// /////////////////////////////////////// ////////////////////////////////////

    // 버튼 클릭 이벤트 핸들러
    public void OnButtonClick()
    {
        if (m_CardState == EState.DECK || m_CardState == EState.USING) return;
        //if (m_IsOnDeck == true || m_IsUseing == true) return;

        //if (m_IsCanUse == true)
        //if(m_IsCharged == true)

        //충전 되었을때만 사용 가능
        //if(m_IsCharged == true)
        if(m_CardState == EState.FOCUS_CHARGED)
        {
            //적 선택 페이즈로 이동해야함
            Debug.Log("적 선택하자");
            if (m_SkillCard.m_Data.m_CardType >= CUtility.ECardType.DEF)
            { UseSkill(); }
            else if(m_SkillMgr.m_Player.m_CanAnim == true) EnemySelect();
        }
        else
        {
            Debug.Log("사용 불가");
            //OnButtonSelect();
        }
    }


    // 버튼 호버 진입 이벤트 핸들러
    public void OnButtonHoverEnter()
    {
        //if (m_IsOnDeck == true) return;
        //m_SkillMgr.m_TMP_Desc.text = m_SkillCard.m_Data.m_Desc;
        ////Debug.Log("Button Hovered!");
        //
        //CheckCanCharge();
        //ShowUseDice();
    }

    // 버튼 호버 종료 이벤트 핸들러
    public void OnButtonHoverExit()
    {
        //if (m_IsOnDeck == true) return;
        //m_SkillMgr.m_TMP_Desc.text = "";
        ////Debug.Log("Button Hover Exit!");
        //
        //HideUseDice();
    }

    // 버튼 선택 이벤트 핸들러
    public void OnButtonSelect()
    {
        if (m_CardState == EState.DECK) return;
        //if (m_IsOnDeck == true) return;
        //m_SkillMgr.m_TMP_Desc.text = m_SkillCard.m_Data.m_Desc;
        Debug.Log(this.transform.parent.name + "card selected");
        
        CGameManager.Instance.m_Input.CB_OnInteraction = OnInteraction;
        CGameManager.Instance.m_Input.SetEscape(OnButtonDeselect);

        m_CardState = EState.FOCUS_NONE;
        if (m_CardAnim != null)  m_CardAnim.Anim_Ready();
        CheckCanCharge();
        ShowUseDice();
    }

    // 버튼 선택 해제 이벤트 핸들러
    public void OnButtonDeselect()
    {
        if (m_CardState == EState.DECK) return;
        //if (m_IsOnDeck == true) return;
        m_SkillMgr.m_TMP_Desc.text = "";

        if(CGameManager.Instance.m_Input.CB_OnInteraction == OnInteraction)
            CGameManager.Instance.m_Input.CB_OnInteraction = null;

        //Debug.Log("Button Deselected!" + name);

        HideUseDice();
        if(m_CardState != EState.USING) CancelUse();
        //if (m_IsUseing == false) CancelUse();
        Debug.Log(this.transform.parent.name + "card deselected");

    }

    //주사위 조건 
    //사용 가능 체크

    public void ShowUseDice()
    {
        if (m_ReserveDices.Count == 0 || m_IsCanUse == false) return;
        foreach (var it in m_ReserveDices)
        { it.m_DiceUI_Img.color = Color.green; }
    }

    public void HideUseDice()
    {
        if (m_ReserveDices.Count == 0) return;
        foreach (var it in m_ReserveDices)
        { it.m_DiceUI_Img.color = Color.white; }
    }

    //카드 교체
    public void OnClick_ChangeCard(bool _noCost = false)
    {
        var player = CGameManager.Instance.m_TurnManager.m_PlayerChar;

        if (_noCost == false)
        {
            Debug.Log(player.m_ManaHandler.m_CurrMana);
            if (player.m_ManaHandler.m_CurrMana < 1) { return; }
            player.m_ManaHandler.UseMana(1);
        }

        var card = player.DrawCard();


        if (m_SkillCard != null)
        {
            m_SkillCard.m_DeckState = CScriptable_CardSkill.ECardState.USED;
            player.m_UsedDeck.Add(m_SkillCard);
            player.m_Hand.Remove(m_SkillCard);
        }

        if (m_CardAnim != null) m_CardAnim.Anim_Change();
        card.m_DeckState = CScriptable_CardSkill.ECardState.HAND;
        player.m_Hand.Add(card);
        SetUIData(card);
    }

    public void OnInputEscape() 
    {
        if (m_CardState == EState.DECK) return;
        //if (m_IsOnDeck == true) return;
        //m_IsUseing = false;
        CancelUse();
        CheckCanCharge();
    }
}
