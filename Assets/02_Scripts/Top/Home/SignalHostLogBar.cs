using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SignalHostLogBar : MonoBehaviour
{
    [SerializeField] Text m_textDay;                      // ���t
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textGuestCnt;                 // �Q�X�g�̎Q���l��
    [SerializeField] Button m_btnAction;                  // �X�e�[�W�ɑJ��or�������ɂȂ�{�^��
    [SerializeField] Text m_textAction;                   // ��̃{�^���̃e�L�X�g
    [SerializeField] Button m_btnDestroy;                 // �j������{�^��
    int m_signalID;

    public void UpdateLog(UISignalManager signalManager, int signalID,DateTime created_at, int stageID, int guestCnt, bool isStageClear)
    {
        m_signalID = signalID;
        m_textDay.text = created_at.ToString("yyyy/MM/dd HH:mm:ss");
        m_textStageID.text = "�X�e�[�W  " + stageID;
        m_textGuestCnt.text = "" + guestCnt;

        if (isStageClear)
        {
            m_textAction.text = "�N���A��";
            m_btnAction.interactable = false;
        }
        else
        {
            m_textAction.text = "�X�e�[�W�ֈړ�";

            // �J�ڃC�x���g�ݒ�
            var manager = GameObject.Find("TopManager").GetComponent<TopManager>();
            m_btnAction.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.HOST, signalID ,stageID));
            m_btnAction.onClick.AddListener(() => signalManager.OnSignalTabButton(0));
        }
    }

    /// <summary>
    /// ��W�������A��W�������O���폜����
    /// </summary>
    public void OnDestroyButton()
    {
        // �~��M���폜����
        StartCoroutine(NetworkManager.Instance.DestroyDistressSignal(
            m_signalID,
            result =>
            {
                if (!result) return;
                SEManager.Instance.PlayCanselSE();
                Destroy(gameObject);
            }));
    }
}
