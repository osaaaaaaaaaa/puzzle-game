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
    int m_signalID;

    public void UpdateSignalBar(int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt)
    {
        m_textDays.text = elapsed_days + "日前";
        m_icon.sprite = icon;
        m_heart.SetActive(isAgreement);
        m_textHostName.text = hostName;
        m_textStageID.text = "ステージ  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;
    }
}
