using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    // インスタンス作成
    public static SEManager Instance;
    private void Awake()
    {
        if (Instance == null)
        {
            // トップ画面の状態を保持する
            Instance = this;

            // シーン遷移しても破棄しないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // シーン遷移して新しく生成される自身を破棄
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
