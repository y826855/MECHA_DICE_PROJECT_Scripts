using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CHitable))]
public class CMonster : CTurnChar
{
    public CEnemyGroup m_Group = null;

    public CUI_Field_Target m_UI_Target = null;

    public CUI_ReadySkill m_UI_ReadySkill = null;

    public CBattle_Act m_Battle_Intro = null;
    [Header("================================")]
    //public List<CUtility.CMonsterSkill> m_UseCostSkill = new List<CUtility.CMonsterSkill>();
    //public List<CUtility.CMonsterSkill> m_GainCostSkill = new List<CUtility.CMonsterSkill>();
    //public List<CUtility.CMonsterSkill> m_SpecialSkill = new List<CUtility.CMonsterSkill>();

    public List<CScriptable_MonsterSkill> m_UseCostSkills = new List<CScriptable_MonsterSkill>();
    public List<CScriptable_MonsterSkill> m_GainCostSkills = new List<CScriptable_MonsterSkill>();
    public List<CScriptable_MonsterSkill> m_SpecialSkills = new List<CScriptable_MonsterSkill>();
    public CScriptable_MonsterSkill m_ReadySpecialSkill = null;
    public CScriptable_MonsterSkill m_ReadySkill = null;

    [SerializeField] protected CHit_Obj m_Hitter = null;

    [Header("================================")]
    public Transform m_Atk_SpawnLoc = null;
    public Transform m_ShoutOutLoc = null;
    public CSoundManager.ECustom m_SoundDie = CSoundManager.ECustom.NONE;

    public virtual void Start()
    {
        Debug.Log("START");

        m_UI_Target.m_Owner = this;
        m_Hitable.m_Owner = this;
        m_Hitable.m_Hit_CB = OnHit;
        m_Hitable.m_Dead_CB = OnDie;
        m_Hitable.m_IsEnemy = true;

        //CGameManager.Instance.m_TurnManager.m_EnemyGroup.
        //    m_Selectable_Target.SpawnEnemy(this);


        //��ų�� �ʱ�ȭ ��Ŵ
        //for (int i = 0; i < m_SpecialSkills.Count; i++) 
        //{
        //    m_SpecialSkills[i] = Instantiate(m_SpecialSkills[i]);
        //    m_SpecialSkills[i].Spawn(m_Hitable);
        //}
        //for (int i = 0; i < m_GainCostSkills.Count; i++)
        //{
        //    m_GainCostSkills[i] = Instantiate(m_GainCostSkills[i]);
        //    m_GainCostSkills[i].Spawn(m_Hitable);
        //}
        //for (int i = 0; i < m_UseCostSkills.Count; i++)
        //{
        //    m_UseCostSkills[i] = Instantiate(m_UseCostSkills[i]);
        //    m_UseCostSkills[i].Spawn(m_Hitable);
        //}

        //����� ��ų ����
        if (m_SpecialSkills.Count > 0)
        {
            m_ReadySpecialSkill = m_SpecialSkills[0];
            m_UI_ReadySkill.SetFrameDivider(m_ReadySpecialSkill.m_Data.m_SpecialCost);
            m_UI_ReadySkill.m_CB_ChangeToSpecial = ChangeToSpecial;
        }

        m_ReadySkill = m_UseCostSkills[0];

        ThinkNext();
    }


    public void Spawn(CUtility.CMonster _data) 
    {
        Debug.Log("SPAWN");
        Debug.Log(_data.m_Skill_Atk.Count);

        m_Hitable.m_MaxHP = _data.m_HP;

        //CGameManager.Instance.m_TurnManager.m_EnemyGroup.
        //    m_Selectable_Target.SpawnEnemy(this);

        m_UseCostSkills.Clear();
        m_SpecialSkills.Clear();
        m_GainCostSkills.Clear();

        foreach (var it in _data.m_Skill_Atk) 
        {
            var inst = Instantiate(it);
            inst.Spawn(m_Hitable);
            m_UseCostSkills.Add(inst);
        }
        foreach (var it in _data.m_Skill_Special)
        {
            var inst = Instantiate(it);
            inst.Spawn(m_Hitable);
            m_SpecialSkills.Add(inst);
        }
        foreach (var it in _data.m_Skill_Def)
        {
            var inst = Instantiate(it);
            inst.Spawn(m_Hitable);
            m_GainCostSkills.Add(inst);
        }

        //����� ��ų ����
        if (m_SpecialSkills.Count > 0)
        {
            m_ReadySpecialSkill = m_SpecialSkills[0];
            m_UI_ReadySkill.SetFrameDivider(m_ReadySpecialSkill.m_Data.m_SpecialCost);
            m_UI_ReadySkill.m_CB_ChangeToSpecial = ChangeToSpecial;
        }

        m_ReadySkill = m_UseCostSkills[0];

        ThinkNext();
    }

    public override void Spawn() 
    {
        
    }

    public override void OnBattle()
    {
        m_Hitable.OnBattle();
    }

    public override void TurnBegin()
    {
        m_Hitable.Begin_Turn();
    }

    public override void TurnEnd()
    {
        if (m_Hitable.m_IsDead == true) return;

        //������ �ϰ� ���� ��ų ��������
        m_Hitable.Calc_TurnEnd();
        if(m_UI_ReadySkill.m_IsGaugeFull == false)
            ThinkNext();
        //������ ����� ������ ���������� ����� ��ų
    }

    public override void TurnAction() 
    {
        SpawnHitter();
    }

    //�ǰݽ� ������ ��
    public void OnHit() 
    {
        Debug.Log("ONHIT_AND ADD GAUGE");
        m_UI_ReadySkill.AddFrameCount();
    }

    //���� ������Ʈ ����
    public void SpawnHitter()
    {
        m_IsEndAttack = true;
        m_Hitter = Instantiate(m_ReadySkill.m_Atk_Info.m_HitObj);
        int count = m_ReadySkill.m_Data.m_Count;

        List<CHitable> hitables = new List<CHitable>();

        CHitable hitable = null;
        if (m_ReadySkill.m_Data.m_Type == CUtility.ECardType.DEF ||
            m_ReadySkill.m_Data.m_Type == CUtility.ECardType.DEF_BURN ||
            m_ReadySkill.m_Data.m_Type == CUtility.ECardType.DEF_ELEC ||
            m_ReadySkill.m_Data.m_Type == CUtility.ECardType.DEF_ROCK) 
        { hitable = m_Hitable; } //����� ����
        else //�����̸� �÷��̾� ���
        { hitable = m_Group.m_PlayerChar.m_Hitable; }

        for (int i = 0; i < count; i++)
        { hitables.Add(hitable); }

        m_Hitter.Spawn(
            m_Atk_SpawnLoc,
            this,
            hitables,
            m_ReadySkill.m_Atk_Info);

        m_Hitter.GenSkill(m_ReadySkill.m_Data.m_GenType, count);
        //m_Hitter.CheckMoreAttack();
    }

    //������ų ���� �Լ�
    public void ThinkNext() 
    {
        //Debug.Log("THINK!");
        
        //�ڽ�Ʈ ����ؼ� ��� ������ ��ų ��������
        int cost = m_Group.m_CurrCost;

        //CUtility.CMonsterSkill use = null;

        //m_NextSkill = null;
        CScriptable_MonsterSkill randSkill = null;
        List<int> rand_In_Atk = new List<int>();

        //��� ������ ���ݽ�ų ã��
        for (int i = 0; i < m_UseCostSkills.Count; i++)
        {
            if (m_UseCostSkills[i].m_Data.m_Cost <= cost)
            {
                rand_In_Atk.Add(i);
                //Debug.Log(i);
            }
        }

        //��� ���� ���� ��ų �߿��� ����
        if (rand_In_Atk.Count > 0)
        {
            randSkill = m_UseCostSkills[rand_In_Atk[Random.Range(0, rand_In_Atk.Count)]];
            //���� ��ų�� ������ ���� ��� ��ų������
            if (randSkill == m_ReadySkill) randSkill = null;
        }

        if (randSkill == null)
        {
            //���� ��� ���ϸ� �ڽ�Ʈ ȸ�� ��ų �����
            randSkill = m_GainCostSkills[Random.Range(0, m_GainCostSkills.Count)];
        }

        m_ReadySkill = randSkill;

        m_Group.UseCost(m_ReadySkill.m_Data.m_Cost);
        //m_Group.m_CurrCost -= m_NextSkill.m_Cost;
        //if (m_Group.m_MaxCost < m_Group.m_CurrCost) m_Group.m_CurrCost = m_Group.m_MaxCost;

        rand_In_Atk.Clear();
        ShowWaitAction();
    }

    public void ShowWaitAction() 
    {
        m_UI_ReadySkill.SetData(m_ReadySkill);
    }

    //��ų�� ����ȷ� �ٲ�
    public void ChangeToSpecial() 
    {
        Debug.Log("��ų ����!");

        if (m_ReadySkill == m_ReadySpecialSkill) return;
        m_ReadySkill = m_ReadySpecialSkill;
        ShowWaitAction();
    }

    //�ִϸ��̼� ��ų
    public override void Attack()
    {
        //Debug.Log("ATK");
        if (m_Hitter == null) return;
        m_Hitter.Anim_Attack();
    }

    //�ִϸ��̼� ����
    public override void EndAttack()
    {
        if (m_Hitter == null) return;

        if (m_Hitter.m_Info.m_GenType == CUtility.EATK_GenType.Combo)
            m_Hitter.CheckMoreAttack();
        else
        {
            m_Anim.SetBool("EndCombo", true);
            DoneAnim();
        }
    }

    //��� ����
    public override void DoneAnim()
    {
        //Debug.Log("END ANIM");
        ActionDone();
    }

    //��ų ����
    public override void DoneSkill()
    {
        //Debug.Log("END SKILL");
        ActionDone();
    }

    //�ൿ ���� //��ų, ��� �Ѵ� ����Ǿ����
    public void ActionDone() 
    {
        if (m_IsEndAttack == false)
        {
            m_IsEndAttack = true;
            return;
        }

        m_Hitter.DestroySelf();
        m_Hitter = null;
        m_Group.m_ReadyMonster++;

        //����� ��ų ���
        if (m_UI_ReadySkill.m_IsGaugeFull == true)
        { NextSpecialSkill(); }
        else
        { m_UI_ReadySkill.AddFrameCount(-m_ReadySkill.m_Data.m_SpecialCost); }
    }

    //����� ��ų ��� �� �ٸ���ų�� �ٲ�
    public void NextSpecialSkill() 
    {
        //������� �ϳ��� ������ ������0���� �ٲٰ� ��ȯ
        m_UI_ReadySkill.m_IsGaugeFull = false;
        m_UI_ReadySkill.SetFrameCount(0);
        if (m_SpecialSkills.Count == 0)
        { return; }

        int idx = m_SpecialSkills.IndexOf(m_ReadySpecialSkill);
        if (++idx < m_SpecialSkills.Count)
        { m_ReadySpecialSkill = m_SpecialSkills[idx]; }
        else m_ReadySpecialSkill = m_SpecialSkills[0];

        m_UI_ReadySkill.SetFrameDivider(m_ReadySpecialSkill.m_Data.m_SpecialCost);
    }


    //��ų �̸� ��ġ��
    public void ShoutOut() 
    {
        if (m_ReadySkill.m_Data.m_ShoutOutDesc == "") return;

        Debug.Log("SHOUT OUT");

        var mgr = CGameManager.Instance.m_TurnManager;
        var pref = mgr.m_Pref_ShoutOut;
        var parent = mgr.m_LogPool.transform;

        var inst = Instantiate(pref, parent);
        inst.transform.position = m_ShoutOutLoc.position;
        inst.SetText(m_ReadySkill.m_Data.m_ShoutOutDesc);
    }

    public override void OnDie()
    {
        m_Anim.SetTrigger("Dead");
        m_UI_Target.SetDisable();
        //CGameManager.Instance.m_TurnManager.m_EnemyGroup.
        //    m_Selectable_Target.RemoveEnemy(this);

        CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_SoundDie);
    }

    public void Anim_Die() 
    {
        Debug.Log("DIE");
        m_Hitable.m_Field_Info.gameObject.SetActive(false);
        m_Group.OnDieMonster(this);
    }

}
