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
    [SerializeField] Button m_btnTransition;              // �X�e�[�W�ɑJ�ڃ{�^��
    [SerializeField] Text m_textTransition;               // �J�ڃ{�^���̃e�L�X�g
    [SerializeField] GameObject m_btnDestroy;             // �j������{�^��
    [SerializeField] GameObject m_btnReward;              // ��V�󂯎��{�^��
    [SerializeField] Sprite m_spriteReward;               // ��V�A�C�e���̉摜
    LoadingContainer m_loading;
    UISignalManager m_signalManager;
    GameObject m_logBar;
    int m_signalID;

    public void UpdateLogBar(UISignalManager signalManager,int signalID, int elapsed_days, Sprite icon, bool isAgreement, string hostName, int stageID, int guestCnt, bool action,bool is_rewarded)
    {
        m_loading = GameObject.Find("LoadingContainer").GetComponent<LoadingContainer>();
        m_signalManager = signalManager;
        m_logBar = this.gameObject;
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
                m_btnReward.SetActive(false);
                m_textTransition.text = "�N���A�ς�";
            }
            else
            {
                m_btnDestroy.SetActive(false);
                m_btnTransition.interactable = false;
                m_textTransition.text = "��V���\";
            }
        }
        else
        {
            m_btnReward.SetActive(false);
        }

        // �J�ڃC�x���g�ݒ�
        var manager = GameObject.Find("TopManager").GetComponent<TopManager>();
        m_btnTransition.onClick.AddListener(() => signalManager.OnSignalTabButton(0));
        m_btnTransition.onClick.AddListener(() => manager.OnPlayStageButton(TopSceneDirector.PLAYMODE.GUEST, signalID, stageID, action));

        // �폜�m�F�p�l����\������C�x���g�ݒ�
        m_btnDestroy.GetComponent<Button>().onClick.
            AddListener(() => m_signalManager.ShowPanelConfirmationGuest("�Q�����������܂����H",this));
    }

    /// <summary>
    /// ��V�󂯎��{�^��
    /// </summary>
    public void OnRewardButton()
    {
        SEManager.Instance.PlayButtonSE();
        m_loading.ToggleLoadingUIVisibility(1);
        // �Q�X�g�̕�V�󂯎�菈��
        StartCoroutine(NetworkManager.Instance.UpdateSignalGuestReward(
            m_signalID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (result == null)
                {
                    m_signalManager.ShowPanelError("�ʐM�G���[���������܂���");
                    return;
                };

                GameObject.Find("ItemDetail").GetComponent<PanelItemDetails>().SetPanelContent("�A�C�e���l��", result.Amount + "�|�C���g�l���I", m_spriteReward);

                m_btnDestroy.SetActive(true);
                m_btnReward.SetActive(false);
                m_btnTransition.interactable = true;
                m_textTransition.text = "�X�e�[�W�ֈړ�";
            }));
    }

    /// <summary>
    /// �Q���������E���O�폜����
    /// </summary>
    public void OnDestroyButton()
    {
        SEManager.Instance.PlayCanselSE();
        m_loading.ToggleLoadingUIVisibility(1);
        // �Q�X�g�폜����
        StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
            m_signalID,
            NetworkManager.Instance.UserID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (!result)
                {
                    m_signalManager.ShowPanelError("�ʐM�G���[���������܂���");
                    return;
                };
                Destroy(m_logBar);
            }));
    }
}
