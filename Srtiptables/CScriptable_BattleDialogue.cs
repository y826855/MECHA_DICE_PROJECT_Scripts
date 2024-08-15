using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/Battle_Dialogue")]

public class CScriptable_BattleDialogue : CScriptable_CSVData<CScriptable_BattleDialogue>
{

#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/Battle_Dialogue/";
#endif
    public CUtility.CMini_Events m_Data = new CUtility.CMini_Events();


#if UNITY_EDITOR
    static public CScriptable_BattleDialogue CreatePrefab(CUtility.CSkillCard _data)
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CMini_Events>();
        return t;
    }
#endif

}
