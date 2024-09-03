using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankingUserProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ���[�U�[��
    [SerializeField] Text m_textAchievementTitle;         // �̍���
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textScore;                    // �X�R�A
    [SerializeField] Image m_icon;                        // �A�C�R��
    [SerializeField] GameObject m_hertUI;                 // ���݃t�H���[,�t�H�����[������UI
    [SerializeField] Text m_textRank;                     // ����
    [SerializeField] Image m_imgRank;                     // ����
    [SerializeField] List<Sprite> texRanks;               // 1~3�ʂ̉摜
    [SerializeField] Sprite m_texMyBG;                    // ���g���킩��悤�ɂ���摜

    /// <summary>
    /// �v���t�B�[�������X�V����
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
