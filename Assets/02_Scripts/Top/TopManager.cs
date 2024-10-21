using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public class TopManager : MonoBehaviour
{
    [SerializeField] GameObject m_nullAuthTokenPanel;
    [SerializeField] Text m_textEmpty;
    [SerializeField] LoadingContainer m_loading;

    [SerializeField] UIUserManager m_uiUserManager;
    [SerializeField] UISignalManager m_uiSignalManager;

    [SerializeField] GameObject m_mainCamera;
    [SerializeField] GameObject m_characterControllerPrefab;
    GameObject m_characterController;   // 生存確認用

    [SerializeField] GameObject m_parent_top;
    [SerializeField] GameObject m_ui_startTextParent;
    [SerializeField] Image m_panelImage;                    // 非表示にするパネルのイメージ
    [SerializeField] Text m_uiUserName;                     // ユーザー名
    [SerializeField] AssetDownLoader m_assetDownLoader;     // アセットダウンローダー
    [SerializeField] GameObject m_boxStage;                 // ステージシーンに入る前のウインドウ
    [SerializeField] Image m_imgSignalButton;               // 救難信号のボタンの画像
    [SerializeField] GameObject m_panelSignalButtonError;   // 救難信号ボタンを押したときに表示されるウインドウ
    bool isOnStageButton;   //ステージシーンに遷移するボタンをクリックしたかどうか

    // システム画面のパネルリスト
    [SerializeField] List<GameObject> m_sys_panelList;
    // システムボタンの連番
    public enum SYSTEM
    {
        PROFILE = 0,
        MAILBOX,
        FOLLOWBOX,
        RANKING,
        D_SIGNAL,
        ACHIEVEMENT
    }

    /// <summary>
    /// タイトルをクリックしたかどうか
    /// </summary>
    public static bool m_isClickTitle { get; private set; } = false;

    /// <summary>
    /// 最大ステージ数
    /// </summary>
    public static int stageMax { get; private set; }

    /// <summary>
    /// 選択したステージID
    /// </summary>
    public static int stageID { get; set; }

    /// <summary>
    /// アイテムを使用しているかどうか
    /// </summary>
    public static bool isUseItem { get; set; }

    /// <summary>
    /// アイコンデザインのリスト
    /// </summary>
    [SerializeField] List<Sprite> m_texIcons;
    public static List<Sprite> TexIcons { get; private set; }

    public enum ScoreRank
    {
        S = 9999,
        A = 1200,
        B = 800,
        C = 500,
        X = 0,
    }

    private void OnEnable()
    {
        isOnStageButton = false;
        isUseItem = false;

        // ユーザー情報を取得済の場合
        if (NetworkManager.Instance.UserID != 0) m_characterController = Instantiate(m_characterControllerPrefab, transform.parent.transform);
    }

    void Start()
    {
        stageID = 0;
        m_panelImage.enabled = false;
        TexIcons = m_texIcons;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_isClickTitle)
        {
            m_isClickTitle = true;

            // アセットバンドルが更新可能かどうかチェック
            m_assetDownLoader.StartCoroutine(m_assetDownLoader.checkCatalog());
        }
    }

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    public void StoreUser()
    {
        // 最大ステージ数を取得
        m_loading.ToggleLoadingUIVisibility(1);
        StartCoroutine(NetworkManager.Instance.GetConstant(
            1,
            result =>
            {
                stageMax = result.Constant;
                m_loading.ToggleLoadingUIVisibility(-1);
            }
            ));

        // ユーザーデータが保存されていない場合
        if (!NetworkManager.Instance.LoadUserData())
        {
            // ユーザー登録処理
            m_loading.ToggleLoadingUIVisibility(1);
            StartCoroutine(NetworkManager.Instance.StoreUser(
                Guid.NewGuid().ToString(),
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);
                    if (result) OnClickTitleWindow();
                }));    // 登録処理後の処理
        }
        else
        {
            // APIトークンの存在チェック
            if (NetworkManager.Instance.AuthToken == null || NetworkManager.Instance.AuthToken == "")
            {
                m_loading.ToggleLoadingUIVisibility(1);

                // トークン生成処理
                StartCoroutine(NetworkManager.Instance.CreateToken(
                    result => {

                        m_loading.ToggleLoadingUIVisibility(-1);

                        if (!result)
                        {
                            m_nullAuthTokenPanel.SetActive(true);
                            return;
                        }
                        GetUserDatas();
                    }
                    ));
            }
            else
            {
                GetUserDatas();
            }
        }
    }

    void GetUserDatas()
    {
        m_loading.ToggleLoadingUIVisibility(3);

        // 所持アイテムを取得する
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            3,
            result => { m_loading.ToggleLoadingUIVisibility(-1); }
            ));

        // 自分が募集中の救難信号を取得する
        StartCoroutine(NetworkManager.Instance.GetDistressSignalList(
            result => { m_loading.ToggleLoadingUIVisibility(-1); }
            ));

        // ユーザー情報を取得する
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result =>
            {
                    // キャラクターコントローラーを生成する
                    if (m_characterController == null) m_characterController = Instantiate(m_characterControllerPrefab, transform.parent.transform);

                    // ステージのリザルト情報を取得する
                    StartCoroutine(NetworkManager.Instance.GetStageResults(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);
                        OnClickTitleWindow();
                    }));
            }
            ));
    }

    /// <summary>
    /// ステージ選択のボタン
    /// </summary>
    public void OnSelectStageButton(int id)
    {
        if (isOnStageButton) return;

        stageID = id;

        // ステージに入る前のウインドウを表示する
        ShowStageResultResponse resultData = NetworkManager.Instance.StageResults.Count < stageID ? null : NetworkManager.Instance.StageResults[stageID - 1];
        m_boxStage.SetActive(true);
        m_boxStage.GetComponent<StageBox>().InitStatus(resultData);
    }

    /// <summary>
    /// ステージシーンに遷移する
    /// </summary>
    public void OnPlayStageButton(TopSceneDirector.PLAYMODE playMode,int signalID, int id, bool isStageClear)
    {
        if (isOnStageButton) return;

        stageID = id == 0 ? stageID : id;   // ソロで遊ぶ場合(id=0)は更新しない
        Debug.Log(stageID);
        TopSceneDirector.Instance.SetPlayMode(playMode, signalID, isStageClear);
        isOnStageButton = true;
        isUseItem = m_boxStage.GetComponent<StageBox>().m_isUseItem;
        m_boxStage.GetComponent<StageBox>().OnCloseButton();
        Destroy(m_characterController);

        // ゲームUIシーンに遷移する
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
    }

    /// <summary>
    /// タイトル画面からホーム画面へ移動する
    /// </summary>
    void OnClickTitleWindow()
    {
        if (isOnStageButton) return;

        // Tween作成
        var sequence = DOTween.Sequence();
        sequence.Join(m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x - 1980f, 0, 0), 0.5f).SetEase(Ease.Linear))
            .Join(m_mainCamera.transform.DOMove(new Vector3(17.83f, 0f, -10f), 0.5f).SetEase(Ease.Linear));

        // ユーザーのプロフィールを更新
        m_uiUserManager.UpdateUserDataUI(true, sequence);

        // 未受け取りの報酬があるかどうかチェック
        m_uiUserManager.CheckRewardUnclaimed();
        m_uiSignalManager.CheckRewardUnclaimed();

        // 救難信号ボタンのカラー変更
        m_imgSignalButton.color = NetworkManager.Instance.IsDistressSignalEnabled ? new Color(1f, 1f, 1f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    /// <summary>
    /// ホーム画面からタイトル画面へ戻る
    /// </summary>
    public void OnBackButtonHome()
    {
        if (isOnStageButton) return;

        m_ui_startTextParent.SetActive(true);

        //m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
         //   .OnComplete(()=> { m_isClickTitle = false; });

        var sequence = DOTween.Sequence();
        sequence.Join(m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(() => { m_isClickTitle = false; }))
            .Join(m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 0.5f).SetEase(Ease.Linear));
        sequence.Play();
    }

    /// <summary>
    /// ホーム画面からシステム画面(プロフィール、メールボックスなど)へ移動する
    /// </summary>
    /// <param name="systemNum">SYSTEM（システムボタンの連番）参照</param>
    public void OnButtonSystemPanel(int systemNum)
    {
        if (isOnStageButton) return;

        // 全てのシステム画面を非表示にする
        foreach (GameObject item in m_sys_panelList)
        {
            item.SetActive(false);
        }

        // 表示処理
        m_sys_panelList[systemNum].SetActive(true);     // 選択したシステム画面

        var sequence = DOTween.Sequence();
        sequence.Join(m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, -1080, 0), 0.5f).SetEase(Ease.Linear))
            .Join(m_mainCamera.transform.DOMove(new Vector3(17.83f, 9.9f, -10f), 0.5f).SetEase(Ease.Linear));
        sequence.Play();

        // 未受け取りのUIを隠す
        if (systemNum == (int)SYSTEM.MAILBOX) m_uiUserManager.HideMailUnclaimedUI();
        if(systemNum == (int)SYSTEM.ACHIEVEMENT) m_uiUserManager.HideRewardUnclaimedUI();
        if(systemNum == (int)SYSTEM.D_SIGNAL) m_uiSignalManager.HideRewardUnclaimedUI();
    }

    /// <summary>
    /// システム画面からホーム画面へ戻る
    /// </summary>
    public void OnBackButtonSystemPanel()
    {
        if (isOnStageButton) return;

        m_textEmpty.text = "";

        // 救難信号ボタンのカラー変更
        m_imgSignalButton.color = NetworkManager.Instance.IsDistressSignalEnabled ? new Color(1f, 1f, 1f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);

        m_uiUserManager.ResetErrorText();

        var sequence = DOTween.Sequence();
        sequence.Join(m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, 0, 0), 0.5f).SetEase(Ease.Linear))
            .Join(m_mainCamera.transform.DOMove(new Vector3(17.83f, 0f, -10f), 0.5f).SetEase(Ease.Linear));
        sequence.Play();
    }

    /// <summary>
    /// 救難信号ボタン処理
    /// </summary>
    public void OnDistressSignalButton()
    {
        if (!NetworkManager.Instance.IsDistressSignalEnabled)
        {
            TogglePanelDistressErrorVisibility(true);
            return;
        }

        OnButtonSystemPanel((int)SYSTEM.D_SIGNAL);

        if (TopSceneDirector.Instance != null && TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.SOLO && !NetworkManager.Instance.IsDistressSignalTutrial)
        {
            NetworkManager.Instance.TutrialViewed();        // チュートリアルを見たことにする
            m_uiSignalManager.ToggleWindowVisibility(1);    // チュートリアル画面を表示する
        }
        else
        {
            // チュートリアル画面を見たことがある場合は救難信号の画面を表示する
            m_uiSignalManager.ToggleWindowVisibility(0);
        }
    }

    public void TogglePanelDistressErrorVisibility(bool isVisibility)
    {
        m_panelSignalButtonError.SetActive(isVisibility);
    }

    /// <summary>
    /// ランクを取得
    /// </summary>
    /// <returns></returns>
    public static Sprite GetScoreRank(List<Sprite> spriteRanks, int score)
    {
        // 呼び出しがXから行われる , spriteRanksは上からS~Xの順で格納されている
        int i = spriteRanks.Count - 1;
        foreach (var value in Enum.GetValues(typeof(TopManager.ScoreRank)))
        {
            if ((int)value > score)
            {
                return spriteRanks[i];
            }
            i--;
        }

        // どれにも当てはまらなかった場合は最低値のランク
        return spriteRanks[spriteRanks.Count - 1];
    }
}
