using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CUI_Event_Day : MonoBehaviour
{
    public CScriptable_SceneInfo m_SceneInfo = null;
    [Header("==========================")]

    public Image m_Icon = null;
    public Image m_Frame = null;
    public TextMeshProUGUI m_TMP_TypeName;

    [Header("==========================")]
    public int m_ID = -1;
    public bool m_IsInQueue = false;

    [Header("==========================")]
    public CUI_Edit_Week m_EditWeek = null;
    public System.Action m_CB_OnClick= null;

    [SerializeField] CUI_Day_Holder m_Holder = null;

    public void Spawn(CScriptable_SceneInfo _info, CUI_Edit_Week _editWeek,
        CUI_Day_Holder _holder) 
    {
        m_SceneInfo = _info;
        m_EditWeek = _editWeek;

        //에디터가 아니면 활성화 안시킴
        if (_editWeek == null) m_Frame.raycastTarget = false;

        m_Holder = _holder;
        m_Icon.sprite = _info.m_SP_Icon;
        m_TMP_TypeName.text = _info.m_Data.m_Name;
    }

    public void SetData(CScriptable_SceneInfo _info, int _id) 
    {
        m_SceneInfo = _info;
        m_Icon.sprite = _info.m_SP_Icon;
        m_TMP_TypeName.text = _info.m_Data.m_Name;
        m_ID = _id;
    }

    public void ResetPos() 
    {
        this.transform.SetParent(m_Holder.transform);
        this.transform.localPosition = Vector3.zero;
    }

    public void OnClick_Event() 
    {
        if (m_IsInQueue == false) m_EditWeek.AddToWeek(this);
        else 
        { m_EditWeek.ReturnToBag(this); }
            //m_EditWeek.ReturnToBag(this);
        m_Holder.OnClick_SwapToWeek();


        CGameManager.Instance.m_SoundMgr.PlaySoundEff
            (CSoundManager.ECustom.S_ScheduleCardSelect);
    }

    public void RemoveBySubmit(CUI_Day_Holder _holder) 
    {
        m_Holder.m_UI_EventDay = null;
        _holder.m_UI_EventDay = this;

        if (m_Holder.m_IsBuyable == true)
        {
            var bag = CGameManager.Instance.m_PlayerData.m_DaysBag;
            bag.Remove(m_SceneInfo);
        }

        this.m_Holder = _holder;
    }
}
