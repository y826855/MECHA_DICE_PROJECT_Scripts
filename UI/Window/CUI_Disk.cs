using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class CUI_Disk : MonoBehaviour
{
    public TMPro.TextMeshProUGUI m_TMP_Name = null;
    public CUI_PropertyGroup m_Properties = null;
    

    public CScriptable_Disk m_Disk = null;

    public void SetData(CScriptable_Disk _disk, CUtility.ENumableIcon _stateEff) 
    {
        m_Disk = _disk;

        m_TMP_Name.text = _disk.m_Data.m_Name;
        m_Properties.SetData(m_Disk, 0, _stateEff);
    }

}
