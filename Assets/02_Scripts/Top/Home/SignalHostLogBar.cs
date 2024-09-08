using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalHostLogBar : MonoBehaviour
{
    [SerializeField] Text m_textDay;                      // 日付
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textGuestCnt;                 // ゲストの参加人数
    [SerializeField] Button m_btnAction;                  // ステージに遷移or無効化になるボタン
    [SerializeField] Text m_textAction;                   // 上のボタンのテキスト
    [SerializeField] Button m_btnDestroy;                 // 破棄するボタン
    int m_signalID;

    public void UpdateLog(UISignalManager signalManager, int signalID,DateTime created_at, int stageID, int guestCnt, bool isStageClear)
    {
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
            m_btnAction.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.HOST, signalID ,stageID));
            m_btnAction.onClick.AddListener(() => signalManager.OnSignalTabButton(0));
        }
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
                if (!result) return;
                SEManager.Instance.PlayCanselSE();
                Destroy(gameObject);
            }));
    }
}
