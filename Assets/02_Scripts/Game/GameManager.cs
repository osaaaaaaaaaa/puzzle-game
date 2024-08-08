using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // ポーズ中かどうか
    public bool m_isPause;

    // UIコントローラー
    [SerializeField] UiController m_UiController;
    // プレイヤー
    GameObject m_player;

    #region メインカメラのアニメーション関係
    GameObject m_mainCamera;   // メインカメラ
    GameObject goal;           // ゴール地点
    public bool m_isEndAnim;   // アニメーションが終了したかどうか
    #endregion

    // ゲームをクリアしたかどうか
    public bool m_isStageClear;

    private void Awake()
    {
        // トップ画面を非表示にする
        if (Singleton.Instance != null) Singleton.Instance.ChangeActive(false);

        // 壁を非表示にする
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_T").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // ゲームオブジェクト取得
        m_mainCamera = GameObject.Find("MainCamera_Game");
        m_player = GameObject.Find("Player");
        goal = GameObject.Find("Goal");

        // フラグOFF
        m_isPause = false;
        m_isStageClear = false;
    }

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        // カメラの初期地点を取得
        Vector3 startPos = m_mainCamera.transform.position;

        // カメラをアニメーションする開始座標を設定
        float posX = goal.transform.position.x;
        float posY = goal.transform.position.y;
        m_mainCamera.transform.position = new Vector3(posX, posY, -10);

        // メインカメラのアニメーション
        var sequence = DOTween.Sequence();
        sequence.Append(m_mainCamera.transform.DOMove(startPos, 2f).SetEase(Ease.InOutSine).SetDelay(1f))
                .OnComplete(() => m_isEndAnim = true);
        sequence.Play();
#else
        m_isEndAnim = true;
#endif

        // 最終ステージの場合
        if (TopManager.stageID >= TopManager.stageMax)
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
        // ステージをクリアしたことにする
        m_isStageClear = true;

        // UIをゲームクリア用に設定する
        m_UiController.SetGameClearUI();
    }

    /// <summary>
    /// リトライ処理
    /// </summary>
    public void OnRetryButton()
    {
        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
    }

    /// <summary>
    /// 次のステージへ遷移する
    /// </summary>
    public void OnNextStageButton()
    {
        TopManager.stageID++;   // ステージIDを更新する

        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
    }

    /// <summary>
    /// トップ画面へ遷移する
    /// </summary>
    public void OnTopButton()
    {
        Initiate.Fade("01_TopScene", Color.black, 1.0f);
    }

    /// <summary>
    /// ゲームリセット
    /// </summary>
    public void OnGameReset()
    {
        m_player.GetComponent<Player>().ResetPlayer();
    }
}
