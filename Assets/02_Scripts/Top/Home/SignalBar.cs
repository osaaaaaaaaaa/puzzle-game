using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalBar : MonoBehaviour
{
    [SerializeField] Image m_icon;                        // アイコン
    [SerializeField] GameObject m_heart;                  // 相互フォローのマーク
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textHostName;                 // ホスト名
    [SerializeField] Text m_textGuestCnt;                 // ゲストの参加人数
    [SerializeField] Text m_textDays;                     // 経過日数
    LoadingContainer m_loading;
    UISignalManager m_signalManager;
    int m_signalID;
    int m_stageID;

    public void UpdateSignalBar(UISignalManager signalManager,int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt)
    {
        m_loading = GameObject.Find("LoadingContainer").GetComponent<LoadingContainer>();
        m_signalManager = signalManager;
        m_signalID = signalID;
        m_stageID = stageID;

        m_textDays.text = elapsed_days + "日前";
        m_icon.sprite = icon;
        m_heart.SetActive(isAgreement);
        m_textHostName.text = hostName;
        m_textStageID.text = "ステージ  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;
    }

    /// <summary>
    /// 救難信号のボタンを押して、ゲストとして参加する処理
    /// </summary>
    public void OnSignalBarButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        // ゲスト登録(救難信号参加)処理
        StartCoroutine(NetworkManager.Instance.UpdateSignalGuest(
            m_signalID,
            Vector3.zero.ToString(),
            Vector3.zero.ToString(),
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                SEManager.Instance.PlayButtonSE();
                if (result != null)
                {
                    m_signalManager.ShowPanelError(result);
                    return;
                };

                // 成功した場合
                var signalManager = GameObject.Find("UIDistressSignalManager").GetComponent<UISignalManager>();
                var managerTop = GameObject.Find("TopManager").GetComponent<TopManager>();
                signalManager.OnSignalTabButton(0);
                managerTop.OnPlayStageButton(TopSceneDirector.PLAYMODE.GUEST, m_signalID, m_stageID, false);
            }));
    }
}
