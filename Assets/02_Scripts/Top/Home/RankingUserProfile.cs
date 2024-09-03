using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUserProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ユーザー名
    [SerializeField] Text m_textAchievementTitle;         // 称号名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textScore;                    // スコア
    [SerializeField] Image m_icon;                        // アイコン
    [SerializeField] GameObject m_hertUI;                 // 相互フォロー,フォロワーを示すUI
    [SerializeField] Text m_textRank;                     // 順位
    [SerializeField] Image m_imgRank;                     // 順位
    [SerializeField] List<Sprite> texRanks;               // 1~3位の画像
    [SerializeField] Sprite m_texMyBG;                    // 自身がわかるようにする画像

    /// <summary>
    /// プロフィール情報を更新する
    /// </summary>
    public void UpdateProfile(int rank,bool isMyProfile,string name,string achievementTitle,int stageID,int score,Sprite icon,bool isAgreement)
    {
        if (isMyProfile) GetComponent<Image>().sprite = m_texMyBG;

        m_textUserName.text = name;
        m_textAchievementTitle.text = achievementTitle;
        m_textStageID.text = "" + stageID;
        m_textScore.text = "" + score;
        m_icon.sprite = icon;
        m_hertUI.SetActive(isAgreement);

        if(rank <= 3)
        {
            m_imgRank.enabled = true;
            m_imgRank.sprite = texRanks[rank - 1];
            m_textRank.enabled = false;
        }
        else
        {
            m_imgRank.enabled = false;
            m_textRank.text = "#" + rank;
        }
    }
}
