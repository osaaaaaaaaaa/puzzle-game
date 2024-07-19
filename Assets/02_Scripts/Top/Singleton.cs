using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            // �g�b�v��ʂ̏�Ԃ�ێ�����
            Instance = this;

            // �V�[���J�ڂ��Ă��j�����Ȃ��悤�ɂ���
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �g�b�v��ʂ�\������
            Instance.ChangeActive(true);

            // �V�[���J�ڂ��ĐV������������鎩�g��j��
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �g�b�v��ʂ̕\���E��\����؂�ւ���
    /// </summary>
    public void ChangeActive(bool _active)
    {
        var obj = Instance.transform.GetChild(0).gameObject;
        obj.SetActive(_active); // �\���؂�ւ�����
    }
}
