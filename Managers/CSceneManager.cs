using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using UnityEngine.SceneManagement;

public class CSceneManager : MonoBehaviour
{
    public bool m_IsSceneEntered = false;
    public bool m_LoadBattle = false;
    //public Map.NodeType m_CurrNode = Map.NodeType.LowEnemy;

    [Header("----------------------------------------------")]
    public Image m_ImgFade = null;
    public float m_FadeDuration = 0.6f;

    private void Awake()
    {
        if (CGameManager.Instance.m_SceneMgr == null)
        {
            CGameManager.Instance.m_SceneMgr = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else Destroy(this.gameObject);
    }

    public enum EScene 
    {
        LOGO =0,
        MAP = 1,
        BATTLE = 2,
        ALTAR = 3,
        STORE = 4,
        EVENT = 5,
        SKILL_UPGRADE = 6,
    }


    public void ChangeSceneSkillUpgrade()
    {
        StartCoroutine(CoLoadSceneWithFade((int)EScene.SKILL_UPGRADE));
        //SceneManager.LoadScene(6);
    }
    public void ChangeSceneEvent()
    {
        StartCoroutine(CoLoadSceneWithFade((int)EScene.EVENT));
        //SceneManager.LoadScene(5);
    }
    public void ChangeSceneStore()
    {
        StartCoroutine(CoLoadSceneWithFade((int)EScene.STORE));
        //SceneManager.LoadScene(4);
    }
    public void ChangeSceneDiceAltar() 
    {
        StartCoroutine(CoLoadSceneWithFade((int)EScene.ALTAR));
        //SceneManager.LoadScene(3);
    }

    public void ChangeSceneBattle() 
    {
        StartCoroutine(CoLoadSceneWithFade((int)EScene.BATTLE));
        //SceneManager.LoadScene(2);
    }
    public void ChangeSceneMap()
    {
        StartCoroutine(CoLoadSceneWithFade((int)EScene.MAP));
        //SceneManager.LoadScene(1);
    }

    public void ChangeSceneLogo()
    {
        m_LoadBattle = false;
        m_IsSceneEntered = false;
        StartCoroutine(CoLoadSceneWithFade((int)EScene.LOGO));
        //SceneManager.LoadScene(0);
    }

    //public void ChangeScene(Map.NodeType _type) 
    //{
    //    m_IsSceneEntered = true;

    //    m_CurrNode = _type;
    //    switch (m_CurrNode)
    //    {
    //        case Map.NodeType.LowEnemy:
    //        case Map.NodeType.MidEnemy:
    //        case Map.NodeType.EliteEnemy:
    //            Debug.Log("BATTLE SCENE");
    //            ChangeSceneBattle();
    //            break;

    //        case Map.NodeType.Forge:
    //            ChangeSceneSkillUpgrade();
    //            break;
    //        case Map.NodeType.Store:
    //            ChangeSceneStore();
    //            break;
    //        case Map.NodeType.DiceAltar:
    //            ChangeSceneDiceAltar();
    //            break;

    //        case Map.NodeType.BigEvent:
    //        case Map.NodeType.MidEvent:
    //        case Map.NodeType.LowEvent:
    //            Debug.Log("EVENTS");
    //            ChangeSceneEvent();
    //            break;

    //        case Map.NodeType.Boss:
    //            Debug.Log("BOSS_SCENE");
    //            ChangeSceneBattle();
    //            break;
    //    }
    //}


    IEnumerator CoLoadSceneWithFade(int _sceneIdx)
    {
        yield return StartCoroutine(CoFade(m_FadeDuration, true));

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(_sceneIdx);
        asyncOperation.allowSceneActivation = false;

        //씬전환 가능한지 체크
        while (!asyncOperation.isDone) 
        {

            if (asyncOperation.progress >= 0.9f)
            {
                // 씬 활성화
                asyncOperation.allowSceneActivation = true;
            }
            yield return null;
        }

        // 페이드 인
        yield return StartCoroutine(CoFade(m_FadeDuration, false));

        yield return null;
    }


    //씬전환 연출
    IEnumerator CoFade(float _duration, bool _fadeOut = false)
    {
        float t = 0;
        
        m_ImgFade.enabled = true;

        while (t < _duration) 
        {
            t += Time.deltaTime;
            if (_fadeOut == true) m_ImgFade.color = Color.Lerp(m_ImgFade.color, Color.black, t);
            else m_ImgFade.color = Color.Lerp(m_ImgFade.color, Color.clear, t);
            yield return null;
        }

        if(_fadeOut == false) m_ImgFade.enabled = false;
    }
}
