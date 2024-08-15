using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "RANDOM_HELPER", menuName = "ScriptableData/RANDOM_HELPER")]
//[System.Serializable]
public class CRandomHelper : ScriptableObject
{
    public List<Vector2Int> m_CardTears = new List<Vector2Int>();
    public List<Vector2Int> m_DiskTears = new List<Vector2Int>();

    //발견력에 따른 카드 티어 반환
    public int CardRandom(int _discovery) 
    {
        //바로 1티어 반환
        if (_discovery < m_CardTears[1].x) return 1;

        int rand = m_CardTears[0].y;
        int lastInterval = 0;
        int maxTear = m_CardTears.Count;

        //티어에 속하는 만큼 랜덤에 집어넣음
        for (int i = 1; i < m_CardTears.Count; i++)
        {
            if (_discovery >= m_CardTears[i].x)
            {
                maxTear--;
                if (_discovery < m_CardTears[i].y)
                {
                    int interval = Mathf.Clamp(_discovery - m_CardTears[i].x, 5, 99);
                    rand += interval;
                    lastInterval = interval;
                    break;
                }
                else rand += m_CardTears[i].y - m_CardTears[i].x;
            }
        }

        int max = rand;
        rand = Random.Range(0, rand);

        //뽑을 수 있는 최고 티어부터 검수
        for (int i = m_CardTears.Count - maxTear; i >= 0; i--)
        {
            max -= lastInterval;
            if (rand >= max) 
            { return i + 1; }
            else lastInterval = m_CardTears[i].y - m_CardTears[i].x;
        }

        return 0;
    }
    
    
    //일단 최대치만 박음ㅋ,,
    public int DiskTearRandom( int _discovery) 
    {
        if (_discovery < m_DiskTears[1].x) return 1;
        else if (_discovery < m_DiskTears[2].x) return 2;
        else return 3;
    }
}
