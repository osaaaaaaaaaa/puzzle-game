using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // ステージのプレファブ
    [SerializeField] GameObject[] m_stagePrefabs;
    // UIコントローラー
    [SerializeField] UiController m_UiController;

    [SerializeField] 

    // 息子
    GameObject m_son;

    #region メインカメラのアニメーション関係
    [SerializeField] GameObject m_mainCamera;   // メインカメラ
    GameObject goal;                            // ゴール地点
    public bool m_isEndAnim;                    // アニメーションが終了したかどうか
    #endregion

    private void Awake()
    {
        // トップ画面を非表示にする
        if (Singleton.Instance != null) Singleton.Instance.ChangeActive(false);

        // ステージを生成する
        if (TopManager.stageID != 0)
        {
            Instantiate(m_stagePrefabs[TopManager.stageID - 1], Vector3.zero, Quaternion.identity);
        }

        // 壁を非表示にする
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // ゲームオブジェクト取得
        m_son = GameObject.Find("Son");
        goal = GameObject.Find("Goal");
    }

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        // メインカメラの初期地点
        float posX = goal.transform.position.x;
        float posY = goal.transform.position.y < 0 ? 0 : goal.transform.position.y + 2; // +2は調整
        m_mainCamera.transform.position = new Vector3(posX, posY, -10);

        // メインカメラのアニメーション
        var sequence = DOTween.Sequence();
        sequence.Append(m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 2f).SetEase(Ease.InOutSine).SetDelay(1f))
                .OnComplete(() => m_isEndAnim = true);
        sequence.Play();
#else
        m_isEndAnim = true;
#endif

        // 最終ステージの場合
        if (TopManager.stageID >= m_stagePrefabs.Length)
        {
            // 次のステージへ遷移するボタンを無効化する
            m_UiController.DisableNextStageButton();
        }
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

    /// <summary>
    /// 次のステージへ遷移する
    /// </summary>
    public void OnNextStageButton()
    {
        TopManager.stageID++;   // ステージIDを更新する

        SceneManager.LoadScene("02_GameScene");
    }

    /// <summary>
    /// トップ画面へ遷移する
    /// </summary>
    public void OnTopButton()
    {
        SceneManager.LoadScene("01_TopScene");
    }

    /// <summary>
    /// ゲームリセット
    /// </summary>
    public void OnGameReset()
    {
        m_son.GetComponent<Son>().Reset();
    }
}
