using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class CTesting : MonoBehaviour
{
    public List<CScriptable_Disk> m_Disks = new List<CScriptable_Disk>();

    public CScriptable_CardSkill m_AddableCard = null;

    public List<CScriptable_Disk> m_Result_Search_Disks = new List<CScriptable_Disk>();
    public void Dictionary_Add_TestCase() 
    {
        CGameManager.Instance.m_Dictionary.m_Disk_Tear1.Clear();
        foreach (var it in m_Disks)
        { CGameManager.Instance.m_Dictionary.m_Disk_Tear1.Add(it.m_Data.m_ID, it); }
    }

    public void Search_CanAddDisks() 
    {
        if (m_AddableCard == null) return;

        m_Result_Search_Disks = CGameManager.Instance.m_Dictionary.GetDisks_By_Tear(m_AddableCard, 1);
        foreach (var it in m_Result_Search_Disks)
        {
            Debug.Log(m_AddableCard.m_Data.CanAddDisk(it.m_Data));
        }
    }
}
