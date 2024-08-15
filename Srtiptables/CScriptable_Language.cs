using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEngine.UI;
using UnityEditor;
#endif

public class CScriptable_Language : CScriptable_CSVData<CScriptable_Language>
{
#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/";
#endif

    public SerializeDictionary<uint, CUtility.CLanguageTag> m_Texts = null;
#if UNITY_EDITOR
    static public CScriptable_Language CreatePrefab(
        SerializeDictionary<uint, CUtility.CLanguageTag> _get)
    {
        var t = CreateInst(m_FolderPath, "LANGUAGES");
        EditorUtility.SetDirty(t);
        t.m_Texts = _get;
        return t;
    }
#endif
}
