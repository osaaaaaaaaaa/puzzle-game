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
        if (collision.gameObject.tag == "Ghost") return;    // ゴーストの場合

        // 息子の場合
        if (collision.gameObject.layer == 6)
        {
            collision.gameObject.SetActive(false);
            collision.gameObject.transform.position = transform.position;
            m_stats = STATS.BEDIN;
            m_animator.Play("BedAnimation");
        }
    }

    /// <summary>
    /// 寝息のSEを鳴らす
    /// </summary>
    void PlaySleeperSE()
    {
        m_audio.PlayOneShot(m_sleeperSE);
    }
}
