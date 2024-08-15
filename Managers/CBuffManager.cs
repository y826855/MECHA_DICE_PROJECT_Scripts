using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuffManager : MonoBehaviour
{


    private void Awake()
    {
        CGameManager.Instance.m_BuffMgr = this;
    }
}
