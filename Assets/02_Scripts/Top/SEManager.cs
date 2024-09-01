using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SEManager : MonoBehaviour
{
    AudioSource m_audio;
    [SerializeField] AudioClip m_ScreanMoveSE;
    [SerializeField] AudioClip m_buttonSE;
    [SerializeField] AudioClip m_canselSE;
    [SerializeField] AudioClip m_stageClearSE;

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
}
