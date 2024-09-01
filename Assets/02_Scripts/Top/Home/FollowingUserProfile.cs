using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FollowingUserProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ���[�U�[��
    [SerializeField] Text m_textAchievementTitle;         // �̍���
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textScore;                    // �X�R�A
    [SerializeField] Image m_icon;                        // �A�C�R��
    [SerializeField] GameObject m_hertUI;                 // ���݃t�H���[,�t�H�����[������UI
    [SerializeField] GameObject m_actionButton;           // �t�H���[ or �t�H���[��������{�^��

    /// <summary>
    /// �v���t�B�[�������X�V����
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
