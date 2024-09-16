using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class LoadingAnims : MonoBehaviour
{
    private const float DURATION = 1f;  // ���[�f�B���O�A�j���[�V�����̊Ԋu

    void Start()
    {
        Image[] circles = GetComponentsInChildren<Image>(); // �q�I�u�W�F�N�g���擾����

        for (var i = 0; i < circles.Length; i++)
        {// �擾�����摜�̖��������[�v
            var angle = -2 * Mathf.PI * i / circles.Length;
            circles[i].rectTransform.anchoredPosition = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * 50f;
            circles[i].DOFade(0f, DURATION).SetLoops(-1, LoopType.Yoyo).SetDelay(DURATION * i / circles.Length);
        }
    }
}
