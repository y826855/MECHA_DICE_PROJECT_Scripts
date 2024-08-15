using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/Event/Logs")]

public class CScriptable_EventLog : CScriptable_CSVData<CScriptable_EventLog>
{
#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/Event/";
#endif


    public List<CUtility.CEventLog> m_Logs = null;
    public uint m_ID = 0;
    public string m_EventName = "";
    public int m_MaxLog = 0;
    public int m_Chapter = 0;

#if UNITY_EDITOR
    static public CScriptable_EventLog CreatePrefab(List<CUtility.CEventLog> _logs,
        string _name, 
        //uint _name,
        int _MaxLog, int _chapter)
    {
        //string name = CGameManager.Instance.m_Dictionary.m_Language.m_Texts[_name].m_Text[0];
        string name = _name;

        var t = CreateInst(m_FolderPath, name);
        EditorUtility.SetDirty(t);

        t.m_EventName = name;
        t.m_MaxLog = _MaxLog;
        t.m_Chapter = _chapter;
        t.m_Logs = _logs;
        return t;
    }
#endif

}
