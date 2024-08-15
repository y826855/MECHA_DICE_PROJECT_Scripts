using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif


[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/Disk")]
public class CScriptable_Disk : CScriptable_CSVData<CScriptable_Disk>
{
#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/Disk/";
#endif

    public CUtility.CDisk m_Data = new CUtility.CDisk();


#if UNITY_EDITOR
    static public CScriptable_Disk CreatePrefab(CUtility.CDisk _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CDisk>();
        t.m_Data.SetProperties();
        return t;
    }
#endif

    public void Spawn(CUtility.ECardType _type)
    {
        switch (_type)
        {
            case CUtility.ECardType.ATK:
                CalcByType(CGameManager.Instance.m_DISK_CALC_Normal); break;
            case CUtility.ECardType.ATK_ELEC:
                CalcByType(CGameManager.Instance.m_DISK_CALC_Elec); break;
            case CUtility.ECardType.ATK_BURN:
                CalcByType(CGameManager.Instance.m_DISK_CALC_Burn); break;
            case CUtility.ECardType.ATK_ROCK:
                CalcByType(CGameManager.Instance.m_DISK_CALC_Rock); break;
        }
    }

    void CalcByType(CUtility.CDisk_Calc _calc)
    {
        if(m_Data.m_Damage.m_Num != 0) 
            m_Data.m_Damage.m_Num
                = Mathf.Clamp(Mathf.FloorToInt(m_Data.m_Damage.m_Num * _calc.m_ATK), 1, 99);

        if (m_Data.m_Defend.m_Num != 0)
            m_Data.m_Defend.m_Num
                = Mathf.Clamp(Mathf.FloorToInt(m_Data.m_Defend.m_Num * _calc.m_DEF), 1, 99);

        if (m_Data.m_StatusEff.m_Num != 0)
            m_Data.m_StatusEff.m_Num
                = Mathf.Clamp(Mathf.FloorToInt(m_Data.m_StatusEff.m_Num * _calc.m_STACK), 1, 99);

        if (m_Data.m_Targets.m_Num != 0)
            m_Data.m_Targets.m_Num
                = Mathf.Clamp(Mathf.FloorToInt(m_Data.m_Targets.m_Num * _calc.m_TARGET), 1, 99);

        if (m_Data.m_Debuff != 0)
            m_Data.m_Debuff
                = Mathf.Clamp(Mathf.FloorToInt(m_Data.m_Debuff * _calc.m_DEBUFF), 1, 99);
    }
}
