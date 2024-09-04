using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UserController : MonoBehaviour
{
    #region ユーザー情報
    [SerializeField] StageButtonController m_stageButtonController;
    [SerializeField] Text m_textUserName;                 // ユーザー名
    [SerializeField] InputField m_inputUserName;          // ユーザー名入力欄
    [SerializeField] List<Text> m_textAchievementTitle;   // 称号名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textScore;                    // ステージID
    [SerializeField] List<Image> m_icons;                 // アイコン
    [SerializeField] List<Sprite> m_texIcons;             // アイコン画像
    #endregion

    #region 編集モードで使用するオブジェクト
    [SerializeField] List<Sprite> m_texTabs;    // タブの画像 [1:アクティブな画像,0:非アクティブな画像]
    #region プロフィール
    [SerializeField] GameObject m_editProfileWindow;        // プロフィールの編集ウィンドウ
    [SerializeField] Text m_errorTextProfile;
    #endregion
    #region アイコン
    [SerializeField] GameObject m_editIconWindow;           // アイコン用の編集ウィンドウ
    [SerializeField] GameObject m_iconListParent;           // アイコンリスト
    [SerializeField] GameObject m_selectIconButtonPrefab;   // アイコンボタンのプレファブ
    #endregion
    #region フォローリスト
    [SerializeField] GameObject m_followUserScloleView;         // フォローリスト
    [SerializeField] GameObject m_recommendedUserScloleView;    // おすすめのユーザーリスト
    [SerializeField] GameObject m_tabFollow;                    // フォローリストを表示するタブ
    [SerializeField] GameObject m_tabCandidate;                 // おすすめのユーザーリストを表示するタブ
    [SerializeField] GameObject m_profileFollowPrefab;          // フォローしているユーザーのプロフィールプレファブ
    [SerializeField] GameObject m_profileRecommendedPrefab;     // おすすめのユーザーのプロフィールプレファブ
    [SerializeField] Text m_followCntText;                      // フォローしている人数を示すテキスト
    [SerializeField] Text m_errorTextFollow;
    #endregion
    #region ランキング
    [SerializeField] GameObject m_rankingScloleView;            // 全ユーザー内でのランキングビュー
    [SerializeField] GameObject m_followRankingScloleView;      // フォロー内でのランキングビュー
    [SerializeField] GameObject m_tabRanking;                   // 全ユーザー内を表示するタブ
    [SerializeField] GameObject m_tabFollowRanking;             // フォロー内を表示するタブ
    [SerializeField] GameObject m_profileRankingPrefab;         // ランキングに表示するユーザープロフィールプレファブ
    #endregion
    #region 救難信号
    [SerializeField] GameObject m_tabLog;                    // ログを表示するタブ
    [SerializeField] GameObject m_logMenuBtnParent;          // メニューボタンの親オブジェクト
    [SerializeField] GameObject m_logScloleView;             // ログを表示するビュー
    [SerializeField] GameObject m_barHostLogPrefab;          // ホストのログバー
    [SerializeField] GameObject m_barGuestLogPrefab;         // ゲストのログバー
    [SerializeField] GameObject m_tabRecruiting;             // 救難信号の募集リストを表示するタブ
    #endregion
    #endregion

    /// <summary>
    /// プロフィールの編集モード
    /// </summary>
    public enum EDITMODE
    {
        PROFILE = 0,
        ICON,
        ACHIEVE
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
    enum RANKINF_MODE
    {
        USERS = 0,        // 全ユーザー内
        FOLLOW            // フォロー内
    }

    /// <summary>
    /// 救難信号の表示モード
    /// </summary>
    enum SIGNAL_LIST_MODE
    {
        LOG_MENU = 0,       // ログを表示する選択メニュー
        RECRUITING,         // 募集一覧
    }

    /// <summary>
    /// ログの表示モード
    /// </summary>
    enum SIGNAL_LOG_LIST_MODE
    {
        LOG_HOST = 0,       // ホストのときのログ一覧
        LOG_GUEST,          // ゲストのときのログ一覧
    }

    void OnEnable()
    {
        m_inputUserName.enabled = false;

        if (TopManager.m_isClickTitle)
        {
            UpdateUserDataUI(false,null);
        }   
    }

    /// <summary>
    /// ユーザー情報を更新する
    /// </summary>
    public void UpdateUserDataUI(bool isMoveTopUI,GameObject parent_top)
    {
        // ユーザー情報取得処理
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result =>
            {
                // ボタンを生成する
                m_stageButtonController.GenerateButtons(result.StageID);

                // アイコンを更新する
                foreach (Image img in m_icons)
                {
                    img.sprite = m_texIcons[result.IconID - 1];
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
                    parent_top.transform.DOLocalMove(new Vector3(parent_top.transform.localPosition.x - 1980f, 0, 0), 0.5f).SetEase(Ease.Linear);
                }
            }));
    }

    /// <summary>
    /// フォローリストを更新する
    /// </summary>
    void UpdateFollowListUI(FOLLOW_LIST_MODE mode)
    {
        switch (mode)
        {
            case FOLLOW_LIST_MODE.FOLLOW:

                // プロフィールのプレファブの格納先を取得する
                GameObject contentFollow = m_followUserScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // 現在、存在する古いプロフィールを全て削除する
                foreach (Transform oldProfile in contentFollow.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // フォローリスト取得処理
                StartCoroutine(NetworkManager.Instance.GetFollowList(
                    result =>
                    {
                        m_followCntText.text = "" + result.Length;

                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        foreach (ShowUserFollowResponse user in result)
                        {
                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_profileFollowPrefab, contentFollow.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject,user.UserID,
                                user.Name, user.AchievementTitle,user.StageID, user.TotalScore, 
                                m_texIcons[user.IconID - 1], user.IsAgreement);
                        }
                    }));
                break;
            case FOLLOW_LIST_MODE.RECOMMENDED:

                // プロフィールのプレファブの格納先を取得する
                GameObject contentRecommended = m_recommendedUserScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // 現在、存在する古いプロフィールを全て削除する
                foreach (Transform oldProfile in contentRecommended.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // おすすめのユーザーリスト取得処理
                StartCoroutine(NetworkManager.Instance.GetRecommendedUserList(
                    result =>
                    {
                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        foreach (ShowUserRecommendedResponse user in result)
                        {
                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_profileRecommendedPrefab, contentRecommended.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject,user.UserID, 
                                user.Name, user.AchievementTitle,user.StageID, user.TotalScore, 
                                m_texIcons[user.IconID - 1], user.IsFollower);
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// ランキングを更新する
    /// </summary>
    void UpdateRankingUI(RANKINF_MODE mode)
    {
        switch (mode)
        {
            case RANKINF_MODE.USERS:

                // プロフィールのプレファブの格納先を取得する
                GameObject contentRanking = m_rankingScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // 現在、存在する古いプロフィールを全て削除する
                foreach (Transform oldProfile in contentRanking.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // ランキング取得処理
                StartCoroutine(NetworkManager.Instance.GetRankingList(
                    result =>
                    {
                        m_followCntText.text = "" + result.Length;

                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        int i = 0;
                        foreach (ShowRankingResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;

                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_profileRankingPrefab, contentRanking.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i+1, isMyData,
                                user.Name, user.AchievementTitle, user.StageID, user.TotalScore,
                                m_texIcons[user.IconID - 1], user.IsAgreement);
                            i++;
                        }
                    }));
                break;
            case RANKINF_MODE.FOLLOW:

                // プロフィールのプレファブの格納先を取得する
                GameObject contentRecommended = m_followRankingScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // 現在、存在する古いプロフィールを全て削除する
                foreach (Transform oldProfile in contentRecommended.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // おすすめのユーザーリスト取得処理
                StartCoroutine(NetworkManager.Instance.GetFollowRankingList(
                    result =>
                    {
                        // 取得したフォローリストの情報を元に各ユーザーのプロフィールを作成する
                        int i = 0;
                        foreach (ShowRankingResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;
                            // プロフィールを生成する
                            GameObject profile = Instantiate(m_profileRankingPrefab, contentRecommended.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i+1, isMyData,
                                user.Name, user.AchievementTitle, user.StageID, user.TotalScore,
                                m_texIcons[user.IconID - 1], user.IsAgreement);
                            i++;
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// ログを更新する
    /// </summary>
    void UpdateSignalLogUI(SIGNAL_LOG_LIST_MODE mode)
    {
        // ログプレファブの格納先を取得する
        GameObject contentLog = m_logScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

        // 現在、存在する古いログを全て削除する
        foreach (Transform oldProfile in contentLog.transform)
        {
            Destroy(oldProfile.gameObject);
        }

        switch (mode)
        {
            case SIGNAL_LOG_LIST_MODE.LOG_HOST:
                // ホストのログ取得処理
                StartCoroutine(NetworkManager.Instance.GetSignalHostLogList(
                    result =>
                    {
                        if (result == null) return;

                        foreach (ShowHostLogResponse log in result)
                        {
                            // ログを生成する
                            GameObject logHost = Instantiate(m_barHostLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalHostLogBar>().UpdateLog(
                                log.SignalID,log.CreateDay,log.StageID,log.GuestCnt,log.IsStageClear);
                        }
                    }));
                break;
            case SIGNAL_LOG_LIST_MODE.LOG_GUEST:
                // ゲストのログ取得処理
                StartCoroutine(NetworkManager.Instance.GetSignalGuestLogList(
                    result =>
                    {
                        if (result == null) return;

                        foreach (ShowGuestLogResponse log in result)
                        {
                            // ログを生成する
                            GameObject logHost = Instantiate(m_barGuestLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalGuestLogBar>().UpdateLogBar(
                                log.SignalID, log.CreateDay,log.HostName, log.StageID, log.GuestCnt, log.IsStageClear,log.IsRewarded);
                        }
                    }));
                break;
        }
    }

    public void ResetErrorText()
    {
        m_errorTextProfile.text = "";
    }

    /// <summary>
    /// 現在の編集モードによってUIの親オブジェクトを表示・非表示にする
    /// </summary>
    public void SetActiveParents(EDITMODE currentEditMode)
    {
        ResetErrorText();
        m_editProfileWindow.SetActive(currentEditMode == EDITMODE.PROFILE ? true : false);
        m_editIconWindow.SetActive(currentEditMode == EDITMODE.ICON ? true : false);
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
            // ユーザー更新処理
            StartCoroutine(NetworkManager.Instance.UpdateUser(
                m_inputUserName.text,
                NetworkManager.Instance.AchievementID,
                NetworkManager.Instance.StageID,
                NetworkManager.Instance.IconID,
                result =>
                {
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
                // 所持しているアイコンのみ生成する
                for (int i = 0; i < result.Length; i++)
                {
                    GameObject button = Instantiate(m_selectIconButtonPrefab, m_iconListParent.transform);
                    button.GetComponent<Image>().sprite = m_texIcons[result[i].Effect - 1];

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
        // ユーザー更新処理
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            NetworkManager.Instance.UserName,
            NetworkManager.Instance.AchievementID,
            NetworkManager.Instance.StageID,
            iconID,
            result =>
            {
                // エラー文が返ってきた場合はリターン
                if (result != null) return;

                // アイコンを更新する
                foreach (Image img in m_icons)
                {
                    img.sprite = m_texIcons[iconID - 1];
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
        m_errorTextFollow.text = "";

        switch (mode)
        {
            case 0: // フォローリストを表示する
                m_followUserScloleView.SetActive(true);
                m_recommendedUserScloleView.SetActive(false);
                m_tabFollow.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabCandidate.GetComponent<Image>().sprite = m_texTabs[0];

                // フォローしているユーザーの情報を取得する
                UpdateFollowListUI(FOLLOW_LIST_MODE.FOLLOW);
                break;
            case 1: // おすすめのユーザーリストを表示する
                m_followUserScloleView.SetActive(false);
                m_recommendedUserScloleView.SetActive(true);
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
        if (isActive)
        {
            // フォロー処理
            StartCoroutine(NetworkManager.Instance.StoreUserFollow(
                user_id,
                result =>
                {
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
        switch (mode)
        {
            case 0: // 全ユーザー内のランキングを表示する
                m_rankingScloleView.SetActive(true);
                m_followRankingScloleView.SetActive(false);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[0];

                // ランキングリストを取得
                UpdateRankingUI(RANKINF_MODE.USERS);
                break;
            case 1: // フォロー内のランキングを表示する
                m_rankingScloleView.SetActive(false);
                m_followRankingScloleView.SetActive(true);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // フォロー内でのランキング取得
                UpdateRankingUI(RANKINF_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// 救難信号の内容を切り替える
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE参照</param>
    public void OnSignalTabButton(int mode)
    {
        m_logScloleView.SetActive(false);
        switch (mode)
        {
            case 0: // ログの選択メニューを表示
                m_logMenuBtnParent.SetActive(true);
                m_tabLog.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabRecruiting.GetComponent<Image>().sprite = m_texTabs[0];

                break;
            case 1: // 救難信号の募集リストを表示
                //m_tabLog.SetActive(false);
                //m_tabRecruiting.SetActive(true);
                //m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                //m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // 募集一覧を取得
                //UpdateRankingUI(RANKINF_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// ログの内容を切り替える
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE</param>
    public void OnSelectMenuLogButton(int mode)
    {
        m_logMenuBtnParent.SetActive(false);
        m_logScloleView.SetActive(true);
        switch (mode)
        {
            case 0: // ログの選択メニューを表示
                UpdateSignalLogUI(SIGNAL_LOG_LIST_MODE.LOG_HOST);
                break;
            case 1: // 救難信号の募集リストを表示
                UpdateSignalLogUI(SIGNAL_LOG_LIST_MODE.LOG_GUEST);
                break;
        }
    }


    /// <summary>
    /// アチーブメントのリストを表示する
    /// </summary>
    public void OnEditAchieveButton(int mode)
    {
        switch (mode)
        {
            case 0: // ホストのログリストを表示
                m_rankingScloleView.SetActive(true);
                m_followRankingScloleView.SetActive(false);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[0];

                break;
            case 1: // ゲストのログリストを表示
                m_rankingScloleView.SetActive(false);
                m_followRankingScloleView.SetActive(true);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // 募集一覧を取得
                //UpdateRankingUI(RANKINF_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// アチーブメントを変更する
    /// </summary>
    public void OnDoneAchieveButton()
    {

    }
}
