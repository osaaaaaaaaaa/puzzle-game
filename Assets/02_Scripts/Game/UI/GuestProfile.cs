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
    [SerializeField] Button m_buttonDestroy;              // �Q�X�g�폜�{�^��
    [SerializeField] Button m_buttonToggle;               // �Q�X�g�̃I�u�W�F�N�g��\���E��\������{�^��
    [SerializeField] List<Sprite> m_texToggleBtn;         // �g�O���{�^���̉摜 [0:off�̉摜 , 1:on�̉摜]
    [SerializeField] GameObject m_window;                 // �m�F�E�C���h�E
    GameObject m_guestObj;
    int m_userID;

    /// <summary>
    /// �v���t�B�[�������X�V����
    /// </summary>
    public void UpdateProfile(GameObject guestObj,int user_id,string name,string achievementTitle,int stageID,int score,Sprite icon,bool isAgreement)
    {
        // �I�u�W�F�N�g�����݂��Ȃ��ꍇ�͉����Ȃ��悤�ɂ���
        if (guestObj == null) m_buttonToggle.interactable = false;
        m_guestObj = guestObj;
        m_userID = user_id;
        m_textUserName.text = name;
        m_textAchievementTitle.text = achievementTitle;
        m_textStageID.text = "" + stageID;
        m_textScore.text = "" + score;
        m_icon.sprite = icon;
        m_hertUI.SetActive(isAgreement);

        // ���g���Q�X�g�̏ꍇ�͔j���{�^���������Ȃ��悤�ɂ���
        if(TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) m_buttonDestroy.interactable = false;
    }

    /// <summary>
    /// �Q�X�g�폜����
    /// </summary>
    public void OnGuestDestroyButton()
    {
        // �Q�X�g�폜����
        StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
            TopSceneDirector.Instance.DistressSignalID,
            m_userID,
            result =>
            {
                if (!result) return;
                //SEManager.Instance.PlayCanselSE();
                if (m_guestObj != null) Destroy(m_guestObj);
                Destroy(gameObject);
            }));
    }

    /// <summary>
    /// �m�F�E�C���h�E�̕\���E��\��
    /// </summary>
    public void OnToggleWindowVisibility(bool isVisible)
    {
        m_window.SetActive(isVisible);
    }

    /// <summary>
    /// �Q�X�g��\���E��\������
    /// </summary>
    public void OnToggleButtonVisibility()
    {
        // �\���E��\����؂�ւ���
        m_guestObj.SetActive(!m_guestObj.activeSelf);
        m_buttonToggle.GetComponent<Image>().sprite = !m_guestObj.activeSelf ? m_texToggleBtn[0] : m_texToggleBtn[1];

        // �Q�X�g�̋N���\����\���E��\������
        m_guestObj.GetComponent<Guest>().ToggleLineVisibility(m_guestObj.activeSelf);
    }
}
