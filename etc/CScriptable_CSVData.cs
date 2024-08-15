using System.Collections;
using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
using System.Linq;
#endif

public class CScriptable_CSVData<T> : ScriptableObject where T : ScriptableObject
{
    static public string m_FolderPath = "Assets/Game/Scriptable/";
    static public string m_AssetFolder = "Skill";


#if UNITY_EDITOR
    //
    static public T CreateInst(string _path, string _name) 
    {
        //파일 존재하는지 먼저 불러보기
        //T get = Resources.Load<T>(string.Format("{0}/{1}", path, name));
        T get = AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}{1}.asset", _path, _name));

        //string directory = string.Format("{0}/{1}", path, name);

        if (get == null)
        {
            //폴더 경로 확인. 없다면 폴더 생성
            if (AssetDatabase.IsValidFolder(_path) == false)
            {
                //AssetDatabase.CreateFolder(m_FolderPath, m_AssetFolder);
                System.IO.Directory.CreateDirectory(_path);
            }

            //경로에 다시 접근
            //get = AssetDatabase.LoadAssetAtPath<T>(path);
            get = AssetDatabase.LoadAssetAtPath<T>(string.Format("{0}{1}.asset", _path, _name));

            //다시 접근해도 없다면
            if (get == null)
            {//객체 생성
                get = CreateInstance<T>();
                Debug.Log(string.Format("Create Asset : {0}{1}.asset", _path, _name));
                AssetDatabase.CreateAsset(get, string.Format("{0}{1}.asset", _path, _name));
            }
        }

        return get;
    }

    //텍스쳐 불러오기
    static public Sprite LoadTexture(string _path, string _name, string _default = "Default")
    {
        _name = _name.Replace("+", "");

        //png
        var texture = AssetDatabase.LoadAssetAtPath<Sprite>
                (string.Format("{0}SP_{1}.png", _path, _name));

        //jpg
        if (texture == null)
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("{0}SP_{1}.jpg", _path, _name));

        //없으면 기본 스프라이트
        if (texture == null)
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("{0}SP_{1}.png", _path, _default));

        //없으면 기본 스프라이트
        if (texture == null)
        {
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("Assets/Game/Textures/SP_Default.png"));
        }
        return texture;
    }

    //텍스쳐 불러오기
    static public Sprite LoadTextureOrigin(string _path, string _name, string _default = "Default")
    {
        _name = _name.Replace("+", "");

        //png
        var texture = AssetDatabase.LoadAssetAtPath<Sprite>
                (string.Format("{0}{1}.png", _path, _name));

        //jpg
        if (texture == null)
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("{0}{1}.jpg", _path, _name));

        //없으면 폴더의 기본 스프라이트
        if (texture == null)
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("{0}{1}.png", _path, _default));

        //없으면 기본 스프라이트
        if (texture == null)
        {
            texture = AssetDatabase.LoadAssetAtPath<Sprite>
            (string.Format("Assets/Game/Textures/SP_Default.png"));
        }

        Debug.Log(texture);

        return texture;
    }

    static public Sprite LoadTexture_Multi(string _path, string _nameMulti, 
        ref SerializeDictionary<string, Sprite> _dic)
    {

        List<Sprite> sprites = AssetDatabase.LoadAllAssetsAtPath(
            string.Format("{0}{1}.png", _path, _nameMulti)).OfType<Sprite>().ToList();

        foreach (var it in sprites) 
        { _dic.Add(it.name, it); }
        
        return null;
    }

    //프리펩 불러오기
    static public GameObject LoadPref(string _path, string _name, string _default = "Default") 
    {
        var pref = AssetDatabase.LoadAssetAtPath<GameObject>
            (string.Format("{0}Pref_{1}.prefab", _path, _name));
        Debug.Log(string.Format("{0}Pref_{1}.prefab", _path, _name));
        if (pref == null) 
        {
            pref = AssetDatabase.LoadAssetAtPath<GameObject>
            (string.Format("{0}Pref_{1}.prefab", _path, _default));
        }

        if (pref == null) Debug.LogError("불러올 파일이 경로에 존재하지 않음");

        return pref;
    }

    //프리펩 불러오기
    static public GameObject LoadPrefOrigin(string _path, string _name, string _default = "Default")
    {
        var pref = AssetDatabase.LoadAssetAtPath<GameObject>
            (string.Format("{0}{1}.prefab", _path, _name));
        Debug.Log(string.Format("{0}{1}.prefab", _path, _name));

        if (pref == null)
        {
            pref = AssetDatabase.LoadAssetAtPath<GameObject>
            (string.Format("{0}{1}.prefab", _path, _default));
            Debug.Log(string.Format("{0}{1}.prefab", _path, _default));
        }

        if (pref == null) Debug.LogError("불러올 파일이 경로에 존재하지 않음");

        return pref;
    }

    //에셋 불러오기
    static public Object LoadAsset(string _path, string _name)
    {
        var myAsset = AssetDatabase.LoadAssetAtPath<Object>
            (string.Format("{0}{1}.asset", _path, _name));

        if(myAsset == null)
            Debug.LogError(string.Format("{0}{1}.asset is not exist", _path, _name));
        return myAsset;
    }
#endif


    virtual public T Data_To_Show(T _inst) 
    {
        return _inst;
    }

}
