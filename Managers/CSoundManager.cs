using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CSoundManager : MonoBehaviour
{
    public enum EBGM 
    {
        BGM_01 = 0 ,
        BGM_02,
        BGM_03,
        BGM_04,
    }

    public enum ELoaded
    {
        //TODO : 코드적으로 참조할 모든 사운드의 이름 적어야함.
        NONE = -1,
        SheildBreak = 0,
        ShieldGain,
        SkillReady,

        S_Def_Up,
        S_FireMagic,
        S_JustDMG,
        S_Magic,
        S_StoneMagic,
        S_ThunderMagic,
        S_Gun,
        S_Gun2,
        S_MonsterBite,
    }

    public enum ECustom
    {
        //내가 따로 추가할 사운드들. 자유롭게 기제함
        NONE = -1,
        S_DICE_ROLL,
        S_DICE_GRAB,

        Bat_SlimeDie,
        DragonDie,
        DragonIntro,
        HumanDie,
        HumanHit,
        OrcDie,

        S_BagOff,
        S_BagOpen,
        S_ScheduleCardSelect,
        S_Schedule_intro,
        S_ScheduleConfirm,
        S_StoreBuy,

        Hit_Organic,
    }

    public SerializeDictionary<EBGM, AudioClip> m_Clips_BGM_Loop = new SerializeDictionary<EBGM, AudioClip>();
    public SerializeDictionary<ELoaded, AudioClip> m_Clips_SFX_Loaded = new SerializeDictionary<ELoaded, AudioClip>();
    public SerializeDictionary<ECustom, AudioClip> m_Clips_SFX_Custom = new SerializeDictionary<ECustom, AudioClip>();
    //public List<AudioClip> m_Clips_SFX_Custom = new List<AudioClip>();

    //public List<AudioClip> m_Clips_BGM_Loop = new List<AudioClip>();
    //public List<AudioClip> m_Clips_SFX_Loaded = new List<AudioClip>();
    //public List<AudioClip> m_Clips_SFX_Custom = new List<AudioClip>();

    [Header("=================================")]
    public AudioSource m_BgmAudioSource = null;
    public float m_BGM_Volume = 0.5f;

    public int m_SFX_Channels = 16;
    public List<AudioSource> m_SFX_AudioSources = new List<AudioSource>();
    public float m_SFX_Volume = 0.5f;

    [Header("=================================")]
    public float m_FadePower = 0.5f;
    public float m_FadeIn_Time = 0.1f;
    public float m_FadeOut_Time = 0.1f;

    private void Awake()
    {
        if (CGameManager.Instance.m_SoundMgr == null)
        {
            CGameManager.Instance.m_SoundMgr = this;
            DontDestroyOnLoad(this.gameObject);

            InitBGM();
            InitSFX();
        }
        else Destroy(this.gameObject);
    }

#if UNITY_EDITOR
    //사운드 클립 받기

    public string m_Sound_Path = "Assets/Game/Sounds/";
    
    public void LoadSoundes()
    {
        m_Clips_SFX_Loaded.Clear();
        foreach (var it in System.Enum.GetValues(typeof(ELoaded))) 
        {
            if (it.ToString() == "NONE") continue;

            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>
                (string.Format("{0}{1}{2}.wav", m_Sound_Path, "Loaded/", it.ToString()));

            if (clip == null)
                clip = AssetDatabase.LoadAssetAtPath<AudioClip>
                (string.Format("{0}{1}{2}.mp3", m_Sound_Path, "Loaded/", it.ToString()));

            if (clip == null)
            { Debug.Log("Loaded/" +it.ToString() + "사운드 없음"); continue; }

            var key = (ELoaded)it;
            if (m_Clips_SFX_Loaded.ContainsKey(key) == false || m_Clips_SFX_Loaded[key] == null)
                m_Clips_SFX_Loaded.Add((ELoaded)it, clip);
        }

        m_Clips_SFX_Custom.Clear();
        foreach (var it in System.Enum.GetValues(typeof(ECustom)))
        {
            if (it.ToString() == "NONE") continue;

            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>
                (string.Format("{0}{1}{2}.wav", m_Sound_Path, "Custom/", it.ToString()));

            if (clip == null)
                clip = AssetDatabase.LoadAssetAtPath<AudioClip>
                (string.Format("{0}{1}{2}.mp3", m_Sound_Path, "Custom/", it.ToString()));

            if (clip == null)
            { Debug.Log("Custom/" + it.ToString() + "사운드 없음"); continue; }

            var key = (ECustom)it;
            if (m_Clips_SFX_Custom.ContainsKey(key) == false || m_Clips_SFX_Custom[key] == null)
                m_Clips_SFX_Custom.Add((ECustom)it, clip);
        }

        m_Clips_BGM_Loop.Clear();
        foreach (var it in System.Enum.GetValues(typeof(EBGM)))
        {
            var clip = AssetDatabase.LoadAssetAtPath<AudioClip>
                (string.Format("{0}{1}{2}.wav", m_Sound_Path, "BGM/", it.ToString()));

            if (clip == null)
                clip = AssetDatabase.LoadAssetAtPath<AudioClip>
                (string.Format("{0}{1}{2}.mp3", m_Sound_Path, "BGM/", it.ToString()));

            if (clip == null)
            { Debug.Log("BGM/" + it.ToString() + "사운드 없음"); continue; }

            var key = (EBGM)it;
            if (m_Clips_BGM_Loop.ContainsKey(key) == false || m_Clips_BGM_Loop[key] == null)
                m_Clips_BGM_Loop.Add((EBGM)it, clip);
        }

        Debug.Log("사운드 불러옴");
    }

#endif
    private void InitBGM()
    {
        GameObject bgmPlayer = new GameObject("BGM_Player");
        bgmPlayer.transform.parent = this.transform;
        m_BgmAudioSource = bgmPlayer.AddComponent<AudioSource>();

        m_BgmAudioSource.playOnAwake = false;
        m_BgmAudioSource.loop = true;
    }


    private void InitSFX()
    {
        GameObject sfxPlayer = new GameObject("SFX_Player");
        sfxPlayer.transform.parent = this.transform;
        m_SFX_AudioSources.Clear();

        for (int i = 0; i < m_SFX_Channels; i++) 
        {
            var audiosource = sfxPlayer.AddComponent<AudioSource>();
            m_SFX_AudioSources.Add(audiosource);
            audiosource.playOnAwake = false;
            audiosource.loop = false;
        }
    }
    public void PlaySoundBGM(EBGM _idx)
    {
        if (m_Clips_BGM_Loop.Count == 0) return;

        //m_BgmAudioSource.clip = m_Clips_BGM_Loop[(int)_idx];
        m_BgmAudioSource.clip = m_Clips_BGM_Loop[_idx];
        m_BgmAudioSource.volume = m_BGM_Volume;
        m_BgmAudioSource.Play();
    }


    Coroutine coWaitForSFXEnd = null;
    Tweener bgmFadeOut = null;
    Tweener bgmFadeIn = null;
    IEnumerator CoWaitForSFXEnd() 
    {
        var fadeOut = m_BgmAudioSource.volume * m_FadePower;

        if (bgmFadeIn != null) bgmFadeIn.Kill();
        bgmFadeOut = DOTween.To(
            () => m_BgmAudioSource.volume,
            volume => m_BgmAudioSource.volume = volume,
            fadeOut, m_FadeOut_Time);

        while (true)
        {
            bool canFadeIn = true;

            yield return CUtility.m_WFS_DOT1;
            foreach (var it in m_SFX_AudioSources)
            {
                if (it.isPlaying == true) 
                { canFadeIn = false; break; }
            }

            if (canFadeIn == true) break;
        }

        coWaitForSFXEnd = null;
        bgmFadeIn = DOTween.To(
            () => m_BgmAudioSource.volume,
            volume => m_BgmAudioSource.volume = volume,
            m_BGM_Volume, m_FadeIn_Time);
    }

    //사운드 이펙트 재생
    void PlaySoundSFX(AudioClip _clip, bool _fadeBGM) 
    {
        if (m_SFX_AudioSources == null || m_SFX_AudioSources.Count == 0) return;

        var temp = m_SFX_AudioSources[0];
        temp.clip = _clip;
        temp.volume = m_SFX_Volume;
        temp.Play();
        m_SFX_AudioSources.RemoveAt(0);
        m_SFX_AudioSources.Add(temp);

        if (_fadeBGM == false) return;
        if (coWaitForSFXEnd == null)
            coWaitForSFXEnd = StartCoroutine(CoWaitForSFXEnd());
    }


    //BGM안줄이는 효과음
    void PlaySoundSFX(AudioClip _clip, float _mulVol = 1f) 
    {
        var temp = m_SFX_AudioSources[0];
        temp.clip = _clip;
        temp.volume = m_SFX_Volume * _mulVol;
        temp.Play();

        m_SFX_AudioSources.RemoveAt(0);
        m_SFX_AudioSources.Add(temp);
    }


    public void PlaySoundEff(ECustom _idx, bool _fadeBGM = false)
    {
        if (_idx == ECustom.NONE) return;
        if(m_Clips_SFX_Custom.ContainsKey(_idx) == true)
            PlaySoundSFX(m_Clips_SFX_Custom[_idx] , _fadeBGM); 
    }
    public void PlaySoundEff_Volume(ECustom _idx, float _mulVol = 1f)
    {
        if (_idx == ECustom.NONE) return;
        if (m_Clips_SFX_Custom.ContainsKey(_idx) == true)
            PlaySoundSFX(m_Clips_SFX_Custom[_idx], _mulVol);  
    }
    public void PlaySoundEff(ELoaded _idx, bool _fadeBGM = false)
    {
        if (_idx == ELoaded.NONE) return;
        if (m_Clips_SFX_Loaded.ContainsKey(_idx) == true)
            PlaySoundSFX(m_Clips_SFX_Loaded[_idx], _fadeBGM);
    }
    public void PlaySoundEff_Volume(ELoaded _idx, float _mulVol = 1f)
    {
        if (_idx == ELoaded.NONE) return;
        if (m_Clips_SFX_Loaded.ContainsKey(_idx) == true)
            PlaySoundSFX(m_Clips_SFX_Loaded[_idx], _mulVol); 
    }

    //로비 BGM
    //맵 BGM
    //보스 BGM
    //승리 BGM

    //이펙트 사운드

    //효과음 나올때 배경음 작게
}
/*
 public class CSoundManager : MonoBehaviour
{
    public enum EBGM { BGM_MAIN = 0 }
    public enum EBATTLE { 
        SHIELDBREAK = 0, SHIELDGAIN, SKILLREADY
    }

    public enum ESKILL_ATK
    {
        NONE = -1,
        CURSE_BOOM = 0,
        CURSE,
        ELECTRO,
        FIRE,
        STONE_Sharp,
        STONE_HIT,
        FIRE_BIG,
        Bite,
        ROCK_DESTROY,
        Slash,
        MagicShot,
    }
    public enum ESKILL_DEF
    {
        NONE = -1,
        SHIELD_GAIN_01 = 0,
        MAGIC_SHIELD,
        SHIELD_BROKEN,
    }
    public enum ESKILL_BUF
    {
        NONE = -1,
        BUFF_DEFAULT_01 = 0,
        BUFF_DEFAULT_02,
        BUFF_DEFAULT_03,
    }

    public enum ESkillType 
    {
        ATK,
        DEF,
        BUF,
    }


    [System.Serializable]
    public struct S_Skill_Sound 
    {
        public ESkillType m_Type;
        public ESKILL_ATK m_ATK;
        public ESKILL_DEF m_DEF;
        public ESKILL_BUF m_BUF;

        public S_Skill_Sound(int _t =0) 
        {
            m_Type = ESkillType.ATK;
            m_ATK = ESKILL_ATK.NONE;
            m_DEF = ESKILL_DEF.NONE;
            m_BUF = ESKILL_BUF.NONE;
        }
    }

    public enum EEFF { 
        CHANGE_CATEGORY = 0, DICE_ROLL, DICEROLL_GRAB,
        SETDICE, SETDICE_ROLL
    }


    public List<AudioClip> m_Clips_BGM_Loop = new List<AudioClip>();
    public List<AudioClip> m_Clips_SFX_Battles = new List<AudioClip>();
    public List<AudioClip> m_Clips_SFX_Skills_ATK = new List<AudioClip>();
    public List<AudioClip> m_Clips_SFX_Skills_DEF = new List<AudioClip>();
    public List<AudioClip> m_Clips_SFX_Skills_BUF = new List<AudioClip>();
    public List<AudioClip> m_Clips_SFX_Effs = new List<AudioClip>();

    [Header("=================================")]
    public AudioSource m_BgmAudioSource = null;
    public float m_BGM_Volume = 0.5f;

    public int m_SFX_Channels = 16;
    public List<AudioSource> m_SFX_AudioSources = new List<AudioSource>();
    public float m_SFX_Volume = 0.5f;

    [Header("=================================")]
    public float m_FadePower = 0.5f;
    public float m_FadeIn_Time = 0.1f;
    public float m_FadeOut_Time = 0.1f;

    private void Awake()
    {
        if (CGameManager.Instance.m_SoundMgr == null)
        {
            CGameManager.Instance.m_SoundMgr = this;
            DontDestroyOnLoad(this.gameObject);

            InitBGM();
            InitSFX();

            PlaySoundBGM(EBGM.BGM_MAIN);
        }
        else Destroy(this.gameObject);
    }

    private void InitBGM()
    {
        GameObject bgmPlayer = new GameObject("BGM_Player");
        bgmPlayer.transform.parent = this.transform;
        m_BgmAudioSource = bgmPlayer.AddComponent<AudioSource>();

        m_BgmAudioSource.playOnAwake = false;
        m_BgmAudioSource.loop = true;
    }
    private void InitSFX()
    {
        GameObject sfxPlayer = new GameObject("SFX_Player");
        sfxPlayer.transform.parent = this.transform;
        m_SFX_AudioSources.Clear();

        for (int i = 0; i < m_SFX_Channels; i++) 
        {
            var audiosource = sfxPlayer.AddComponent<AudioSource>();
            m_SFX_AudioSources.Add(audiosource);
            audiosource.playOnAwake = false;
            audiosource.loop = false;
        }
    }



    public void PlaySoundBGM(EBGM _idx)
    {
        if (m_Clips_BGM_Loop.Count == 0) return;

        m_BgmAudioSource.clip = m_Clips_BGM_Loop[(int)_idx];
        m_BgmAudioSource.volume = m_BGM_Volume;
        m_BgmAudioSource.Play();
    }


    Coroutine coWaitForSFXEnd = null;
    Tweener bgmFadeOut = null;
    Tweener bgmFadeIn = null;
    IEnumerator CoWaitForSFXEnd() 
    {
        var fadeOut = m_BgmAudioSource.volume * m_FadePower;

        if (bgmFadeIn != null) bgmFadeIn.Kill();
        bgmFadeOut = DOTween.To(
            () => m_BgmAudioSource.volume,
            volume => m_BgmAudioSource.volume = volume,
            fadeOut, m_FadeOut_Time);

        while (true)
        {
            bool canFadeIn = true;

            yield return CUtility.m_WFS_DOT1;
            foreach (var it in m_SFX_AudioSources)
            {
                if (it.isPlaying == true) 
                { canFadeIn = false; break; }
            }

            if (canFadeIn == true) break;
        }

        coWaitForSFXEnd = null;
        bgmFadeIn = DOTween.To(
            () => m_BgmAudioSource.volume,
            volume => m_BgmAudioSource.volume = volume,
            m_BGM_Volume, m_FadeIn_Time);
    }

    //사운드 이펙트 재생
    void PlaySoundSFX(List<AudioClip> _clips, int _idx, bool _fadeBGM) 
    {
        if (_clips.Count == 0) 
        {
            Debug.LogWarning("사운드 클립이 존재하지 않음"); return;
        }

        if (m_SFX_AudioSources== null ||
            m_SFX_AudioSources.Count == 0) return;
        //if (m_SFX_AudioSources[0].isPlaying == false)
        {
            var temp = m_SFX_AudioSources[0];
            temp.clip = _clips[_idx];
            temp.volume = m_SFX_Volume;
            temp.Play();

            m_SFX_AudioSources.RemoveAt(0);
            m_SFX_AudioSources.Add(temp);
        }

        if (_fadeBGM == false) return;
        if(coWaitForSFXEnd == null)
            coWaitForSFXEnd = StartCoroutine(CoWaitForSFXEnd());
    }

    void PlaySoundSFX(List<AudioClip> _clips, int _idx, float _mulVol = 1f)
    {
        if (_clips.Count == 0) return;
        Debug.Log(_clips.Count);

        var temp = m_SFX_AudioSources[0];
        temp.clip = _clips[_idx];
        temp.volume = m_SFX_Volume * _mulVol;
        temp.Play();

        m_SFX_AudioSources.RemoveAt(0);
        m_SFX_AudioSources.Add(temp);
    }

    public void PlaySoundEff_ByType(S_Skill_Sound _type) 
    {
        switch (_type.m_Type) 
        {
            case ESkillType.ATK:  PlaySoundEff(_type.m_ATK); break;
            case ESkillType.DEF:  PlaySoundEff(_type.m_DEF); break;
            case ESkillType.BUF:  PlaySoundEff(_type.m_BUF); break;
        }
    }

    public void PlaySoundEff(EBATTLE _idx, bool _fadeBGM = false) 
    {
        PlaySoundSFX(m_Clips_SFX_Battles, (int)_idx, _fadeBGM);
    }
    public void PlaySoundEff(ESKILL_ATK _idx, bool _fadeBGM = true)
    {
        if (_idx == ESKILL_ATK.NONE) return;
        PlaySoundSFX(m_Clips_SFX_Skills_ATK, (int)_idx, _fadeBGM);
    }
    public void PlaySoundEff(ESKILL_DEF _idx, bool _fadeBGM = true)
    {
        if (_idx == ESKILL_DEF.NONE) return;
        PlaySoundSFX(m_Clips_SFX_Skills_DEF, (int)_idx, _fadeBGM);
    }
    public void PlaySoundEff(ESKILL_BUF _idx, bool _fadeBGM = true)
    {
        if (_idx == ESKILL_BUF.NONE) return;
        PlaySoundSFX(m_Clips_SFX_Skills_BUF, (int)_idx, _fadeBGM);
    }
    public void PlaySoundEff(EEFF _idx, bool _fadeBGM = false)
    {
        PlaySoundSFX(m_Clips_SFX_Effs, (int)_idx, _fadeBGM);
    }

    //
    public void PlaySoundEff_Volume(EEFF _idx, float _mulVol = 1f)
    {
        PlaySoundSFX(m_Clips_SFX_Effs, (int)_idx, _mulVol);
    }

    //로비 BGM
    //맵 BGM
    //보스 BGM
    //승리 BGM

    //이펙트 사운드

    //효과음 나올때 배경음 작게
}

 */