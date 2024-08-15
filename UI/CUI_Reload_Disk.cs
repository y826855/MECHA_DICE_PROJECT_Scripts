using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CUI_Reload_Disk : MonoBehaviour
{
    public RectTransform m_Upper = null;
    public RectTransform m_Mid = null;
    public RectTransform m_Under = null;

    [SerializeField] List<RectTransform> m_Infos = new List<RectTransform>();
    [SerializeField] List<CUI_PropertyGroup> m_InfoGroup = new List<CUI_PropertyGroup>();
    //public List<RectTransform> m_ShowInfo = new List<RectTransform>();
    public List<CUI_PropertyGroup> m_ShowInfo = new List<CUI_PropertyGroup>();
    //public RectTransform m_Last_Info = null;
    public CUI_PropertyGroup m_Last_Info = null;

    public int m_CurrIdx = 0;

    public float m_Duration = 0.5f;

    private void OnEnable()
    { m_CurrIdx = 0; }

    public void Init(CScriptable_CardSkill _skill) 
    {
        //Debug.Log("DISK INIT");

        m_ShowInfo.Clear();
        //m_CurrIdx = 0;
        
        //0과 마지막은 텍스트로 구성됨
        //m_InfoGroup[0].SetData(_skill);
        m_ShowInfo.Add(m_InfoGroup[0]);
        for (int i = 1; i < m_InfoGroup.Count; i++)
        {
            if (i < _skill.m_Disks.Count+1)
            {//디스크 정보 뿌림
                m_ShowInfo.Add(m_InfoGroup[i]);
                m_InfoGroup[i].SetData(_skill.m_Disks[i - 1]);
                m_InfoGroup[i].gameObject.SetActive(true);
            }
            else
            {
                m_InfoGroup[i].ClearDataNum();
                m_InfoGroup[i].gameObject.SetActive(false);
            }
        }

        m_Last_Info.gameObject.SetActive(true);
        m_ShowInfo.Add(m_Last_Info);
    }

    //public void AddDisk_Dynamic(CScriptable_CardSkill _skill, CScriptable_Disk _disk) 
    //{
    //    int idx = _skill.m_Disks.Count + 1;
    //    m_ShowInfo[idx].gameObject.SetActive(true);
    //    m_ShowInfo[idx].SetData(_disk);
    //}

    Sequence seqMoveUp = null;
    Sequence seqReset = null;

    public void MoveUp() 
    {
        seqMoveUp = DOTween.Sequence();
        seqMoveUp.Join(m_ShowInfo[m_CurrIdx].m_Rect.DOLocalMove(m_Upper.localPosition, m_Duration));
        ++m_CurrIdx;
        seqMoveUp.Join(m_ShowInfo[m_CurrIdx].m_Rect.DOLocalMove(m_Mid.localPosition, m_Duration));
    }

    public void MoveReset() 
    {
        if (m_CurrIdx == 0) return;

        Debug.Log("reset reload");

       //위로 움직이는 시퀀스 제거
        if (seqMoveUp != null) seqMoveUp.Kill();
        seqMoveUp = DOTween.Sequence();

        if (seqReset != null) seqReset.Kill();
        seqReset = DOTween.Sequence();

        float duration = m_Duration / m_CurrIdx;

        //Debug.Log("RESET TEST");
        for (int i = m_CurrIdx; i >= 0; i--) 
        {
            //Debug.Log("RESET");
            var it = m_ShowInfo[i];
            if (it == m_ShowInfo[0]) seqReset.Append(m_ShowInfo[0].m_Rect.DOLocalMove(m_Mid.localPosition, duration));
            else seqReset.Append(it.m_Rect.DOLocalMove(m_Under.localPosition, duration));
        }
      
        m_CurrIdx = 0;
    }
}
