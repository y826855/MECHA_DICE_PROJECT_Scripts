using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Febucci.UI;

public class CTest_TextGen : MonoBehaviour
{
    public CTextShoutOut m_Pref = null;

    public string m_Text = "";

    public void GenText() 
    {
        var inst = Instantiate(m_Pref, this.transform);
        //inst.SetText(m_Text);
        inst.TMPChange(m_Text);
    }
}
