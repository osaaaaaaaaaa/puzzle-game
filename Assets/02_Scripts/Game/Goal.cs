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

    // カウンター
    int m_timer;

    private void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_textCounter = GameObject.Find("TextCounter");
        m_timer = 3;
    }

    /// <summary>
    /// 除外レイヤーでSonレイヤーのみ接触判定を拾うよう設定済み
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_gameManager.m_isStageClear) return;

        // カウント開始
        m_textCounter.SetActive(true);
        InvokeRepeating("StartCountDown", 0, 1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_gameManager.m_isStageClear) return;

        // カウントキャンセル
        m_textCounter.SetActive(false);
        CancelInvoke("StartCountDown");
        m_timer = 3;
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
}
