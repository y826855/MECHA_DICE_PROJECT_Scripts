using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.SceneManagement;


public class CGameManager : CScriptableSingletone<CGameManager>
{
    //public CPlayerInput m_PlayerInput = null;

    //adding on Runtime
    //public CPlayer m_Player = null;
    //public C_PC_Hosting m_Network = null;
    //public CUI_SkillPage m_SkillPage = null;
    //public CPlayerInput_Controller m_PlayerInput = null;
    [Header("---For Runtime---")]
    public CTurnManager m_TurnManager = null;
    public CEventManager m_EventManager = null;
    public CDiceMananger m_DiceManager = null;
    public CBuffManager m_BuffMgr = null;
    public CScheduleManager m_ScheduleMgr = null;
    //public CVIewDetailManager m_DetailManager = null;
    //public CSelectableArea_Handler m_SelectableHandler = null;


    [Header("---For DontDestroy---")]
    //public CPlayerInput_Test m_PlayerInput = null;
    public CPlayerInput m_Input = null;
    public CSceneManager m_SceneMgr = null;
    public CSoundManager m_SoundMgr = null;

    //public Map.MapManager m_MapMgr = null;

    //[Header("---For Events---")]
    //public CEvent_Skill_Upgrade m_Event_Skill_Upgrade_Ctrl = null;
    //public CEvent_Choice m_Event_Choice = null;
    //public CEvent_Shop m_Event_Shop = null;

    //adding on
    [Header("---PrefabDatas---")]
    public CCSVDictionary m_Dictionary = null;
    public CPlayerData m_PlayerData = null;


    [Header("---For UTILITY_COLOR---")]
    public bool m_DEBUG_GAMEMODE = true;

    [Header("---For UTILITY_COLOR---")]
    public Color color_Disable;
    public Color color_Wait;

    public Color color_ATK;
    public Color color_DEF;
    public Color color_BUF;

    public Color color_USE;
    public Color color_AREA;
    public Color color_QUEST;

    public Color color_Tear1;
    public Color color_Tear2;
    public Color color_Tear3;
    public Color color_Tear4;

    [Header("---For UTILITY_SPRITE---")]
    public List<Sprite> m_TextableIcons = new List<Sprite>();

    [Header("---DISK CALC---")]
    public CUtility.CDisk_Calc m_DISK_CALC_Normal;
    public CUtility.CDisk_Calc m_DISK_CALC_Elec;
    public CUtility.CDisk_Calc m_DISK_CALC_Burn;
    public CUtility.CDisk_Calc m_DISK_CALC_Rock;

    float gameSpeed = 1;

    public float m_GameSpeed 
    {
        get { return Time.deltaTime* gameSpeed; }
        set { gameSpeed = value; }
    }
    public float m_OriginGameSpeed 
    {
        get { return gameSpeed; }
    }
    

    public override void BeforeLoadInstance()
    {
        m_PlayerData.TestSpawn();
    }



    //TODO : 게임 종료시 저장 여기서 하자
    public void Save() 
    {
        Debug.Log("QUIT");
        
    }


    ////////////////////////////////

    public void OnClick_EndGame()
    {
        //SceneManager.LoadSceneAsync("Scene_Lobby");
    }

    public void OnClick_StartGame() 
    {
        //SceneManager.LoadSceneAsync("Scene_Snake");
    }

}
