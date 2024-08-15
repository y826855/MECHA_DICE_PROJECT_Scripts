using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMonster_WeaponSwap : CMonster
{
    public Transform m_HandLoc_L = null;
    public Transform m_HandLoc_R = null;

    [System.Serializable]
    public class CWeapon 
    {
        public GameObject m_Model = null;
        public Transform m_ProjSpawnLoc = null;
        public Transform m_WaitPos = null;

        public bool m_HandL = false;
        public bool m_Handle_Always = false;

        public void SetEnable(Transform _handLoc) 
        {
            if (m_Model.activeSelf == false)
                m_Model.SetActive(true);

            m_Model.transform.SetParent(_handLoc);
            m_Model.transform.localPosition = Vector3.zero;
            m_Model.transform.localRotation = Quaternion.identity;
        }

        public void SetDisable() 
        {
            if (m_Handle_Always == true) return;

            if (m_WaitPos == null)
            { m_Model.SetActive(false); }
            else
            {
                m_Model.transform.SetParent(m_WaitPos);
                m_Model.transform.localPosition = Vector3.zero;
                m_Model.transform.localRotation = Quaternion.identity;
            }
        }
    }


    public CWeapon m_DefaultWeapon = new CWeapon();
    public CWeapon m_SideWeapon = new CWeapon();
    public CWeapon m_CurrWeapon = null;

    public override void Start()
    {
        base.Start();

        m_CurrWeapon = m_DefaultWeapon;
        m_SideWeapon.SetDisable();

        m_Atk_SpawnLoc = m_CurrWeapon.m_ProjSpawnLoc;
    }

    public void SwapWeapon() 
    {
        if (m_CurrWeapon == m_DefaultWeapon)
        {
            m_CurrWeapon.SetDisable();
            if(m_SideWeapon.m_HandL == true)
                m_SideWeapon.SetEnable(m_HandLoc_L);
            else m_SideWeapon.SetEnable(m_HandLoc_R);
            m_CurrWeapon = m_SideWeapon;
        }
        else
        {
            m_CurrWeapon.SetDisable();

            if (m_DefaultWeapon.m_HandL == true)
                m_DefaultWeapon.SetEnable(m_HandLoc_L);
            else m_DefaultWeapon.SetEnable(m_HandLoc_R);
            m_CurrWeapon = m_DefaultWeapon;
        }

        m_Atk_SpawnLoc = m_CurrWeapon.m_ProjSpawnLoc;
        if (m_Hitter != null) m_Hitter.m_SpawnLoc = m_CurrWeapon.m_ProjSpawnLoc;
    }
}
