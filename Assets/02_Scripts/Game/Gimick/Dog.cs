using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dog : MonoBehaviour
{
    #region SE関係
    [SerializeField] AudioClip m_roarSE;
    AudioSource m_audio;
    #endregion

    Player m_player;
    GameObject m_son;
    SonController m_sonController;
    Animator m_animator;
    bool m_coolTime;

    // Start is called before the first frame update
    void Start()
    {
        m_audio = GetComponent<AudioSource>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_son = GameObject.Find("Son");
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
        m_animator = GetComponent<Animator>();
        m_coolTime = false;
    }

    private void Update()
    {
        // プレイヤーがいる方向を向く
        int direction = transform.position.x < m_son.transform.position.x ? 1 : -1;
        transform.localScale = new Vector3(direction, transform.localScale.y, transform.localScale.z);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ghost") return;    // ゴーストの場合

        // 吠えることが可能かつ息子が触れた場合
        if (!m_coolTime && collision.gameObject.layer == 6 && m_player.m_isKicked)
        {
            m_coolTime = true;
            m_animator.Play("RoarAnimation");

            // 息子を走らせる
            int direction = transform.position.x < collision.gameObject.transform.position.x ? 1 : -1;
            m_sonController.ChangeRunTexture(direction, transform.position.y);
        }
    }

    /// <summary>
    /// 吠える処理
    /// </summary>
    void DORoar()
    {
        m_audio.PlayOneShot(m_roarSE);
    }

    /// <summary>
    /// クールダウン処理
    /// </summary>
    void CoolDown()
    {
        m_coolTime = false;
    }
}
