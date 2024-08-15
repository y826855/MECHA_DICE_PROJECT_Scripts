using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/MONSTER")]
public class CScriptable_Monster : CScriptable_CSVData<CScriptable_Monster>
{

#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/Monster/";
    static public string m_PrefabPath = "Assets/Game/Prefabs/Enemy/";

    static public CScriptable_Monster CreatePrefab(CUtility.CMonster _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CMonster>();

        t.m_Pref = LoadPref(m_PrefabPath, _data.m_Name)?.GetComponent<CMonster>();

        return t;
    }
#endif

    public CUtility.CMonster m_Data = new CUtility.CMonster();
    public CMonster m_Pref = null;
}
