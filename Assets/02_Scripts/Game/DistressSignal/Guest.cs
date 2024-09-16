using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guest : MonoBehaviour
{
    #region SE
    AudioSource m_audioSouse;
    [SerializeField] AudioClip m_kickSE;
    [SerializeField] AudioClip m_cowSE;
    #endregion

    [SerializeField] Text m_textName;

    [SerializeField] GameObject m_simulationController;
    [SerializeField] GameObject m_line;

    // ゲームマネージャー
    GameManager m_gameManager;
    // プレイヤー
    Player m_player;
    // 息子のコントローラー
    SonController m_sonController;

    // 蹴り飛ばしたかどうか
    public bool m_isKicked;
    // 蹴るときのベクトル
    Vector3 VectorKick = Vector3.zero;

    private void Update()
    {
        if (VectorKick == Vector3.zero || m_sonController == null) return;
        m_simulationController.GetComponent<SimulationController>().Simulation(transform.position + m_sonController.GetSonOfset());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

            // 息子の場合
            m_isKicked = true;
            DOKick(son);
        }
    }

    /// <summary>
    /// 蹴り飛ばす処理
    /// </summary>
    void DOKick(GameObject son)
    {
        if (m_gameManager.m_isEndGame) return;
        Debug.Log(son.name);

        // 母親のアニメーションを再生する
        GetComponent<PlayerAnimController>().PlayKickAnim();  // 蹴るアニメ

        // 息子を蹴り飛ばす処理
        if (son.GetComponent<Son>())
        {
            son.GetComponent<Son>().ResetSon(transform.position);
            son.GetComponent<Son>().DOKick(VectorKick, true);
        }
        else
        {
            son.GetComponent<SonCow>().ResetSonCow(transform.position);
            son.GetComponent<SonCow>().DOKick(VectorKick);
            m_audioSouse.PlayOneShot(m_cowSE);
        }
        m_audioSouse.PlayOneShot(m_kickSE);
    }

    /// <summary>
    /// 状態をリセットする
    /// </summary>
    public void ResetGuest()
    {
        // 再度蹴ることができるようにする
        m_isKicked = false;

        // 母親のアニメーションを再生する
        GetComponent<PlayerAnimController>().PlayStandbyAnim();  // 通常スキンのIdleアニメ
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    /// <param name="name"></param>
    /// <param name="position"></param>
    /// <param name="vector"></param>
    public void InitMemberVariable(string name,Vector3 position,Vector3 vector)
    {
        m_isKicked = false;

        // オブジェクト・コンポーネントを取得する
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
        m_audioSouse = GetComponent<AudioSource>();

        // パラメータ設定
        m_textName.text = name;
        transform.position = position;
        VectorKick = (vector / m_player.m_mulPower) * 50f;   // アイテムを使用しないで蹴るときの大きさに修正;

        // 起動予測線の描画開始
        m_simulationController.GetComponent<SimulationController>().vecKick = VectorKick;
    }

    /// <summary>
    /// 起動予測関係を表示・非表示する
    /// </summary>
    public void ToggleLineVisibility(bool isVisibility)
    {
        m_simulationController.SetActive(isVisibility);
        m_line.SetActive(isVisibility);
    }
}
