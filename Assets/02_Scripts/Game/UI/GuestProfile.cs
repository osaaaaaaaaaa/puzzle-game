using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuestProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ユーザー名
    [SerializeField] Text m_textAchievementTitle;         // 称号名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textScore;                    // スコア
    [SerializeField] Image m_icon;                        // アイコン
    [SerializeField] GameObject m_hertUI;                 // 相互フォロー,フォロワーを示すUI
    int m_userID;

    /// <summary>
    /// プロフィール情報を更新する
    /// </summary>
    public void UpdateProfile(int user_id,string name,string achievementTitle,int stageID,int score,Sprite icon,bool isAgreement)
    {
        m_userID = user_id;
        m_textUserName.text = name;
        m_textAchievementTitle.text = achievementTitle;
        m_textStageID.text = "" + stageID;
        m_textScore.text = "" + score;
        m_icon.sprite = icon;
        m_hertUI.SetActive(isAgreement);
    }

    public void OnGuestDestroyButton()
    {
        // ゲスト削除処理
        //StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
        //    m_signalID,
        //    result =>
        //    {
        //        if (!result) return;
        //        SEManager.Instance.PlayCanselSE();
        //        Destroy(gameObject);
        //    }));
    }
}
