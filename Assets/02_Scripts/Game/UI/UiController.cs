using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiController : MonoBehaviour
{
    #region パネル
    [SerializeField] GameObject m_uiPanelGame;          // ゲームのUI
    [SerializeField] GameObject m_uiPanelTutorial;      // チュートリアルのUI
    [SerializeField] GameObject m_uiPanelHome;          // ホーム画面に戻るかの確認するUI
    [SerializeField] GameObject m_uiPanelResult;        // リザルトのUI
    [SerializeField] GameObject m_uiPanelGameOver;      // ゲームオーバーのUI
    [SerializeField] GameObject m_uiPanelJsonError;     // 通信エラー時のパネル
    [SerializeField] GameObject m_uiPanelParamError;    // パラメータ異常時のパネル
    #endregion

    #region ボタン
    [SerializeField] GameObject m_buttonReset;      // リセットボタン
    [SerializeField] GameObject m_buttonNextStage;  // 次のステージへ進むボタン
    [SerializeField] GameObject m_buttonZoomIn;     // ズームインボタン
    public GameObject ButtonZoomIn { get { return m_buttonZoomIn; }}
    [SerializeField] GameObject m_buttonZoomOut;    // ズームアウトボタン
    public GameObject ButtonZoomOut { get { return m_buttonZoomOut; } }
    #endregion

    #region ゲスト関係
    [SerializeField] Text m_textEmpty;
    [SerializeField] GameObject m_buttonGuest;         // ゲストのプロフィールを表示するボタン
    [SerializeField] GameObject m_uiPanelGuests;       // ゲストのUIパネル
    [SerializeField] GameObject m_guestScrollContent;  // プレファブの格納先
    [SerializeField] GameObject m_profileGuestPrefab;  // ゲストのプロフィールプレファブ
    [SerializeField] GameObject m_buttonEditDone;      // ゲストの配置決定ボタン
    [SerializeField] GameObject m_buttonEdit;          // ゲストの編集開始ボタン
    [SerializeField] GameObject m_buttonReplay;        // リプレイ再生ボタン
    #endregion

    #region ゲーム
    [SerializeField] Text m_textTimer;
    [SerializeField] GameObject m_textSubTimePrefab;
    [SerializeField] Text m_textStageID;
    #endregion

    #region リザルト
    [SerializeField] List<Image> m_medalContainers;
    [SerializeField] List<Sprite> m_texMedals;
    [SerializeField] Text m_textPlateResult;
    [SerializeField] Text m_textClearTime;
    [SerializeField] Text m_textScore;
    [SerializeField] Image m_imgRank;
    [SerializeField] List<Sprite> m_texRanks;
    #endregion

    #region 非アクティブにするUIシーンのオブジェクト
    [SerializeField] Image m_uiPanelImage;
    [SerializeField] GameObject m_uiCamera;
    [SerializeField] GameObject m_uiClearCamera;
    #endregion

    #region 鍵のUI
    [SerializeField] GameObject m_uiKeyParent;
    [SerializeField] GameObject m_uiKeyPrefab;
    #endregion

    // ゲームマネージャー
    [SerializeField] GameManager m_gameManager;

    private void Start()
    {
        m_textStageID.text = "ステージ " + TopManager.stageID;
        m_textTimer.text = "40:00";

        // ボタンをソロ・ホストで遊ぶときのものに設定
        m_buttonGuest.SetActive(false);
        m_buttonEditDone.SetActive(false);
        m_buttonEdit.SetActive(false);
        m_buttonReplay.SetActive(false);

        // 非アクティブにする
        m_uiCamera.SetActive(false);
        m_uiClearCamera.SetActive(false);
        m_uiPanelResult.SetActive(false);
        m_uiPanelGameOver.SetActive(false);

        // 無効化する
        m_buttonReset.GetComponent<Button>().interactable = false;
    }

    public void InitGuestUI()
    {
        // ソロで遊ぶ場合は処理を終了
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.SOLO) return;

        // ゲストのプロフィール一覧を表示するボタン
        m_buttonGuest.SetActive(true);

        // ゲストのときのUIに変更
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST)
        {
            if (!TopSceneDirector.Instance.IsSignalStageClear)
            {
                // まだホストが救難信号のステージをクリアしていない場合
                m_buttonEditDone.SetActive(true);
                m_buttonEdit.SetActive(true);
            }
            else
            {
                // クリア済みの場合
                m_buttonReset.SetActive(false);
            }

            // ゲームモードを編集完了モードに変更する
            m_gameManager.UpdateGameMode(GameManager.GAMEMODE.EditDone);
            m_buttonEdit.GetComponent<Button>().interactable = true;
            m_buttonEditDone.GetComponent<Button>().interactable = false;
            m_buttonReset.GetComponent<Button>().interactable = false;
        }

        // ゲストのプロフィール取得処理
        StartCoroutine(NetworkManager.Instance.GetSignalUserProfile(
            TopSceneDirector.Instance.DistressSignalID,
            result =>
            {
                m_textEmpty.text = result == null ? "参加しているユーザーが見つかりませんでした。" : "";
                if (result == null) return;

                // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                int i = 0;
                foreach (ShowUserProfileResponse user in result)
                {
                    // ゲストのオブジェクト
                    GameObject guestObj = i < m_gameManager.m_guestList.Count ? m_gameManager.m_guestList[i] : null;

                    // プロフィールを生成する
                    GameObject profile = Instantiate(m_profileGuestPrefab, m_guestScrollContent.transform);
                    profile.GetComponent<GuestProfile>().UpdateProfile(guestObj, user.UserID, user.Name,
                        user.Title, user.StageID, user.TotalScore,
                        TopManager.TexIcons[user.IconID - 1], user.IsAgreement);
                }
            }));
    }

    /// <summary>
    /// ゲストの編集開始ボタン
    /// </summary>
    public void OnGuestEditButton()
    {
        m_gameManager.UpdateGameMode(GameManager.GAMEMODE.Edit);
        m_buttonEdit.GetComponent<Button>().interactable = false;
        m_buttonEditDone.GetComponent<Button>().interactable = true;
        m_buttonReset.GetComponent<Button>().interactable = true;
    }

    /// <summary>
    /// ゲストの配置決定ボタン処理
    /// </summary>
    public void OnGuestEditDoneButton()
    {
        m_gameManager.UpdateGameMode(GameManager.GAMEMODE.EditDone);
        m_gameManager.OnGameReset();

        m_buttonEdit.GetComponent<Button>().interactable = true;
        m_buttonEditDone.GetComponent<Button>().interactable = false;
        m_buttonReset.GetComponent<Button>().interactable = false;

        // ゲストの配置情報を取得する
        var requestData = m_gameManager.GetGuestEditData();
        if (NetworkManager.Instance.StringToVector3(requestData.Vector) == Vector3.zero)
        {
            // 蹴り飛ばすベクトルを設定していない場合はエラー
            TogglePanelParamErrorVisibility(true);
            return;
        }

        // ゲストの配置情報更新処理
        StartCoroutine(NetworkManager.Instance.UpdateSignalGuest(
            requestData.SignalID,
            requestData.Pos,
            requestData.Vector,
            result =>
            {
                if (result != null)
                {
                    ShowPanelJsonError();
                    return;
                };
            }));
    }

    public void OnReplayButton()
    {
        m_gameManager.StartReplay();
    }

    /// <summary>
    /// 救難信号に参加しているユーザーのプロフィール表示・非表示
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveGuestProfile(bool isActive)
    {
        if (isActive) SEManager.Instance.PlayButtonSE();
        if (!isActive) SEManager.Instance.PlayCanselSE();

        m_uiPanelGuests.SetActive(isActive);
        m_uiPanelGame.SetActive(!isActive);

        if (!isActive) EventPause(false);
    }

    /// <summary>
    /// ゲームのパネルUIを表示切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveGameUI(bool isActive)
    {
        m_uiPanelGame.SetActive(isActive);
    }

    /// <summary>
    /// リセットボタンを表示・非表示
    /// </summary>
    /// <param name="isActive">表示・非表示</param>
    public void SetActiveButtonReset(bool isActive)
    {
        m_buttonReset.SetActive(isActive);
    }

    /// <summary>
    /// リセットボタンを無効・有効化にする
    /// </summary>
    /// <param name="isActive"></param>
    public void SetInteractableButtonReset(bool isActive)
    {
        m_buttonReset.GetComponent<Button>().interactable = isActive;
    }

    /// <summary>
    /// チュートリアル表示・非表示処理
    /// </summary>
    public void OnTutorialButton(bool isActive)
    {
        m_uiPanelTutorial.SetActive(isActive);
        m_uiPanelGame.SetActive(!isActive);

        EventPause(isActive);
    }

    /// <summary>
    /// ホーム画面に戻るかの確認UIを表示・非表示
    /// </summary>
    public void OnButtoneHome(bool isActive)
    {
        m_uiPanelHome.SetActive(isActive);

        // リザルト画面ではない場合
        if (!m_uiPanelResult.activeSelf)
        {
            m_uiPanelGame.SetActive(!isActive);
        }


        // パネルを閉じる場合はポーズOFFにする
        if (!isActive) EventPause(false);
    }

    /// <summary>
    /// ゲームオーバー時のUIを表示する
    /// </summary>
    public void SetGameOverUI(bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        m_uiPanelTutorial.SetActive(false);
        m_uiPanelGuests.SetActive(false);
        m_uiPanelHome.SetActive(false);
        m_uiPanelGame.SetActive(false);
        m_uiPanelGameOver.SetActive(true);

        m_uiPanelGameOver.GetComponent<GameOverUI>().PlayAnim(this,isMedal1,isMedal2,time,score,isStageClear);
    }

    /// <summary>
    /// リザルトのUIを表示する
    /// </summary>
    public void SetResultUI(bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        // ホストの状態でクリアした場合は非アクティブ化
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.HOST) m_buttonNextStage.SetActive(false);

        // メダルのUIを更新する
        if (isMedal1) m_medalContainers[0].sprite = m_texMedals[0];
        if (isMedal2) m_medalContainers[1].sprite = m_texMedals[1];

        // クリアタイム表記
        string text = "" + Mathf.Floor(time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textClearTime.text = text.Insert(2, ":");

        // スコアを表記
        m_textScore.text = "" + score;

        // 評価を表記
        m_imgRank.color = new Color(1, 1, 1, 1);
        if (isStageClear) m_imgRank.sprite = TopManager.GetScoreRank(m_texRanks, score);
        if (!isStageClear) m_imgRank.sprite = m_texRanks[m_texRanks.Count - 1];

        // クリアしたかどうかで動的に設定
        m_textPlateResult.text = isStageClear ? "ステージクリア！" : "しっぱい...";
        // クリア済みのステージの場合はボタンを押せるようにする、そうでない場合は押せないようにする
        m_buttonNextStage.GetComponent<Button>().interactable = NetworkManager.Instance.StageResults.Count < TopManager.stageID ?
            false : true;

        // パネルをリザルトに設定する
        m_uiPanelTutorial.SetActive(false);
        m_uiPanelGuests.SetActive(false);
        m_uiPanelHome.SetActive(false);
        m_uiPanelGameOver.SetActive(false);
        m_uiPanelResult.SetActive(true);
        m_uiPanelGame.SetActive(false);
    }

    /// <summary>
    /// 次のステージに遷移するボタンを無効化する
    /// </summary>
    public void DisableNextStageButton()
    {
        m_buttonNextStage.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// 鍵のUIを更新する
    /// </summary>
    public void UpdateKeyUI(int keyNum)
    {
        switch (keyNum)
        {
            case -1:    // 破棄する
                Destroy(m_uiKeyParent.transform.GetChild(0).gameObject);
                break;
            case 1:     // 追加する
                Instantiate(m_uiKeyPrefab, m_uiKeyParent.transform.position, Quaternion.Euler(0f, 0f, -45f), m_uiKeyParent.transform);
                break;
        }
    }

    /// <summary>
    /// 鍵の個数を取得する
    /// </summary>
    /// <returns></returns>
    public int GetKeyCount()
    {
        return m_uiKeyParent.transform.childCount;
    }

    /// <summary>
    /// タイマーテキストを更新
    /// </summary>
    public void UpdateTextTimer(float time)
    {
        string text = "" + Mathf.Floor(time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textTimer.text = text.Insert(2,":");
    }

    /// <summary>
    /// 減った時間を表示するテキストを生成する
    /// </summary>
    public void GenerateSubTimeText(int subTime)
    {
        GameObject subTimeObj = Instantiate(m_textSubTimePrefab, m_uiPanelGame.transform);
        subTimeObj.GetComponent<Text>().text = "-" + subTime;

        // 徐々に透明・移動して削除
        var sequence = DOTween.Sequence();
        sequence.Append(subTimeObj.transform.DOLocalMoveY(487, 1f).SetEase(Ease.Linear))
            .Join(subTimeObj.GetComponent<Text>().DOFade(0,1).OnComplete(() => { Destroy(subTimeObj.gameObject); }));
        sequence.Play();
    }

    /// <summary>
    /// 通信エラー時のパネルを表示する
    /// </summary>
    public void ShowPanelJsonError()
    {
        SetActiveGameUI(false);
        m_uiPanelJsonError.SetActive(true);
    }

    /// <summary>
    /// パラメータ異常時のパネルを表示・非表示処理
    /// </summary>
    public void TogglePanelParamErrorVisibility(bool isActive)
    {
        SetActiveGameUI(!isActive);
        m_uiPanelParamError.SetActive(isActive);
    }

    /// <summary>
    /// ボタンにカーソルが入ったor抜けたときに処理
    /// </summary>
    public void EventPause(bool frag)
    {
        m_gameManager.m_isPause = frag;
    }
}
