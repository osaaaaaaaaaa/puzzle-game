using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using System;
using System.Linq;

public class GameManager : MonoBehaviour
{
    [SerializeField] LoadingContainer m_loading;

    // ポーズ中かどうか
    public bool m_isPause;

    // UIコントローラー
    [SerializeField] UiController m_UiController;
    // プレイヤー
    GameObject m_player;
    // カメラコントローラ
    CameraController m_cameraController;
    // ステージ
    GameObject m_stage;

    #region リプレイ関係
    List<ReplayData> replayDatas = new List<ReplayData>();
    [SerializeField] GameObject m_buttonReplay;        // リプレイ再生ボタン
    [SerializeField] GameObject m_replayRecorder;      // ホストのリプレイを録画する
    [SerializeField] GameObject m_replayPlayer;        // リプレイを再生する
    bool m_isReplayEnd = true;
    public bool IsReplayEnd { get { return m_isReplayEnd; }set { m_isReplayEnd = value; } }
    #endregion

    #region ゲスト関係
    [SerializeField] GameObject m_guestSetPrefab;
    public List<GameObject> m_guestList { get; private set; }
    #endregion

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
        m_guestList = new List<GameObject>();

        // ゲームモードの初期化
        if (TopSceneDirector.Instance != null)
        {
            // ゲームモードを設定する
            var playMode = TopSceneDirector.Instance.PlayMode;
            var gameMode = playMode != TopSceneDirector.PLAYMODE.GUEST ? GAMEMODE.Play : GAMEMODE.EditDone;
            GameMode = gameMode;
            // ホストで遊ぶ場合はリプレイの録画開始
            m_replayRecorder.SetActive(playMode == TopSceneDirector.PLAYMODE.HOST);

            if (playMode == TopSceneDirector.PLAYMODE.GUEST)
            {
                m_loading.ToggleLoadingUIVisibility(1);

                // ホストのリプレイ情報取得処理
                StartCoroutine(NetworkManager.Instance.GetReplayData(
                    TopSceneDirector.Instance.DistressSignalID,
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);
                        m_buttonReplay.SetActive(true);
                        if (result == null || result.Count == 0)
                        {
                            m_buttonReplay.GetComponent<Button>().interactable = false;
                            return;
                        };

                        replayDatas = result;
                        m_buttonReplay.GetComponent<Button>().interactable = true;
                    }));
            }
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

        if (TopSceneDirector.Instance != null)
        {
            // 前回メダルを取得している場合は表示を変更する 、 リザルトが存在しない場合は必ずfalse
            bool isMedal1 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal1;
            bool isMedal2 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
                false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal2;
            GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(isMedal1);
            GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(isMedal2);

            // プレファブの格納先
            m_stage = GameObject.Find("Stage");
            // リプレイプレイヤーを子オブジェクトにしてセッティングを完了する
            m_replayPlayer.transform.parent = m_stage.transform;
            m_replayPlayer.transform.position = Vector3.zero;

            if (TopSceneDirector.Instance.PlayMode != TopSceneDirector.PLAYMODE.SOLO)
            {
                m_loading.ToggleLoadingUIVisibility(1);
                // 参加しているゲストの配置情報を取得する
                StartCoroutine(NetworkManager.Instance.GetSignalGuest(
                    TopSceneDirector.Instance.DistressSignalID,
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);
                        if (result == null)
                        {
                            // ゲストのプロフィールを表示する
                            m_UiController.InitGuestUI();
                            return;
                        };

                        foreach (ShowSignalGuestResponse user in result)
                        {
                            // 正常に変換できるかチェック , 登録してまだ設置が完了していない場合はスキップ
                            Vector3 pos = NetworkManager.Instance.StringToVector3(user.Pos);
                            Vector3 vec = NetworkManager.Instance.StringToVector3(user.Vector);
                            if (pos == Vector3.zero || vec == Vector3.zero)
                            {
                                m_guestList.Add(null);
                                continue;
                            }


                            if (user.UserID == NetworkManager.Instance.UserID)
                            {
                                // 自分自身の場合は最後に登録した場所へ移動させる
                                GameObject.Find("Player").transform.position = pos;
                                continue;
                            }

                            // ゲストを生成する
                            GameObject guestSet = Instantiate(m_guestSetPrefab, m_stage.transform);
                            guestSet.transform.position = Vector3.zero;
                            GameObject guest = guestSet.transform.GetChild(0).gameObject;
                            guest.GetComponent<Guest>().InitMemberVariable(user.UserName, pos, vec);

                            // 生成して初期化が済んだらリストに追加
                            m_guestList.Add(guest);
                        }

                        // ゲストのプロフィールを表示する
                        m_UiController.InitGuestUI();
                    }));
            }
        }
        else
        {
            // ゲームシーンから始めた場合
            GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(false);
            GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(false);
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

        // プレファブの格納先
        m_stage = GameObject.Find("Stage");
        // リプレイプレイヤーを子オブジェクトにしてセッティングを完了する
        m_replayPlayer.transform.parent = m_stage.transform;
        m_replayPlayer.transform.position = Vector3.zero;

        if (TopSceneDirector.Instance.PlayMode != TopSceneDirector.PLAYMODE.SOLO)
        {
            m_loading.ToggleLoadingUIVisibility(1);
            // 参加しているゲストの配置情報を取得する
            StartCoroutine(NetworkManager.Instance.GetSignalGuest(
                TopSceneDirector.Instance.DistressSignalID,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);
                    if (result == null)
                    {
                        // ゲストのプロフィールを表示する
                        m_UiController.InitGuestUI();
                        return;
                    };

                    foreach (ShowSignalGuestResponse user in result)
                    {
                        // 正常に変換できるかチェック , 登録してまだ設置が完了していない場合はスキップ
                        Vector3 pos = NetworkManager.Instance.StringToVector3(user.Pos);
                        Vector3 vec = NetworkManager.Instance.StringToVector3(user.Vector);
                        if (pos == Vector3.zero || vec == Vector3.zero)
                        {
                            m_guestList.Add(null);
                            continue;
                        }


                        if (user.UserID == NetworkManager.Instance.UserID)
                        {
                                // 自分自身の場合は最後に登録した場所へ移動させる
                                GameObject.Find("Player").transform.position = pos;
                            continue;
                        }

                        // ゲストを生成する
                        GameObject guestSet = Instantiate(m_guestSetPrefab, m_stage.transform);
                        guestSet.transform.position = Vector3.zero;
                        GameObject guest = guestSet.transform.GetChild(0).gameObject;
                        guest.GetComponent<Guest>().InitMemberVariable(user.UserName, pos, vec);

                            // 生成して初期化が済んだらリストに追加
                            m_guestList.Add(guest);
                    }

                        // ゲストのプロフィールを表示する
                        m_UiController.InitGuestUI();
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
        var goal = GameObject.Find("Goal");

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
        if (TopSceneDirector.Instance != null && TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST || m_isPause) return;

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

        if (TopManager.stageID == 1) m_UiController.OnTutorialButton(true);
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

        // メダルを初獲得した||ハイスコアを上回ったかどうかチェック
        bool isUpdateResult = false;
        if (!isUpdateStageID)
        {
            if (NetworkManager.Instance.StageResults.Count < TopManager.stageID)
            {
                // まだリザルトが存在しない場合
                isUpdateResult = true;
            }
            else
            {
                ShowStageResultResponse currentResult = NetworkManager.Instance.StageResults[TopManager.stageID - 1];
                isUpdateResult = !currentResult.IsMedal1 && m_isMedal1
                    || !currentResult.IsMedal2 && m_isMedal2
                    || currentResult.Score < score;
            }
        }

        // リザルトを更新する必要がある場合
        if (isUpdateStageID || isUpdateResult)
        {
            m_loading.ToggleLoadingUIVisibility(2);
            // ステージクリア処理
            StartCoroutine(NetworkManager.Instance.UpdateStageClear(
                isUpdateStageID,
                new ShowStageResultResponse
                {
                    StageID = TopManager.stageID,
                    IsMedal1 = m_isMedal1,
                    IsMedal2 = m_isMedal2,
                    Time = time,
                    Score = score
                },
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    // ステージをクリアしたことにする
                    m_isEndGame = true;

                    // UIをゲームクリア用に設定する
                    m_UiController.SetResultUI(m_isMedal1, m_isMedal2, m_gameTimer, score, true);

                    // 初クリア演出
                    PlayStageClearEffect();
                }));

            // アチーブメント達成状況更新処理 [トータルスコア]
            StartCoroutine(NetworkManager.Instance.UpdateUserAchievement(
                2,
                0,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);
                    if (!result) return;
                }));

            if (isUpdateStageID)
            {
                m_loading.ToggleLoadingUIVisibility(1);
                // アチーブメント達成状況更新処理 [ステージ初回クリア]
                StartCoroutine(NetworkManager.Instance.UpdateUserAchievement(
                    1,
                    TopManager.stageID,
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);
                        if (!result) return;
                    }));
            }

        }
        else
        {
            // ステージをクリアしたことにする
            m_isEndGame = true;

            // UIをゲームクリア用に設定する
            m_UiController.SetResultUI(m_isMedal1, m_isMedal2, m_gameTimer, score, true);

            // 初クリア演出
            PlayStageClearEffect();
        }

        // アイテムを使用している場合
        if (TopManager.isUseItem)
        {
            m_loading.ToggleLoadingUIVisibility(1);
            TopManager.isUseItem = false;
            // 所持アイテム更新処理
            StartCoroutine(NetworkManager.Instance.UpdateUserItem(
                NetworkManager.Instance.GolfClubItemID,
                2,
                -1,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);
                    if (!result) return;
                }));
        }

        // 自身がホストの場合
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.HOST) DistressSignalClear();
    }

    /// <summary>
    /// 救難信号のステージクリア処理
    /// </summary>
    void DistressSignalClear()
    {
        // クリア済みかどうかチェック
        if (!NetworkManager.Instance.dSignalList.Any(item => item.SignalID == TopSceneDirector.Instance.DistressSignalID)) return;

        m_loading.ToggleLoadingUIVisibility(1);

        // ステージクリア処理
        StartCoroutine(NetworkManager.Instance.UpdateDistressSignal(
            TopSceneDirector.Instance.DistressSignalID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
            }));
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
        SEManager.Instance.PlayButtonSE();
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
        SEManager.Instance.PlayButtonSE();
        Initiate.Fade("01_TopScene", Color.black, 1.0f);
    }

    /// <summary>
    /// ゲームリセット
    /// </summary>
    public void OnGameReset()
    {
        SEManager.Instance.PlayButtonSE();

        m_isPause = false;
        m_gameTimer -= 2f;
        m_UiController.GetComponent<UiController>().GenerateSubTimeText(2);
        m_player.GetComponent<Player>().ResetPlayer();

        // ゲストの状態もリセット
        foreach(GameObject guest in m_guestList)
        {
            if (guest == null) continue;
            guest.GetComponent<Guest>().ResetGuest();
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

    /// <summary>
    /// リプレイプレイヤーを生成して再生する
    /// </summary>
    public void StartReplay()
    {
        // リプレイを再生できない場合
        if (!m_isReplayEnd || replayDatas.Count == 0) return;

        m_buttonReplay.GetComponent<Button>().interactable = false;
        m_isReplayEnd = false;
        m_replayPlayer.SetActive(true);
        var rPlayer = m_replayPlayer.GetComponent<ReplayPlayer>();
        rPlayer.StartCoroutine(rPlayer.ReplayCoroutine(this, replayDatas));
    }
}
