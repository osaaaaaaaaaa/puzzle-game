using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalGuestLogBar : MonoBehaviour
{
    [SerializeField] Text m_textDay;                      // 日付
    [SerializeField] Text m_textHostName;                 // ホスト名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textGuestCnt;                 // ゲストの参加人数
    [SerializeField] Button m_btnAction;                  // ステージに遷移、報酬を受け取るボタン
    [SerializeField] Text m_textAction;                   // 上のボタンのテキスト
    [SerializeField] Button m_btnDestroy;                 // 破棄するボタン
    int m_signalID;

    public void UpdateLogBar(int signalID,DateTime created_at,string hostName, int stageID, int guestCnt, bool action,bool is_rewarded)
    {
        m_textDay.text = created_at.ToString("yyyy/MM/dd HH:mm:ss");
        m_textHostName.text = hostName;
        m_textStageID.text = "" + stageID;
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
        }

        // 一旦押せないようにしておく
        m_btnAction.interactable = false;
        m_btnDestroy.interactable = false;
    }
}
