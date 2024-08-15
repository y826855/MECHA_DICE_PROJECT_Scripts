using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CEnemyGroup : MonoBehaviour
{
    public CPlayerChar m_PlayerChar = null;

    public CScriptable_MonsterGroup m_MonsterGroup = null;
    public List<Transform> m_MonsterSpawnPoints = new List<Transform>();

    public List<CMonster> m_SpawnedMonsters = new List<CMonster>();
    //public CSelectable_TargetEnemy m_Selectable_Target = null;

    ///공격 하기 위한 코스트들
    public int m_MaxCost = 0;
    public int m_CurrCost = 0;

    public int m_ReadyMonster = 0;

    private void OnEnable()
    {
        //if (m_PlayerChar == null)
        //    m_PlayerChar = CGameManager.Instance.m_TurnManager.m_PlayerChar;

        foreach (var it in m_SpawnedMonsters) 
        { 
            it.m_Group = this;
            it.LookAt(m_PlayerChar.m_Body);
        }
    }

    public void ResetHitLog() 
    {
        foreach(var it in m_SpawnedMonsters)
        { it.m_Hitable.m_Field_Info.ClearHit(); }
    }

    public void SetMonsterGroup(uint _idx) 
    { 
        m_MonsterGroup = CGameManager.Instance.m_Dictionary.GetMonsterGroup_By_ID(_idx);
        m_MaxCost = m_MonsterGroup.m_Data.m_MaxCost;
        m_CurrCost = m_MonsterGroup.m_Data.m_StartCost;
    }

    public void SetMonsterGroup(int _tear) 
    { 
        m_MonsterGroup = CGameManager.Instance.m_Dictionary.GetMonsterGroup_By_Tear(_tear);
        m_MaxCost = m_MonsterGroup.m_Data.m_MaxCost;
        m_CurrCost = m_MonsterGroup.m_Data.m_StartCost;
    }

    public void SpawnEnemies() 
    {
        CGameManager.Instance.m_ScheduleMgr.m_UI_Reword.m_Discovery += m_MonsterGroup.m_Data.m_Reward_Discovery;

        var monsters = m_MonsterGroup.m_Data.m_Slots;
        var intros = CGameManager.Instance.m_TurnManager.m_Battle_Event.m_Battle_Intros;
        var outros = CGameManager.Instance.m_TurnManager.m_Battle_Event.m_Battle_Outros;

        for (int i = 0; i < monsters.Count; i++) 
        {
            var it = monsters[i];
            if (it == null) continue;

            var inst = Instantiate(it.m_Pref, m_MonsterSpawnPoints[i]);
            inst.m_Group = this;
            inst.Spawn(it.m_Data);
            inst.LookAt(m_PlayerChar.m_Body);

            m_SpawnedMonsters.Add(inst);
            //인트로 있다면 저장함
            if (inst.m_Battle_Intro != null)
            {
                if(inst.m_Battle_Intro.m_Event_Intros != null)
                    intros.Add(inst.m_Battle_Intro);
                if (inst.m_Battle_Intro.m_Event_Outros != null)
                    outros.Add(inst.m_Battle_Intro);
            }
        }
    }

    //전투 준비(UI 활성화)
    public void OnBattle() 
    {
        foreach (var it in m_SpawnedMonsters)
            it.OnBattle();
    }

    public void UseCost(int _cost)
    {
        m_CurrCost -= _cost;
        if (m_CurrCost < 0) m_CurrCost = 0;
        if (m_CurrCost > m_MaxCost) m_CurrCost = m_MaxCost;
    }

    public void EnemiesBeginTurn() 
    {
        foreach (var it in m_SpawnedMonsters)
            it.TurnBegin();
    }

    Coroutine coEnemyActon = null;
    public void MonsterActions() 
    {
        coEnemyActon = StartCoroutine(CoEnemyAction());
    }

    //죽은 몬스터 체크하고 제거함
    public void CheckDeadMonster() 
    {
        List<CMonster> deadMonsters = null;
        for (int i = 0; i < m_SpawnedMonsters.Count; i++)
        {
            var it = m_SpawnedMonsters[i];
            if (it.m_Hitable.m_IsDead == true)
            {
                if (deadMonsters == null) deadMonsters = new List<CMonster>();
                deadMonsters.Add(it);
            }
        }

        if (deadMonsters == null) return;

        foreach (var it in deadMonsters)
        { 
            //TODO : 삭제 필요
            m_SpawnedMonsters.Remove(it); 
        }
    }

    //몬스터 다 죽으면 보상 떠야함
    public void OnDieMonster(CMonster _monster)
    {
        foreach (var it in m_SpawnedMonsters) 
        {
            if (it.m_Hitable.m_IsDead == false) return;
        }

        //CGameManager.Instance.m_TurnManager.EndBattle();
        //CGameManager.Instance.m_ScheduleMgr.SetMode(CScheduleManager.EModeScheduler.REWORD);
        CGameManager.Instance.m_TurnManager.EndBattle();
    }


    IEnumerator CoEnemyAction()
    {
        m_ReadyMonster = 0;
        int idx = 0;
        foreach (var it in m_SpawnedMonsters)
        {
            if (it.m_Hitable.m_IsDead == true)
            {//죽은놈 스킵
                yield return CUtility.m_WFS_DOT2;
                continue;
            }

            it.TurnAction();
            //몬스터의 행동이 끝나면 +1
            while (idx == m_ReadyMonster) yield return null;

            idx++;
            yield return CUtility.m_WFS_DOT2;
        }

        if (m_SpawnedMonsters.Count < 3) yield return CUtility.m_WFS_DOT2;

        //적 턴 종료 페이즈 진입
        CGameManager.Instance.m_TurnManager.EnemyTurn_BeforeEnd();
        coEnemyActon = null;
    }

    //적군 턴 종료
    public void EnemyTurnEnd()
    {
        //CheckDeadMonster();

        foreach (var it in m_SpawnedMonsters)
        { it.TurnEnd(); }

        //foreach (var it in m_Monsters)
        //{ it.EndTurn(); }
        ////Debug.Log("몬스터 턴 끝남");
        //CGameManager.Instance.m_TurnManager.m_IsTurnEnd = true;
        //RemoveDeadEnemy();
        //foreach (var it in m_Monsters)
        //{ if (it.gameObject.activeSelf == true) it.ThinkNext(); }
    }


    //적군제거
    public void RemoveEnemies() 
    {
        if (coEnemyActon != null) StopCoroutine(coEnemyActon);

        for (int i = 0; i < m_SpawnedMonsters.Count; i++) 
        {
            var it = m_SpawnedMonsters[i];
            Destroy(it.gameObject);
        }

        m_SpawnedMonsters.Clear();
    }


    //TargetEnemy 여기로 합치자

    [Header("===============Targeting==================")]
    public int m_SelectCount = 0;
    public TMPro.TextMeshProUGUI m_TMP_Count = null;
    public CUI_SkillCard m_UI_SkillCard = null;
    public bool m_IsCanSelect_ForSkill = false;

    public void OnSelectMode(int _count, CUI_SkillCard _card)
    {
        CGameManager.Instance.m_Input.SetEscape(Escape);
        m_IsCanSelect_ForSkill = true;
        m_PlayerChar.m_SkillMgr.ToggleCanvasCard(false);

        m_UI_SkillCard = _card;
        m_SelectCount = _count;
        m_TMP_Count.text = m_SelectCount.ToString();
        m_TMP_Count.gameObject.SetActive(true);
    }

    //적 선택
    public void SelectedEnemy(CHitable _target)
    {
        if (m_IsCanSelect_ForSkill == false) return;

        m_SelectCount--;
        m_TMP_Count.text = m_SelectCount.ToString();

        _target.m_Field_Info.AddTarget();
        m_PlayerChar.m_SkillTargets.Add(_target);
        Debug.Log("ON TARGET!");

        if (m_SelectCount <= 0)
        { SelectAllTarget(); }
    }
    //모든 타겟 선택
    public void SelectAllTarget()
    {
        //if (m_CB_UseSkill != null) m_CB_UseSkill();
        //Close();
        m_UI_SkillCard.UseSkill();
        m_TMP_Count.gameObject.SetActive(false);
        m_PlayerChar.m_SkillMgr.ToggleCanvasCard(true);
        m_IsCanSelect_ForSkill = false;
    }

    //공격 종료
    public void AttackDone()
    {
        if (m_PlayerChar.m_IsEndAttack == false)
        {
            m_PlayerChar.m_IsEndAttack = true;
            return;
        }

        //Debug.Log("DONE");

        ClearTargetTextIcon();
        m_PlayerChar.m_Hitter.DestroySelf();
    }

    //타겟 초기화 시킴
    public void ClearTargetTextIcon()
    {
        foreach (var it in m_PlayerChar.m_SkillTargets)
            it.m_Field_Info.ClearTarget();
        m_PlayerChar.m_SkillTargets.Clear();
        m_PlayerChar.m_IsEndAttack = false;
    }

    public void Escape()
    {
        Debug.Log("ESCAPE SELECT ENEMY__");

        if (m_IsCanSelect_ForSkill == false) return;
        Debug.Log("ESCAPE SELECT ENEMY");

        m_PlayerChar.m_SkillMgr.ToggleCanvasCard(true);

        m_UI_SkillCard.OnInputEscape();
        //reset targets
        foreach (var it in m_PlayerChar.m_SkillTargets)
        { it.m_Field_Info.RemoveTarget(); }

        m_TMP_Count.gameObject.SetActive(false);
        m_PlayerChar.m_SkillTargets.Clear();
    }
}
