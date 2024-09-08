using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuestProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ���[�U�[��
    [SerializeField] Text m_textAchievementTitle;         // �̍���
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textScore;                    // �X�R�A
    [SerializeField] Image m_icon;                        // �A�C�R��
    [SerializeField] GameObject m_hertUI;                 // ���݃t�H���[,�t�H�����[������UI
    int m_userID;

    /// <summary>
    /// �v���t�B�[�������X�V����
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
        // �Q�X�g�폜����
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
