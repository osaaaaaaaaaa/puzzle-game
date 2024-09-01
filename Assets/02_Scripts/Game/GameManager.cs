using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    // ポーズ中かどうか
    public bool m_isPause;

    // UIコントローラー
    [SerializeField] UiController m_UiController;
    // SEマネージャー
    [SerializeField] SEManager m_seManager;
    // プレイヤー
    GameObject m_player;
    // カメラコントローラ
    CameraController m_cameraController;

    #region ゲームクリア時の演出
    [SerializeField] GameObject m_stageClearEffect;
    Vector3 m_offsetEffectL = new Vector3(-10.43f, -7.48999f,5);
    Vector3 m_offsetEffectR = new Vector3(10.39f, -7.48999f,5);
    #endregion

    #region メインカメラのアニメーション関係
    GameObject m_mainCamera;   // メインカメラ
    GameObject goal;           // ゴール地点
    public bool m_isEndAnim;   // アニメーションが終了したかどうか
    #endregion

    // ゲームをクリアしたかどうか
    public bool m_isStageClear;

    private void Awake()
    {
        m_isEndAnim = false;

#if UNITY_EDITOR
        // 各種スクリプトのメンバ変数初期化処理
        GameObject.Find("CameraController").GetComponent<CameraController>().
            InitMemberVariable(m_UiController.ButtonZoomIn, m_UiController.ButtonZoomOut);
        GameObject.Find("ride_cow").GetComponent<SonCow>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();
#endif

        // トップ画面を非表示にする
        if (Singleton.Instance != null) Singleton.Instance.ChangeActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // アセットバンドルを使用する場合、Startメソッドの型をIEnumeratorに変更すること
        // ビルドするときは、全体に!をつけること
#if !UNITY_EDITOR
        // ゲームシーンを読み込むまで待機する
        var op = Addressables.LoadSceneAsync(TopManager.stageID + "_GameScene", LoadSceneMode.Additive);
        yield return op;

        // 各種スクリプトのメンバ変数初期化処理
        GameObject.Find("CameraController").GetComponent<CameraController>().
            InitMemberVariable(m_UiController.ButtonZoomIn, m_UiController.ButtonZoomOut);
        GameObject.Find("ride_cow").GetComponent<SonCow>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();
#endif

        // 壁を非表示にする
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_T").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // ゲームオブジェクト取得
        m_mainCamera = GameObject.Find("MainCamera_Game");
        m_player = GameObject.Find("Player");
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        goal = GameObject.Find("Goal");

        // フラグOFF
        m_isPause = false;
        m_isStageClear = false;

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
                .OnComplete(StartGame) ;
        sequence.Play();
#else
        StartGame();
#endif

        // 最終ステージの場合
        if (TopManager.stageID >= TopManager.stageMax)
        {
            // 次のステージへ遷移するボタンを無効化する
            m_UiController.DisableNextStageButton();
        }
    }

    /// <summary>
    /// ゲーム開始処理
    /// </summary>
    public void StartGame()
    {
        m_cameraController.ZoomOut(1f);

        m_UiController.GetComponent<UiController>().SetActiveGameUI(true);

        m_isEndAnim = true;
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public void GameClear()
    {
        bool isUpdateStageID = NetworkManager.Instance.StageID < TopManager.stageMax && NetworkManager.Instance.StageID == TopManager.stageID;

        // 最新のステージをクリアした場合
        if (isUpdateStageID)
        {
            // ユーザー更新処理
            StartCoroutine(NetworkManager.Instance.UpdateUser(
                NetworkManager.Instance.UserName,
                NetworkManager.Instance.AchievementID,
                NetworkManager.Instance.StageID + 1,
                NetworkManager.Instance.IconID,
                result =>
                {
                    // ステージをクリアしたことにする
                    m_isStageClear = true;

                    // UIをゲームクリア用に設定する
                    m_UiController.SetGameClearUI();

                    // 初クリア演出
                    PlayStageClearEffect();
                }));
        }
        else
        {
            // ステージをクリアしたことにする
            m_isStageClear = true;

            // UIをゲームクリア用に設定する
            m_UiController.SetGameClearUI();

            // 初クリア演出
            PlayStageClearEffect();
        }
    }

    /// <summary>
    /// ステージクリアの演出
    /// </summary>
    void PlayStageClearEffect()
    {
        // エフェクトを生成する
        GameObject effectL = Instantiate(m_stageClearEffect, m_mainCamera.transform);
        effectL.transform.localPosition = m_offsetEffectL;
        GameObject effectR = Instantiate(m_stageClearEffect, m_mainCamera.transform);
        effectR.transform.localPosition = m_offsetEffectR;
        effectR.transform.localScale = new Vector3(-effectR.transform.localScale.x, effectR.transform.localScale.y, effectR.transform.localScale.z);

        if(m_cameraController.m_cameraMode == CameraController.CAMERAMODE.ZOOMOUT)
        {
            // 現在のカメラがズームアウト状態の場合
            effectL.transform.localScale *= 2;
            effectL.transform.localPosition *= 2;
            effectR.transform.localScale *= 2;
            effectR.transform.localPosition *= 2;
        }

        m_seManager.PlayStageClearSE();
    }

    /// <summary>
    /// リトライ処理
    /// </summary>
    public void OnRetryButton()
    {
#if !UNITY_EDITOR
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
#endif
    }

    /// <summary>
    /// 次のステージへ遷移する
    /// </summary>
    public void OnNextStageButton()
    {
        TopManager.stageID++;   // ステージIDを更新する

#if !UNITY_EDITOR
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
#endif
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
