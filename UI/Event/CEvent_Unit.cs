using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEvent_Unit : MonoBehaviour
{
    public uint m_ID = 0;
    public List<CNPC> m_NPCs = new List<CNPC>();

    public List<CNPC> ActiveEvent() 
    {
        this.gameObject.SetActive(true);
        return m_NPCs;
    }
}
