using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using System.Linq;

public class CUI_Reword_Disk : MonoBehaviour
{
    public GameObject m_ParentCanvas = null;

    public List<Ctnr_Card> m_CardContainers = new List<Ctnr_Card>();
    public CUI_SkillCard m_FocusCardUI = null;
    public List<CUI_Disk> m_Disks = new List<CUI_Disk>();

    [Header("================================")]
    public List<CScriptable_CardSkill> m_OriginCards = new List<CScriptable_CardSkill>();


    [Header("================================")]
    [SerializeField] Button m_Btn_Submit = null;
    [SerializeField] RectTransform m_FocusImg = null;

    [Header("================================")]
    [SerializeField] int m_UI_Card_Count = 4;
    [SerializeField] int maxDiskCount = 0;
    [SerializeField] int diskIdx = 0;
    [SerializeField] bool isCharged = false;

    public void OnEnable()
    {
        //Target_UpgradeCards();
        
        foreach (var it in m_CardContainers)
            it.m_CB_Submit = OnFocus_Card;

        m_Btn_Submit.interactable = false;

        if (CGameManager.Instance.m_Input != null) 
            CGameManager.Instance.m_Input.CB_OnInteraction += OnInteraction;
    }

    public void SetData(int _tear = 0)
    {
        Target_UpgradeCards(_tear);
    }

    //업그레이드 대상 카드 찾기
    public void Target_UpgradeCards(int _tear = 0) 
    {
        var player = CGameManager.Instance.m_PlayerData;
        //var discovery = player.m_Discovery;
        var discovery = CGameManager.Instance.m_ScheduleMgr.m_UI_Reword.m_Discovery;
        var dic = CGameManager.Instance.m_Dictionary;
        var cards = player.m_Deck;

         m_OriginCards.Clear();

        //디스크 박힐 수 있는 카드들
        List<CScriptable_CardSkill> targetCards = new List<CScriptable_CardSkill>();

        foreach (var it in cards)
        {
            //디스크 갯수가 최대치보다 작은 카드 찾음
            //if (it.m_Disks.Count < it.m_Data.m_Sockets.Count)
            if (it.m_Disks.Count < CUtility.MaxSocketCount)
                targetCards.Add(it);
        }

        //UI표기될 숫자만큼 돌림
        //for (int i = 0; i < m_UI_Card_Count; i++) 
        for (int i = 0; i < m_CardContainers.Count; i++) 
        {
            CScriptable_CardSkill card = null;
            if (targetCards.Count > 0)
            {
                int idx = Random.Range(0, targetCards.Count);

                card = Instantiate(targetCards[idx]);

                m_CardContainers[i].transform.parent.gameObject.SetActive(true);

                //티어에 따른 디스크 찾기
                if (_tear == 0)
                    Find_Target_Disk(card, i, dic.m_RandomHelper.DiskTearRandom(discovery));
                else
                    Find_Target_Disk(card, i, _tear);

                m_CardContainers[i].m_UI_Card.SetUIData(Instantiate(card));

                m_OriginCards.Add(targetCards[idx]);
                targetCards.RemoveAt(idx);
            }
            else
            {
                m_CardContainers[i].transform.parent.gameObject.SetActive(false);
                m_OriginCards.Add(null);
            }
        }
    }

    //티어에 맞는 디스크 찾음
    public void Find_Target_Disk(CScriptable_CardSkill _card, int _idx, int _tear) 
    {
        //var res = CGameManager.Instance.m_Dictionary.GetDisks_By_Tear(_card, 1);
        var res = CGameManager.Instance.m_Dictionary.GetDisks_By_Tear(_card, _tear);

        var disk = Instantiate(res[Random.Range(0, res.Count)]);
        //var disk = res[Random.Range(0, res.Count)];
        disk.Spawn(_card.m_Data.m_CardType);
        m_Disks[_idx].SetData(disk, _card.m_Data.GetStatusIcon());

        _card.AddDisk(disk);
    }

    //카드 선택
    public void OnFocus_Card(CScriptable_CardSkill _card)
    {
        m_Btn_Submit.interactable = true;

        if (m_FocusCardUI != null) OnClick_Reset();

        foreach (var it in m_CardContainers)
            if (it.m_UI_Card.m_SkillCard == _card)
            { 
                m_FocusCardUI = it.m_UI_Card;
                maxDiskCount = m_FocusCardUI.m_SkillCard.m_Disks.Count;
                isCharged = false;

                if (m_FocusImg.gameObject.activeSelf == false) m_FocusImg.gameObject.SetActive(true);
                 m_FocusImg.transform.parent = m_FocusCardUI.transform;
                m_FocusImg.transform.localPosition = Vector3.zero;

                break; 
            }
    }

    public void OnClick_Up()
    {
        if (diskIdx >= maxDiskCount) return;

        m_FocusCardUI.m_Reload_Disk.MoveUp();

        if (isCharged == false)
        {
            isCharged = true;
            foreach (var it in m_FocusCardUI.m_Img_Dices_Start)
            { m_FocusCardUI.ChargeDice(it, 0); }
        }
        else
        {
            var icon = m_FocusCardUI.m_Img_Dices_Add[diskIdx];
            m_FocusCardUI.ChargeDice(icon, 0);
            m_FocusCardUI.m_SkillCard.SumDiskData(1, ++diskIdx);
        }

        m_FocusCardUI.SetTextData();
    }

    public void OnClick_Reset()
    {
        if (m_FocusCardUI == null) return;

        m_FocusCardUI.m_Reload_Disk.MoveReset();
        m_FocusCardUI.m_SkillCard.m_CalcedDisk.Clear();

        foreach (var it in m_FocusCardUI.m_Img_Dices_Start)
        { m_FocusCardUI.DischargeDice(it); }
        foreach (var it in m_FocusCardUI.m_Img_Dices_Add)
        { m_FocusCardUI.DischargeDice(it); }

        m_FocusCardUI.SetUIData(m_FocusCardUI.m_SkillCard);
        m_FocusCardUI.SetTextData(_isAnim: false);

        isCharged = false;
        diskIdx = 0;
    }

    public void OnInteraction() 
    {
        if (m_FocusCardUI != null)
        { OnClick_Up(); }
    }

    public System.Action m_CB_SelectedRewrod = null;
    //디스크 적용할 카드 결정
    public void OnClick_Submit() 
    {
        if (m_FocusCardUI == null) return;
        
        if (m_CB_SelectedRewrod != null) m_CB_SelectedRewrod();

        

        for (int i = 0; i < m_CardContainers.Count; i++)
        {
            if (m_CardContainers[i].m_UI_Card == m_FocusCardUI)
            {
                m_OriginCards[i].AddDisk(m_Disks[i].m_Disk);
                CGameManager.Instance.m_ScheduleMgr.m_UI_Deck.UpdateDiskData(m_OriginCards[i]);
                m_Disks[i].m_Disk = null;
            }
        }

        m_Btn_Submit.interactable = false;
        m_ParentCanvas.gameObject.SetActive(false);
        //TODO : 디스크 추가되는 연출
    }

    private void OnDisable()
    {
        OnClick_Reset();

        if (CGameManager.Instance.m_Input != null)
            CGameManager.Instance.m_Input.CB_OnInteraction -= OnInteraction;

        maxDiskCount = 0;
        isCharged = false;

        m_FocusCardUI = null;
        m_FocusImg.gameObject.SetActive(false);

        //생성된 디스크 제거 
        foreach (var it in m_Disks)
        { if(it.m_Disk != null) Destroy(it.m_Disk); }
            

    }

    //눌러서 증가 어떻게 하지??
}
