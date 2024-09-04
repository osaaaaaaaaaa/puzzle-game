using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalGuestLogBar : MonoBehaviour
{
    [SerializeField] Text m_textDay;                      // ���t
    [SerializeField] Text m_textHostName;                 // �z�X�g��
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textGuestCnt;                 // �Q�X�g�̎Q���l��
    [SerializeField] Button m_btnAction;                  // �X�e�[�W�ɑJ�ځA��V���󂯎��{�^��
    [SerializeField] Text m_textAction;                   // ��̃{�^���̃e�L�X�g
    [SerializeField] Button m_btnDestroy;                 // �j������{�^��
    int m_signalID;

    public void UpdateLogBar(int signalID,DateTime created_at,string hostName, int stageID, int guestCnt, bool action,bool is_rewarded)
    {
        m_textDay.text = created_at.ToString("yyyy/MM/dd HH:mm:ss");
        m_textHostName.text = hostName;
        m_textStageID.text = "" + stageID;
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
        }

        // ��U�����Ȃ��悤�ɂ��Ă���
        m_btnAction.interactable = false;
        m_btnDestroy.interactable = false;
    }
}
