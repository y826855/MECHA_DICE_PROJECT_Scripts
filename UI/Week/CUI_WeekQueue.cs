using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_WeekQueue : MonoBehaviour
{
    public int m_CountOfDays = 5;
    public int m_CurrDay = 0;
    public TMPro.TextMeshProUGUI m_TMP_Stage = null;
    public Transform m_UI_Location = null;

    //�̸� �غ�� ť. event day�� �̸� ��ġ�Ǿ� �ִ�.
    public List<CScriptable_SceneInfo> m_WeekQueue = new List<CScriptable_SceneInfo>();

    public List<CUI_Day_Holder> m_DayHolders = new List<CUI_Day_Holder>();

    public CUI_Event_Day m_Pref_Day = null;
    public CUI_Day_Holder m_Pref_Holder = null;

    public UnityEngine.UI.Button m_weekReword = null;
    public RectTransform m_UI_DayPointer = null;

    //public void ReadyToSet(CUI_Day_Holder _pref)
    public void ReadyToSet(System.Action CB_OpenWeekReword = null)
    {
        if (m_DayHolders != null) 
        {
            m_UI_DayPointer.parent = this.transform;

            for (int i = 0; i < m_DayHolders.Count; i++)
                Destroy(m_DayHolders[i].gameObject);
            m_DayHolders.Clear();
        }

        foreach (var it in m_WeekQueue) 
        {
            var instHolder = Instantiate(m_Pref_Holder, m_UI_Location);
            instHolder.SetBtn(false);

            if (it != null)
            {
                var instDay = Instantiate(m_Pref_Day, instHolder.transform);
                instHolder.m_UI_EventDay = instDay;
                instDay.Spawn(it, null, instHolder);
                instHolder.Toggle_Canvas(false, true);
            }

            m_DayHolders.Add(instHolder);
        }

        if (CB_OpenWeekReword != null)
        { m_weekReword.onClick.AddListener(() => CB_OpenWeekReword()); }
    }

    //����� �迭 ������ ������Ʈ
    public void UpdateData() 
    {
        //ReadyToSet ���Ŀ� ����� ���̱� ������, weekQueue�� holder�� ������ ����
        for (int i = 0; i < m_WeekQueue.Count; i++) 
        {
            var it = m_WeekQueue[i];
            var holder = m_DayHolders[i];

            //���� ������ �ٸ��� ����UI ���� �� ����
            if (holder.m_UI_EventDay == null || 
                holder.m_UI_EventDay.m_SceneInfo != it) 
            {
                //���� ��������� ����
                if (holder.m_UI_EventDay != null)
                { Destroy(holder.m_UI_EventDay.gameObject); }

                if (it == null) continue;

                var instDay = Instantiate(m_Pref_Day, holder.transform);
                holder.m_UI_EventDay = instDay;
                holder.Toggle_Canvas(false, true);
                instDay.Spawn(it, null, holder);
            }
        }
    }

    public bool CheckIsFull() 
    {
        foreach (var it in m_DayHolders)
            if (it.m_UI_EventDay == null) return false;
        return true;
    }

    //�̺�Ʈ �� �߰�
    public void AddDay(CUI_Event_Day _ui) 
    {
        foreach (var it in m_DayHolders) 
        {
            if (it.m_UI_EventDay == null) 
            {
                it.m_UI_EventDay = _ui;
                _ui.transform.SetParent(it.transform);
                _ui.transform.localPosition = Vector3.zero;
                break;
            }
        }
    }

    //���� ������
    public void DecideWeek() 
    {
        if (CheckIsFull() == false) return;

        var playerWeek = CGameManager.Instance.m_PlayerData.m_SubmitedWeek;

        for (int i = 0; i < m_DayHolders.Count; i++) 
        {
            var it = m_DayHolders[i];
            it.m_UI_EventDay.RemoveBySubmit(it);
            m_WeekQueue[i] = it.m_UI_EventDay.m_SceneInfo;
        }
    }

    //��¥ �̵�
    public bool MovePointerToday(int _day) 
    {
        if (_day < 0 || _day >= m_DayHolders.Count)
        { m_UI_DayPointer.gameObject.SetActive(false); return true; }

        else m_UI_DayPointer.gameObject.SetActive(true);

        m_UI_DayPointer.parent = m_DayHolders[_day].transform;
        m_UI_DayPointer.localPosition = Vector3.zero;
        return false;
    }
}
