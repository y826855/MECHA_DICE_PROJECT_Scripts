using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CUI_Scene_Reword : MonoBehaviour
{
    public int m_Count_DC = 0;
    public TMPro.TextMeshProUGUI m_TMP_SelectCount = null;
    public CanvasGroup m_Disk_Or_Card = null;

    [Header("====================")]
    public CScriptable_SceneInfo m_SceneInfo = null;
    public TMPro.TextMeshProUGUI m_TMP_IsFull = null;
    public CUI_Event_Day m_EventDay = null;

    [Header("====================")]
    public TMPro.TextMeshProUGUI m_TMP_Gold;
    public TMPro.TextMeshProUGUI m_TMP_Tokken;
    public TMPro.TextMeshProUGUI m_TMP_HP;
    public TMPro.TextMeshProUGUI m_TMP_MaxHP;
    public TMPro.TextMeshProUGUI m_TMP_MaxMana;


    public Button m_Btn_Card = null;
    public Button m_Btn_Disk = null;

    [Header("====================")]
    public CUI_Reword_Card m_Reword_Card = null;
    public CUI_Reword_Disk m_Reword_Disk = null;

    public CUtility.CReward_Day m_CurrReward = new CUtility.CReward_Day();
    public int m_Discovery = 0;
    private void OnEnable()
    {
        m_Reword_Card.m_CB_SelectedRewrod = () => { m_Btn_Card.interactable = false; };
        m_Reword_Disk.m_CB_SelectedRewrod = () => { m_Btn_Disk.interactable = false; };

        m_TMP_SelectCount.text = m_Count_DC.ToString();
        bCardShowed = false;
        bDiskShowed = false;
    }

    private void OnDisable()
    {
        ResetData();
    }

    public void ResetData() 
    {
        m_CurrReward.Clear();

        m_Btn_Card.gameObject.SetActive(false);
        m_Btn_Disk.gameObject.SetActive(false);
        m_TMP_IsFull.gameObject.SetActive(false);
        m_EventDay.gameObject.SetActive(false);

        m_TMP_Gold.gameObject.SetActive(false);
        m_TMP_Tokken.gameObject.SetActive(false);
        m_TMP_HP.gameObject.SetActive(false);
        m_TMP_MaxHP.gameObject.SetActive(false);
        m_TMP_MaxMana.gameObject.SetActive(false);
        m_EventDay.gameObject.SetActive(false);

        m_Discovery = 0;
    }

    public void SetInfo() 
    {
        //플레이어 발견력에 따른 추가보상 어떡하지?

        //Debug.Log(m_SceneInfo.m_Data.m_Name);
        //Debug.Log(m_CurrReward);
        //m_CurrReward.Add(m_SceneInfo.m_Data.m_BasicReward);

        //m_Count_DC = m_CurrRewrod.m_CountOf_CardOrDisk;
        //m_TMP_SelectCount.text = m_CurrRewrod.m_CountOf_CardOrDisk.ToString();

        m_SceneInfo = CGameManager.Instance.m_ScheduleMgr.m_CurrMap;
        m_CurrReward.Add(m_SceneInfo.m_Data.m_BasicReward);

        //------------------------------
        m_TMP_Gold.gameObject.SetActive(m_CurrReward.m_Gold > 0);
        m_TMP_Gold.text = m_CurrReward.m_Gold.ToString();
        CGameManager.Instance.m_PlayerData.AddGold(m_CurrReward.m_Gold);

        m_TMP_Tokken.gameObject.SetActive(m_CurrReward.m_Tokken > 0);
        m_TMP_Tokken.text = m_CurrReward.m_Tokken.ToString();

        m_TMP_HP.gameObject.SetActive(m_CurrReward.m_HP > 0);
        m_TMP_HP.text = m_CurrReward.m_HP.ToString();
        CGameManager.Instance.m_PlayerData.AddHP(m_CurrReward.m_HP);

        m_TMP_MaxHP.gameObject.SetActive(m_CurrReward.m_Max_HP > 0);
        m_TMP_MaxHP.text = m_CurrReward.m_Max_HP.ToString();
        CGameManager.Instance.m_PlayerData.AddMaxHP(m_CurrReward.m_Max_HP);


        m_TMP_MaxMana.gameObject.SetActive(m_CurrReward.m_MaxMana > 0);
        m_TMP_MaxMana.text = m_CurrReward.m_MaxMana.ToString();
        CGameManager.Instance.m_PlayerData.AddMaxMana(m_CurrReward.m_MaxMana);


        //Todo : 추가 보상 아이콘 관련 처리 해야함~
        if (m_CurrReward.m_Cards.Count > 0)
        {
            m_Btn_Card.interactable = true;
            m_Btn_Card.gameObject.SetActive(true);
        }
        if (m_CurrReward.m_DiskTear >= 0)
        {
            m_Btn_Disk.interactable = true;
            m_Btn_Disk.gameObject.SetActive(true);
        }
        //------------------------------

        //씬 보상
        if (m_CurrReward.m_Scene != 0) 
        {
            //가방 꽉차면 텍스트 표기
            if (CGameManager.Instance.m_PlayerData.IsDayBagFull() == true)
                m_TMP_IsFull.gameObject.SetActive(true);

            switch (m_CurrReward.m_Scene)
            {
                case 12007: //상점
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[105];
                    break;

                case 12006: //도적_1
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[107];
                    break;
                case 10010://도적_2
                case 10011:
                case 10012:
                case 10013:
                case 10014:
                case 10015: 
                case 10016: 
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[108];
                    break;
                case 12008: //도적_3
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[114];
                    break;

                case 10017: //테스트 엘리트 
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[108];
                    break;
                
                case 116: //랜덤 엘리트
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[116];
                    break;
                default: //랜덤 이벤트
                    m_SceneInfo = CGameManager.Instance.m_Dictionary.m_SceneInfos[104];
                    break;
            }

            m_EventDay.gameObject.SetActive(true);
            m_EventDay.SetData(m_SceneInfo, 0);
        }
    }

    [SerializeField] bool bCardShowed = false;
    [SerializeField] bool bDiskShowed = false;
    public void OnClick_Card() 
    {
        m_Reword_Card.m_ParentCanvas.SetActive(true);
        if (bCardShowed == false)
            m_Reword_Card.SetData(m_CurrReward.m_Cards, m_CurrReward.m_FillCards, m_Discovery);
        bCardShowed = true;
    }
    public void OnClick_Disk() 
    {
        m_Reword_Disk.m_ParentCanvas.SetActive(true);
        if (bDiskShowed == false)
            m_Reword_Disk.SetData(m_CurrReward.m_DiskTear);
        bDiskShowed = true;
    }

    public void OnClick_Next() 
    {
        if(m_EventDay.gameObject.activeSelf == true)
            if (CGameManager.Instance.m_PlayerData.IsDayBagFull() != true)
                CGameManager.Instance.m_ScheduleMgr.m_UI_EditWeek.m_UserBag.AddDay(m_SceneInfo);

        CGameManager.Instance.m_ScheduleMgr.MoveToNextDay();
    }
}
