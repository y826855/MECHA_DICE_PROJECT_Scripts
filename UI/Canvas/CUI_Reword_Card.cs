using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Reword_Card : MonoBehaviour
{
    public GameObject m_ParentCanvas = null;
    public List<Ctnr_Card> m_Cards = new List<Ctnr_Card>();
    
    [SerializeField] CUI_CardInfo_Handler m_UI_CardInfo = null;


    public void OnEnable()
    {
        foreach (var it in m_Cards)
            it.m_CB_Submit = OnClick_ShowInfo;
    }

    public void SetData() 
    {
        RandomCards();
    }

    public void SetData(List<uint> _get, bool _fill = true, int _discovery = 0)
    {
        var dic = CGameManager.Instance.m_Dictionary;
        //int discovery = CGameManager.Instance.m_PlayerData.m_Discovery;
        //int discovery = CGameManager.Instance.m_ScheduleMgr.m_UI_Reword.m_Discovery;

        int idx = 0;
        foreach (var it in m_Cards)
        {
            if (_get.Count > idx && dic.m_AllCard.ContainsKey(_get[idx]))
            {//준비된 리스트의 카드 생성
                var card = dic.m_AllCard[_get[idx]];
                card = Instantiate(card);
                it.transform.parent.gameObject.SetActive(true);

                var remain = _discovery - card.m_Data.m_Discovery;
                AddDisk_By_Discovery(ref card, _discovery);

                it.m_UI_Card.SetUIData(card);
            }
            else if (_fill == true)
            {//나머지는 그냥 채움
                var card = dic.GetCard_By_Discovery(_discovery);
                card = Instantiate(card);
                it.transform.parent.gameObject.SetActive(true);

                var remain = _discovery  - card.m_Data.m_Discovery;
                AddDisk_By_Discovery(ref card, _discovery);
                
                it.m_UI_Card.SetUIData(card);
            }
            else 
            {
                it.transform.parent.gameObject.SetActive(false);
            }
            idx++;
        }
    }

    //남는 발견력으로 디스크 추가하기
    public void AddDisk_By_Discovery(ref CScriptable_CardSkill _card, int _disco) 
    {
        var dic = CGameManager.Instance.m_Dictionary;

        while (_disco >= 20 && _card.m_Disks.Count < 3) 
        {
            var diskTear = dic.m_RandomHelper.DiskTearRandom(_disco);
            var disks = dic.GetDisks_By_Tear(_card, diskTear);
            var disk = Instantiate(disks[Random.Range(0, disks.Count)]);
            _card.AddDisk(disk);

            _disco -= diskTear * 20;
        }
    }


    //카드 랜덤 돌림. 대상 카드 인스턴싱
    public void RandomCards() 
    {
        //int discovery = CGameManager.Instance.m_PlayerData.m_Discovery;
        int discovery = CGameManager.Instance.m_ScheduleMgr.m_UI_Reword.m_Discovery;

        var dic = CGameManager.Instance.m_Dictionary;

        foreach (var it in m_Cards) 
        {
            var card = dic.GetCard_By_Discovery(discovery);
            card = Instantiate(card);
            it.transform.parent.gameObject.SetActive(true);
            it.m_UI_Card.SetUIData(card);

            //발견력 남으면 디스크 추가
            var remain = discovery - card.m_Data.m_Discovery;
            AddDisk_By_Discovery(ref card, discovery);
        }
    }

    //클릭시 정보창 열기. 선택 버튼도 활성화
    public void OnClick_ShowInfo(CScriptable_CardSkill _card)
    {
        foreach (var it in m_Cards) 
        {
            if (it.m_UI_Card.m_SkillCard == _card)
            {
                m_UI_CardInfo.m_FocusCard = _card;
                m_UI_CardInfo.SetData(_instanced: true);
                m_UI_CardInfo.gameObject.SetActive(true);
                m_UI_CardInfo.m_BtnSelect.gameObject.SetActive(true);
                break;
            }
        }
    }

    public System.Action m_CB_SelectedRewrod = null;
    CScriptable_CardSkill selectedCard = null;
    //카드 선택완료
    public void OnClick_SelectCard(CUI_SkillCard _cardUI) 
    {
        selectedCard = _cardUI.m_SkillCard;
        m_ParentCanvas.SetActive(false);
        if (m_CB_SelectedRewrod != null) m_CB_SelectedRewrod();
    }

    //비활성화
    private void OnDisable()
    {
        foreach (var it in m_Cards) 
        {
            //선택된 카드는 삭제 안함
            if (it.m_UI_Card.m_SkillCard == selectedCard) continue;
            Destroy(it.m_UI_Card.m_SkillCard); 
        }

        selectedCard = null;
    }
}
