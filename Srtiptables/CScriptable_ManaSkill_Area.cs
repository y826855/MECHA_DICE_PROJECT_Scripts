using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/Mana_Skill_Area")]
public class CScriptable_ManaSkill_Area : CScriptable_CSVData<CScriptable_ManaSkill_Area>
{
    public CUtility.CManaSkill_Area m_Data = new CUtility.CManaSkill_Area();
    public Color m_DiceColor = Color.red;

    public CProjectile m_BulletType = null;
    [HideInInspector] public CArea_Random m_Area_RandomATK = null;

    public CArea m_AreaType = null;
    [HideInInspector] public CArea_ATK_ALL m_AreaATK = null;


#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/ManaSkill/";
    static public string m_AreabPath = "Assets/Game/Prefabs/Hit/Area/";
    static public string m_BulletPath = "Assets/Game/Prefabs/Hit/Projectile/";
    static public string m_TexturePath = "Assets/Game/Textures/ManaSkills/";

    static public CScriptable_ManaSkill_Area CreatePrefab(CUtility.CManaSkill_Area _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CManaSkill_Area>();

        switch (t.m_Data.m_Type)
        {
            case CUtility.EManaSkillType.DMG_ALL:
                t.m_AreaType = LoadPrefOrigin(m_AreabPath, _data.m_Particle).GetComponent<CArea>();
                break;
            case CUtility.EManaSkillType.DMG_RANDOM:
                t.m_BulletType = LoadPrefOrigin(m_BulletPath, _data.m_Particle).GetComponent<CProjectile>();
                break;
        }

        t.m_Data.m_Icon = LoadTextureOrigin(m_TexturePath, t.m_Data.m_ID.ToString());

        return t;
    }
#endif

    public void ActiveManaSkill() 
    {
        var playerChar = CGameManager.Instance.m_TurnManager.m_PlayerChar;

        //if (m_Data.m_Use_GainDef > 0)
        //    playerChar.m_Hitable.GainDef(m_Data.m_Use_GainDef);
        //if (m_Data.m_Use_GainMana > 0)
        //    playerChar.m_ManaHandler.UseMana(-m_Data.m_Use_GainMana);
    }

    public void DoAction(int _sum) 
    {
        var playerChar = CGameManager.Instance.m_TurnManager.m_PlayerChar;

        playerChar.m_UI_ManaArea.m_TMP_Sum.text = _sum.ToString();

        switch (m_Data.m_Type)
        {
            case CUtility.EManaSkillType.DMG_RANDOM:

                playerChar.ATK_Skill(
                    () => {
                        playerChar.m_Anim.SetTrigger("ATK_Cast");
                        m_Area_RandomATK.m_Proj = m_BulletType;
                        m_Area_RandomATK.Spawn(_sum);
                    }); break;

            case CUtility.EManaSkillType.DMG_ALL:
                playerChar.ATK_Skill(
                    () => {
                        playerChar.m_Anim.SetTrigger("ATK_Cast");
                        m_AreaATK.m_Area = m_AreaType; 
                        m_AreaATK.Spawn(_sum);
                    }); break;
            case CUtility.EManaSkillType.DEF:
                playerChar.ATK_Skill(
                () => {
                    playerChar.m_Anim.SetTrigger("Defend02");
                    playerChar.m_Hitable.GainDef(_sum);
                    playerChar.DoneSkill();
                }); break;

        }
        
    }


    //public void Skill_DiceAdd()
    //{
    //    var diceMgr = CGameManager.Instance.m_DiceManager;
    //    var dices = diceMgr.m_DiceChoiceArea.m_Dices;

    //    foreach (var it in dices)
    //    {
    //        var eye = it.m_eye + 1;
    //        if (eye > 6) eye = 1;
    //        it.ChangeDiceRot_By_Eye(eye);
    //    }
    //}
}
