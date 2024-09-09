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
    [SerializeField] Button m_btnTransition;              // ステージに遷移ボタン
    [SerializeField] Text m_textTransition;               // 遷移ボタンのテキスト
    [SerializeField] GameObject m_btnDestroy;             // 破棄するボタン
    [SerializeField] GameObject m_btnReward;              // 報酬受け取りボタン
    int m_signalID;

    public void UpdateLogBar(UISignalManager signalManager,int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt, bool action,bool is_rewarded)
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
                m_btnReward.SetActive(false);
            }
            else
            {
                m_btnDestroy.SetActive(false);
                m_btnTransition.interactable = false;
                m_textTransition.text = "報酬受取可能";
            }
        }
        else
        {
            m_btnReward.SetActive(false);
        }

        // 遷移イベント設定
        var manager = GameObject.Find("TopManager").GetComponent<TopManager>();
        m_btnTransition.onClick.AddListener(() => signalManager.OnSignalTabButton(0));
        m_btnTransition.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.GUEST, signalID, stageID, action));

    }

    /// <summary>
    /// 報酬受け取りボタン
    /// </summary>
    public void OnRewardButton()
    {
        m_btnDestroy.SetActive(true);
        m_btnReward.SetActive(false);
        m_btnTransition.interactable = true;
        m_textTransition.text = "ステージへ移動";
    }

    /// <summary>
    /// 参加取り消し・ログ削除処理
    /// </summary>
    public void OnDestroyButton()
    {
        // ゲスト削除処理
        StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
            m_signalID,
            NetworkManager.Instance.UserID,
            result =>
            {
                if (!result) return;
                SEManager.Instance.PlayCanselSE();
                Destroy(gameObject);
            }));
    }
}
