using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CSatellite : MonoBehaviour
{

    public float m_Height = 3f;
    [Range(0f,1f)]
    public float m_Duration = 0.2f;

    float downTime = 0;
    public void Spawn(float _lifeTime) 
    {
        float upTime = _lifeTime * m_Duration;
        downTime = _lifeTime - upTime;

        this.transform.DOLocalMoveY(m_Height, _lifeTime * m_Duration)
            .SetEase(Ease.OutQuad)
            .OnComplete(MoveDown);

        StartCoroutine(CoLookAt());
    }

    void MoveDown() 
    {
        this.transform.DOLocalMoveY(0, downTime)
            .SetEase(Ease.OutQuad);
    }

    IEnumerator CoLookAt() 
    {
        Vector3 beforePos = this.transform.position;
        yield return null;

        while (true) 
        {
            var curr = this.transform.position - beforePos;

            if (curr == Vector3.zero)
                this.transform.forward = Vector3.forward;
            else this.transform.forward = curr;

            beforePos = this.transform.position;
            yield return null;
        }
    }


    private void OnDisable()
    {
        StopAllCoroutines();
    }
}
