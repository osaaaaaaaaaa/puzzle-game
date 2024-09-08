using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalBar : MonoBehaviour
{
    [SerializeField] Image m_icon;                        // �A�C�R��
    [SerializeField] GameObject m_heart;                  // ���݃t�H���[�̃}�[�N
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textHostName;                 // �z�X�g��
    [SerializeField] Text m_textGuestCnt;                 // �Q�X�g�̎Q���l��
    [SerializeField] Text m_textDays;                     // �o�ߓ���
    int m_signalID;

    public void UpdateSignalBar(int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt)
    {
        m_signalID = signalID;
        m_textDays.text = elapsed_days + "���O";
        m_icon.sprite = icon;
        m_heart.SetActive(isAgreement);
        m_textHostName.text = hostName;
        m_textStageID.text = "�X�e�[�W  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;
    }

    /// <summary>
    /// �~��M���̃{�^���������āA�Q�X�g�Ƃ��ĎQ�����鏈��
    /// </summary>
    public void OnSignalBarButton()
    {
        // �Q�X�g�o�^(�~��M���Q��)����
        StartCoroutine(NetworkManager.Instance.UpdateSignalGuest(
            m_signalID,
            Vector3.zero.ToString(),
            Vector3.zero.ToString(),
            result =>
            {
                if (!result) return;
                SEManager.Instance.PlayButtonSE();
                Destroy(gameObject);
            }));
    }
}
