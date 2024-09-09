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
    GameObject m_uiPanelError;
    int m_signalID;
    int m_stageID;

    public void UpdateSignalBar(GameObject errorPanel,int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt)
    {
        m_uiPanelError = errorPanel;
        m_signalID = signalID;
        m_stageID = stageID;

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
                SEManager.Instance.PlayButtonSE();
                if (!result)
                {
                    m_uiPanelError.SetActive(true);
                    return;
                };

                // ���������ꍇ
                var signalManager = GameObject.Find("UIDistressSignalManager").GetComponent<UISignalManager>();
                var managerTop = GameObject.Find("TopManager").GetComponent<TopManager>();
                signalManager.OnSignalTabButton(0);
                managerTop.OnPlayStageButton(TopSceneDirector.PLAYMODE.GUEST, m_signalID, m_stageID, false);
            }));
    }
}
