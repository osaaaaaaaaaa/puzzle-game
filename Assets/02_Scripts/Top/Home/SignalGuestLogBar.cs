using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalGuestLogBar : MonoBehaviour
{
    [SerializeField] Image m_icon;                        // アイコン
    [SerializeField] GameObject m_heart;                  // 相互フォローのマーク
    [SerializeField] Text m_textDays;                     // 経過日数
    [SerializeField] Text m_textHostName;                 // ホスト名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textGuestCnt;                 // ゲストの参加人数
    [SerializeField] Button m_btnAction;                  // ステージに遷移、報酬を受け取るボタン
    [SerializeField] Text m_textAction;                   // 上のボタンのテキスト
    [SerializeField] Button m_btnDestroy;                 // 破棄するボタン
    int m_signalID;

    public void UpdateLogBar(int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt, bool action,bool is_rewarded)
    {
        m_signalID = signalID;
        m_textDays.text = elapsed_days + "日前";
        m_icon.sprite = icon;
        m_heart.SetActive(isAgreement);
        m_textHostName.text = hostName;
        m_textStageID.text = "ステージ  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;

        if (action)
        {
            if (is_rewarded)
            {
                m_textAction.text = "報酬を受け取る";
                m_btnAction.interactable = true;

                // 報酬受け取りイベント追加
            }
            else
            {
                m_textAction.text = "報酬受取済み";
                m_btnAction.interactable = false;
            }
        }
        else
        {
            m_textAction.text = "ステージへ移動";

            // 遷移イベント設定
            var manager = GameObject.Find("TopManager").GetComponent<TopManager>();
            m_btnAction.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.GUEST, signalID, stageID));
        }

        // 一旦押せないようにしておく
        // m_btnDestroy.interactable = false;
    }

    /// <summary>
    /// 参加取り消し・ログ削除処理
    /// </summary>
    public void OnDestroyButton()
    {
        // ゲスト削除処理
        StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
            m_signalID,
            result =>
            {
                if (!result) return;
                SEManager.Instance.PlayCanselSE();
                Destroy(gameObject);
            }));
    }
}
