using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CTurnManager : MonoBehaviour
{
    public Cinemachine.CinemachineVirtualCamera m_GenCamera = null;
    public Cinemachine.CinemachineVirtualCamera m_BattleCamera = null;
    
    public CPlayerChar m_PlayerChar = null;
    public CDiceMananger m_DiceMgr = null;
    public CEnemyGroup m_EnemyGroup = null;
    public CUI_LogPool m_LogPool = null;

    public Button m_Btn_TurnEnd = null;

    //[Header("======================DEBUG======================")]

    public CTextShoutOut m_Pref_ShoutOut = null;

    [Header("======================BattleMap======================")]
    public List<GameObject> m_BattleMaps = new List<GameObject>();
    public GameObject m_CurrMap = null;
    public CBattleBegin m_Battle_Event = null;
    [Header("======================HIT EFF======================")]
    public ParticleSystem m_Hit_Elec = null;
    public ParticleSystem m_Hit_Burn = null;
    public ParticleSystem m_Hit_Rock = null;

    [Header("======================GLOBAL ATK======================")]
    public CArea_Random m_Area_RandomATK = null;
    public CArea_ATK_ALL m_Area_ATK_ALL = null;

    public enum ETurnState
    { 
        BEGIN_BATTLE = 0,

        PLAYER_TURN_BEGIN,
        PLAYER_TURN_ACTION,
        PLYAER_TURN_BEFORE_END,
        PLAYER_TURN_END,

        ENEMY_TURN_BEGIN,
        ENEMY_TURN_ACTION,
        ENEMY_TURN_BEFORE_END,
        ENEMY_TURN_END,

        END_BATTLE,
        REWARD,
    }

    public ETurnState m_TurnState = ETurnState.BEGIN_BATTLE;
    Coroutine coState = null;

    private void Awake()
    {
        CGameManager.Instance.m_TurnManager = this;

        foreach (var it in m_BattleMaps)
            if (it.gameObject.activeSelf == true) it.gameObject.SetActive(false);
    }

    public void Start()
    {
        Debug.Log("Turn manager start");

        Setting_BattleMap();
        ReadyToBattle();

        BeginBattle();

        
    }

    public void EndThisScene() 
    {
        Debug.Log("END THIS SCENE");

        m_GenCamera.gameObject.SetActive(true);
        m_BattleCamera.gameObject.SetActive(false);

        m_EnemyGroup.RemoveEnemies();
        m_PlayerChar.m_Walker.SetStartPos();

        m_DiceMgr.m_State = CDiceMananger.EDiceRollState.NONE;
        m_DiceMgr.m_DiceSaveArea.ResetDiceData();
        m_DiceMgr.m_DiceChoiceArea.ResetDiceData();
    }

    //플레이어 소환
    //몬스터 소환
    public void ReadyToBattle() 
    {
        Debug.Log("턴 매니터 전투 시작 준비!");
        m_PlayerChar.Spawn();

        //지정된 몬스터 그룹 있다면 불러옴
        var map = CGameManager.Instance.m_ScheduleMgr.m_CurrMap;
        if (map.m_Data.m_Connection_ID > 0)
        { m_EnemyGroup.SetMonsterGroup(map.m_Data.m_Connection_ID); }
        else 
        {
            switch (map.m_Data.m_Type) 
            {
                case CUtility.CSceneInfo.ESceneType.BATTLE_LOW:
                    m_EnemyGroup.SetMonsterGroup(_tear: 1); break;
                case CUtility.CSceneInfo.ESceneType.BATTLE_MID:
                    m_EnemyGroup.SetMonsterGroup(_tear: 2); break;
                case CUtility.CSceneInfo.ESceneType.BATTLE_HIGH:
                    m_EnemyGroup.SetMonsterGroup(_tear: 3); break;
                case CUtility.CSceneInfo.ESceneType.BATTLE_ELITE:
                    m_EnemyGroup.SetMonsterGroup(_tear: 4); break;
            }
        }

        m_EnemyGroup.SpawnEnemies();
    }

    public void BeginBattle() 
    {
        m_TurnState = ETurnState.BEGIN_BATTLE;
        coState = StartCoroutine(CoBeginBattle());
    }

    IEnumerator CoBeginBattle() 
    {
        yield return CUtility.GetSecD1To5s(0.5f);

        //캐릭터 이동
        var walker = m_PlayerChar.m_Walker;
        var walkDuration = Mathf.Clamp(Mathf.Floor(walker.MoveTo()), 1, 100);

        //현재 일정 보이게 하기

        //이동 딜레이
        yield return CUtility.GetSecD1To5s(walkDuration);
        //카메라 워킹
        m_GenCamera.gameObject.SetActive(false);
        m_BattleCamera.gameObject.SetActive(true);

        //전투 전 등장 비슷한 연출 할 예정
        if (m_Battle_Event != null)
            yield return StartCoroutine(m_Battle_Event.CoActBeforeBattle());

        yield return CUtility.GetSecD1To5s(0.5f);

        //턴 객체들 전투 준비
        m_PlayerChar.OnBattle();
        m_EnemyGroup.OnBattle();
        yield return CUtility.GetSecD1To5s(0.5f);

        PlayerTurn_Begin();
    }

    //==================================================//
    public void PlayerTurn_Begin()
    {
        //피격 누적 지움
        m_PlayerChar.m_Hitable.m_Field_Info.ClearHit();
        m_TurnState = ETurnState.PLAYER_TURN_BEGIN;
        coState = StartCoroutine(CoPlayerTurn_Begin());
    }
    IEnumerator CoPlayerTurn_Begin()
    {
        m_DiceMgr.DiceFirstRoll();
        m_PlayerChar.TurnBegin();
        yield return null;
        PlayerTurn_Action();
    }


    public void PlayerTurn_Action()
    {
        m_TurnState = ETurnState.PLAYER_TURN_ACTION;
        m_Btn_TurnEnd.interactable = true;
    }

    public void PlayerTurn_BeforeEnd()
    {
        m_Btn_TurnEnd.interactable = false;

        //남긴 주사위 만큼 마나 회복
        int gainMana = 0;
        foreach (var it in m_DiceMgr.m_Dices)
        {
            Debug.Log(it.gameObject.activeSelf);
            if (it.gameObject.activeSelf == true) gainMana++;
        }
        if (gainMana > 0) m_PlayerChar.m_ManaHandler.UseMana(-gainMana);


        m_DiceMgr.ForceQuitTurn();

        m_TurnState = ETurnState.PLYAER_TURN_BEFORE_END;
        coState = StartCoroutine(CoPlayerTurn_BeforeEnd());
    }
    IEnumerator CoPlayerTurn_BeforeEnd()
    {
        //TODO 기다리는 작업 처리 어떻게 할까?
        yield return null;
        PlayerTurn_End();
    }

    public void PlayerTurn_End()
    {
        m_TurnState = ETurnState.PLAYER_TURN_END;
        coState = StartCoroutine(CoPlayerTurn_End());



        m_DiceMgr.m_State = CDiceMananger.EDiceRollState.NONE;
        m_DiceMgr.m_DiceSaveArea.ResetDiceData();
        m_DiceMgr.m_DiceChoiceArea.ResetDiceData();
        

    }
    IEnumerator CoPlayerTurn_End()
    {
        //플레이어 행동 끝났는지 체크
        while (m_PlayerChar.m_SkillUseQueue.Count > 0 || m_PlayerChar.m_CanAnim == false)
            yield return CUtility.GetSecD1To5s(0.1f);

        m_PlayerChar.TurnEnd();

        yield return CUtility.GetSecD1To5s(0.5f);
        EnemyTurn_Begin();
    }

    //==================================================//
    public void EnemyTurn_Begin()
    {
        m_TurnState = ETurnState.ENEMY_TURN_BEGIN;
        coState = StartCoroutine(CoEnemyTurn_Begin());
    }

    IEnumerator CoEnemyTurn_Begin() 
    {
        m_EnemyGroup.EnemiesBeginTurn();
        yield return null;
        EnemyTurn_Action();
    }

    public void EnemyTurn_Action()
    {
        m_TurnState = ETurnState.ENEMY_TURN_ACTION;
        m_EnemyGroup.MonsterActions();
        
        //coState = StartCoroutine(CoEnemyTurn_Action());
    }
    IEnumerator CoEnemyTurn_Action()
    {
        //m_EnemyGroup.MonsterActions();
        yield return null;
        //EnemyTurn_BeforeEnd();
    }

    public void EnemyTurn_BeforeEnd()
    {
        m_TurnState = ETurnState.ENEMY_TURN_BEFORE_END;
        EnemyTurn_End();
    }

    public void EnemyTurn_End()
    {
        m_TurnState = ETurnState.ENEMY_TURN_END;
        coState = StartCoroutine(CoEnemyTurn_End());

    }
    IEnumerator CoEnemyTurn_End()
    {
        m_EnemyGroup.EnemyTurnEnd();
        yield return null;
        PlayerTurn_Begin();
    }

    //==================================================//

    public void Setting_BattleMap()
    {
        m_CurrMap = m_BattleMaps[Random.Range(0, m_BattleMaps.Count)];
        m_CurrMap.SetActive(true);
    }



    public void EndBattle() 
    {
        m_TurnState = ETurnState.END_BATTLE;
        StopAllCoroutines();
        coState = StartCoroutine(CoEndBattle());
    }

    IEnumerator CoEndBattle()
    {
        yield return CUtility.GetSecD1To5s(1f);

        //보상창 열기 전에 몬스터들 이후 애님 재생
        if (m_Battle_Event != null)
        {
            Debug.Log("Outro");
            yield return StartCoroutine(m_Battle_Event.CoActAfterBattle());
        }

        CGameManager.Instance.m_ScheduleMgr.SetMode(CScheduleManager.EModeScheduler.REWARD);
    }
    //IEnumerator CoEndBattle()
    //{
    //    yield return CUtility.GetSecD1To5s(1f);
    //
    //    CGameManager.Instance.m_ScheduleMgr.m_VisionCtrl.Cover(_soft: true);
    //    yield return CUtility.GetSecD1To5s(1f);
    //    // 보상 먼저나와야함. 일단 다음날로 보내자
    //    EndThisScene();
    //
    //    yield return CUtility.GetSecD1To5s(1f);
    //    CGameManager.Instance.m_ScheduleMgr.MoveToNextDay();
    //}

    public void Reward() 
    {
    }

}
