using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHit_Area : MonoBehaviour
{
    [SerializeField] CHitable User = null;
    [SerializeField] CHitable Target = null;
    [SerializeField] int Stack = 0;

    public Vector3 m_SpawnPos = Vector3.zero;

    public SphereCollider m_Col = null;

    public List<CHitable> m_Targets = new List<CHitable>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Monster") 
        { m_Targets.Add(other.GetComponent<CHitable>()); }
    }

    //범위 보여주기
    public void FoundOnTarget() 
    {
        m_Col.enabled = true;

    }

    private void OnDisable()
    {
        m_Col.enabled = false;
        m_Targets.Clear();
    }
}
