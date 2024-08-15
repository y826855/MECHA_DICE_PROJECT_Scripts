using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class CUI_HitLog : MonoBehaviour
{
    public TextMeshProUGUI m_TMP = null;


    public Vector3 m_MoveDir = Vector3.up;
    public float m_MoveLR_Rand = 10f;

    public void Spawn(string _data) 
    {
        m_TMP.text = _data;
        this.gameObject.SetActive(true);

        var currPos = this.transform.localPosition;
        m_MoveDir.x = Random.Range(-m_MoveLR_Rand, m_MoveLR_Rand);

        // DOTween을 사용하여 트윈 설정
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(m_TMP.DOFade(1f, 0.5f));             // 0.5초 동안 투명도가 1로 나타남
        mySequence.Join(this.transform.DOLocalMove(currPos + m_MoveDir, 1f));    // 1초 동안 Y 위치가 1만큼 상승
        mySequence.Join(this.transform.DOScale(1f, 0.5f));  // 0.5초 동안 크기가 1로 커짐
        mySequence.Join(m_TMP.DOFade(1f, 0.5f));             // 0.5초 동안 투명도가 1로 나타남
        mySequence.Append(this.transform.DOLocalMove(currPos + m_MoveDir, 1f));    // 1초 동안 Y 위치가 1만큼 상승
        mySequence.Join(m_TMP.DOFade(0f, 0.5f));             // 0.5초 동안 투명도가 0으로 줄어들어 투명해짐
        mySequence.Join(this.transform.DOScale(0f, 0.5f));  // 0.5초 동안 크기가 0으로 작아짐

        // 트윈이 끝날 때 비활성화 (선택사항)
        mySequence.OnComplete(() => gameObject.SetActive(false));
    }
}
