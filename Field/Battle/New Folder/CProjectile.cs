using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CProjectile : MonoBehaviour
{
    public ParticleSystem m_MainParticle = null;
    public CSatellite m_Child = null;

    [Header("=============================")]
    public float m_RotateSpeed = 0;

    System.Action<int> CB_MoveDone = null;
    int targetIdx = 0;

    //TODO : 순서대로 둥글게 나가는것도 하면 좋을거같은데.. 방법이 있을까?

    public void Spawn(Transform _target, float _time, System.Action<int> _CB_MoveDone, 
        int _targetIdx, Ease _ease = Ease.OutQuad)
    {
        if(m_MainParticle != null) m_MainParticle.Play();

        CB_MoveDone = _CB_MoveDone;
        targetIdx = _targetIdx;

        this.transform.LookAt(_target);
        //시작 랜덤 회전
        var randomAngle = Random.Range(0f, 360f);
        //특정 각도에서 땅으로 들어가기 때문에 그냥 반전시킴
        if (randomAngle > 100f && randomAngle < 260f) randomAngle = randomAngle + 180f;
        this.transform.Rotate(Vector3.forward, randomAngle);
        this.transform.DOMove(_target.position, _time)
            .SetEase(_ease)
            .OnComplete(MoveDone);

        //회전 있으면 계속 회전함
        if (m_RotateSpeed > 0)
        {
            this.transform.DOLocalRotate(Vector3.forward * 360, m_RotateSpeed, RotateMode.FastBeyond360)
                .SetEase(_ease)
                .SetLoops(-1);
        }

        if (m_Child != null) m_Child.Spawn(_time);
    }

    public void MoveDone()
    {
        StartCoroutine(CoMoveDone());
    }

    IEnumerator CoMoveDone()
    {
        //파티클 트레일 사라질 시간 여기서 기다리게 해야할듯
        if (CB_MoveDone != null) CB_MoveDone(targetIdx);

        if(m_MainParticle != null) m_MainParticle.Stop();
        yield return CUtility.GetSecD1To5s(1f);

        Destroy(this.gameObject);
    }
}
