using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CUI_Canvas_Bag : MonoBehaviour
{
    [SerializeField] List<GameObject> m_BagList = new List<GameObject>();

    public int m_Bag_Idx = 0;

    [Header("==========MANA SKILLS==========")]
    [SerializeField] CUI_Info_ManaSkill m_Info_Quest = null;
    [SerializeField] CUI_Info_ManaSkill m_Info_Area = null;
    [SerializeField] CUI_Info_ManaSkill m_Info_Use = null;

    public CSoundManager.ECustom m_Sound_Open = CSoundManager.ECustom.NONE;
    public CSoundManager.ECustom m_Sound_Close = CSoundManager.ECustom.NONE;

    public enum EBagType { NONE = -1, WEEK = 0, CARD, MANA, MANUAL, SETTING };

    public void SetManaSkill_Info() 
    {
        var player = CGameManager.Instance.m_PlayerData;
        m_Info_Quest.SetData(player.m_Quest);
        m_Info_Area.SetData(player.m_AreaSkill);
        m_Info_Use.SetData(player.m_UseSkill);
    }

    public void OpenBag(int _idx) 
    {
        if (m_Bag_Idx != _idx)
        {
            m_BagList[m_Bag_Idx].gameObject.SetActive(false);
            m_Bag_Idx = _idx;

            switch ((EBagType)m_Bag_Idx) 
            {
                case EBagType.MANA: SetManaSkill_Info(); break;
            }
        }
        OpenBag();
    }

    public void OpenBag() 
    {
        if(this.gameObject.activeSelf == false)
            this.gameObject.SetActive(true);

        CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_Sound_Open);

        if (m_BagList[m_Bag_Idx].gameObject.activeSelf == false)
        { m_BagList[m_Bag_Idx].gameObject.SetActive(true); }

        SetManaSkill_Info();

        CGameManager.Instance.m_Input.AddEscape(() => { Escape(); });
    }

    public void Escape() 
    {
        CGameManager.Instance.m_SoundMgr.PlaySoundEff(m_Sound_Close);
        this.gameObject.SetActive(false);
    }

    public void OnClick_Lobby() 
    {
        CGameManager.Instance.m_ScheduleMgr.GoTo_Lobby();
    }
    public void OnClick_Quit() 
    {
        CGameManager.Instance.m_ScheduleMgr.QuitGame();
    }
}
