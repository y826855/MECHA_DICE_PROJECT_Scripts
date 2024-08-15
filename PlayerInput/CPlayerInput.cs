using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CPlayerInput : MonoBehaviour
{
    public EventSystem m_EventSystem = null;

    Vector3 m_StartMousePos = Vector3.zero;
    Vector3 m_DragtMousePos = Vector3.zero;
    Transform dragObj = null;

    //public RectTransform m_Cursor = null;
    public float m_CursorMoveSpeed = 1f;
    //public RectTransform m_CursorLockArea = null;

    //[SerializeField] Camera m_DiceCamera = null;
    [SerializeField] LayerMask m_Pickable;

    public bool m_IsHoldCup = false;


    public CDiceHolder diceHolder = null;

    public void Awake()
    {
        if (CGameManager.Instance.m_Input == null)
            CGameManager.Instance.m_Input = this;
        else
            Destroy(this.gameObject);
    }
    private void Start()
    {
        DontDestroyOnLoad(this);
        //if(diceHolder != null)
        //    diceHolder = CGameManager.Instance.m_DiceManager.m_DiceHolder;
    }

    //Mouse Grap Cup
    public void RayCastOnCursor()
    {
        //if (m_CursorMode == true)
        {
            m_StartMousePos = diceHolder.m_DiceCam.ScreenToWorldPoint(diceHolder.m_Cursor.position);
            m_DragtMousePos = m_StartMousePos;
            var ray = diceHolder.m_DiceCam.ScreenPointToRay(diceHolder.m_Cursor.position);

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, m_Pickable, QueryTriggerInteraction.Collide) == true)
            {
                if (hit.collider.tag == "Cup")
                {
                    
                    //마우스로 컵 잡음
                    Debug.Log("CUP!");
                    diceHolder.m_DiceManager.LockAndRoll();

                    dragObj = hit.collider.transform;
                    if (coDrag == null) coDrag = StartCoroutine(CoDrag(dragObj));
                    diceHolder.m_CursorLockArea = diceHolder.m_UI_DiceRollArea;
                    m_IsHoldCup = true;
                    diceHolder.GrabCup();
                }
            }

        }
    }

    Coroutine coDrag = null;
    IEnumerator CoDrag(Transform _dragObj)
    {
        Vector3 currPos = Vector3.zero;

        while (true)
        {
            currPos = diceHolder.m_DiceCam.ScreenToWorldPoint(diceHolder.m_Cursor.position);
            _dragObj.position += currPos - m_DragtMousePos;
            m_DragtMousePos = currPos;
            yield return null;
        }
    }

    public System.Action CB_OnInteraction = null;
    public void OnInteraction(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                Debug.Log("F");
                if (CB_OnInteraction != null) CB_OnInteraction();
                else Debug.Log("Interaction NONE");
                break;
        }
    }

    //확인 버튼
    public void OnSubmit(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                break;
        }
    }

    //포커싱 중인 오브젝트에서 나가기
    //System.Action CB_OnEscape = null;

    //포커싱 중인 오브젝트에서 나가기
    List<System.Action> CB_OnEscape_Stack = new List<System.Action>();

    public void ClearEscapeStack() 
    { CB_OnEscape_Stack.Clear(); }

    public void SetEscape(System.Action  _callback) 
    {
        //if (CB_OnEscape != null) CB_OnEscape();
        //CB_OnEscape = _callback;

        if (CB_OnEscape_Stack.Count > 0)
        {//이전 스택에 쌓인 취소 함수 실행
            foreach (var it in CB_OnEscape_Stack)
                if (it != _callback) it();
            CB_OnEscape_Stack.Clear();
        }

        if (_callback == null) return;

        CB_OnEscape_Stack.Add(_callback);
    }

    public void AddEscape(System.Action _callback) 
    {
        if (CB_OnEscape_Stack.Contains(_callback) == true) return;
        CB_OnEscape_Stack.Add(_callback);
    }

    public void OnEscape(InputAction.CallbackContext _context)
    {
        if (_context.phase == InputActionPhase.Performed)
        {
            if (CB_OnEscape_Stack.Count > 0) 
            {
                Debug.Log("ESCAPE" + CB_OnEscape_Stack.Count);
                int idx = CB_OnEscape_Stack.Count - 1;
                Debug.Log(idx);
                CB_OnEscape_Stack[idx]();
                CB_OnEscape_Stack.RemoveAt(idx);
                m_EventSystem.SetSelectedGameObject(null);
            }

            //if (CB_OnEscape != null) 
            //{ 
            //    CB_OnEscape();
            //    CB_OnEscape = null;
            //    m_EventSystem.SetSelectedGameObject(null);
            //}

            //if(CGameManager.Instance.m_ScheduleMgr != null)
            //    CGameManager.Instance.m_ScheduleMgr.m_UI_ToolBar.Escape();
        }
    }

    /////SHIFT
    //public void OnHoldCup(InputAction.CallbackContext _context)
    //{
    //    switch (_context.phase)
    //    {
    //        case InputActionPhase.Performed:
    //            if (m_CurrSelectable.tag == "Cup")
    //            {
    //                Debug.Log("HOLD CUP");
    //                m_CursorLockArea = m_DiceRollArea;
    //                m_Cursor.gameObject.SetActive(true);
    //                m_Cursor.position = m_DiceCamera.WorldToScreenPoint(m_CurrSelectable.transform.position);
    //                RayCastOnCursor();

    //                m_CurrSelectable = m_Reroll;
    //                FocusToSelectable();

    //                m_DiceHolder.m_DiceManager.
    //                    m_DiceChoiceArea.m_SelectableArea.m_CanvasGroup.interactable = false;
    //                //m_IsHoldCup = true;
    //            }
    //            break;


    //        case InputActionPhase.Canceled:
    //            if (m_IsHoldCup == true)
    //            {
    //                Debug.Log("STOP HOLD");
    //                m_IsHoldCup = false;
    //                m_Cursor.gameObject.SetActive(false);
    //                ThrowDice();
    //            }
    //            break;
    //    }
    //}

    ////커서 움직임 입력
    //public void OnCursorMove(InputAction.CallbackContext _context)
    //{
    //    if (m_IsHoldCup == true)
    //    {
    //        cursorMove = _context.ReadValue<Vector2>();
    //        if (coCursorMove == null &&
    //            cursorMove != Vector2.zero) coCursorMove = StartCoroutine(CoCursorMove());
    //    }
    //}

    public void OnMouseMove(InputAction.CallbackContext _context)
    {
        if (diceHolder == null) return;

        if (diceHolder.m_Cursor.gameObject.activeSelf == false) diceHolder.m_Cursor.gameObject.SetActive(true);
        var pos = _context.ReadValue<Vector2>();

        //TODO : 해상도 바뀌면 못쓰는 코드
        pos.y -= 1080;

        diceHolder.m_Cursor.anchoredPosition = pos;
    }

    public void OnMouseLClick(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                //Debug.Log("CLICK");
                if(diceHolder != null)
                    RayCastOnCursor();
                break;
            case InputActionPhase.Canceled:
                //Debug.Log("CLICK DONE");
                if (m_IsHoldCup == true)
                {
                    ThrowDice();
                    m_IsHoldCup = false;
                }
                break;
        }
    }

    public void ThrowDice()
    {
        if (coDrag != null) StopCoroutine(coDrag);
        coDrag = null;
        diceHolder.ThrowDice();
    }

    public void OnDebug_Input(InputAction.CallbackContext _context)
    {
        switch (_context.phase)
        {
            case InputActionPhase.Performed:
                CGameManager.Instance.m_TurnManager.m_PlayerChar.TestATK();
                break;
        }
    }
}
