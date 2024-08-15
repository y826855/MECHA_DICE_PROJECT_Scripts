using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArea_Scan : MonoBehaviour
{

    public List<CHitable> m_Targets = new List<CHitable>();

    public void SetSize() 
    {
        
    }

    public void AttackConfrim() 
    {
        if (m_Targets.Count > 0)
        {
            CGameManager.Instance.m_TurnManager.m_PlayerChar.m_SkillTargets
                = m_Targets;
        }
        else Debug.Log("대상이 없음");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster")
        { m_Targets.Add(other.GetComponent<CHitable>()); }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Monster")
        {
            var target = other.GetComponent<CHitable>();
            if (m_Targets.Contains(target) == true) m_Targets.Remove(target);
        }
    }

    private void OnDisable()
    {
        m_Targets.Clear();
    }
}
