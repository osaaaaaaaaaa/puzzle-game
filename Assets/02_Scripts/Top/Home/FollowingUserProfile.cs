using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowingUserProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ユーザー名
    [SerializeField] Text m_textAchievementTitle;         // 称号名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textScore;                    // スコア
    [SerializeField] Image m_icon;                        // アイコン
    [SerializeField] GameObject m_hertUI;                 // 相互フォロー,フォロワーを示すUI
    [SerializeField] GameObject m_actionButton;           // フォロー or フォロー解除するボタン

    /// <summary>
    /// プロフィール情報を更新する
    /// </summary>
    public void UpdateProfile(GameObject userController,int user_id,string name,string achievementTitle,int stageID,int score,Sprite icon,bool isAgreement)
    {
        m_textUserName.text = name;
        m_textAchievementTitle.text = achievementTitle;
        m_textStageID.text = "" + stageID;
        m_textScore.text = "" + score;
        m_icon.sprite = icon;
        m_hertUI.SetActive(isAgreement);
        m_actionButton.GetComponent<FollowActionButton>().Init(userController,user_id);
    }
}
