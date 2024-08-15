using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CLoadGameData : MonoBehaviour
{
    public GameObject m_Btn_NewGame = null;
    public GameObject m_Btn_LoadGame = null;

    public void Start()
    {
        if (PlayerPrefs.HasKey("Map") == false)
            m_Btn_LoadGame.gameObject.SetActive(false);
    }

    public void OnClick_NewGame() 
    {
        PlayerPrefs.DeleteKey("Map");
        //CGameManager.Instance.m_PlayerData.RemovePlayerData();
        //CGameManager.Instance.m_PlayerData.NewPlayer();
        CGameManager.Instance.m_SceneMgr.ChangeSceneMap();
    }

    public void OnClick_Continue() 
    {
        CGameManager.Instance.m_SceneMgr.m_LoadBattle = true;
        CGameManager.Instance.m_SceneMgr.ChangeSceneMap();
    }

}
