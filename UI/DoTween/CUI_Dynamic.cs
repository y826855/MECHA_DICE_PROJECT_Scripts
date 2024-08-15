using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class CUI_Dynamic : MonoBehaviour
{
    
    public CanvasGroup m_CG = null;
    public TMPro.TextMeshProUGUI m_TMP_Base = null;
    public Image m_Img_Base = null;

    [Header("==============Time===================")]
    public float m_FadeIn_Time = 0.1f;
    public float m_FadeOut_Time = 0.1f;

    [Header("==============Scale===================")]
    public bool m_bSacleFade_InOut = false;
    public Vector3 m_Scale_In = Vector3.zero;
    public Vector3 m_Sacle_Out = Vector3.zero;
    public Vector3 m_Sacle_Origin = Vector3.zero;

    [Header("==============Location================")]
    public bool m_bPosFade_InOut = false;
    public Vector3 m_Pos_In = Vector3.zero;
    public Vector3 m_Pos_Out = Vector3.zero;
    public Vector3 m_Pos_Origin = Vector3.zero;

    [Header("==============Local_Location================")]
    public bool m_bLocalPosFade_InOut = false;
    public Vector3 m_LocalPos_In = Vector3.zero;
    public Vector3 m_LocalPos_Out = Vector3.zero;

    [Header("==============Alpha===================")]
    public bool m_bAlphaFade_InOut = false;
    public float m_Alpha_FadeIn = 0;
    public float m_Alpha_FadeOut = 0;

    [Header("======================================")]
    public Ease m_Ease = Ease.Unset;
    //[Header("==============CallBack================")]
    public System.Action m_Func_Disappear = null;
    public System.Action m_Func_Appear = null;
    public bool m_CanEnable = true;

    private void Awake()
    {
        m_Sacle_Origin = this.transform.localScale;
        m_Pos_Origin = this.transform.position;
    }

    //비활성 상태시 강제로 켜기
    public void SetForceEnable() 
    {
        m_CanEnable = true;
        SetEnable();
    }

    //
    public virtual void OnClick_Disable() 
    {
        m_CanEnable = false;
        SetDisable();
    }
    
    public void SetEnable() 
    {
        if (m_CanEnable == false) return;

        this.gameObject.SetActive(true);

        if (m_bAlphaFade_InOut == true)
        {//알파값 페이드 인
            m_CG.interactable = false;
            m_CG.alpha = m_Alpha_FadeIn;

            DOTween.To(
                () => m_CG.alpha,
                x => m_CG.alpha = x, 1,
                m_FadeIn_Time)
                .OnComplete(Appear);

        }

        if (m_bSacleFade_InOut == true)
        {//사이즈 페이드 인
            m_CG.interactable = false;
            this.transform.localScale = m_Scale_In;
            this.transform.DOScale(m_Sacle_Origin, m_FadeIn_Time)
                .OnComplete(Appear);
        }

        if (m_bPosFade_InOut == true)
        {//위치 페이드 인
            m_CG.interactable = false;
            this.transform.position = m_Pos_Origin + m_Pos_In;
            this.transform.DOMove(m_Pos_Origin, m_FadeIn_Time)
                .OnComplete(Appear);

            //this.transform.localPosition = m_Pos_In;
            //this.transform.DOLocalMove(Vector3.zero, m_FadeIn_Time)
            //    .OnComplete(Appear);
        }

        else if (m_bLocalPosFade_InOut == true)
        {//로컬위치 페이드 인
            m_CG.interactable = false;
            this.transform.localPosition = m_LocalPos_In;
            this.transform.DOLocalMove(Vector3.zero, m_FadeIn_Time)
                .OnComplete(Appear);
        }
    }

    public void Appear() 
    {//등장 완료 콜백
        m_CG.interactable = true;
        if (m_Func_Appear != null) m_Func_Appear();
    }

    [SerializeField]bool disappearing = false;

    public void SetDisable()
    {
        if (disappearing == false)
        {
            //m_Pos_Origin = this.transform.position;

            m_CG.interactable = false;
            disappearing = true;
            this.gameObject.SetActive(true);

            if (m_bAlphaFade_InOut == true)
            {//알파값 페이드 아웃
                DOTween.To(
                    () => m_CG.alpha,
                    x => m_CG.alpha = x, m_Alpha_FadeOut, m_FadeIn_Time)
                    .OnComplete(Disappear);
            }

            if (m_bSacleFade_InOut == true)
            {//사이즈 페이드 아웃
                this.transform.localScale = m_Scale_In;
                this.transform.DOScale(m_Sacle_Out, m_FadeOut_Time)
                    .OnComplete(Disappear);
            }

            if (m_bPosFade_InOut == true)
            {//위치 페이드 아웃
                this.transform.position = m_Pos_Origin ;
                this.transform.DOMove(m_Pos_Origin + m_Pos_Out, m_FadeIn_Time)
                    .OnComplete(Disappear);
                //this.transform.DOLocalMove(m_Pos_Out, m_FadeOut_Time)
                //    .OnComplete(Disappear);
            }

            else if (m_bLocalPosFade_InOut == true)
            {//로컬위치 페이드 아웃
                Debug.Log("LOCAL");
                this.transform.localPosition = Vector3.zero;
                this.transform.DOLocalMove(m_LocalPos_Out, m_FadeOut_Time)
                    .OnComplete(Disappear);
            }
        }
    }

    private void OnDisable()
    {
        //this.transform.localPosition = Vector3.zero;
        this.transform.position = m_Pos_Origin;
        this.transform.localScale = m_Sacle_Origin;
        m_CG.alpha = 1;
        m_CG.interactable = true;
        disappearing = false;
    }

    public void Disappear()
    {//퇴장 완료 콜백
        if (m_CG.interactable == true) return;
        if (m_Func_Disappear != null) m_Func_Disappear();
        //this.transform.localPosition = Vector3.zero;

        if(m_bLocalPosFade_InOut == true) this.transform.localPosition = Vector3.zero;
        else if (m_bPosFade_InOut == true) this.transform.position = m_Pos_Origin;

        m_CG.alpha = 1;
        m_CG.interactable = true;
        this.gameObject.SetActive(false);
    }
}
