using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/SKILL_CARD")]
public class CScriptable_CardSkill : CScriptable_CSVData<CScriptable_CardSkill>
{
#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/Card/";
    static public string m_ATK_Info_Path = "Assets/Game/Scriptables/ATK_Info/";
#endif

    public CUtility.CSkillCard m_Data = new CUtility.CSkillCard();
    public CAttack_Info m_Atk_Info = null;

    public List<CScriptable_Disk> m_Disks = new List<CScriptable_Disk>();
    public CUtility.CDisk m_CalcedDisk = new CUtility.CDisk();

    public enum ECardState { DECK = 0, USED = 1, HAND = 2 }
    [Header("===============================================")]
    public ECardState m_DeckState = ECardState.DECK;

#if UNITY_EDITOR
    static public CScriptable_CardSkill CreatePrefab(CUtility.CSkillCard _data, string _particle)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CSkillCard>();
        t.m_Data.SetProperties();

        t.m_Atk_Info = LoadAsset(m_ATK_Info_Path, _particle) as CAttack_Info;
        t.SumProperties();
        return t;
    }
#endif

    public void SumProperties()
    {
        m_Data.SetProperties();

        foreach (var it in m_Disks)
            m_Data.m_CurrProperty.m_Granted |= it.m_Data.m_CurrProperty.m_Granted;
    }

    public void Spawn(CHitable _user) 
    {
        m_Atk_Info = Instantiate(m_Atk_Info);
        m_Atk_Info.m_MonsterSkill = null;
        m_Atk_Info.m_CardSkill = this;
        m_Atk_Info.m_User = _user;
    }

    public void SumDiskData(int _avr, int _idx)
    {
        if (m_Disks.Count <= _idx) _idx = m_Disks.Count;
        m_CalcedDisk.Clear();

        //for (int i = 0; i < m_Disks.Count; i++) 
        for (int i = 0; i < _idx; i++)
        {
            var it = m_Disks[i].m_Data;
            m_CalcedDisk.AddData(it, _avr);
        }
    }

    public void ReadyToUse()
    {
        //m_Data_Calc = m_Data.Clone<CUtility.CSkillCard>();
    }

    public void AddDisk(CScriptable_Disk _disk)
    {
        //if (m_Disks.Count >= m_Data.m_Sockets.Count) return;
        if (m_Disks.Count >= CUtility.MaxSocketCount) return;

        m_Disks.Add(_disk);
        SumProperties();
    }

    public int GetGoldCost() 
    {
        int cost = 0;
        
        foreach (var it in m_Disks) 
        { cost += it.m_Data.m_Tear * 50; }
        //cost += m_Data.m_Discovery * 10 + Random.Range(50, 60);
        cost += m_Data.m_Discovery * 10 + 50;

        return cost;
    }
}
