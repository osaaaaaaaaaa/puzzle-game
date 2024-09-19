using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    #region SE
    AudioSource m_audioSouse;
    [SerializeField] AudioClip m_kickSE;
    [SerializeField] AudioClip m_cowSE;
    [SerializeField] AudioClip m_horseSE;
    #endregion

    // プレイヤーのシュミレーションコントローラー
    [SerializeField] SimulationController m_playerSimulationController;

    // プレイヤー
    Player m_player;
    // 息子のコントローラー
    SonController m_sonController;

    // 蹴り飛ばしたかどうか
    public bool m_isKicked;
    // 蹴るときのベクトル
    Vector3 VectorKick = Vector3.zero;
    // 息子とのオフセット
    Vector3 m_offset;

    private void Start()
    {
        // パラメータ設定
        m_isKicked = false;
        m_audioSouse = GetComponent<AudioSource>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
        VectorKick = Vector3.zero;
        m_offset = Vector3.zero;
    }

    private void Update()
    {
        if (TopSceneDirector.Instance == null || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;

        if (!m_player.m_isKicked) ResetParam();
        if (!m_isKicked) VectorKick = m_playerSimulationController.vecKick;
        if (VectorKick == Vector3.zero || m_sonController == null) return;

        if (VectorKick.x <= 0) 
        {
            // 右向きの場合
            m_offset = new Vector3(-2.46f, 1.27f, 0f);
            transform.localScale = new Vector3(1f, 0.8f, 1f);
        }
        if (VectorKick.x >= 0)
        {
            // 左向きの場合
            m_offset = new Vector3(2.46f, 1.27f, 0f);
            transform.localScale = new Vector3(-1f, 0.8f, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TopSceneDirector.Instance == null || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;
        if (m_isKicked) return;
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 11)
        {
            // 息子のレイヤーか、息子に乗られてる牛の場合
            GameObject son = collision.gameObject;
            if (collision.gameObject.name == "son_run")
            {
                // 走っている状態から元のテクスチャに戻す
                m_sonController.ChangeDefaultTexture();
                son = m_sonController.Son;
            }

            m_isKicked = true;
            DOKick(son);
        }
    }

    /// <summary>
    /// 蹴り飛ばす処理
    /// </summary>
    void DOKick(GameObject son)
    {
        // 蹴り飛ばすアニメーションを再生する
        GetComponent<Animator>().Play("KickAnim");  // 蹴るアニメ

        // プレイヤーが蹴るときのベクトルを取得(アイテムなしの強さ)
        VectorKick = (VectorKick / m_player.m_mulPower) * 50f;

        // 息子を蹴り飛ばす処理
        if (son.GetComponent<Son>())
        {
            son.GetComponent<Son>().ResetSon(transform.position);
            son.transform.position = transform.position + m_offset;
            son.GetComponent<Son>().DOKick(VectorKick, true);
        }
        else
        {
            son.GetComponent<SonCow>().ResetSonCow(transform.position);
            son.transform.position = transform.position + m_offset;
            son.GetComponent<SonCow>().DOKick(VectorKick);
            m_audioSouse.PlayOneShot(m_cowSE);
        }

        m_audioSouse.PlayOneShot(m_horseSE);
        m_audioSouse.PlayOneShot(m_kickSE);
    }

    /// <summary>
    /// 状態をリセットする
    /// </summary>
    void ResetParam()
    {
        // 再度蹴ることができるようにする
        m_isKicked = false;

        // Idleアニメーションを再生する
        GetComponent<Animator>().Play("IdleAnimanim");
    }
}
