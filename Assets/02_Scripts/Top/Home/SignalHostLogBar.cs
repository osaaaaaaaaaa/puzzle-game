using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalHostLogBar : MonoBehaviour
{
    [SerializeField] GameObject m_uiPanelConfirmation;    // 削除確認パネル

    [SerializeField] Text m_textDay;                      // 日付
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textGuestCnt;                 // ゲストの参加人数
    [SerializeField] Button m_btnAction;                  // ステージに遷移or無効化になるボタン
    [SerializeField] Text m_textAction;                   // 上のボタンのテキスト
    [SerializeField] Button m_btnDestroy;                 // 破棄するボタン
    UISignalManager m_signalManager;
    GameObject m_logBar;
    int m_signalID;

    public void UpdateLog(UISignalManager signalManager, int signalID,DateTime created_at, int stageID, int guestCnt, bool isStageClear)
    {
        m_signalManager = signalManager;
        m_logBar = this.gameObject;
        m_signalID = signalID;
        m_textDay.text = created_at.ToString("yyyy/MM/dd HH:mm:ss");
        m_textStageID.text = "ステージ  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;

        if (isStageClear)
        {
            m_textAction.text = "クリア済";
            m_btnAction.interactable = false;
        }
        else
        {
            m_textAction.text = "ステージへ移動";

            // 遷移イベント設定
            var manager = GameObject.Find("TopManager").GetComponent<TopManager>();
            m_btnAction.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.HOST, signalID ,stageID, isStageClear));
            m_btnAction.onClick.AddListener(() => signalManager.OnSignalTabButton(0));
        }

        // 削除確認パネルを表示するイベント設定
        m_btnDestroy.GetComponent<Button>().onClick.
            AddListener(() => m_signalManager.ShowPanelConfirmationHost("募集を取り消しますか？", this));
    }

    /// <summary>
    /// 募集取り消し、募集したログを削除処理
    /// </summary>
    public void OnDestroyButton()
    {
        // 救難信号削除処理
        StartCoroutine(NetworkManager.Instance.DestroyDistressSignal(
            m_signalID,
            result =>
            {
                if (!result) 
                {
                    m_signalManager.ShowPanelError("通信エラーが発生しました");
                    return;
                };
                SEManager.Instance.PlayCanselSE();
                Destroy(m_logBar);
            }));
    }
}
