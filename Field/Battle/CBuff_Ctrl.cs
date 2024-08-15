using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CBuff_Ctrl : MonoBehaviour
{
    //public SerializeDictionary<int, CUtility.CBuff_Infinite> m_Buff_OnEndTurn
    //= new SerializeDictionary<int, CUtility.CBuff_Infinite>();

    public CUtility.CBuff m_Buff = new CUtility.CBuff();

    //public CUtility.CBuff_Infinite m_Buff_Inf = new CUtility.CBuff_Infinite();
    //public CUtility.CBuff_Infinite m_CurrBuff = new CUtility.CBuff_Infinite();



    //public void AddBuff(SerializeDictionary<int, CUtility.CBuff_Infinite> _buff) 
    //{
    //    m_Buff_Inf.AddBuff(_buff[0]);
    //    _buff.Remove(0);

    //    foreach (var it in _buff) 
    //    {
    //        if (m_Buff_OnEndTurn.ContainsKey(it.Key) == false)
    //            m_Buff_OnEndTurn[it.Key] = new CUtility.CBuff_Infinite();
    //        m_Buff_OnEndTurn[it.Key].AddBuff(it.Value);
    //    }

    //    CalcCurrBuff();
    //}

    ////턴 시작시 카운팅
    //public void TurnBuffCount()
    //{
    //    var keys = new List<int>(m_Buff_OnEndTurn.Keys);
    //    keys.Sort();

    //    foreach (var key in keys)
    //    {
    //        int countKey = key - 1;
    //        var value = m_Buff_OnEndTurn[key];
    //        m_Buff_OnEndTurn.Remove(key, out value);
    //        if (countKey <= 0)
    //        {
    //            //버프 아이콘 제거 이펙트 떠야함
    //        }
    //        else
    //        { m_Buff_OnEndTurn.Add(countKey, value); }
    //    }

    //    CalcCurrBuff();
    //    //RemoveBuffIcon();
    //}

    ////턴시작시 활성화되는 버프
    //public void BeginTurnActive()
    //{
    //    if (m_Buff_Inf.m_Burn > 0)
    //    {
    //        //m_Owner.m_Hitable.Gain_Burn_Dmg(m_OnBuff_Infinity.m_Burn);
    //        if (m_Buff_Inf.m_Burn == 1)
    //            m_Buff_Inf.m_Burn -= 1;
    //        else m_Buff_Inf.m_Burn /= 2;
    //    }

    //    CalcCurrBuff();
    //}

    ////버프 합산
    //public void CalcCurrBuff()
    //{
    //    m_CurrBuff.ClearData();

    //    foreach (var it in m_Buff_OnEndTurn) m_CurrBuff.AddBuff(it.Value);
    //    m_CurrBuff.AddBuff(m_Buff_Inf);


    //    //if (m_Func_Update_Cool != null) m_Func_Update_Cool();
    //    //if (m_Func_Update_Data != null) m_Func_Update_Data();
    //    //CheckBuffIcon();
    //}
}
