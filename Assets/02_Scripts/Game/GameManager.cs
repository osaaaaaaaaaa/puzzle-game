using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using System;

public class GameManager : MonoBehaviour
{
    // ポーズ中かどうか
    public bool m_isPause;

    // UIコントローラー
    [SerializeField] UiController m_UiController;
    // プレイヤー
    GameObject m_player;
    // カメラコントローラ
    CameraController m_cameraController;

    // ゲスト
    [SerializeField] GameObject m_guestSetPrefab;
    List<Guest> m_guestList;

    #region ステージの進捗情報
    bool m_isMedal1;
    bool m_isMedal2;
    float m_gameTimer;
    public bool m_isEndGame { get; private set; }
    #endregion

    #region ゲームクリア時の演出
    [SerializeField] GameObject m_stageClearEffect;
    Vector3 m_offsetEffectL = new Vector3(-10.43f, -7.48999f,5);
    Vector3 m_offsetEffectR = new Vector3(10.39f, -7.48999f,5);
    #endregion

    #region メインカメラのアニメーション関係
    GameObject m_mainCamera;   // メインカメラ
    public bool m_isEndAnim;   // アニメーションが終了したかどうか
    #endregion

    /// <summary>
    /// ゲームモード
    /// </summary>
    public enum GAMEMODE
    {
        Play,       // 通常通りに遊べる
        Edit,       // ゲストの編集モード
        EditDone    // ゲストの編集完了モード
    }
    public GAMEMODE GameMode { get; private set; }

    private void Awake()
    {
        m_guestList = new List<Guest>();

        // ゲームモードの初期化
        if (TopSceneDirector.Instance != null)
        {
            var playMode = TopSceneDirector.Instance.PlayMode;
            var gameMode = playMode != TopSceneDirector.PLAYMODE.GUEST ? GAMEMODE.Play : GAMEMODE.EditDone;
            GameMode = gameMode;

            // 参加しているゲストの配置情報を取得する
            StartCoroutine(NetworkManager.Instance.GetSignalGuest(
                TopSceneDirector.Instance.DistressSignalID,
                result =>
                {
                    if (result == null) return;

                    // ゲストを生成する
                }));
        }
        else
        {
            GameMode = GAMEMODE.Play;
        }

        // パラメータ初期化
        m_isEndAnim = false;
        m_isEndGame = false;
        m_isMedal1 = false;
        m_isMedal2 = false;
        m_gameTimer = 40;

#if UNITY_EDITOR
        // 各種スクリプトのメンバ変数初期化処理
        GameObject.Find("CameraController").GetComponent<CameraController>().
            InitMemberVariable(m_UiController.ButtonZoomIn, m_UiController.ButtonZoomOut);
        GameObject.Find("ride_cow").GetComponent<SonCow>().InitMemberVariable();
        GameObject.Find("Son").GetComponent<Son>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();

        // 前回メダルを取得している場合は表示を変更する 、 リザルトが存在しない場合は必ずfalse
        if (TopSceneDirector.Instance != null)
        {
            bool isMedal1 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal1;
            bool isMedal2 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
                false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal2;
            GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(isMedal1);
            GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(isMedal2);
        }
        else
        {
            GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(false);
            GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(false);
        }

        if (TopSceneDirector.Instance != null)
        {
            if (TopSceneDirector.Instance.PlayMode != TopSceneDirector.PLAYMODE.SOLO)
            {
                // 参加しているゲストの配置情報を取得する
                StartCoroutine(NetworkManager.Instance.GetSignalGuest(
                    TopSceneDirector.Instance.DistressSignalID,
                    result =>
                    {
                        if (result == null) return;

                        foreach (ShowSignalGuestResponse user in result)
                        {
                            // 登録してまだ設置が完了していない場合はスキップ
                            if (NetworkManager.Instance.StringToVector3(user.Pos) == Vector3.zero) continue;

                            Vector3 pos = NetworkManager.Instance.StringToVector3(user.Pos);
                            Vector3 vec = NetworkManager.Instance.StringToVector3(user.Vector);

                            if (user.UserID == NetworkManager.Instance.UserID)
                            {
                                // 自分自身の場合は最後に登録した場所へ移動させる
                                GameObject.Find("Player").transform.position = pos;
                                continue;
                            }

                            // ゲストを生成する
                            GameObject stage = GameObject.Find("Stage");
                            GameObject guestSet = Instantiate(m_guestSetPrefab, stage.transform);
                            guestSet.transform.position = Vector3.zero;
                            GameObject guest = guestSet.transform.GetChild(0).gameObject;
                            guest.GetComponent<Guest>().InitMemberVariable(user.UserName,pos, vec);

                            // 生成して初期化が済んだらリストに追加
                            m_guestList.Add(guest.GetComponent<Guest>());
                        }
                }));
            }
        }
#endif

        // トップ画面を非表示にする
        if (TopSceneDirector.Instance != null) TopSceneDirector.Instance.ChangeActive(false);
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
        GameObject.Find("Son").GetComponent<Son>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();

        // 前回メダルを取得している場合は表示を変更する 、 リザルトが存在しない場合は必ずfalse
        bool isMedal1 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal1;
        bool isMedal2 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal2;
        GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(isMedal1);
        GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(isMedal2);

        if (TopSceneDirector.Instance.PlayMode != TopSceneDirector.PLAYMODE.SOLO)
        {
            // 参加しているゲストの配置情報を取得する
            StartCoroutine(NetworkManager.Instance.GetSignalGuest(
                TopSceneDirector.Instance.DistressSignalID,
                result =>
                {
                    if (result == null) return;

                    foreach (ShowSignalGuestResponse user in result)
                    {
                        // 登録してまだ設置が完了していない場合はスキップ
                        if (NetworkManager.Instance.StringToVector3(user.Pos) == Vector3.zero) continue;

                        // ゲストを生成する
                        GameObject stage = GameObject.Find("Stage");
                        GameObject guestSet = Instantiate(m_guestSetPrefab, stage.transform);
                        guestSet.transform.position = Vector3.zero;

                        GameObject guest = guestSet.transform.GetChild(0).gameObject;
                        guest.GetComponent<Guest>().InitMemberVariable(user.UserName,
                            NetworkManager.Instance.StringToVector3(user.Pos), 
                            NetworkManager.Instance.StringToVector3(user.Vector));

                        // 生成して初期化が済んだらリストに追加
                        m_guestList.Add(guest.GetComponent<Guest>());
                    }
                }));
        }
#endif

        // 壁を非表示にする
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_T").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // ゲームオブジェクト取得
        m_mainCamera = GameObject.Find("MainCamera_Game");
        m_player = GameObject.Find("Player");
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        // フラグOFF
        m_isPause = false;
        m_isEndGame = false;

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

    private void Update()
    {
        if (TopSceneDirector.Instance == null) return;
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;

        // カウントダウン
        if (m_isEndAnim && !m_isEndGame)
        {
            m_gameTimer -= Time.deltaTime;
            m_gameTimer = m_gameTimer <= 0 ? 0 : m_gameTimer;
            m_isEndGame = m_gameTimer <= 0 ? true : false;

            // タイマーテキストを更新
            m_UiController.UpdateTextTimer(m_gameTimer);

            if (m_isEndGame) GameOver();
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
    /// 集計したスコア取得
    /// </summary>
    /// <returns></returns>
    int GetResultScore(float time)
    {
        // スコア集計
        int medalCnt = m_isMedal1 ? 1 : 0;
        medalCnt += m_isMedal2 ? 1 : 0;
        int score = (int)(time * 15) + (medalCnt * 500);
        score = score <= 0 ? 0 : score;
        return score;
    }

    /// <summary>
    /// ゲームオーバー処理
    /// </summary>
    public void GameOver()
    {
        SEManager.Instance.PlayGameOverSE();
        m_UiController.SetGameOverUI(m_isMedal1, m_isMedal2, m_gameTimer, GetResultScore(m_gameTimer), false);
    }

    /// <summary>
    /// ゲームクリア処理
    /// </summary>
    public void GameClear()
    {
        float time = m_gameTimer;
        int score = GetResultScore(time);

        // 現在のステージが上限以下＆＆最新のステージをクリアしたかどうか
        bool isUpdateStageID = NetworkManager.Instance.StageID < TopManager.stageMax && NetworkManager.Instance.StageID == TopManager.stageID;

        // メダルを初獲得した||ハイスコアを上回ったかどうか
        bool isUpdateResult = false;
        if (!isUpdateStageID)
        {
            ShowStageResultResponse currentResult = NetworkManager.Instance.StageResults[TopManager.stageID - 1];
            isUpdateResult = !currentResult.IsMedal1 && m_isMedal1
                || !currentResult.IsMedal2 && m_isMedal2
                || currentResult.Score < score;
        }

        // リザルトを更新する必要がある場合
        if (isUpdateStageID || isUpdateResult)
        {
            // ステージクリア処理
            StartCoroutine(NetworkManager.Instance.UpdateStageClear(
                isUpdateStageID,
                new ShowStageResultResponse { 
                    StageID = TopManager.stageID, 
                    IsMedal1 = m_isMedal1, 
                    IsMedal2 = m_isMedal2, 
                    Time = time,
                    Score = score 
                },
                result =>
                {
                    // ステージをクリアしたことにする
                    m_isEndGame = true;

                    // UIをゲームクリア用に設定する
                    m_UiController.SetResultUI(m_isMedal1,m_isMedal2, m_gameTimer, score, true);

                    // 初クリア演出
                    PlayStageClearEffect();
                }));
        }
        else
        {
            // ステージをクリアしたことにする
            m_isEndGame = true;

            // UIをゲームクリア用に設定する
            m_UiController.SetResultUI(m_isMedal1, m_isMedal2,m_gameTimer, score,true);

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

        if (m_cameraController.m_cameraMode == CameraController.CAMERAMODE.ZOOMOUT)
        {
            // 現在のカメラがズームアウト状態の場合
            effectL.transform.localScale *= 2;
            effectL.transform.localPosition *= 2;
            effectR.transform.localScale *= 2;
            effectR.transform.localPosition *= 2;
        }

        SEManager.Instance.PlayStageClearSE();
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
        m_gameTimer -= 2f;
        m_UiController.GetComponent<UiController>().GenerateSubTimeText(2);
        m_player.GetComponent<Player>().ResetPlayer();

        // ゲストの状態もリセット
        foreach(Guest guest in m_guestList)
        {
            guest.ResetGuest();
        }
    }

    public void UpdateMedalFrag(int medarID)
    {
        switch (medarID)
        {
            case 1:
                m_isMedal1 = true;
                break;
            case 2:
                m_isMedal2 = true;
                break;
        }
    }

    /// <summary>
    /// ゲームモード更新
    /// </summary>
    public void UpdateGameMode(GAMEMODE mode)
    {
        GameMode = mode;
    }

    /// <summary>
    /// ゲストの配置情報を取得
    /// </summary>
    /// <returns></returns>
    public UpdateSignalGuestRequest GetGuestEditData()
    {
        return new UpdateSignalGuestRequest()
        {
            SignalID = TopSceneDirector.Instance.DistressSignalID,
            UserID = NetworkManager.Instance.UserID,
            Pos = m_player.transform.position.ToString(),
            Vector = m_player.GetComponent<Player>().VectorKick.ToString()
        };
    }
}
