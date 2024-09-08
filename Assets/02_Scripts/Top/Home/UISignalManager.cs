using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISignalManager : MonoBehaviour
{
    #region ユーザー情報
    [SerializeField] List<Sprite> m_texIcons;             // アイコン画像
    #endregion

    #region 救難信号
    [SerializeField] List<Sprite> m_texTabs;                 // タブの画像 [1:アクティブな画像,0:非アクティブな画像]
    [SerializeField] GameObject m_tabLog;                    // ログを表示するタブ
    [SerializeField] GameObject m_logMenuBtnParent;          // メニューボタンの親オブジェクト
    [SerializeField] GameObject m_logScloleView;             // ログを表示するビュー
    [SerializeField] GameObject m_barHostLogPrefab;          // ホストのログバー
    [SerializeField] GameObject m_barGuestLogPrefab;         // ゲストのログバー
    [SerializeField] GameObject m_tabRecruiting;             // 救難信号の募集リストを表示するタブ
    [SerializeField] GameObject m_signalScloleView;          // 救難信号の募集を表示するビュー
    [SerializeField] GameObject m_signalPrefab;              // 救難信号の募集のプレファブ
    #endregion

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
                                this,log.SignalID, log.CreateDay, log.StageID, log.GuestCnt, log.IsStageClear);
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
                            logHost.GetComponent<SignalGuestLogBar>().UpdateLogBar(this,log.SignalID, log.ElapsedDay,
                                m_texIcons[log.IconID - 1], log.IsAgreement, log.HostName, log.StageID, log.GuestCnt, log.IsStageClear, log.IsRewarded);
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
                if (result == null) return;

                // 取得した情報を元に各救難信号を作成する
                foreach (ShowRndSignalResponse signal in result)
                {
                    // 救難信号を生成する
                    GameObject signalBar = Instantiate(m_signalPrefab, content.transform);
                    signalBar.GetComponent<SignalBar>().UpdateSignalBar(signal.SignalID, signal.ElapsedDay,
                        m_texIcons[signal.IconID - 1], signal.IsAgreement, signal.HostName, signal.StageID, signal.GuestCnt);
                }
            }));
    }

    /// <summary>
    /// 救難信号の内容を切り替える
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE参照</param>
    public void OnSignalTabButton(int mode)
    {
        m_logScloleView.SetActive(false);
        m_signalScloleView.SetActive(false);
        m_logMenuBtnParent.SetActive(false);
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
}
