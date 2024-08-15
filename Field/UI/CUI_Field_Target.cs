using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CUI_Field_Target : MonoBehaviour
    , IPointerEnterHandler, IPointerExitHandler,
    ISelectHandler, IDeselectHandler
{
    public CMonster m_Owner = null;

    public Image m_Selection = null;
    public Image m_Cursor_Img = null;

    public Color m_NormalColor = Color.white;
    public Color m_HoverColor = Color.white;
    public Color m_TargetColor = Color.white;

    public Selectable m_Selectable = null;

    CPlayerChar m_Player = null;

    public CUI_Field_Info m_Field_Info = null;
    

    public void Start()
    {
        this.transform.parent.transform.forward = -Vector3.up;

        if (CGameManager.Instance.m_TurnManager == null) return;
        m_Player = CGameManager.Instance.m_TurnManager.m_PlayerChar;
    }

    public void SetDisable() 
    {
        Untarget();
        this.gameObject.SetActive(false);
    }

    void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
    {
        if (m_Player.m_TargetEnemy != m_Owner)
        { m_Selection.color = m_HoverColor; }
        else m_Selection.color = m_TargetColor;

        m_Cursor_Img.gameObject.SetActive(true);
    }

    void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
    {
        if (m_Player.m_TargetEnemy != m_Owner)
        { m_Selection.color = m_NormalColor; }
        else m_Selection.color = m_TargetColor;

        m_Cursor_Img.gameObject.SetActive(false);
    }

    void ISelectHandler.OnSelect(BaseEventData eventData)
    {
        Debug.Log("SELECT");

        if (m_Player.m_TargetEnemy != m_Owner)
        { m_Selection.color = m_HoverColor; }
        else m_Selection.color = m_TargetColor;

        m_Cursor_Img.gameObject.SetActive(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        Debug.Log("UN SELECT");

        if (m_Player.m_TargetEnemy != m_Owner)
        { m_Selection.color = m_NormalColor; }
        else m_Selection.color = m_TargetColor;

        m_Cursor_Img.gameObject.SetActive(false);
    }

    public void Untarget() 
    {
        m_Selection.color = m_NormalColor;
        m_Cursor_Img.gameObject.SetActive(false);
    }

    public void OnTarget() 
    {
        Debug.Log(this.gameObject.name);

        var group = CGameManager.Instance.m_TurnManager.m_EnemyGroup;
        //group.m_Selectable_Target.SelectedEnemy(m_Owner.m_Hitable);
        group.SelectedEnemy(m_Owner.m_Hitable);


        if (m_Player.m_TargetEnemy == m_Owner) return;
        if (m_Player.m_TargetEnemy != null) 
        { m_Player.m_TargetEnemy.m_UI_Target.Untarget(); }
        m_Player.m_TargetEnemy = m_Owner;
    }
}
