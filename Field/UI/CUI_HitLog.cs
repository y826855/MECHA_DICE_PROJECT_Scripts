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

        // DOTween�� ����Ͽ� Ʈ�� ����
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(m_TMP.DOFade(1f, 0.5f));             // 0.5�� ���� ������ 1�� ��Ÿ��
        mySequence.Join(this.transform.DOLocalMove(currPos + m_MoveDir, 1f));    // 1�� ���� Y ��ġ�� 1��ŭ ���
        mySequence.Join(this.transform.DOScale(1f, 0.5f));  // 0.5�� ���� ũ�Ⱑ 1�� Ŀ��
        mySequence.Join(m_TMP.DOFade(1f, 0.5f));             // 0.5�� ���� ������ 1�� ��Ÿ��
        mySequence.Append(this.transform.DOLocalMove(currPos + m_MoveDir, 1f));    // 1�� ���� Y ��ġ�� 1��ŭ ���
        mySequence.Join(m_TMP.DOFade(0f, 0.5f));             // 0.5�� ���� ������ 0���� �پ��� ��������
        mySequence.Join(this.transform.DOScale(0f, 0.5f));  // 0.5�� ���� ũ�Ⱑ 0���� �۾���

        // Ʈ���� ���� �� ��Ȱ��ȭ (���û���)
        mySequence.OnComplete(() => gameObject.SetActive(false));
    }
}
