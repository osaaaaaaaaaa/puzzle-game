using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIUserManager : MonoBehaviour
{
    [SerializeField] GameObject m_textEmpty;
    [SerializeField] LoadingContainer m_loading;

    #region ユーザー情報
    [SerializeField] StageButtonController m_stageButtonController;
    [SerializeField] Text m_textUserName;                 // ユーザー名
    [SerializeField] InputField m_inputUserName;          // ユーザー名入力欄
    [SerializeField] List<Text> m_textAchievementTitle;   // 称号名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textScore;                    // ステージID
    [SerializeField] List<Image> m_icons;                 // アイコン
    #endregion

    #region 編集モードで使用するオブジェクト
    [SerializeField] List<Sprite> m_texTabs;    // タブの画像 [1:アクティブな画像,0:非アクティブな画像]
    #region プロフィール
    [SerializeField] GameObject m_editProfileWindow;        // プロフィールの編集ウィンドウ
    [SerializeField] Text m_errorTextProfile;
    #endregion
    #region アイコン
    [SerializeField] GameObject m_editIconWindow;           // アイコン用の編集ウィンドウ
    [SerializeField] GameObject m_iconListParent;           // アイコンボタンの格納先
    [SerializeField] GameObject m_selectIconButtonPrefab;   // アイコンボタンのプレファブ
    #endregion
    #region 称号
    [SerializeField] GameObject m_editTitleWindow;           // 称号用の編集ウィンドウ
    [SerializeField] GameObject m_contentTitle;              // 称号ボタンの格納先
    [SerializeField] GameObject m_selectTitleButtonPrefab;   // 称号ボタンのプレファブ
    #endregion
    #region フォローリスト
    [SerializeField] GameObject m_followUserScrollView;         // フォローリスト
    [SerializeField] GameObject m_recommendedUserScrollView;    // おすすめのユーザーリスト
    [SerializeField] GameObject m_tabFollow;                    // フォローリストを表示するタブ
    [SerializeField] GameObject m_tabCandidate;                 // おすすめのユーザーリストを表示するタブ
    [SerializeField] GameObject m_profileFollowPrefab;          // フォローしているユーザーのプロフィールプレファブ
    [SerializeField] GameObject m_profileRecommendedPrefab;     // おすすめのユーザーのプロフィールプレファブ
    [SerializeField] Text m_followMaxCnt;                       // フォロー最大人数のテキスト
    [SerializeField] Text m_followCntText;                      // フォローしている人数を示すテキスト
    [SerializeField] Text m_errorTextFollow;
    #endregion
    #region ランキング
    [SerializeField] GameObject m_rankingScrollView;                // 全ユーザー内でのランキングビュー
    [SerializeField] GameObject m_followRankingScrollView;          // フォロー内でのランキングビュー
    [SerializeField] GameObject m_tabRanking;                       // 全ユーザー内を表示するタブ
    [SerializeField] GameObject m_tabFollowRanking;                 // フォロー内を表示するタブ
    [SerializeField] GameObject m_buttonRankingPrefab;              // ランキングのプレファブ
    [SerializeField] GameObject m_followRankingPrefab;              // フォロー内ランキングのプレファブ
    [SerializeField] PanelRankingUserFollow m_rankingUserFollow;    // ランキングのユーザーをフォロー・フォロー解除するパネル
    #endregion
    #region アチーブメント一覧
    [SerializeField] Text m_textTotalPoint;                 // 合計所持ポイント
    [SerializeField] GameObject m_taskScrollView;           // タスクのビュー
    [SerializeField] GameObject m_rewardScrollView;         // 報酬のビュー
    [SerializeField] GameObject m_tabTask;                  // タスク一覧を表示するタブ
    [SerializeField] GameObject m_tabReward;                // 報酬一覧を表示するタブ
    [SerializeField] GameObject m_taskBarPrefab;            // タスクのプレファブ
    [SerializeField] GameObject m_rewardBarPrefab;          // 報酬のプレファブ
    [SerializeField] PanelItemDetails m_panelItemDetails;   // アイテム詳細パネルコンポーネント
    #endregion
    #region メールボックス
    [SerializeField] Sprite m_spriteReceivedMail;           // メール開封済みのUI
    [SerializeField] GameObject m_textMailEmpty;            // 空のときに表示する
    [SerializeField] GameObject m_mailScrollView;           // プレファブの格納先
    [SerializeField] GameObject m_mailPrefab;               // メールのプレファブ
    [SerializeField] MailContent m_mailContent;
    #endregion
    #endregion

    [SerializeField] GameObject m_uiMailUnread;             // 未開封のメールがあるかどうかのUI
    [SerializeField] GameObject m_uiRewardUnclaimed;        // 未受け取りのアチーブメント報酬があるかどうかのUI

    /// <summary>
    /// プロフィールの編集モード
    /// </summary>
    public enum EDITMODE
    {
        PROFILE = 0,
        ICON,
        TITLE
    }

    /// <summary>
    /// フォローリストの表示モード
    /// </summary>
    enum FOLLOW_LIST_MODE
    {
        FOLLOW = 0,       // フォローリスト
        RECOMMENDED       // おすすめのユーザー
    }

    /// <summary>
    /// ランキングの表示モード
    /// </summary>
    enum RANKING_MODE
    {
        USERS = 0,        // 全ユーザー内
        FOLLOW            // フォロー内
    }

    void OnEnable()
    {
        m_inputUserName.enabled = false;

        if (TopManager.m_isClickTitle)
        {
            UpdateUserDataUI(false, null);
        }

        if (NetworkManager.Instance == null || NetworkManager.Instance.UserID == 0) return;
        CheckRewardUnclaimed();
    }

    /// <summary>
    /// 未受け取りの報酬があるかどうかチェック
    /// </summary>
    public void CheckRewardUnclaimed()
    {
        // 未開封の受信メールがあるかどうか
        StartCoroutine(NetworkManager.Instance.GetUserMailList(
            result =>
            {
                if (result == null || result.Length == 0)
                {
                    m_uiMailUnread.SetActive(false);
                    return;
                }

                foreach (ShowUserMailResponse mail in result)
                {
                    if (!mail.IsReceived)
                    {
                        m_uiMailUnread.SetActive(true);
                        return;
                    }
                    m_uiMailUnread.SetActive(false);
                }
            }));

        // 末受け取りのアチーブメント報酬があるかどうか
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            6,
            result =>
            {
                int totalPoint = 0;
                if (result == null || result.Length == 0)
                {
                    m_uiRewardUnclaimed.SetActive(false);
                    return;
                }
                totalPoint = result[0].Amount;
                StartCoroutine(NetworkManager.Instance.GetAchievementList(
                    result =>
                    {
                        foreach (ShowAchievementResponse achieve in result)
                        {
                            if (achieve.Type == 3 && !achieve.IsReceivedItem && achieve.AchievedVal <= totalPoint)
                            {
                                m_uiRewardUnclaimed.SetActive(true);
                                return;
                            }

                            m_uiRewardUnclaimed.SetActive(false);
                        }
                    }));

            }));
    }

    public void HideMailUnclaimedUI()
    {
        m_uiMailUnread.SetActive(false);
    }

    public void HideRewardUnclaimedUI()
    {
        m_uiRewardUnclaimed.SetActive(false);
    }

    public void ResetErrorText()
    {
        m_errorTextProfile.text = "";
    }

    public void ToggleTextEmpty(string text, bool isVisibility)
    {
        m_textEmpty.SetActive(isVisibility);
        m_textEmpty.GetComponent<Text>().text = text;
    }

    GameObject DestroyScrollContentChildren(GameObject scroll)
    {
        GameObject content = scroll.transform.GetChild(0).transform.GetChild(0).gameObject;

        // 現在存在する古い子オブジェクトを全て削除する
        foreach (Transform oldProfile in content.transform)
        {
            Destroy(oldProfile.gameObject);
        }

        return content;
    }

    void ChangeHierarchyOrder(List<GameObject> targetList)
    {
        foreach(var targeet in targetList)
        {
            targeet.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    public void UpdateUserDataUI(bool isMoveTopUI, Sequence sequence)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        // ユーザー情報取得処理
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // ボタンを生成する
                m_stageButtonController.GenerateButtons(result.StageID);    // エラーが発生する場合はローカルのユーザーデータ削除すること

                // アイコンを更新する
                foreach (Image img in m_icons)
                {
                    img.sprite = TopManager.TexIcons[result.IconID - 1];
                }

                // ユーザー名を更新する
                m_inputUserName.enabled = true;
                m_inputUserName.text = result.Name;
                m_textUserName.text = result.Name;
                m_inputUserName.enabled = false;

                // アチーブメントのタイトル更新する
                foreach (Text text in m_textAchievementTitle)
                {
                    text.text = result.AchievementTitle;
                }

                // ステージIDを更新する
                m_textStageID.text = "" + result.StageID;

                // スコアを更新する
                m_textScore.text = "" + result.TotalScore;

                if (isMoveTopUI)
                {
                    // 表示するUIをホームへ移動する
                    sequence.Play();
                }
            }));
    }

    /// <summary>
    /// フォローリストを更新する
    /// </summary>
    void UpdateFollowListUI(FOLLOW_LIST_MODE mode)
    {
        m_loading.ToggleLoadingUIVisibility(2);


        // フォローできる最大数を取得
        StartCoroutine(NetworkManager.Instance.GetConstant(
            2,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                if (result == null)
                {
                    m_followMaxCnt.text = "/30";
                    return;
                }
                m_followMaxCnt.text = "/" + result.Constant;
            }
            ));

        switch (mode)
        {
            case FOLLOW_LIST_MODE.FOLLOW:

                // 古いプロフィールを全て削除、格納先を取得する
                GameObject contentFollow = DestroyScrollContentChildren(m_followUserScrollView);

                // フォローリスト取得処理
                StartCoroutine(NetworkManager.Instance.GetFollowList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("ユーザーが見つかりませんでした。", result.Length == 0);
                        m_followCntText.text = "" + result.Length;

                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        foreach (ShowUserFollowResponse user in result)
                        {
                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_profileFollowPrefab, contentFollow.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject, user.UserID,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsAgreement);
                        }
                    }));
                break;
            case FOLLOW_LIST_MODE.RECOMMENDED:

                // 古いプロフィールを全て削除、格納先を取得する
                GameObject contentRecommended = DestroyScrollContentChildren(m_recommendedUserScrollView);

                // おすすめのユーザーリスト取得処理
                StartCoroutine(NetworkManager.Instance.GetRecommendedUserList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        m_textEmpty.SetActive(result.Length == 0);
                        m_textEmpty.GetComponent<Text>().text = result.Length == 0 ? "ユーザーが見つかりませんでした。" : "";

                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        foreach (ShowUserRecommendedResponse user in result)
                        {
                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_profileRecommendedPrefab, contentRecommended.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject, user.UserID,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsFollower);
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// ランキングを更新する
    /// </summary>
    void UpdateRankingUI(RANKING_MODE mode)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        switch (mode)
        {
            case RANKING_MODE.USERS:

                // 古いプロフィールを全て削除、格納先を取得する
                GameObject contentRanking = DestroyScrollContentChildren(m_rankingScrollView);

                // ランキング取得処理
                StartCoroutine(NetworkManager.Instance.GetRankingList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("ユーザーが見つかりませんでした。", result.Length == 0);
                        m_followCntText.text = "" + result.Length;

                        m_rankingUserFollow.IniRankingtUserData(result);

                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        int i = 0;
                        foreach (ShowRankingResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;

                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_buttonRankingPrefab, contentRanking.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i + 1, isMyData,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsAgreement);

                            // フォロー・フォロー解除イベントを設定する
                            int index = new int();
                            index = i;
                            profile.GetComponent<Button>().onClick.AddListener(() => 
                            {
                                SEManager.Instance.PlayButtonSE();
                                m_rankingUserFollow.SetPanelContent(user.UserID, user.Name, index); 
                            });
                            i++;
                        }
                    }));
                break;
            case RANKING_MODE.FOLLOW:

                // 古いプロフィールを全て削除、格納先を取得する
                GameObject contentRankingFollow = DestroyScrollContentChildren(m_followRankingScrollView);

                // フォロー内でのランキング取得処理
                StartCoroutine(NetworkManager.Instance.GetFollowRankingList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("ユーザーが見つかりませんでした。", result.Length <= 1);
                        if (result.Length <= 1) return;

                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        int i = 0;
                        foreach (ShowUserProfileResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;
                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_followRankingPrefab, contentRankingFollow.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i + 1, isMyData,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsAgreement);
                            i++;
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// アチーブメント一覧を更新する(アチーブメント一覧表示ボタンから呼ぶ)
    /// </summary>
    public void UpdateAchievementUI()
    {
        m_loading.ToggleLoadingUIVisibility(1);
        m_textTotalPoint.text = "pt";

        // 古いアチーブメントを全て削除、格納先を取得する
        GameObject contentTask = DestroyScrollContentChildren(m_taskScrollView);
        GameObject contentReward = DestroyScrollContentChildren(m_rewardScrollView);

        // 合計ポイントを取得する
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            6,
            result =>
            {
                int totalPoint = 0;
                if (result.Length != 0) totalPoint = result[0].Amount;
                m_textTotalPoint.text = totalPoint + "pt";

                // アチーブメント一覧取得処理
                StartCoroutine(NetworkManager.Instance.GetAchievementList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("通信エラーが発生しました。", result.Length == 0);

                        List<GameObject> receivedAchieve = new List<GameObject>();
                        foreach (ShowAchievementResponse achieve in result)
                        {
                            GameObject barAchieve;
                            if (achieve.Type == 1 || achieve.Type == 2)
                            {
                                // タスク一覧を生成する
                                barAchieve = Instantiate(m_taskBarPrefab, contentTask.transform);
                                barAchieve.GetComponent<TaskBar>().UpdateTask(achieve.Text, achieve.AchievedVal, achieve.ProgressVal,
                                    achieve.RewardItem.Amount, achieve.IsReceivedItem);
                            }
                            else
                            {
                                // 報酬一覧を生成する
                                barAchieve = Instantiate(m_rewardBarPrefab, contentReward.transform);
                                barAchieve.GetComponent<RewardBar>().UpdateReward(m_panelItemDetails, achieve.AchievementID,
                                    achieve.RewardItem, achieve.AchievedVal, totalPoint, achieve.IsReceivedItem);
                            }

                            if (achieve.IsReceivedItem) receivedAchieve.Add(barAchieve);
                        }

                        // 報酬受取済みのオブジェクトのヒエラルキー順を変更する
                        ChangeHierarchyOrder(receivedAchieve);
                    }));

            }));
    }

    /// <summary>
    /// メール一覧を更新する(メールのシステムボタンから呼ぶ)
    /// </summary>
    public void UpdateMailUI()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        ToggleTextEmpty("", false);
        m_textMailEmpty.SetActive(false);

        // 古いメールを全て削除、格納先を取得する
        GameObject content = DestroyScrollContentChildren(m_mailScrollView);

        // 受信メールを取得する
        StartCoroutine(NetworkManager.Instance.GetUserMailList(
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                if (result.Length == 0)
                {
                    m_textMailEmpty.SetActive(true);
                    return;
                }

                foreach (ShowUserMailResponse mail in result)
                {
                    // メール一覧(セレクトボタン)を生成する
                    GameObject mailButton = Instantiate(m_mailPrefab, content.transform);
                    if (mail.IsReceived) mailButton.GetComponent<Image>().sprite = m_spriteReceivedMail;
                    mailButton.transform.GetChild(0).GetComponent<Text>().text = mail.Title;
                    mailButton.GetComponent<Button>().onClick.AddListener(() => 
                    {
                        SEManager.Instance.PlayButtonSE();
                        m_mailContent.SetMailContent(mailButton,mail.UserMailID,mail.Title, mail.CreatedAt, mail.ElapsedDay,mail.Text, mail.IsReceived); 
                    });
                }
            }));
    }

    /// <summary>
    /// 現在の編集モードによってUIの親オブジェクトを表示・非表示にする
    /// </summary>
    public void SetActiveParents(EDITMODE currentEditMode)
    {
        ResetErrorText();
        m_editProfileWindow.SetActive(currentEditMode == EDITMODE.PROFILE ? true : false);
        m_editIconWindow.SetActive(currentEditMode == EDITMODE.ICON ? true : false);
        m_editTitleWindow.SetActive(currentEditMode == EDITMODE.TITLE ? true : false);
    }

    /// <summary>
    /// 編集画面を閉じる
    /// </summary>
    public void OnCloseEditWindowButton()
    {
        SetActiveParents(EDITMODE.PROFILE);
    }

    /// <summary>
    /// 名前入力欄にフォーカスを当てる
    /// </summary>
    public void OnEditUserNameButton()
    {
        m_inputUserName.enabled = true;
        m_inputUserName.Select();

        // 空文字にする
        m_inputUserName.text = "";
    }

    /// <summary>
    /// 名前変更
    /// </summary>
    public void OnDoneUserNameButton()
    {
        if (m_inputUserName.text != "")
        {
            m_loading.ToggleLoadingUIVisibility(1);

            // ユーザー更新処理
            StartCoroutine(NetworkManager.Instance.UpdateUser(
                m_inputUserName.text,
                NetworkManager.Instance.TitleID,
                NetworkManager.Instance.StageID,
                NetworkManager.Instance.IconID,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    if (result == null)
                    {
                        // 正常に処理ができた場合、ユーザー名を更新する
                        m_inputUserName.text = NetworkManager.Instance.UserName;
                        m_textUserName.text = NetworkManager.Instance.UserName;
                        m_inputUserName.enabled = false;
                        ResetErrorText();
                        return;
                    }

                    // エラー文が返ってきた場合
                    m_errorTextProfile.text = result.Error;

                    // ユーザー名を元に戻す
                    m_inputUserName.text = NetworkManager.Instance.UserName;
                    m_textUserName.text = NetworkManager.Instance.UserName;
                    m_inputUserName.enabled = false;
                }));
        }
        else
        {
            // ユーザー名を元に戻す
            m_inputUserName.text = NetworkManager.Instance.UserName;
            m_textUserName.text = NetworkManager.Instance.UserName;
            m_inputUserName.enabled = false;
        }
    }

    /// <summary>
    /// アイコンのリストを表示する
    /// </summary>
    public void OnEditIconButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SetActiveParents(EDITMODE.ICON);

        // 現在存在するアイコンの選択ボタンを全て破棄する
        foreach (Transform child in m_iconListParent.transform)
        {
            Destroy(child.gameObject);
        }

        // 所持しているアイコン情報を取得する
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            1,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // 所持しているアイコンのみ生成する
                for (int i = 0; i < result.Length; i++)
                {
                    GameObject button = Instantiate(m_selectIconButtonPrefab, m_iconListParent.transform);
                    button.GetComponent<Image>().sprite = TopManager.TexIcons[result[i].Effect - 1];

                    // アイコン変更イベントを追加する
                    int iconID = new int();   // アドレス更新
                    iconID = result[i].Effect;
                    button.GetComponent<Button>().onClick.AddListener(() => OnDoneIconButton(iconID));
                }
            }));
    }

    /// <summary>
    /// アイコンを変更する
    /// </summary>
    public void OnDoneIconButton(int iconID)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SEManager.Instance.PlayButtonSE();
        // ユーザー更新処理
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            NetworkManager.Instance.UserName,
            NetworkManager.Instance.TitleID,
            NetworkManager.Instance.StageID,
            iconID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // エラー文が返ってきた場合はリターン
                if (result != null) return;

                // アイコンを更新する
                foreach (Image img in m_icons)
                {
                    img.sprite = TopManager.TexIcons[iconID - 1];
                }

                OnCloseEditWindowButton();
            }));
    }

    /// <summary>
    /// 称号のリストを表示する
    /// </summary>
    public void OnEditTitleButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SetActiveParents(EDITMODE.TITLE);

        // 現在存在するアイコンの選択ボタンを全て破棄する
        foreach (Transform child in m_contentTitle.transform)
        {
            Destroy(child.gameObject);
        }

        // 所持している称号情報を取得する
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            2,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // 称号を解除する項目を作成
                GameObject buttonRelease = Instantiate(m_selectTitleButtonPrefab, m_contentTitle.transform);
                buttonRelease.transform.GetChild(0).GetComponent<Text>().text = "×";
                buttonRelease.GetComponent<Button>().onClick.AddListener(() => OnDoneTitleButton(0, ""));

                // 所持している称号のみ生成する
                for (int i = 0; i < result.Length; i++)
                {
                    GameObject button = Instantiate(m_selectTitleButtonPrefab, m_contentTitle.transform);
                    button.transform.GetChild(0).GetComponent<Text>().text = result[i].Name;

                    // 称号変更イベントを追加する
                    int titleID = new int();   // アドレス更新
                    string name = result[i].Name; ;
                    titleID = result[i].ItemID;
                    button.GetComponent<Button>().onClick.AddListener(() => OnDoneTitleButton(titleID, name));
                }
            }));
    }

    /// <summary>
    /// 称号を変更する
    /// </summary>
    public void OnDoneTitleButton(int titleID,string title)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SEManager.Instance.PlayButtonSE();
        // ユーザー更新処理
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            NetworkManager.Instance.UserName,
            titleID,
            NetworkManager.Instance.StageID,
            NetworkManager.Instance.IconID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // エラー文が返ってきた場合はリターン
                if (result != null) return;

                // 称号を更新する
                foreach (Text text in m_textAchievementTitle)
                {
                    text.text = title;
                }

                OnCloseEditWindowButton();
            }));
    }

    /// <summary>
    /// フォローリストの内容を切り替える
    /// </summary>
    /// <param name="mode">FOLLOW_LIST_MODE参照</param>
    public void OnFollowTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        m_errorTextFollow.text = "";

        switch (mode)
        {
            case 0: // フォローリストを表示する
                m_followUserScrollView.SetActive(true);
                m_recommendedUserScrollView.SetActive(false);
                m_tabFollow.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabCandidate.GetComponent<Image>().sprite = m_texTabs[0];

                // フォローしているユーザーの情報を取得する
                UpdateFollowListUI(FOLLOW_LIST_MODE.FOLLOW);
                break;
            case 1: // おすすめのユーザーリストを表示する
                m_followUserScrollView.SetActive(false);
                m_recommendedUserScrollView.SetActive(true);
                m_tabFollow.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabCandidate.GetComponent<Image>().sprite = m_texTabs[1];

                // おすすめのユーザーリスト取得
                UpdateFollowListUI(FOLLOW_LIST_MODE.RECOMMENDED);
                break;
        }
    }

    /// <summary>
    /// フォロー・フォロー解除処理
    /// </summary>
    public void ActionFollow(bool isActive,int user_id,GameObject btnObj)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        if (isActive)
        {
            // フォロー処理
            StartCoroutine(NetworkManager.Instance.StoreUserFollow(
                user_id,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    if (result != null)
                    {
                        // エラー文を表示する
                        m_errorTextFollow.text = result.Error;
                    }
                    else
                    {
                        m_errorTextFollow.text = "";

                        // 成功した場合はテキストを更新する
                        m_followCntText.text = "" + (int.Parse(m_followCntText.text) + 1);
                        btnObj.GetComponent<FollowActionButton>().Invert();
                    }
                }));
        }
        else
        {
            // フォロー解除処理
            StartCoroutine(NetworkManager.Instance.DestroyUserFollow(
                user_id,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    if (!result) return;

                    m_errorTextFollow.text = "";
                    m_followCntText.text = "" + (int.Parse(m_followCntText.text) - 1);
                    btnObj.GetComponent<FollowActionButton>().Invert();
                }));
        }
    }

    /// <summary>
    /// ランキングの内容を切り替える
    /// </summary>
    /// <param name="mode">RANKINF_MODE参照</param>
    public void OnRankingTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        switch (mode)
        {
            case 0: // 全ユーザー内のランキングを表示する
                m_rankingScrollView.SetActive(true);
                m_followRankingScrollView.SetActive(false);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[0];

                // ランキングリストを取得
                UpdateRankingUI(RANKING_MODE.USERS);
                break;
            case 1: // フォロー内のランキングを表示する
                m_rankingScrollView.SetActive(false);
                m_followRankingScrollView.SetActive(true);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // フォロー内でのランキング取得
                UpdateRankingUI(RANKING_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// アチーブメント一覧の内容を切り替える
    /// </summary>
    /// <param name="mode">ACHIEVEMENT_MODE参照</param>
    public void OnAchievementTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        switch (mode)
        {
            case 0: // タスク一覧を表示する
                m_taskScrollView.SetActive(true);
                m_rewardScrollView.SetActive(false);
                m_tabTask.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabReward.GetComponent<Image>().sprite = m_texTabs[0];
                break;
            case 1: // 報酬一覧を表示する
                m_taskScrollView.SetActive(false);
                m_rewardScrollView.SetActive(true);
                m_tabTask.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabReward.GetComponent<Image>().sprite = m_texTabs[1];
                break;
        }
    }
}
