using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayerInput_Mouse : MonoBehaviour
{
    [SerializeField] Camera m_DiceCamera = null;
    [SerializeField] LayerMask m_Pickable;

    Vector3 m_StartMousePos = Vector3.zero;
    Vector3 m_DragtMousePos = Vector3.zero;

    bool isMouseDown = false;
    Transform dragObj = null;

    public CDiceHolder m_DiceHolder = null;

    // Update is called once per frame
    //void Update()
    public void Update_OLD() 
    {
        //if (Input.GetKeyDown(KeyCode.F))
        //{
        //    //CGameManager.Instance.m_SoundMgr.PlaySoundEff(CSoundManager.EBATTLE.SHIELDBREAK);
        //}

        //if (Input.GetMouseButtonDown(0) == true)
        //{
        //    isMouseDown = true;

        //    m_StartMousePos = m_DiceCamera.ScreenToWorldPoint(Input.mousePosition);
        //    m_DragtMousePos = m_StartMousePos;
        //    var ray = m_DiceCamera.ScreenPointToRay(Input.mousePosition);


        //    RaycastHit hit;
        //    if (Physics.Raycast(ray, out hit, 100f, m_Pickable,
        //        QueryTriggerInteraction.Collide) == true)
        //    {
        //        switch (hit.collider.tag)
        //        {
        //            case "Dice": //주사위 집음
        //                break;
        //            case "Cup": //컵 집음
        //                dragObj = hit.collider.transform;
        //                Debug.Log("HELLO");
        //                GrabCup();
        //                break;
        //        }
        //    }
        //}

        ////드래그해서 움직임
        //if (Input.GetMouseButton(0) && isMouseDown == true)
        //{
        //    if (dragObj != null)
        //    {
        //        Vector3 currPos = Vector3.zero;

        //        switch (dragObj.tag)
        //        {
        //            case "Dice":
        //                currPos = Input.mousePosition;
        //                break;
        //            case "Cup":
        //                currPos = m_DiceCamera.ScreenToWorldPoint(Input.mousePosition);
        //                break;
        //        }

        //        dragObj.position += currPos - m_DragtMousePos;
        //        m_DragtMousePos = currPos;
        //    }
        //}

        //if (Input.GetMouseButtonUp(0) == true)
        //{
        //    if (dragObj != null)
        //    {
        //        switch (dragObj.tag)
        //        {
        //            case "Dice": break;
        //            case "Cup": ThrowDice(); break;
        //        }
        //    }
        //    dragObj = null;
        //}
    }

    public void GrabCup()
    {
        m_DiceHolder.GrabCup();
    }

    public void ThrowDice()
    {
        m_DiceHolder.ThrowDice();
    }
}
