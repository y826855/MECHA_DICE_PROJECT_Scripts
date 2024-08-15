using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandMadeData", menuName = "ScriptableData/SceneInfo")]
public class CScriptable_SceneInfo : CScriptable_CSVData<CScriptable_SceneInfo>
{
#if UNITY_EDITOR
    static new public string m_FolderPath = "Assets/Game/Scriptables/SceneInfo/";
    static public string m_IconPath = "Assets/Game/Textures/Icon/";
#endif
    public CUtility.CSceneInfo m_Data = new CUtility.CSceneInfo();

    public Sprite m_SP_Icon = null;

    //한번도 들른 적 없는 맵
    public bool m_IsNew = false;

#if UNITY_EDITOR
    static public CScriptable_SceneInfo CreatePrefab(CUtility.CSceneInfo _data, string _iconPath = "")
    {
        var t = CreateInst(m_FolderPath, _data.m_Name);
        EditorUtility.SetDirty(t);
        t.m_Data = _data.Clone<CUtility.CSceneInfo>();
        t.m_SP_Icon = LoadTexture(m_IconPath, _data.m_Name);

        return t;
    }
#endif

}
