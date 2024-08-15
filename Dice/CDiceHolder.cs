using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CDiceHolder : MonoBehaviour
{
    public Camera m_DiceCam = null;
    public Vector2 m_AreaSize = new Vector2(5, 5);
    public float m_Ceil = 8f;

    public Transform m_DiceRollArea = null;
    public RectTransform m_UI_DiceRollArea = null;

    public RectTransform m_Cursor = null;
    public RectTransform m_CursorLockArea = null;

    public CDiceMananger m_DiceManager = null;

    [SerializeField] float randMin = 0.5f;

    public List<CDice> m_Dices = new List<CDice>();

    //public CSelectableArea_New m_CupArea = null;
    public GameObject m_CupArea = null;
    public Transform m_Cup = null;
    public Vector3 m_Cup_DefaultPos = Vector3.zero;

    [Header("---------DICE MOVE---------")]
    public float m_DiceScale = 1.05f;
    public float m_ThrowPower = 5f;
    public float m_ThrowPower_Upper = 5f;
    public float m_TorquePower = 5f;
    public float m_Damp = 0.9f;
    public float m_StopMagnitude = 1;
    public float m_MaxWaitTime = 1f;

    [Header("-------------------")]
    public float m_Eff_RSP = 10f; //require spawn power;

    //public CYacht_Pad Yacht_Pad = null;
    //public CUI_DiceHandler m_DiceHandler = null;
    public bool m_IsKeeping = false;

    [Header("-------------------")]
    public CSoundManager.ECustom m_CupGrabSound = CSoundManager.ECustom.S_DICE_GRAB;


    private void Awake()
    {
        //화면비 조절됬을때 카메라 비율
        float aspect = (float)Screen.width / Screen.height;

        //var rect = m_DiceCam.rect;
        //rect.height = aspect / 3.5555555555555555555f;
        //m_DiceCam.rect = rect;
    }



    private void Start()
    {
        CGameManager.Instance.m_Input.diceHolder = this;

        //스케일 맞춰줌
        //m_AreaSize.x = m_RollArea.localScale.x * m_AreaSize.x - m_DiceSize;
        //m_AreaSize.y = m_RollArea.localScale.z * m_AreaSize.y - m_DiceSize;
        //InverseTransformPoint에서 적용되기 떄문에 스케일 적용 불필요
        m_AreaSize.x = m_AreaSize.x - m_DiceScale;
        m_AreaSize.y = m_AreaSize.y - m_DiceScale;

        m_Cup_DefaultPos = m_Cup.position;

        //m_Dices.Clear();
        //foreach (Transform it in this.transform) 
        //{
        //    var dice = it.GetComponent<CDice>();
        //    dice.m_Rigid.isKinematic = true;
        //    dice.m_SpawnPos = dice.transform.position;
        //    m_Dices.Add(dice);
        //}
    }

    private void OnDisable()
    { m_Cup.gameObject.SetActive(false); }


    public IEnumerator CoSetReadyDiceRoll(List<CDice> _dices)
    {
        //m_Dices.Clear();
        //m_CupArea.Open_And_FocusIn();
        m_CupArea.gameObject.SetActive(true);

        m_Dices = _dices;
        foreach (var it in m_Dices)
        {
            it.ToggleUIMode(false);
            StartCoroutine(it.CoMoveToDisplay(m_Cup.position));
        }

        yield return CUtility.m_WFS_DOT2;

        m_Cup.gameObject.SetActive(true);
    }


    //주사위 활성화
    public void ActiveDice(int _num)
    {
        for (int i = 0; i < _num; i++)
        { m_Dices[i].gameObject.SetActive(true); }
        m_Cup.gameObject.SetActive(true);
        ResetDice();
    }

    public void ActiveDice(List<CDice> _dices)
    {
        foreach (var it in _dices)
        { it.gameObject.SetActive(true); }
        m_Cup.gameObject.SetActive(true);
        ResetDice();
    }


    //주사위 굴리는거 지루할까봐 만듦
    [SerializeField] float SkipAcc = 10f;
    [SerializeField] GameObject m_Btn_Skip = null;
    public void SkipRoll() 
    {
        foreach (var it in m_Dices)
        {
            //it.m_Rigid.AddForce(Vector3.down * SkillAcc);

            var pos = it.transform.position; pos.y = -75f;
            it.m_Rigid.MovePosition(pos);

            it.m_Rigid.velocity *= SkipAcc/3f;
            it.m_Rigid.mass = SkipAcc;
            it.m_Rigid.drag = SkipAcc;
            it.m_Rigid.angularDrag = SkipAcc;
        }

    }

    public void DiceRigidReset() 
    {
        foreach (var it in m_Dices)
        {
            it.m_Rigid.mass = 1;
            it.m_Rigid.drag = 0;
            it.m_Rigid.angularDrag = 0.05f;
        }
    }


    //영역 나가는지 체크
    //public void CheckIsOut(Rigidbody _dice) 
    public void CheckIsOut(CDice _dice) 
    {
        Rigidbody rigid = _dice.m_Rigid;
        Vector3 pos = m_DiceRollArea.InverseTransformPoint(rigid.position) + (rigid.velocity * Time.deltaTime);
        Vector3 changeVelocity = rigid.velocity;

        bool isHitWall = false;

        //벽체크
        if (-m_AreaSize.x > pos.x) 
        { changeVelocity.x = Mathf.Abs(changeVelocity.x); pos.x = -m_AreaSize.x; isHitWall = true; }
        else if (m_AreaSize.x < pos.x) 
        { changeVelocity.x = -Mathf.Abs(changeVelocity.x); pos.x = m_AreaSize.x; isHitWall = true; }

        if (-m_AreaSize.y > pos.z) 
        { changeVelocity.z = Mathf.Abs(changeVelocity.z); pos.z = -m_AreaSize.y; isHitWall = true; }
        else if (m_AreaSize.y < pos.z) 
        { changeVelocity.z = -Mathf.Abs(changeVelocity.z); pos.z = m_AreaSize.y; isHitWall = true; }

        //천장체크
        if (pos.y > m_Ceil) 
        {
            changeVelocity.y = -changeVelocity.y;
            pos.y = m_Ceil;
            isHitWall = true;
        }//바닥체크
        else if (pos.y < 0)
        {
            changeVelocity.y = -changeVelocity.y;
            pos.y = 0;
            Debug.Log("OUT");
        }

        if (isHitWall == true) 
        {//벽 부딛히면 감속
            float y = changeVelocity.y;
            changeVelocity *= m_Damp;
            changeVelocity.y = y;

            if (m_Eff_RSP <= changeVelocity.sqrMagnitude) _dice.SpawnParticle(changeVelocity.sqrMagnitude);
        }

        //위치 월드로 바꿈
        pos = m_DiceRollArea.TransformPoint(pos);
        _dice.transform.position = pos;
        rigid.velocity = changeVelocity;
    }


    //원안에 있게함
    public void CheckInCup(Rigidbody _dice) 
    {
        Vector3 pos = m_Cup.InverseTransformPoint(_dice.position) + (_dice.velocity * Time.deltaTime);
        Vector3 changeVelocity = _dice.velocity;

        if (pos.magnitude > 0.5f) 
        {
            pos = pos.normalized * 0.5f;
            changeVelocity = -changeVelocity * m_Damp;
        }

        pos = m_Cup.TransformPoint(pos);
        _dice.transform.position = pos;
        _dice.velocity = changeVelocity;
    }


    Coroutine coDiceRoll = null;
    bool isCupHold = true;

    Vector3 cupLastMove = Vector3.right;
    IEnumerator CoDiceRoll() 
    {

        Vector3 cupPrevPos = m_Cup.transform.position;
        cupLastMove = Vector3.right;
        yield return null;


        while (true) 
        {//컵에서 충돌체크
            foreach (var it in m_Dices)
            { CheckInCup(it.m_Rigid); }

            //컵의 이전 움직임 계산
            Vector3 cupCurrMove = m_Cup.transform.position;
            cupLastMove = cupCurrMove - cupPrevPos;
            cupPrevPos = cupCurrMove;

            if (isCupHold == false) break;
            //if (m_Dices[0].m_Rigid.velocity.sqrMagnitude > 0) break;
            yield return null;
        }
        m_Cup.gameObject.SetActive(false);
        //주사위 판 충돌체크
        Debug.Log("DICE ROLL");
        m_Btn_Skip.SetActive(true);

        float maxTime = m_MaxWaitTime;
        float stopTime = 1f;
        while (true)
        {
            maxTime -= Time.deltaTime;

            bool isStop = true;
            foreach (var it in m_Dices)
            {
                CheckIsOut(it);
                //CheckIsOut(it.m_Rigid);
                if (it.m_Rigid.velocity.sqrMagnitude >
                    m_StopMagnitude * m_StopMagnitude)
                { stopTime = 1f; isStop = false; } //하나라도 덜멈추면 계속 체크
            }

            //최대 시간까지 안멈추면 강제 멈춤
            if (maxTime <= 0) 
            { stopTime = 0; isStop = true; }

            //대기 시간동안 충분히 멈췄나 체크
            if (stopTime > 0) 
            { 
                stopTime -= Time.deltaTime;
                yield return null; continue;
            }

            if (isStop == true)
            {
                DiceRigidReset();
                foreach (var it in m_Dices)
                {
                    if (it.gameObject.activeSelf == false) continue;
                    it.m_Rigid.isKinematic = true;
                    it.SetDiceEye();
                }
                Debug.Log("STOP");

                m_Dices.Sort((a,b) => { return a.CompareTo(b); });

                m_DiceManager.DiceIsStopped();
                yield return 1f;

                break;
            }
            yield return null;
        }

        //굴린 주사위 모두 비활성화 시킴
        //foreach (var it in m_Dices) it.gameObject.SetActive(false);

        m_Btn_Skip.SetActive(false);
        yield return null;
    }


    //주사위 초기화
    public void ResetDice() 
    {
        if (coDiceRoll != null) StopCoroutine(coDiceRoll);
        coDiceRoll = null;

        for (int i = 0; i < m_Dices.Count; i++) 
        {
            m_Dices[i].m_Rigid.isKinematic = true;
            m_Dices[i].m_Rigid.velocity = Vector3.zero;
            m_Dices[i].transform.rotation = Quaternion.identity;
            //m_Dices[i].transform.position = m_Dices[i].m_SpawnPos;
            m_Dices[i].transform.localPosition = Vector3.zero;
        }
    }

    //컵 잡음
    public void GrabCup() 
    {
        RollDiceInCup(m_ThrowPower);
        isCupHold = true;
        coDiceRoll = StartCoroutine(CoDiceRoll());

        CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_CupGrabSound);
    }

    //던지기
    public void ThrowDice() 
    {
        //던질때 주사위에게 힘 더줌
        float s = m_ThrowPower;
        //Vector3 power = new Vector3(
        //        Random.Range(-s, s),
        //        Random.Range(-s, s),
        //        Random.Range(-s, s));

        Vector3 power = cupLastMove.normalized;

        power *= m_ThrowPower;
        power.y = m_ThrowPower_Upper;

        foreach (var it in m_Dices)
        {
            it.m_Rigid.AddForce(power, ForceMode.Impulse);
            it.m_Rigid.AddTorque(Vector3.right * m_TorquePower, ForceMode.Impulse);
        }

        isCupHold = false;
        m_Cup.position = m_Cup_DefaultPos;
    }

    //주사위 굴리기
    public void RollDiceInCup(float Power) 
    {
        foreach (var it in m_Dices) 
        {
            it.m_Rigid.isKinematic = false;
            Vector3 power = new Vector3(
                Random.Range(randMin, 1f), 
                Random.Range(-randMin, randMin), 
                Random.Range(0.2f, 0.2f));
            it.m_Rigid.AddForce(power * m_ThrowPower, ForceMode.Impulse);
            it.m_Rigid.AddTorque(power * m_TorquePower, ForceMode.Impulse);
        }
    }


    public void ForceStop() 
    {
        StopAllCoroutines();
        m_Cup.gameObject.SetActive(false);
        foreach (var it in m_Dices)
        {
            it.m_Rigid.isKinematic = true;
            it.gameObject.SetActive(false);
        }
    }
}
