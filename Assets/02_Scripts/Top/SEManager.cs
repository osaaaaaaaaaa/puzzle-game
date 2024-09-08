using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    // �C���X�^���X�쐬
    public static SEManager Instance;
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
            // �V�[���J�ڂ��ĐV������������鎩�g��j��
            Destroy(gameObject);
        }
    }

    AudioSource m_audio;
    [SerializeField] AudioClip m_ScreanMoveSE;
    [SerializeField] AudioClip m_buttonSE;
    [SerializeField] AudioClip m_canselSE;
    [SerializeField] AudioClip m_stageClearSE;
    [SerializeField] AudioClip m_stageGameOverSE;

    private void Start()
    {
        m_audio = GetComponent<AudioSource>();
    }

    public void PlayScreanMoveSE()
    {
        m_audio.PlayOneShot(m_ScreanMoveSE);
    }

    public void PlayButtonSE()
    {
        m_audio.PlayOneShot(m_buttonSE);
    }

    public void PlayCanselSE()
    {
        m_audio.PlayOneShot(m_canselSE);
    }

    public void PlayStageClearSE()
    {
        m_audio.PlayOneShot(m_stageClearSE);
    }

    public void PlayGameOverSE()
    {
        m_audio.PlayOneShot(m_stageGameOverSE);
    }
}
