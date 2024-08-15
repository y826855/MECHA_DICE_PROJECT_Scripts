using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ctnr_Card : MonoBehaviour
{
    public CUI_SkillCard m_UI_Card = null;
    public System.Action<CScriptable_CardSkill> m_CB_Submit;
    public TMPro.TextMeshProUGUI m_TMP = null;

    public void OnSubmit()
    {
        Debug.Log("SUBMIT!");

        if (m_CB_Submit != null)
        {
            m_CB_Submit(m_UI_Card.m_SkillCard);
            Debug.Log("SELECTED");
        }
    }
}
