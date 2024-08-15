using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/QUEST")]
public class CScriptable_Quest : CScriptable_CSVData<CScriptable_Quest>
{
#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/Quest/";
    static public string m_AreabPath = "Assets/Game/Prefabs/Hit/Area/";
    static public string m_BulletPath = "Assets/Game/Prefabs/Hit/Projectile/";
    static public string m_TexturePath = "Assets/Game/Textures/ManaSkills/";

    static public CScriptable_Quest CreatePrefab(CUtility.CQuest _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CQuest>();

        switch (t.m_Data.m_Reward_Type)
        {
            case CUtility.EManaSkillType.DMG_ALL:
                t.m_AreaType = LoadPrefOrigin(m_AreabPath, _data.m_Particle).GetComponent<CArea>();
                break;
            case CUtility.EManaSkillType.DMG_RANDOM:
                t.m_BulletType = LoadPrefOrigin(m_BulletPath, _data.m_Particle).GetComponent<CProjectile>();
                break;
        }

        //t.m_Pref_Bullet = LoadPref(m_PrefabPath, _data.m_Name).GetComponent<CMonster>();

        t.m_Data.m_Icon = LoadTextureOrigin(m_TexturePath, t.m_Data.m_ID.ToString());

        return t;
    }
#endif

    public void DoAction()
    {
        var playerChar = CGameManager.Instance.m_TurnManager.m_PlayerChar;
        int num = m_Data.m_Reward_Num;
        switch (m_Data.m_Reward_Type)
        {
            case CUtility.EManaSkillType.DMG_RANDOM:

                playerChar.ATK_Skill(
                    () => {
                        playerChar.m_Anim.SetTrigger("ATK_Cast");
                        m_Area_RandomATK.m_Proj = m_BulletType;
                        m_Area_RandomATK.Spawn(num);
                    }); break;

            //m_Area_RandomATK.m_Proj = m_BulletType;
            //m_Area_RandomATK.Spawn(num); break;

            case CUtility.EManaSkillType.DMG_ALL:
                playerChar.ATK_Skill(
                () => {
                    playerChar.m_Anim.SetTrigger("ATK_Cast");
                    m_AreaATK.m_Area = m_AreaType; m_AreaATK.Spawn(num);
                });
                //m_AreaATK.m_Area = m_AreaType;
                //m_AreaATK.Spawn(num); 
                break;

            case CUtility.EManaSkillType.DEF:
                playerChar.m_Hitable.GainDef(num); break;

            case CUtility.EManaSkillType.MANA:
                playerChar.m_ManaHandler.UseMana(-num); break;
        }
    }

    public CUtility.CQuest m_Data = new CUtility.CQuest();

    public CProjectile m_BulletType = null;
    [HideInInspector] public CArea_Random m_Area_RandomATK = null;
    public CArea m_AreaType = null;
    [HideInInspector] public CArea_ATK_ALL m_AreaATK = null;

}
