//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.EventSystems;

//using System.Linq;
//using TMPro;


//public class CUI_SkillCard : CUI_Showable, ISelectEvent
//{
//    public CScriptable_CardSkill m_SkillCard = null;

//    public List<Sprite> m_Icon_Dice = new List<Sprite>();
//    public List<Image> m_Img_Dices_Start = new List<Image>();
//    public List<Image> m_Img_Dices_Add = new List<Image>();

//    [Header("===============================================")]
//    //-------------------------------------------//
//    public TextMeshProUGUI m_TMP_Term = null;

//    [System.Serializable]
//    public class CUI_IconText
//    {
//        public CUtility.ENumableIcon m_Icon = CUtility.ENumableIcon.NONE;
//        public Image m_Img_Icon = null;
//        public TextMeshProUGUI m_TMP_NumUp = null;
//        public TextMeshProUGUI m_TMP_NumDw = null;
//    }

//    public CUI_IconText m_IconText01 = new CUI_IconText();
//    public CUI_IconText m_IconText02 = new CUI_IconText();
//    public CUI_IconText m_IconText03 = new CUI_IconText();
//    public CUI_IconText m_IconText04 = new CUI_IconText();
//    public CUI_IconText m_IconText_Target_Count = new CUI_IconText();


//    public List<CDice> m_ReserveDices = new List<CDice>();
//    public List<CDice> m_ChargedDice = new List<CDice>();
//    //-------------------------------------------//
//    [Header("===============================================")]
//    public GameObject m_ImgCanUse = null;
//    public CUI_Skill_Manager m_SkillMgr = null;

//    [Header("===============================================")]
//    //-------------------------------------------//
//    public Button m_Btn_ChangeCard = null;
//    //-------------------------------------------//

//    [Header("===============================================")]
//    public int m_CurrDiskIdx = 0;
//    public bool m_IsOnDeck = false;
//    public bool m_IsCharged = false;
//    public bool m_IsUseing = false;


//    [SerializeField] bool IsCanUse = false;
//    public bool m_IsCanUse
//    {
//        get { return IsCanUse; }
//        set
//        {
//            IsCanUse = value;
//            m_ImgCanUse.SetActive(IsCanUse);
//        }
//    }

//    public void SetUIData(CScriptable_CardSkill _skill)
//    {
//        m_SkillCard = _skill;
//        var skillData = m_SkillCard.m_Data;

//        m_TMP_Term.text = skillData.m_Term.ToString();

//        var disableColor = CGameManager.Instance.color_Disable;
//        foreach (var it in m_Img_Dices_Start) it.color = disableColor;


//        //필요 주사위 표기
//        for (int i = 0; i < m_Img_Dices_Start.Count; i++)
//        {
//            if (i < skillData.m_NeedDice)
//            {
//                m_Img_Dices_Start[i].gameObject.SetActive(true);
//                m_Img_Dices_Start[i].color = disableColor;
//            }
//            else m_Img_Dices_Start[i].gameObject.SetActive(false);
//        }

//        if (skillData.m_RequireDice[0] == CUtility.EDice.ANY)
//            foreach (var it in m_Img_Dices_Start) it.sprite = m_Icon_Dice[0];

//        //조건에 따른 필요 주사위 표기
//        else if (skillData.m_RequireDice.Count > 0)
//        {
//            //Same, Over, Less
//            if (skillData.m_RequireDice.Count == 1)
//            {
//                //Debug.Log(_skill.m_Require_Dice[0]);
//                var sp = m_Icon_Dice[(int)skillData.m_RequireDice[0]];
//                for (int i = 0; i < skillData.m_NeedDice; i++)
//                { m_Img_Dices_Start[i].sprite = sp; }
//            }
//            //Term == Pick
//            else// if (_skill.m_Require_Dice[0] != CUtility.EDice.ANY)
//            {
//                for (int i = 0; i < skillData.m_NeedDice; i++)
//                { m_Img_Dices_Start[i].sprite = m_Icon_Dice[(int)skillData.m_RequireDice[i]]; }
//            }
//        }



//        var upgradeCount = m_SkillCard.m_Disks.Count;
//        Color color = Color.white;
//        for (int i = 0; i < m_Img_Dices_Add.Count; i++)
//        {
//            if (i < skillData.m_Sockets.Count)
//            {
//                m_Img_Dices_Add[i].gameObject.SetActive(true);
//                switch (skillData.m_Sockets[i])
//                {
//                    case 0: color = CGameManager.Instance.color_Tear1; break;
//                    case 1: color = CGameManager.Instance.color_Tear2; break;
//                    case 2: color = CGameManager.Instance.color_Tear3; break;
//                    default: color = CGameManager.Instance.color_Disable; break;
//                }

//                if (i < upgradeCount)
//                { color.a = CGameManager.Instance.color_Wait.a; }
//                else color.a = CGameManager.Instance.color_Disable.a;
//                m_Img_Dices_Add[i].color = color;
//            }

//            else m_Img_Dices_Add[i].gameObject.SetActive(false);
//        }

//        SetTextData();
//    }

//    public void CheckCanCharge()
//    {
//        if (m_IsOnDeck == true) return;

//        //Debug.Log("CHECK CHARGE");

//        //충전이 되어 있다면 추가 충전 확인
//        if (m_IsCharged == true)
//        {
//            CheckCanCharge_Over();
//            return;
//        }


//        m_ReserveDices.Clear();
//        int needDice = m_SkillCard.m_Data.m_NeedDice;
//        var reqire = m_SkillCard.m_Data.m_RequireDice;

//        //주사위를 전부 dictionary에 넣어 조건 체크 쉽게함
//        var originDices = m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices;
//        Dictionary<int, List<CDice>> dices = new Dictionary<int, List<CDice>>();
//        foreach (var it in originDices)
//        {
//            if (dices.ContainsKey(it.m_eye) == false)
//            { dices[it.m_eye] = new List<CDice>(); }
//            dices[it.m_eye].Add(it);
//        }

//        //주사위 부족
//        if (m_SkillCard.m_Data.m_NeedDice > originDices.Count)
//        {
//            Debug.Log("주사위 부족");
//            SetTextData();
//            m_IsCanUse = false;
//            return;
//        }

//        //조건 체크
//        switch (m_SkillCard.m_Data.m_Term)
//        {
//            case CUtility.EDice_Term.NONE:
//                m_IsCanUse = true;
//                for (int i = 0; i < needDice; i++) m_ReserveDices.Add(originDices[i]);
//                break;
//            case CUtility.EDice_Term.MORE:
//                CheckMoreLess(_more: true, dices, needDice);
//                break;
//            case CUtility.EDice_Term.LESS:
//                CheckMoreLess(_more: false, dices, needDice);
//                break;
//            case CUtility.EDice_Term.SAME: 
//                CheckSame(dices, needDice);
//                break;
//            case CUtility.EDice_Term.SAME_OVER: break;
//            case CUtility.EDice_Term.SAME_LESS: break;
//            case CUtility.EDice_Term.PICK:
//                CheckPick(dices, needDice);
//                break;
//            case CUtility.EDice_Term.STRAIGHT: break;
//            case CUtility.EDice_Term.ODD_NUM: break;
//            case CUtility.EDice_Term.EVEN_NUM: break;
//            default: break;
//        }

//        foreach (var it in dices.Keys)
//            dices[it].Clear();
//        dices.Clear();
//    }

//    //조건체크_이상, 이하
//    void CheckMoreLess(bool _more, Dictionary<int, List<CDice>> _dices, int _needDice)
//    {
//        int req = (int)m_SkillCard.m_Data.m_RequireDice[0];

//        List<CDice> res = new List<CDice>();

//        foreach (var key in _dices.Keys)
//        {
//            //over less 동시에 처리하기
//            bool check =
//                (_more == true && key >= req) ||
//                (_more == false && key <= req);

//            if (check == true && _dices.ContainsKey(key))
//            {
//                res.AddRange(_dices[key]);
//            }
//        }

//        //가까운 순으로 정렬
//        m_ReserveDices = res.OrderBy(eye => eye.transform.GetSiblingIndex()).Take(_needDice).ToList();
//        m_IsCanUse = res.Count >= _needDice;
//    }

//    //조건체크_사용자지정
//    void CheckPick(Dictionary<int, List<CDice>> _dices, int _needDice)
//    {
//        foreach (var it in m_SkillCard.m_Data.m_RequireDice)
//        {
//            if (_dices.ContainsKey((int)it) == false)
//            {
//                m_ReserveDices.Clear();
//                m_IsCanUse = false;
//                return;
//            }

//            var target = _dices[(int)it];
//            m_ReserveDices.Add(target[0]);

//            target.RemoveAt(0);
//            if (target.Count == 0) _dices.Remove((int)it);
//        }
//        m_IsCanUse = true;
//    }

//    void CheckSame(Dictionary<int, List<CDice>> _dices, int _needDice) 
//    {
//        int idx = (int)m_SkillCard.m_Data.m_RequireDice[0];
//        if (_dices.ContainsKey(idx) == true && _dices[idx].Count >= _needDice) 
//        {
//            for (int i = 0; i < _needDice; i++)
//                m_ReserveDices.Add(_dices[idx][i]);
//            m_IsCanUse = true;
//            return;
//        }
//        m_IsCanUse = false;
//    }

//    //조건체크_스트레이트
//    void CheckStr(List<CDice> _dices, int _needDice)
//    {
//        Dictionary<int, CDice> strChecker = new Dictionary<int, CDice>();


//        //List<CDice> str = new List<CDice>() 
//        //var min = _dices[0];
//    }


//    public void SetTextData()
//    {
//        ClearDataNum();
//        var data = m_SkillCard.m_Data;
//        var disk = m_SkillCard.m_CalcedDisk;

//        if (data.m_Damage > 0)
//            SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Damage + disk.m_Damage.m_Num);
//        if (data.m_Defend > 0)
//            SetDataNum(CUtility.ENumableIcon.DAMAGE, data.m_Defend + disk.m_Defend.m_Num);

//        if (data.m_StatusEff > 0)
//        {
//            var icon = CUtility.ENumableIcon.ELEC;
//            switch (data.m_CardType)
//            {
//                case CUtility.ECardType.ATK_ELEC: icon = CUtility.ENumableIcon.ELEC; break;
//                case CUtility.ECardType.ATK_BURN: icon = CUtility.ENumableIcon.BURN; break;
//                case CUtility.ECardType.ATK_CURS: icon = CUtility.ENumableIcon.CURSE; break;
//            }
//            SetDataNum(icon, data.m_StatusEff + disk.m_StatusEff.m_Num);
//        }

//        if (data.m_Targets > 0) 
//        {
//            m_IconText_Target_Count.m_TMP_NumUp.text = string.Format("×{0}", data.m_Targets + disk.m_Targets.m_Num);
//            m_IconText_Target_Count.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[(int)CUtility.ENumableIcon.TARGET];
//        }

//        if (data.m_Count > 0)
//        {
//            m_IconText_Target_Count.m_TMP_NumUp.text = string.Format("×{0}", data.m_Count + disk.m_Count.m_Num);
//            m_IconText_Target_Count.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[(int)CUtility.ENumableIcon.COUNT];
//        }

//        if (disk.m_Mana.m_Num > 0)
//            SetDataNum(CUtility.ENumableIcon.MANA, disk.m_Mana.m_Num);
//        if (disk.m_Return == true)
//            SetDataNum(CUtility.ENumableIcon.RETURN, 0);

//        if (disk.m_Burn_Self > 0)
//            SetDataNum(CUtility.ENumableIcon.BURN, disk.m_Burn_Self, false);
//        if (disk.m_Elec_Self > 0)
//            SetDataNum(CUtility.ENumableIcon.ELEC, disk.m_Elec_Self, false);
//        if (disk.m_Damage_Self > 0)
//            SetDataNum(CUtility.ENumableIcon.DAMAGE, disk.m_Damage_Self, false);
//    }

//    //먼저 들어온 순서에 따라 아이콘에 따른 데이터 삽입
//    public void SetDataNum(CUtility.ENumableIcon _icon, int _num, bool _isUpper = true)
//    {
//        if (IconText_InsertData(m_IconText01, _icon, _num, _isUpper) == true) return;
//        else if (IconText_InsertData(m_IconText02, _icon, _num, _isUpper) == true) return;
//        else if (IconText_InsertData(m_IconText03, _icon, _num, _isUpper) == true) return;
//        else if (IconText_InsertData(m_IconText04, _icon, _num, _isUpper) == true) return;
//    }

//    public void ClearDataNum()
//    {
//        m_IconText01.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[0];
//        m_IconText01.m_TMP_NumDw.text = "-";
//        m_IconText01.m_TMP_NumUp.text = "-";
//        m_IconText02.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[0];
//        m_IconText02.m_TMP_NumDw.text = "-";
//        m_IconText02.m_TMP_NumUp.text = "-";
//        m_IconText03.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[0];
//        m_IconText03.m_TMP_NumDw.text = "-";
//        m_IconText03.m_TMP_NumUp.text = "-";
//        m_IconText04.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[0];
//        m_IconText04.m_TMP_NumDw.text = "-";
//        m_IconText04.m_TMP_NumUp.text = "-";
//    }

//    //아이콘 텍스트에 데이터 삽입
//    public bool IconText_InsertData(CUI_IconText _ict, CUtility.ENumableIcon _icon, int _num, bool _isUpper) 
//    {
//        if (_ict.m_Icon == CUtility.ENumableIcon.NONE || _ict.m_Icon == _icon)
//        {
//            _ict.m_Icon = _icon;
//            _ict.m_Img_Icon.sprite = CGameManager.Instance.m_TextableIcons[(int)_icon];
//            if (_isUpper == true) { _ict.m_TMP_NumUp.text = _num.ToString(); }
//            else { _ict.m_TMP_NumDw.text = _num.ToString(); }
//            return true;
//        }
//        return false;
//    }

//    //충전 가능한지 체크
//    public void CheckCanCharge_Over() 
//    {
//        var savedDice = m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices;
//        //저장 된 주사위 없다면 반환
//        if (savedDice.Count <= 0) return;
//        //디스크 모두 차있으면 반환
//        if (m_SkillCard.m_Disks.Count <= m_CurrDiskIdx) return;

//        m_ReserveDices.Clear();
//        m_ReserveDices.Add(savedDice[0]);
//    }

//    //사용 취소 하고 주사위 돌려줌
//    public void CancelUse() 
//    {
//        Debug.Log("CANCEL");

//        m_SkillCard.m_CalcedDisk.Clear();
//        m_IsCharged = false;
//        m_CurrDiskIdx = 0;
//        DiceSum = 0;

//        foreach (var it in m_Img_Dices_Start)
//            it.color = CGameManager.Instance.color_Disable;

//        foreach (var it in m_Img_Dices_Add)
//        {
//            it.color = CGameManager.Instance.color_Disable;
//            it.sprite = m_Icon_Dice[0];
//        }

//        m_ChargedDice.Reverse();
//        foreach (var it in m_ChargedDice)
//        {
//            it.gameObject.SetActive(true);
//            //m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices.Add(it);
//            m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices.Insert(0, it);
//            it.m_DiceState = CDice.EDiceState.DICE_SAVED;
//        }

//        m_SkillMgr.m_DiceMgr.m_DiceSaveArea.MoveDiceToPos();
//        m_ChargedDice.Clear();

//        SetUIData(m_SkillCard);
//        SetTextData();
//    }

//    public void UseSkill()
//    {
//        //foreach (var it in m_ChargedDice)
//        //{
//        //    it.gameObject.SetActive(false);
//        //    m_SkillMgr.m_DiceMgr.m_DiceSaveArea.m_Dices.Remove(it);
//        //    m_SkillMgr.m_DiceMgr.m_DiceSaveArea.MoveDiceToPos();
//        //}

//        Debug.Log("SKILL USE HERE");

//        m_IsCharged = false;
//        m_CurrDiskIdx = 0;
//        m_IsUseing = false;

//        m_ChargedDice.Clear();
//        m_SkillMgr.m_Player.ATK_SkillCard(m_SkillCard);

//    }

//    public void EnemySelect()
//    {
//        m_IsUseing = true;

//        m_SkillMgr.m_EnemySelectArea.OnSelectMode(m_SkillCard.m_Data.m_Targets);
//        m_SkillMgr.m_EnemySelectArea.m_UI_SkillCard = this;
//        m_SkillMgr.m_EnemySelectArea.m_SelectCount = m_SkillCard.m_Data.m_Targets;
//        CGameManager.Instance.m_SelectableHandler.Push_FocusArea(m_SkillMgr.m_EnemySelectArea);
//    }

//    [SerializeField] int DiceSum = 0;
//    public void OnInteraction() 
//    {
//        Debug.Log("HELLO");

//        if (m_IsCanUse == false) return;
//        if (m_ReserveDices.Count <= 0) return;
//        if (m_CurrDiskIdx >= m_SkillCard.m_Disks.Count) return;

//        m_IsCharged = true;
//        var saveArea = m_SkillMgr.m_DiceMgr.m_DiceSaveArea;

//        if (m_ChargedDice.Count == 0) 
//        {
//            for (int i = 0; i < m_ReserveDices.Count; i++) 
//            { 
//                m_Img_Dices_Start[i].sprite = m_Icon_Dice[m_ReserveDices[i].m_eye];
//                var color = m_Img_Dices_Start[i].color; color.a = 1;
//                m_Img_Dices_Start[i].color = color;
//            }

//            m_ChargedDice.AddRange(m_ReserveDices.ToArray());
//            saveArea.DiceUse(m_ReserveDices);
//            m_SkillCard.ReadyToUse();
//        }
//        else
//        {
//            if (saveArea.m_Dices.Count <= 0) return;
//            var dice = saveArea.m_Dices[0];

//            m_Img_Dices_Add[m_CurrDiskIdx].sprite = m_Icon_Dice[dice.m_eye];
//            var color = m_Img_Dices_Add[m_CurrDiskIdx].color; color.a = 1;
//            m_Img_Dices_Add[m_CurrDiskIdx].color = color;

//            DiceSum += dice.m_eye;
//            m_CurrDiskIdx++;
//            m_SkillCard.SumDiskData(DiceSum / m_CurrDiskIdx, m_CurrDiskIdx);

//            m_ChargedDice.Add(dice);
//            saveArea.DiceUse(0);


//        }

//        CheckCanCharge();
//        ShowUseDice();
//        SetTextData();
//    }
//    /// /////////////////////////////////////// /////////////////////////////////////// ////////////////////////////////////
//    /// /////////////////////////////////////// /////////////////////////////////////// ////////////////////////////////////
//    /// /////////////////////////////////////// /////////////////////////////////////// ////////////////////////////////////

//    // 버튼 클릭 이벤트 핸들러
//    public void OnButtonClick()
//    {
//        if (m_IsOnDeck == true) return;

//        //if (m_IsCanUse == true)
//        if(m_IsCharged == true)
//        {

//            Debug.Log("적 선택하자");
//            EnemySelect();
//        }
//        else
//        { Debug.Log("사용 불가"); }
//    }


//    // 버튼 호버 진입 이벤트 핸들러
//    public void OnButtonHoverEnter()
//    {
//        if (m_IsOnDeck == true) return;
//        m_SkillMgr.m_TMP_Desc.text = m_SkillCard.m_Data.m_Desc;
//        //Debug.Log("Button Hovered!");

//        CheckCanCharge();
//        ShowUseDice();
//    }

//    // 버튼 호버 종료 이벤트 핸들러
//    public void OnButtonHoverExit()
//    {
//        if (m_IsOnDeck == true) return;
//        m_SkillMgr.m_TMP_Desc.text = "";
//        //Debug.Log("Button Hover Exit!");

//        HideUseDice();
//    }

//    // 버튼 선택 이벤트 핸들러
//    public void OnButtonSelect()
//    {
//        if (m_IsOnDeck == true) return;
//        m_SkillMgr.m_TMP_Desc.text = m_SkillCard.m_Data.m_Desc;
//        //Debug.Log("Button Selected!");

//        CheckCanCharge();
//        ShowUseDice();
//    }

//    // 버튼 선택 해제 이벤트 핸들러
//    public void OnButtonDeselect()
//    {
//        if (m_IsOnDeck == true) return;
//        m_SkillMgr.m_TMP_Desc.text = "";
//        //Debug.Log("Button Deselected!");

//        HideUseDice();
//        if(m_IsUseing == false) CancelUse();
//    }

//    //주사위 조건 
//    //사용 가능 체크

//    public void ShowUseDice()
//    {
//        if (m_ReserveDices.Count == 0 || m_IsCanUse == false) return;
//        foreach (var it in m_ReserveDices)
//        { it.m_DiceUI_Img.color = Color.green; }
//    }

//    public void HideUseDice()
//    {
//        if (m_ReserveDices.Count == 0) return;
//        foreach (var it in m_ReserveDices)
//        { it.m_DiceUI_Img.color = Color.white; }
//    }


//    public void OnClick_ChangeCard()
//    {
//        var player = CGameManager.Instance.m_TurnManager.m_PlayerChar;

//        var card = player.DrawCard();

//        if (m_SkillCard != null)
//        {
//            player.AddToUsedDeck(m_SkillCard);
//            player.m_Hand.Remove(m_SkillCard);
//        }

//        player.m_Hand.Add(card);
//        SetUIData(card);
//    }

//    public void OnInputEscape() 
//    {
//        CancelUse();
//        CheckCanCharge();
//    }
//}
