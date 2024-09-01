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
    int m_timer;

    private void Start()
    {
        m_timer = 3;
    }

    /// <summary>
    /// 除外レイヤーでSonレイヤーのみ接触判定を拾うよう設定済み
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_gameManager.m_isStageClear || collision.gameObject.tag == "Ghost" 
            || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;

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
        if (m_gameManager.m_isStageClear || collision.gameObject.tag == "Ghost"
            || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;

        // カウントキャンセル
        m_textCounter.SetActive(false);
        CancelInvoke("StartCountDown");
        m_timer = 3;

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
            m_timer = 3;
        }
        else
        {
            m_textCounter.GetComponent<Text>().text = "" + m_timer;
        }
    }

    /// <summary>
    /// メンバ変数初期化処理
    /// </summary>
    public void InitMemberVariable()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_textCounter = GameObject.Find("TextCounter");
        m_uiController = GameObject.Find("UiController").GetComponent<UiController>();
    }
}
