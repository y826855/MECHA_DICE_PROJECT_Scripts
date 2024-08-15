using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;
using DG.Tweening;

public class CDice : MonoBehaviour
{
    public int m_eye = -1;

    [Header("----------------------------")]
    public Rigidbody m_Rigid = null;
    public Vector3 m_SpawnPos = Vector3.zero;
    public CDiceMananger m_DiceManager = null;
    public Selectable m_Selectable = null;
    public Image m_DiceUI_Img = null;

    [Header("----------------------------")]
    public ParticleSystem m_Particle = null;
    [SerializeField] int maxPool = 10;
    [SerializeField] int capacity = 5;
    public float m_ForceNormal = 80f;
    public float m_Dice_EffDelay = 0.05f;


    public CDice_Skill_Stop m_Skill_Stop = null;

    public CScriptable_ManaSkill_Area m_ManaSkill = null;

    public IObjectPool<GameObject> m_Pool { get; set; }
    List<ParticleSystem> m_Pool_Particle = new List<ParticleSystem>();

    [Header("----------------------------")]
    public InnerDriveStudios.DiceCreator.DieSides m_DiceSide = null;

    [Header("----------------------------")]
    public List<Transform> m_NumUnderLoc = new List<Transform>();

    [Header("----------------------------")]
    public CSoundManager.ECustom m_SFX_Roll = CSoundManager.ECustom.S_DICE_ROLL;
    public CSoundManager.ECustom m_SFX_Click = CSoundManager.ECustom.S_DICE_GRAB;


    public enum EDiceState { NONE, DICE_CHOICE_WAIT, DICE_SAVED };
    public EDiceState m_DiceState = EDiceState.NONE;

    static public List<int> m_DiceEyeToSide = new List<int>() { 2, 5, 0, 1, 3, 4 };

    WaitForSeconds delay = null;

    [SerializeField] Material material_Inst = null;
    private void Awake()
    {
        m_Rigid = this.GetComponent<Rigidbody>();
        m_Rigid.maxAngularVelocity = 50f;
        delay = new WaitForSeconds(m_Dice_EffDelay);

        m_Pool = new ObjectPool<GameObject>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool,
                OnDestroyPoolObject, true, capacity, maxPool);

        material_Inst = this.GetComponent<Renderer>().material;
    }

    public void SetDiceEye()
    {
        m_eye = m_DiceSide.GetDieSideMatchInfo().closestMatch.values[0];
        //Debug.Log(m_eye);
    }

    public CUtility.EDice GetDiceEye() 
    {
        if (m_eye == -1) return CUtility.EDice.ANY;
        return (CUtility.EDice)m_eye;
    }

    bool bIsDelay = false;
    public void SpawnParticle(float _force)
    {
        if (m_Particle == null || bIsDelay == true) return;

        StartCoroutine(CoDelay());
        float pow = _force / m_ForceNormal;
        pow = Mathf.Clamp(pow, 0.4f, 1f);


        var poolObj = m_Pool.Get();
        poolObj.transform.position = this.transform.position;
        poolObj.transform.rotation = Quaternion.identity;
        poolObj.transform.localScale = pow * Vector3.one;

        CGameManager.Instance.m_SoundMgr.PlaySoundEff_Volume(m_SFX_Roll, pow);
    }

    IEnumerator CoDelay()
    {
        bIsDelay = true;
        yield return delay;
        bIsDelay = false;
    }
    private void OnDisable()
    {
        m_DiceState = EDiceState.NONE;
        m_DiceUI_Img.color = Color.white;
        StopAllCoroutines();
    }

    //정렬용 함수
    public int CompareTo(CDice _get)
    {
        if (this.gameObject.activeSelf == false) return 1;
        if (_get.gameObject.activeSelf == false) return -1;


        if (m_eye < _get.m_eye)
            return -1;
        else
            return 1;
    }

    private GameObject CreatePooledItem()
    {
        GameObject poolGo = Instantiate(m_Particle, this.transform.parent).gameObject;
        poolGo.GetComponent<CParticleAutoDestroy>().Pool = m_Pool;
        //Debug.Log("create");

        return poolGo;
    }

    // 사용
    private void OnTakeFromPool(GameObject poolGo)
    {
        poolGo.SetActive(true);
    }

    // 반환
    private void OnReturnedToPool(GameObject poolGo)
    {
        poolGo.SetActive(false);
    }

    // 삭제
    private void OnDestroyPoolObject(GameObject poolGo)
    {
        Destroy(poolGo);
    }

    //////////////////////
    ///

    public CanvasGroup m_CanvasGruop = null;

    public void ToggleUIMode(bool _toggle) 
    {
        m_CanvasGruop.gameObject.SetActive(_toggle);
        m_CanvasGruop.interactable = _toggle;
        m_CanvasGruop.transform.forward = Vector3.down;
    }

    public void OnClickEvent() 
    {
        //switch (m_DiceState)
        //{
        //    case EDiceState.NONE:
        //        m_DiceState = EDiceState.DICE_SAVED;
        //        break;
        //    case EDiceState.DICE_CHOICE_WAIT:
        //        m_DiceState = EDiceState.DICE_SAVED;
        //        break;
        //    case EDiceState.DICE_SAVED:
        //        m_DiceState = EDiceState.DICE_CHOICE_WAIT;
        //        break;
        //}

        m_DiceManager.OnClick_Dice(this);
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_SFX_Click);
    }

    public void ResetManaSkill() 
    {
        if (m_ManaSkill == null) return;
        m_ManaSkill = null;
        material_Inst.color = Color.white;
        Debug.Log("reset dice skill");
    }

    public void GiveManaSkill(CScriptable_ManaSkill_Area _skill) 
    { 
        m_ManaSkill = Instantiate(_skill);
        material_Inst.color = m_ManaSkill.m_DiceColor;
        m_Skill_Stop.SetData(_skill);
    }

    public void UseDice() 
    {
        if (m_ManaSkill == null) return;
        //m_ManaSkill.ActiveManaSkill();
    }


    [SerializeField] float moveDuration = 0.5f;

    //주사위를 표기하는 곳으로 옮기기. 옮기며 회전도 보기좋게 맞춤
    public IEnumerator CoMoveToDisplay(Vector3 _targetPos, bool _toggleUIMode = false) 
    {
        Vector3 beforePos = this.transform.position;
        var beforeRot = this.transform.rotation;
        var targetRot = m_NumUnderLoc[m_DiceSide.GetDieSideMatchInfo().idx].localRotation;

        m_CanvasGruop.interactable = false;

        float t = 1f;
        while (t > 0f) 
        {
            t -= Time.deltaTime / moveDuration;
            this.transform.position = Vector3.Lerp(_targetPos, beforePos, t);
            this.transform.rotation = Quaternion.Lerp(targetRot, beforeRot, t);
            yield return null;
        }

        this.transform.position = _targetPos;
        this.transform.rotation = targetRot;

        m_CanvasGruop.interactable = true;
        ToggleUIMode(_toggleUIMode);
    }

    //주사위 눈 강제로 변경
    public void ChangeDiceRot_By_Eye(int _eye) 
    {
        m_eye = _eye;
        int side = CDice.m_DiceEyeToSide[m_eye - 1];
        var targetRot = m_NumUnderLoc[side].localRotation;
        this.transform.rotation = targetRot;
        
        m_DiceSide.GetDieSideMatchInfo();
        m_CanvasGruop.transform.forward = Vector3.down;
    }

}
