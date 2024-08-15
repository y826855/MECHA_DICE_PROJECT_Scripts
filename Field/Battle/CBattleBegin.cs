using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBattleBegin : MonoBehaviour
{
    public List<CBattle_Act> m_Battle_Intros = null;
    public List<CBattle_Act> m_Battle_Outros = null;


    virtual public IEnumerator CoActBeforeBattle() 
    {
        yield return null;
    }
    virtual public IEnumerator CoActAfterBattle()
    {
        yield return null;
    }
}
