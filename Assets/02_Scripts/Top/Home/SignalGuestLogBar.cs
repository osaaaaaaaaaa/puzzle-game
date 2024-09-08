using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalGuestLogBar : MonoBehaviour
{
    [SerializeField] Image m_icon;                        // �A�C�R��
    [SerializeField] GameObject m_heart;                  // ���݃t�H���[�̃}�[�N
    [SerializeField] Text m_textDays;                     // �o�ߓ���
    [SerializeField] Text m_textHostName;                 // �z�X�g��
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textGuestCnt;                 // �Q�X�g�̎Q���l��
    [SerializeField] Button m_btnAction;                  // �X�e�[�W�ɑJ�ځA��V���󂯎��{�^��
    [SerializeField] Text m_textAction;                   // ��̃{�^���̃e�L�X�g
    [SerializeField] Button m_btnDestroy;                 // �j������{�^��
    int m_signalID;

    public void UpdateLogBar(int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt, bool action,bool is_rewarded)
    {
        m_signalID = signalID;
        m_textDays.text = elapsed_days + "���O";
        m_icon.sprite = icon;
        m_heart.SetActive(isAgreement);
        m_textHostName.text = hostName;
        m_textStageID.text = "�X�e�[�W  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;

        if (action)
        {
            if (is_rewarded)
            {
                m_textAction.text = "��V���󂯎��";
                m_btnAction.interactable = true;

                // ��V�󂯎��C�x���g�ǉ�
            }
            else
            {
                m_textAction.text = "��V���ς�";
                m_btnAction.interactable = false;
            }
        }
        else
        {
            m_textAction.text = "�X�e�[�W�ֈړ�";

            // �J�ڃC�x���g�ݒ�
            var manager = GameObject.Find("TopManager").GetComponent<TopManager>();
            m_btnAction.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.GUEST, signalID, stageID));
        }

        // ��U�����Ȃ��悤�ɂ��Ă���
        // m_btnDestroy.interactable = false;
    }

    /// <summary>
    /// �Q���������E���O�폜����
    /// </summary>
    public void OnDestroyButton()
    {
        // �Q�X�g�폜����
        StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
            m_signalID,
            result =>
            {
                if (!result) return;
                SEManager.Instance.PlayCanselSE();
                Destroy(gameObject);
            }));
    }
}
