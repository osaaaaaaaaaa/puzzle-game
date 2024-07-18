using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region 左右の壁
    [SerializeField] GameObject m_Wall_L;
    [SerializeField] GameObject m_Wall_R;
    #endregion

    [SerializeField] UiController m_UiController;

    // Start is called before the first frame update
    void Start()
    {
        // 壁を非表示にする
        m_Wall_L.GetComponent<Renderer>().enabled = false;
        m_Wall_R.GetComponent<Renderer>().enabled = false;
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public void GameClear()
    {
        // UIをゲームクリア用に設定する
        m_UiController.SetGameClearUI();
    }

    /// <summary>
    /// リトライ処理
    /// </summary>
    public void OnRetryButton()
    {
        SceneManager.LoadScene("02_GameScene");
    }
}
