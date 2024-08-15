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

    //TODO : ������� �ձ۰� �����°͵� �ϸ� �����Ű�����.. ����� ������?

    public void Spawn(Transform _target, float _time, System.Action<int> _CB_MoveDone, 
        int _targetIdx, Ease _ease = Ease.OutQuad)
    {
        if(m_MainParticle != null) m_MainParticle.Play();

        CB_MoveDone = _CB_MoveDone;
        targetIdx = _targetIdx;

        this.transform.LookAt(_target);
        //���� ���� ȸ��
        var randomAngle = Random.Range(0f, 360f);
        //Ư�� �������� ������ ���� ������ �׳� ������Ŵ
        if (randomAngle > 100f && randomAngle < 260f) randomAngle = randomAngle + 180f;
        this.transform.Rotate(Vector3.forward, randomAngle);
        this.transform.DOMove(_target.position, _time)
            .SetEase(_ease)
            .OnComplete(MoveDone);

        //ȸ�� ������ ��� ȸ����
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
        //��ƼŬ Ʈ���� ����� �ð� ���⼭ ��ٸ��� �ؾ��ҵ�
        if (CB_MoveDone != null) CB_MoveDone(targetIdx);

        if(m_MainParticle != null) m_MainParticle.Stop();
        yield return CUtility.GetSecD1To5s(1f);

        Destroy(this.gameObject);
    }
}
