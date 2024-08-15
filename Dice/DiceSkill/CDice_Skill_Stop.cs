using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDice_Skill_Stop : MonoBehaviour
{
    public CDice m_Owner = null;
    public ParticleSystem m_SkillParticle = null;
    public ParticleSystem m_Skill_Inst = null;

    public float m_SkillDefaultSize = 0;
    public float m_SkillSize = 2f;
    public LayerMask m_Layer;


    public enum EStateFunc { NONE = -1, SUM = 0, SAME, }
    public EStateFunc m_CurrFuncState = EStateFunc.NONE;

    public void SetData(CScriptable_ManaSkill_Area _skill)
    {
        //파티클 생성
        m_SkillSize = _skill.m_Data.m_Range;
        
    }

    //범위 표시는 어떻게 할까?
    //일단 원형만 하자
    public float DoAction() 
    {
        Debug.Log("실행됨");

        this.transform.forward = Vector3.forward;
        
        m_SkillParticle.Clear();
        m_SkillParticle.gameObject.SetActive(true);
        m_SkillParticle.transform.localScale = Vector3.one * m_SkillSize;
        m_SkillParticle.Play();

        var mainP = m_SkillParticle.main;

        mainP.startSize = m_SkillSize;

        CheckSphereArea();
        

        return m_SkillParticle.main.duration;
    }

    public IEnumerator CoDoSkillAction() 
    {
        Debug.Log("실행됨");
        float duration = m_SkillParticle.main.duration;


        switch (m_Owner.m_ManaSkill.m_Data.m_Type) 
        {
            case CUtility.EManaSkillType.DICE_ADD:
            case CUtility.EManaSkillType.DICE_SET:
                CheckSphereArea_SAME();
                break;

            case CUtility.EManaSkillType.DEF:
            case CUtility.EManaSkillType.DMG_ALL:
            case CUtility.EManaSkillType.DMG_RANDOM:
            case CUtility.EManaSkillType.HEAL:
                CheckSphereArea_SUM();
                break;
        }

        this.transform.forward = Vector3.forward;
        m_SkillParticle.Clear();
        m_SkillParticle.gameObject.SetActive(true);
        m_SkillParticle.transform.localScale = Vector3.one * m_SkillSize;
        m_SkillParticle.Play();

        var mainP = m_SkillParticle.main;

        mainP.startSize = m_SkillSize;


        yield return CUtility.GetSecD1To5s(duration);
    }

    [SerializeField] int sum = 0;


    public void CheckSphereArea_SUM() 
    {
        float size = m_SkillSize * m_SkillDefaultSize;
        var res = Physics.OverlapSphere(this.transform.position, size, m_Layer);

        sum = 0;

        foreach (var it in res)
        {
            var dice = it.gameObject.GetComponent<CDice>();
            sum += dice.m_eye;
        }

        Debug.Log(sum);
        //TODO : UI에 수치 표기 어캐하지
        m_Owner.m_ManaSkill.DoAction(sum);
    }

    public void CheckSphereArea_SAME() 
    {
        float size = m_SkillSize * m_SkillDefaultSize;
        var res = Physics.OverlapSphere(this.transform.position, size, m_Layer);

        sum = 0;

        int eye = m_Owner.m_eye;
        int set = m_Owner.m_ManaSkill.m_Data.m_Max;
        if (set != 0) eye = set; //주사위를 지정된 숫자로 바꿀 경우

        foreach (var it in res)
        {
            var dice = it.gameObject.GetComponent<CDice>();
            dice.ChangeDiceRot_By_Eye(eye);
        }
    }


    public void CheckSphereArea() 
    {
        float size = m_SkillSize * m_SkillDefaultSize;
        var res = Physics.OverlapSphere(this.transform.position, size, m_Layer);

        sum = 0;

        foreach (var it in res) 
        {
            var dice = it.gameObject.GetComponent<CDice>();

            Debug.Log(dice.gameObject.name);
            Debug.Log(dice.m_eye);
            sum += dice.m_eye;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(this.transform.position, m_SkillDefaultSize * m_SkillSize);
    }
}
