using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Day_Bag : MonoBehaviour
{
    public CUI_Edit_Week m_EditWeek = null;
    public int m_StartIdx = 0;
    public int m_MaxHolder = 7;


    public List<CUI_Day_Holder> m_Holders = new List<CUI_Day_Holder>();


    //홀더 생성 and 디폴트 정보 뿌림
    public virtual void ReadyToSet(CUI_Event_Day _pref_UI_Day, CUI_Day_Holder _pref_Holder,
        List<CScriptable_SceneInfo> _daysInfo, bool _canBuyOrSell = false)
    {
        if (m_Holders.Count > 0)
        {
            for (int i = 0; i < m_Holders.Count; i++)
                Destroy(m_Holders[i].gameObject);
            m_Holders.Clear();
        }

        for (int i = 0; i < m_MaxHolder; i++)
        {
            var instHolder = Instantiate(_pref_Holder, this.transform);
            CUI_Event_Day instDay = null;
            if (_daysInfo.Count > i)
            {
                instDay = Instantiate(_pref_UI_Day, instHolder.transform);
                instDay.Spawn(_daysInfo[i], m_EditWeek, instHolder);
            }

            instHolder.Spawn(m_EditWeek, _canBuyOrSell, instDay);
            m_Holders.Add(instHolder);
        }
    }

    public void SellDay(CUI_Day_Holder _holder)
    {
        var info = _holder.m_UI_EventDay.m_SceneInfo;
        CGameManager.Instance.m_PlayerData.m_DaysBag.Remove(info);

        Destroy(_holder.m_UI_EventDay.gameObject);
        _holder.m_UI_EventDay = null;
    }

    //데이 추가 만들자
    public void AddDay(CScriptable_SceneInfo _info) 
    {
        foreach (var it in m_Holders) 
        {
            if (it.m_UI_EventDay == null) 
            {
                var instDay = Instantiate(m_EditWeek.m_Pref_Day, it.transform);
                instDay.Spawn(_info, m_EditWeek, it);
                CGameManager.Instance.m_PlayerData.m_DaysBag.Add(_info);
                break;
            }
        }
    }


}
