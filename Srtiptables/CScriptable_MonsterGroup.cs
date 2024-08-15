using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/MONSTER_GROUP")]
public class CScriptable_MonsterGroup : CScriptable_CSVData<CScriptable_MonsterGroup>
{

#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/MonsterGroup/";
#endif

    public CUtility.CMonsterGroup m_Data = new CUtility.CMonsterGroup();

#if UNITY_EDITOR
    static public CScriptable_MonsterGroup CreatePrefab(CUtility.CMonsterGroup _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CMonsterGroup>();
        return t;
    }
#endif

}
