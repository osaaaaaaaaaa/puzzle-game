using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Bed : MonoBehaviour
{
    [SerializeField] AudioClip m_sleeperSE;
    AudioSource m_audio;

    Animator m_animator;
    Player m_player;

    enum STATS
    {
        IDLE,
        BEDIN
    }

    STATS m_stats = STATS.IDLE;

    private void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_animator = GetComponent<Animator>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (!m_player.m_isKicked && m_stats == STATS.BEDIN)
        {
            m_stats = STATS.IDLE;
            m_animator.Play("IdleAnimation");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ghost") return;    // �S�[�X�g�̏ꍇ

        // ���q�̏ꍇ
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.SetActive(false);
            collision.gameObject.transform.position = transform.position;
            m_stats = STATS.BEDIN;
            m_animator.Play("BedAnimation");
        }
    }

    /// <summary>
    /// �Q����SE��炷
    /// </summary>
    void PlaySleeperSE()
    {
        m_audio.PlayOneShot(m_sleeperSE);
    }
}
