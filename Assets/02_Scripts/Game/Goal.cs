using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    // ゲームマネージャー
    GameManager m_gameManager;
    // カウンター用のテキスト
    GameObject m_textCounter;
    // UIコントローラー
    UiController m_uiController;

    // カウンター
    const int TIME_LIMIT_MAX = 3;
    int m_timer;

    bool isInit = false;

    private void Start()
    {
        m_timer = TIME_LIMIT_MAX;
    }

    private void Update()
    {
        if (!isInit) return;

        if (m_gameManager.m_isEndGame)
        {
            CancelCountDown();
        }
    }

    /// <summary>
    /// 除外レイヤーでSonレイヤーのみ接触判定を拾うよう設定済み
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TopSceneDirector.Instance == null || m_gameManager.m_isEndGame || collision.gameObject.tag == "Ghost" 
            || collision.gameObject.layer != 6 && collision.gameObject.layer != 10
            || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;

        if(m_textCounter == null)
        {
            m_textCounter = GameObject.Find("TextCounter");
        }
        // カウント開始
        m_textCounter.SetActive(true);
        InvokeRepeating("StartCountDown", 0, 1);

        // リセットボタンを無効にする
        m_uiController.SetInteractableButtonReset(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ghost" || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;
        if (m_gameManager.m_isEndGame) return;

        CancelCountDown();

        // リセットボタンを有効にする
        m_uiController.SetInteractableButtonReset(true);
    }

    /// <summary>
    /// カウンター
    /// </summary>
    private void StartCountDown()
    {
        m_timer--;

        if (m_timer <= 0)
        {
            // ゲームクリア処理
            m_gameManager.GameClear();

            // カウントキャンセル
            CancelInvoke("StartCountDown");
            m_timer = TIME_LIMIT_MAX;
        }
        else
        {
            m_textCounter.GetComponent<Text>().text = "" + m_timer;
        }
    }

    /// <summary>
    /// カウンターをキャンセルする
    /// </summary>
    private void CancelCountDown()
    {
        // カウントキャンセル
        m_textCounter.SetActive(false);
        CancelInvoke("StartCountDown");
        m_timer = TIME_LIMIT_MAX;
    }

    /// <summary>
    /// メンバ変数初期化処理
    /// </summary>
    public void InitMemberVariable()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_textCounter = GameObject.Find("TextCounter");
        m_uiController = GameObject.Find("UiController").GetComponent<UiController>();
        isInit = true;
    }
}
