using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISignalManager : MonoBehaviour
{
    [SerializeField] GameObject m_textEmpty;

    #region 救難信号
    [SerializeField] GameObject m_uiPanelError;              // 通信エラー時のパネル
    [SerializeField] Text m_textError;                       // 通信エラー時のテキスト
    [SerializeField] GameObject m_uiPanelConfirmation;       // 削除確認パネル
    [SerializeField] Text m_textConfirmation;                // 削除確認のテキスト
    [SerializeField] Button m_buttonConfirmation;            // 削除確認のYESボタン
    [SerializeField] List<Sprite> m_texTabs;                 // タブの画像 [1:アクティブな画像,0:非アクティブな画像]
    [SerializeField] GameObject m_tabLog;                    // ログを表示するタブ
    [SerializeField] GameObject m_logMenuBtnParent;          // メニューボタンの親オブジェクト
    [SerializeField] GameObject m_logScloleView;             // ログを表示するビュー
    [SerializeField] GameObject m_barHostLogPrefab;          // ホストのログバー
    [SerializeField] GameObject m_barGuestLogPrefab;         // ゲストのログバー
    [SerializeField] GameObject m_tabRecruiting;             // 救難信号の募集リストを表示するタブ
    [SerializeField] GameObject m_signalScloleView;          // 救難信号の募集を表示するビュー
    [SerializeField] GameObject m_signalPrefab;              // 救難信号の募集のプレファブ
    [SerializeField] Text m_textDataCnt;                     // 取得したデータ数のテキスト
    #endregion

    #region ウィンドウ
    [SerializeField] GameObject m_windowDistressSignal;
    [SerializeField] GameObject m_windowTutrial;
    #endregion

    #region チュートリアル
    [SerializeField] GameObject m_menuTutorial;
    [SerializeField] List<GameObject> m_panelTutrials;
    #endregion

    /// <summary>
    /// ウィンドウの表示モード
    /// </summary>
    enum SHOW_WINDOW_MODE
    {
        DISTRESS_SIGNAL = 0,       // 救難信号のウインドウ
        TUTORIAL,                  // チュートリアルのウインドウ
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

    public void ToggleTextEmpty(string text, bool isVisibility)
    {
        m_textEmpty.SetActive(isVisibility);
        m_textEmpty.GetComponent<Text>().text = text;
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
                        ToggleTextEmpty("募集した履歴が見つかりませんでした。", result == null);
                        m_textDataCnt.text = result == null ? "0/10" : result.Length + "/10";

                        if (result == null) return;

                        foreach (ShowHostLogResponse log in result)
                        {
                            // ログを生成する
                            GameObject logHost = Instantiate(m_barHostLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalHostLogBar>().UpdateLog(
                                this,log.SignalID, log.CreateDay, log.StageID, log.GuestCnt, log.IsStageClear);
                        }
                    }));
                break;
            case SIGNAL_LOG_LIST_MODE.LOG_GUEST:
                // ゲストのログ取得処理
                StartCoroutine(NetworkManager.Instance.GetSignalGuestLogList(
                    result =>
                    {
                        ToggleTextEmpty("参加した履歴が見つかりませんでした。", result == null); 
                        m_textDataCnt.text = result == null ? "0/10" : result.Length + "/10";

                        if (result == null) return;

                        foreach (ShowGuestLogResponse log in result)
                        {
                            // ログを生成する
                            GameObject logHost = Instantiate(m_barGuestLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalGuestLogBar>().UpdateLogBar(this,log.SignalID, log.ElapsedDay,
                                TopManager.TexIcons[log.IconID - 1], log.IsAgreement, log.HostName, log.StageID, log.GuestCnt, log.IsStageClear, log.IsRewarded);
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// 救難信号の募集リストを更新する
    /// </summary>
    void UpdateSignalListUI()
    {
        // 募救難信号のプレファブの格納先を取得する
        GameObject content = m_signalScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

        // 現在、存在する古い救難信号を全て削除する
        foreach (Transform oldProfile in content.transform)
        {
            Destroy(oldProfile.gameObject);
        }

        // ランダムに救難信号取得処理
        StartCoroutine(NetworkManager.Instance.GetRndSignalList(
            result =>
            {
                ToggleTextEmpty("募集が見つかりませんでした。", result == null);
                m_textDataCnt.text = result == null ? "0/10" : result.Length + "/10";

                if (result == null) return;

                // 取得した情報を元に各救難信号を作成する
                foreach (ShowRndSignalResponse signal in result)
                {
                    // 救難信号を生成する
                    GameObject signalBar = Instantiate(m_signalPrefab, content.transform);
                    signalBar.GetComponent<SignalBar>().UpdateSignalBar(this, signal.SignalID, signal.ElapsedDay,
                        TopManager.TexIcons[signal.IconID - 1], signal.IsAgreement, signal.HostName, signal.StageID, signal.GuestCnt);
                }
            }));
    }

    /// <summary>
    /// 救難信号の内容を切り替える
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE参照</param>
    public void OnSignalTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        m_textDataCnt.text = "";
        m_uiPanelError.SetActive(false);
        m_logScloleView.SetActive(false);
        m_signalScloleView.SetActive(false);
        m_logMenuBtnParent.SetActive(false);
        m_windowTutrial.SetActive(false);
        switch (mode)
        {
            case 0: // ログの選択メニューを表示
                m_logMenuBtnParent.SetActive(true);
                m_tabLog.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabRecruiting.GetComponent<Image>().sprite = m_texTabs[0];
                break;
            case 1: // 救難信号の募集リストを表示
                m_signalScloleView.SetActive(true);
                m_tabLog.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabRecruiting.GetComponent<Image>().sprite = m_texTabs[1];

                // 募集一覧を取得
                UpdateSignalListUI();
                break;
        }
    }

    /// <summary>
    /// ログの内容を切り替える
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE</param>
    public void OnSelectMenuLogButton(int mode)
    {
        ToggleTextEmpty("", false);
        m_textDataCnt.text = "";
        m_logMenuBtnParent.SetActive(false);
        m_logScloleView.SetActive(true);
        m_windowTutrial.SetActive(false);
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
    /// ウィンドウを切り替える
    /// </summary>
    /// <param name="mode">SHOW_WINDOW_MODE</param>
    public void ToggleWindowVisibility(int mode)
    {
        ToggleTextEmpty("", false);
        m_textDataCnt.text = "";
        switch (mode)
        {
            case 0: // 救難信号のウインドウ表示
                m_windowTutrial.SetActive(false);
                m_windowDistressSignal.SetActive(true);
                OnSignalTabButton(0);
                break;
            case 1: // チュートリアルのウインドウ表示
                m_windowDistressSignal.SetActive(false);
                m_windowTutrial.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// チュートリアルパネルを表示・非表示にする
    /// </summary>
    /// <param name="panelID">m_panelTutrialsのインデックス番号+1</param>
    public void TogglePanelTutorialVisibility(int panelID)
    {
        m_menuTutorial.SetActive(panelID == 0);

        // IDに0指定で全て非表示にする
        for (int i = 0; i < m_panelTutrials.Count; i++)
        {
            bool isMyID = i + 1 == panelID;
            m_panelTutrials[i].SetActive(isMyID);
        }
    }

    /// <summary>
    /// チュートリアルのパネルを閉じるボタン
    /// </summary>
    public void OnClosePanelTutrialButton()
    {
        TogglePanelTutorialVisibility(0);
        m_windowTutrial.SetActive(true);
    }

    public void ShowPanelError(string error)
    {
        m_textError.text = error;
        m_uiPanelError.SetActive(true);
    }

    public void ShowPanelConfirmationGuest(string text,SignalGuestLogBar logBar)
    {
        m_uiPanelConfirmation.SetActive(true);
        m_textConfirmation.text = text;

        // ログの削除イベントをYesボタンに設定する
        m_buttonConfirmation.onClick.RemoveAllListeners();
        m_buttonConfirmation.onClick.AddListener(() => logBar.OnDestroyButton());
        m_buttonConfirmation.onClick.AddListener(() => HidePanelConfirmation());
    }

    public void ShowPanelConfirmationHost(string text, SignalHostLogBar logBar)
    {
        m_uiPanelConfirmation.SetActive(true);
        m_textConfirmation.text = text;

        // ログの削除イベントをYesボタンに設定する
        m_buttonConfirmation.onClick.RemoveAllListeners();
        m_buttonConfirmation.onClick.AddListener(() => logBar.OnDestroyButton());
        m_buttonConfirmation.onClick.AddListener(() => HidePanelConfirmation());
    }

    public void HidePanelConfirmation()
    {
        m_uiPanelConfirmation.SetActive(false);
    }
}
