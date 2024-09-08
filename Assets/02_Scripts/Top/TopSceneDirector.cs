using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopSceneDirector : MonoBehaviour
{
    public static TopSceneDirector Instance;

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

    [SerializeField] GameObject topObjParent;

    public enum PLAYMODE
    {
        SOLO,
        HOST,
        GUEST
    }
    public PLAYMODE PlayMode { get; private set; }

    public int DistressSignalID { get; private set; }

    /// <summary>
    /// �g�b�v��ʂ̕\���E��\����؂�ւ���
    /// </summary>
    public void ChangeActive(bool _active)
    {
        topObjParent.SetActive(_active); // �\���؂�ւ�����
    }

    public void SetPlayMode(PLAYMODE mode,int signalID)
    {
        PlayMode = mode;
        DistressSignalID = signalID;
    }
}
