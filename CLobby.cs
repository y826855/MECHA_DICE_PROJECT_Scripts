using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLobby : MonoBehaviour
{
    // Start is called before the first frame update

    public CPlayerData m_PlayerDefault = null;
    private void Awake()
    {
        CGameManager.Instance.m_PlayerData.CloneData(m_PlayerDefault);
    }
    void Start()
    {
        CGameManager.Instance.m_SoundMgr.PlaySoundBGM(CSoundManager.EBGM.BGM_01);
    }

    public void ChangeLanguage(int _tag) 
    {
        CGameManager.Instance.m_Dictionary.ChangeLanguage(_tag);
    }
}
