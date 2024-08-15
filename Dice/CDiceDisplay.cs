using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDiceDisplay : MonoBehaviour
{
    public List<Transform> m_DiceDisplayLocs = new List<Transform>();
    public List<CDice> m_Dices = new List<CDice>();
    public CDice.EDiceState m_PlaceState = CDice.EDiceState.NONE;

    //public CSelectableArea m_SelectableArea = null;
    //public CSelectableArea_New m_SelectableArea = null;

    //주사위 추가. 배열
    public void GetDiceData(List<CDice> _dices)
    {
        Debug.Log("GET DICE MULTIE");

        if (this.gameObject.activeSelf == false) this.gameObject.SetActive(true);

        //if (m_SelectableArea == null) m_SelectableArea = this.GetComponent<CSelectableArea_New>();

        m_Dices.AddRange(_dices);
        foreach (var dice in _dices)
        {
            if (dice.gameObject.activeSelf == false) dice.gameObject.SetActive(true);
            dice.transform.parent = this.transform;
            dice.m_DiceState = m_PlaceState;
            //m_SelectableArea.m_ChildSelectables.Add(dice.m_Selectable);
        }
        MoveDiceToPos();

        //m_SelectableArea.m_CanvasGroup.interactable = true;
    }

    //주사위 추가. 단일
    public void GetDiceData(CDice _dice)
    {
        Debug.Log("GET DICE ONE");

        if (m_Dices.Contains(_dice) == true) return;
        if (this.gameObject.activeSelf == false) this.gameObject.SetActive(true);

        m_Dices.Add(_dice);

        if (_dice.m_eye != -1)
        { DiceMapAdd(_dice); }

        if (_dice.gameObject.activeSelf == false) _dice.gameObject.SetActive(true);
        _dice.transform.parent = this.transform;
        _dice.m_DiceState = m_PlaceState;
        //m_SelectableArea.m_ChildSelectables.Add(_dice.m_Selectable);

        var loc = m_DiceDisplayLocs[m_Dices.Count - 1].position;
        StartCoroutine(_dice.CoMoveToDisplay(loc, _toggleUIMode: true));

        //m_SelectableArea.m_CanvasGroup.interactable = true;
    }

    public void DiceRollDone() 
    {
        foreach (var it in m_Dices)
        { DiceMapAdd(it); }
    }

    public void GetBackDice(CDice _dice)
    {
        m_Dices.Insert(0, _dice);
        DiceMapAdd(_dice);

        _dice.gameObject.SetActive(true);
        _dice.transform.parent = this.transform;
        _dice.m_DiceState = m_PlaceState;
    }

    //주사위 사용하거나 옮겨서 생긴 빈칸 땡기는 함수
    public void MoveDiceToPos()
    {
        for (int i = 0; i < m_Dices.Count; i++)
        {
            var dice = m_Dices[i];
            var loc = m_DiceDisplayLocs[i].position;
            StartCoroutine(dice.CoMoveToDisplay(loc, _toggleUIMode: true));
        }
    }

    //강제로 주사위 빈칸 땡김
    public void MoveDiceToPos_Force()
    {
        if (m_Dices.Count == 0) this.gameObject.SetActive(false);

        for (int i = 0; i < m_Dices.Count; i++)
        {
            var dice = m_Dices[i];
            var loc = m_DiceDisplayLocs[i].position;
            dice.transform.position = loc;
        }
    }

    //강제로 주사위 빈칸 땡김
    public void MoveDiceToPos_All()
    {
        if (m_Dices.Count == 0) this.gameObject.SetActive(false);

        for (int i = 0; i < m_Dices.Count; i++)
        {
            var dice = m_Dices[i];
            var loc = m_DiceDisplayLocs[i].position;
            StartCoroutine(dice.CoMoveToDisplay(loc, _toggleUIMode: true));
        }
    }

    //데이터 제거
    public void ReleaseData(CDice _dice)
    {
        m_Dices.Remove(_dice);
        DiceMapRemove(_dice);

        MoveDiceToPos_Force();
        
        //m_SelectableArea.m_ChildSelectables.Remove(_dice.m_Selectable);
        //if (m_SelectableArea.m_LastInput == _dice.m_Selectable) m_SelectableArea.m_LastInput = null;
        //
        //m_SelectableArea.m_CanvasGroup.interactable = m_Dices.Count > 0;
    }

    //데이터 제거 배열
    public void ReleaseData(List<CDice> _dices)
    {
        foreach (var dice in _dices)
        {
            m_Dices.Remove(dice);
            DiceMapRemove(dice);

            //m_SelectableArea.m_ChildSelectables.Remove(dice.m_Selectable);
        }
        //m_SelectableArea.m_LastInput = null;

        MoveDiceToPos_Force();


        //m_SelectableArea.m_CanvasGroup.interactable = m_Dices.Count > 0;
    }

    //데이터 초기화 후 비활성화
    public void ResetDiceData()
    {
        //foreach (var dice in m_Dices)
        //{ m_SelectableArea.m_ChildSelectables.Remove(dice.m_Selectable); }

        m_Dices.Clear();
        foreach (var it in m_DiceMap)
            it.Value.Clear();
        m_DiceMap.Clear();

        this.gameObject.SetActive(false);


        //m_SelectableArea.m_CanvasGroup.interactable = false;
        
    }


    //주사위 사용시 호출, 비활성화
    public void DiceUse(int _idx)
    {
        if (m_Dices.Count <= _idx) return;

        var dice = m_Dices[_idx];
        m_Dices.RemoveAt(_idx);
        DiceMapRemove(dice);

        dice.gameObject.SetActive(false);

        MoveDiceToPos();
    }

    public void DiceUseReady(List<CDice> _dices)
    {
        foreach (var it in _dices)
        {
            m_Dices.Remove(it);
            DiceMapRemove(it);
            it.gameObject.SetActive(false);
        }

        MoveDiceToPos();
    }


    //public Dictionary<int, List<CDice>> m_DiceMap = new Dictionary<int, List<CDice>>();
    [SerializeField] bool bUseDiceMap = true;
    public SerializeDictionary<int, List<CDice>> m_DiceMap = new SerializeDictionary<int, List<CDice>>();

    void DiceMapAdd(CDice _dice) 
    {
        if (bUseDiceMap == false) return;
        if (_dice.m_eye == -1) return;
        if (m_DiceMap.ContainsKey(_dice.m_eye) == false)
        { m_DiceMap[_dice.m_eye] = new List<CDice>(); }
        m_DiceMap[_dice.m_eye].Add(_dice);
    }

    void DiceMapRemove(CDice _dice) 
    {
        if (bUseDiceMap == false) return;
        if (_dice.m_eye == -1) return; 
        m_DiceMap[_dice.m_eye].Remove(_dice);
        if (m_DiceMap[_dice.m_eye].Count == 0)
            m_DiceMap.Remove(_dice.m_eye);
    }

    public void test()
    {
        //주사위를 전부 dictionary에 넣어 조건 체크 쉽게함
        var originDices = m_Dices;
        
        foreach (var it in originDices)
        {
            if (m_DiceMap.ContainsKey(it.m_eye) == false)
            { m_DiceMap[it.m_eye] = new List<CDice>(); }
            m_DiceMap[it.m_eye].Add(it);
        }
    }

}
