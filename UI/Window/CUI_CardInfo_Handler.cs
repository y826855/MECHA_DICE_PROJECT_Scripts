using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_CardInfo_Handler : MonoBehaviour
{
    public CScriptable_CardSkill m_FocusCard = null;

    public CUI_SkillCard m_CardUI = null;
    public List<CUI_Disk> m_Diskes = new List<CUI_Disk>();
    public GameObject m_BtnSelect = null;
    public GameObject m_BtnRemove = null;
    public Button m_BtnBuy = null;
    public TMPro.TextMeshProUGUI m_TMP_Cost = null;

    [Header("=======================================")]
    [SerializeField] int maxDiskCount = 0;
    [SerializeField] int diskIdx = 0;
    [SerializeField] bool isCharged = false;
    [SerializeField] bool isSelected = false;
    bool isDestroy = false;

    public System.Action m_CB_Btn_Click = null;

    private void OnEnable()
    {
        if (CGameManager.Instance.m_Input == null) return;
        CGameManager.Instance.m_Input.CB_OnInteraction += OnClick_Up;
        CGameManager.Instance.m_Input.AddEscape(() => { this.gameObject.SetActive(false); });
    }

    //데이터 초기화
    public void SetData(bool _instanced = false) 
    {
        //if (_instanced == false) m_FocusCard = Instantiate(m_FocusCard);
        m_CardUI.SetUIData(m_FocusCard);

        var diskes = m_FocusCard.m_Disks;
        for (int i = 0; i < m_Diskes.Count; i++) 
        {
            var it = m_Diskes[i];
            if (i < diskes.Count) 
            {
                it.gameObject.SetActive(true);
                it.SetData(diskes[i], m_FocusCard.m_Data.GetStatusIcon());
            }
            else it.gameObject.SetActive(false);
        }

        maxDiskCount = m_FocusCard.m_Disks.Count;
        isCharged = false;
    }


    public void OnClick_Up() 
    {
        if (diskIdx >= maxDiskCount) return;

        m_CardUI.m_Reload_Disk.MoveUp();

        if (isCharged == false)
        {
            isCharged = true;
            foreach (var it in m_CardUI.m_Img_Dices_Start)
            { m_CardUI.ChargeDice(it, 0); }
        }
        else 
        {
            var icon = m_CardUI.m_Img_Dices_Add[diskIdx];
            m_CardUI.ChargeDice(icon, 0);
            m_CardUI.m_SkillCard.SumDiskData(1, ++diskIdx); 
        }

        m_CardUI.SetTextData();
    }

    //카드 상태 초기화
    public void OnClick_Reset()
    {
        m_CardUI.m_Reload_Disk.MoveReset();
        m_CardUI.m_SkillCard.m_CalcedDisk.Clear();

        foreach (var it in m_CardUI.m_Img_Dices_Start)
        { m_CardUI.DischargeDice(it); }
        foreach (var it in m_CardUI.m_Img_Dices_Add)
        { m_CardUI.DischargeDice(it); }

        m_CardUI.SetUIData(m_CardUI.m_SkillCard);
        m_CardUI.SetTextData(_isAnim: false);

        isCharged = false;
        diskIdx = 0;
    }

    //선택된 카드 획득
    public void OnClick_Select() 
    {
        if (m_FocusCard == null) return;
        m_FocusCard.Spawn(null);
        CGameManager.Instance.m_ScheduleMgr.m_UI_Deck.AddCard(m_FocusCard);
        //CGameManager.Instance.m_PlayerData.m_Deck.Add(m_FocusCard);
        isSelected = true;
        this.gameObject.SetActive(false);

        //TODO : 카드 추가 연출
    }

    public void OnClick_Remove() 
    {
        CGameManager.Instance.m_ScheduleMgr.m_UI_Deck.RemoveCard(m_FocusCard);
        this.gameObject.SetActive(false);
    }

    //TODO : 판매모드를 걍 만들자;
    public void OpenBuyMode() 
    {
        this.gameObject.SetActive(true);
        m_BtnBuy.gameObject.SetActive(true);

        int cost = m_FocusCard.GetGoldCost();
        var playerGold = CGameManager.Instance.m_PlayerData.m_GOLD;

        m_BtnBuy.interactable = playerGold >= cost;
        m_TMP_Cost.text = string.Format("BUY:{0}G", m_FocusCard.GetGoldCost());
    }


    //카드 구매
    public void OnClick_Buy() 
    {
        CGameManager.Instance.m_ScheduleMgr.m_UI_Shop.OnClick_BuyCard(m_FocusCard);
        if (m_CB_Btn_Click != null) m_CB_Btn_Click();
        
        OnClick_Select();
    }


    private void OnDisable()
    {
        if(CGameManager.Instance.m_Input != null)
            CGameManager.Instance.m_Input.CB_OnInteraction -= OnClick_Up;
        
        m_BtnSelect.gameObject.SetActive(false);
        m_BtnRemove.gameObject.SetActive(false);
        m_BtnBuy.gameObject.SetActive(false);

        if (m_FocusCard == null) return;

        OnClick_Reset();
        
        //if (isSelected == false) Destroy(m_FocusCard);
        //else isSelected = false;

        m_FocusCard = null;
        m_CB_Btn_Click = null;
    }
}
