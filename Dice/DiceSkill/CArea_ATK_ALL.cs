using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CArea_ATK_ALL : MonoBehaviour
{
    /*
    ��Ÿ ���?
    ��ƼŬ ���� ���ð�.
    ���� ��ƼŬ ���� �� ������ �ֱ�.
    ���� ���� üũ �ʿ�.
     */
    public CArea m_Area = null;

    public Transform m_SpawnLoc = null;
    [SerializeField] int m_Num = 0;
    [SerializeField] CUtility.ECardType m_Eff = CUtility.ECardType.ATK;

    //�ʱ�ȭ
    public void Spawn(int _num) 
    {
        m_Area = Instantiate(m_Area, m_SpawnLoc);

        m_Num = _num;
        m_Eff = m_Area.m_Type;
        m_Area.m_CB_End = EndSkill;
        m_Area.m_CB_Arrival = HIT;
        m_Area.Spawn();
    }

    public void EndSkill() 
    {
        CGameManager.Instance.m_TurnManager.m_PlayerChar.DoneSkill();
    }

    //�ǰ�
    public void HIT()
    {
        var enemyGroup = CGameManager.Instance.m_TurnManager.m_EnemyGroup;
        var player = CGameManager.Instance.m_TurnManager.m_PlayerChar.m_Hitable;

        foreach (var it in enemyGroup.m_SpawnedMonsters)
        {
            if (it.m_Hitable.m_IsDead == true) continue;


            if (m_Eff == CUtility.ECardType.ATK)
                it.m_Hitable.OnHit(m_Num, 0, CUtility.ETextIcon.NONE);
            else 
                it.m_Hitable.CheckDebuffChange(m_Eff,player, m_Num, 0);
        }
    }
}
