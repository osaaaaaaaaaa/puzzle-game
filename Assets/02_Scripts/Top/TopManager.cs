using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEngine.AddressableAssets;

public class TopManager : MonoBehaviour
{
    [SerializeField] GameObject m_parent_top;
    [SerializeField] GameObject m_ui_startTextParent;
    [SerializeField] Image m_panelImage;                 // 非表示にするパネルのイメージ
    [SerializeField] Text m_uiUserName;                  // ユーザー名
    [SerializeField] AssetDownLoader m_assetDownLoader;  // アセットダウンローダー
    [SerializeField] GameObject m_boxStage;              // ステージシーンに入る前のウインドウ
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
        D_SIGNAL
    }

    /// <summary>
    /// タイトルをクリックしたかどうか
    /// </summary>
    public static bool m_isClickTitle { get; private set; } = false;

    /// <summary>
    /// 最大ステージ数
    /// </summary>
    public static int stageMax { get; set; }

    /// <summary>
    /// 選択したステージID
    /// </summary>
    public static int stageID { get; set; }

    public enum ScoreRank
    {
        S = 9999,
        A = 1200,
        B = 800,
        C = 500
    }

    private void OnEnable()
    {
        isOnStageButton = false;
    }

    void Start()
    {
        stageID = 0;
        m_panelImage.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_isClickTitle)
        {
            m_isClickTitle = true;

#if !UNITY_EDITOR
            DOTween.Kill(m_ui_startTextParent.transform);
            m_ui_startTextParent.SetActive(false);

            // アセットバンドルが更新可能かどうかチェック
            m_assetDownLoader.StartCoroutine(m_assetDownLoader.checkCatalog());
#else
            StoreUser();
#endif
        }
    }

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    public void StoreUser()
    {
        // ユーザーデータが保存されていない場合
        if (!NetworkManager.Instance.LoadUserData())
        {
            // ユーザー登録処理
            StartCoroutine(NetworkManager.Instance.StoreUser(
                Guid.NewGuid().ToString(),
                result =>
                {
                    if (result) OnClickTitleWindow();
                }));    // 登録処理後の処理
        }
        else
        {
            // ステージのリザルト情報を取得する
            StartCoroutine(NetworkManager.Instance.GetStageResults(
                result =>
                {
                    OnClickTitleWindow();
                }));
        }
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
    public void OnPlayStageButton()
    {
        if (isOnStageButton) return;

        isOnStageButton = true;
        m_boxStage.GetComponent<StageBox>().OnCloseButton();

#if !UNITY_EDITOR
        // ゲームUIシーンに遷移する
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        // ゲームシーンに遷移する
        Initiate.Fade(stageID + "_GameScene", Color.black, 1.0f);
#endif
    }

    /// <summary>
    /// タイトル画面からホーム画面へ移動する
    /// </summary>
    void OnClickTitleWindow()
    {
        if (isOnStageButton) return;

        // 自信の救難信号(募集中)のリストを取得する



        GetComponent<UserController>().UpdateUserDataUI(true, m_parent_top);
    }

    /// <summary>
    /// ホーム画面からタイトル画面へ戻る
    /// </summary>
    public void OnBackButtonHome()
    {
        if (isOnStageButton) return;

        m_ui_startTextParent.SetActive(true);

        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(()=> { m_isClickTitle = false; });
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
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, -1080, 0), 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// システム画面からホーム画面へ戻る
    /// </summary>
    public void OnBackButtonSystemPanel()
    {
        if (isOnStageButton) return;

        GetComponent<UserController>().ResetErrorText();
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, 0, 0), 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// ランクを取得
    /// </summary>
    /// <returns></returns>
    public static Sprite GetScoreRank(List<Sprite> spriteRanks, int score)
    {
        // 呼び出しがCから行われる , spriteRanksは上からS~Cの順で格納されている
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
