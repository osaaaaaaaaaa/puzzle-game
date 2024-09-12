using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISignalManager : MonoBehaviour
{
    [SerializeField] GameObject m_textEmpty;

    #region �~��M��
    [SerializeField] GameObject m_uiPanelError;              // �ʐM�G���[���̃p�l��
    [SerializeField] Text m_textError;                       // �ʐM�G���[���̃e�L�X�g
    [SerializeField] GameObject m_uiPanelConfirmation;       // �폜�m�F�p�l��
    [SerializeField] Text m_textConfirmation;                // �폜�m�F�̃e�L�X�g
    [SerializeField] Button m_buttonConfirmation;            // �폜�m�F��YES�{�^��
    [SerializeField] List<Sprite> m_texTabs;                 // �^�u�̉摜 [1:�A�N�e�B�u�ȉ摜,0:��A�N�e�B�u�ȉ摜]
    [SerializeField] GameObject m_tabLog;                    // ���O��\������^�u
    [SerializeField] GameObject m_logMenuBtnParent;          // ���j���[�{�^���̐e�I�u�W�F�N�g
    [SerializeField] GameObject m_logScloleView;             // ���O��\������r���[
    [SerializeField] GameObject m_barHostLogPrefab;          // �z�X�g�̃��O�o�[
    [SerializeField] GameObject m_barGuestLogPrefab;         // �Q�X�g�̃��O�o�[
    [SerializeField] GameObject m_tabRecruiting;             // �~��M���̕�W���X�g��\������^�u
    [SerializeField] GameObject m_signalScloleView;          // �~��M���̕�W��\������r���[
    [SerializeField] GameObject m_signalPrefab;              // �~��M���̕�W�̃v���t�@�u
    [SerializeField] Text m_textDataCnt;                     // �擾�����f�[�^���̃e�L�X�g
    #endregion

    #region �E�B���h�E
    [SerializeField] GameObject m_windowDistressSignal;
    [SerializeField] GameObject m_windowTutrial;
    #endregion

    #region �`���[�g���A��
    [SerializeField] GameObject m_menuTutorial;
    [SerializeField] List<GameObject> m_panelTutrials;
    #endregion

    /// <summary>
    /// �E�B���h�E�̕\�����[�h
    /// </summary>
    enum SHOW_WINDOW_MODE
    {
        DISTRESS_SIGNAL = 0,       // �~��M���̃E�C���h�E
        TUTORIAL,                  // �`���[�g���A���̃E�C���h�E
    }

    /// <summary>
    /// �~��M���̕\�����[�h
    /// </summary>
    enum SIGNAL_LIST_MODE
    {
        LOG_MENU = 0,       // ���O��\������I�����j���[
        RECRUITING,         // ��W�ꗗ
    }

    /// <summary>
    /// ���O�̕\�����[�h
    /// </summary>
    enum SIGNAL_LOG_LIST_MODE
    {
        LOG_HOST = 0,       // �z�X�g�̂Ƃ��̃��O�ꗗ
        LOG_GUEST,          // �Q�X�g�̂Ƃ��̃��O�ꗗ
    }

    public void ToggleTextEmpty(string text, bool isVisibility)
    {
        m_textEmpty.SetActive(isVisibility);
        m_textEmpty.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// ���O���X�V����
    /// </summary>
    void UpdateSignalLogUI(SIGNAL_LOG_LIST_MODE mode)
    {
        // ���O�v���t�@�u�̊i�[����擾����
        GameObject contentLog = m_logScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

        // ���݁A���݂���Â����O��S�č폜����
        foreach (Transform oldProfile in contentLog.transform)
        {
            Destroy(oldProfile.gameObject);
        }

        switch (mode)
        {
            case SIGNAL_LOG_LIST_MODE.LOG_HOST:
                // �z�X�g�̃��O�擾����
                StartCoroutine(NetworkManager.Instance.GetSignalHostLogList(
                    result =>
                    {
                        ToggleTextEmpty("��W����������������܂���ł����B", result == null);
                        m_textDataCnt.text = result == null ? "0/10" : result.Length + "/10";

                        if (result == null) return;

                        foreach (ShowHostLogResponse log in result)
                        {
                            // ���O�𐶐�����
                            GameObject logHost = Instantiate(m_barHostLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalHostLogBar>().UpdateLog(
                                this,log.SignalID, log.CreateDay, log.StageID, log.GuestCnt, log.IsStageClear);
                        }
                    }));
                break;
            case SIGNAL_LOG_LIST_MODE.LOG_GUEST:
                // �Q�X�g�̃��O�擾����
                StartCoroutine(NetworkManager.Instance.GetSignalGuestLogList(
                    result =>
                    {
                        ToggleTextEmpty("�Q������������������܂���ł����B", result == null); 
                        m_textDataCnt.text = result == null ? "0/10" : result.Length + "/10";

                        if (result == null) return;

                        foreach (ShowGuestLogResponse log in result)
                        {
                            // ���O�𐶐�����
                            GameObject logHost = Instantiate(m_barGuestLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalGuestLogBar>().UpdateLogBar(this,log.SignalID, log.ElapsedDay,
                                TopManager.TexIcons[log.IconID - 1], log.IsAgreement, log.HostName, log.StageID, log.GuestCnt, log.IsStageClear, log.IsRewarded);
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// �~��M���̕�W���X�g���X�V����
    /// </summary>
    void UpdateSignalListUI()
    {
        // ��~��M���̃v���t�@�u�̊i�[����擾����
        GameObject content = m_signalScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

        // ���݁A���݂���Â��~��M����S�č폜����
        foreach (Transform oldProfile in content.transform)
        {
            Destroy(oldProfile.gameObject);
        }

        // �����_���ɋ~��M���擾����
        StartCoroutine(NetworkManager.Instance.GetRndSignalList(
            result =>
            {
                ToggleTextEmpty("��W��������܂���ł����B", result == null);
                m_textDataCnt.text = result == null ? "0/10" : result.Length + "/10";

                if (result == null) return;

                // �擾�����������Ɋe�~��M�����쐬����
                foreach (ShowRndSignalResponse signal in result)
                {
                    // �~��M���𐶐�����
                    GameObject signalBar = Instantiate(m_signalPrefab, content.transform);
                    signalBar.GetComponent<SignalBar>().UpdateSignalBar(this, signal.SignalID, signal.ElapsedDay,
                        TopManager.TexIcons[signal.IconID - 1], signal.IsAgreement, signal.HostName, signal.StageID, signal.GuestCnt);
                }
            }));
    }

    /// <summary>
    /// �~��M���̓��e��؂�ւ���
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE�Q��</param>
    public void OnSignalTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        m_textDataCnt.text = "";
        m_uiPanelError.SetActive(false);
        m_logScloleView.SetActive(false);
        m_signalScloleView.SetActive(false);
        m_logMenuBtnParent.SetActive(false);
        m_windowTutrial.SetActive(false);
        switch (mode)
        {
            case 0: // ���O�̑I�����j���[��\��
                m_logMenuBtnParent.SetActive(true);
                m_tabLog.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabRecruiting.GetComponent<Image>().sprite = m_texTabs[0];
                break;
            case 1: // �~��M���̕�W���X�g��\��
                m_signalScloleView.SetActive(true);
                m_tabLog.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabRecruiting.GetComponent<Image>().sprite = m_texTabs[1];

                // ��W�ꗗ���擾
                UpdateSignalListUI();
                break;
        }
    }

    /// <summary>
    /// ���O�̓��e��؂�ւ���
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE</param>
    public void OnSelectMenuLogButton(int mode)
    {
        ToggleTextEmpty("", false);
        m_textDataCnt.text = "";
        m_logMenuBtnParent.SetActive(false);
        m_logScloleView.SetActive(true);
        m_windowTutrial.SetActive(false);
        switch (mode)
        {
            case 0: // ���O�̑I�����j���[��\��
                UpdateSignalLogUI(SIGNAL_LOG_LIST_MODE.LOG_HOST);
                break;
            case 1: // �~��M���̕�W���X�g��\��
                UpdateSignalLogUI(SIGNAL_LOG_LIST_MODE.LOG_GUEST);
                break;
        }
    }

    /// <summary>
    /// �E�B���h�E��؂�ւ���
    /// </summary>
    /// <param name="mode">SHOW_WINDOW_MODE</param>
    public void ToggleWindowVisibility(int mode)
    {
        ToggleTextEmpty("", false);
        m_textDataCnt.text = "";
        switch (mode)
        {
            case 0: // �~��M���̃E�C���h�E�\��
                m_windowTutrial.SetActive(false);
                m_windowDistressSignal.SetActive(true);
                OnSignalTabButton(0);
                break;
            case 1: // �`���[�g���A���̃E�C���h�E�\��
                m_windowDistressSignal.SetActive(false);
                m_windowTutrial.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// �`���[�g���A���p�l����\���E��\���ɂ���
    /// </summary>
    /// <param name="panelID">m_panelTutrials�̃C���f�b�N�X�ԍ�+1</param>
    public void TogglePanelTutorialVisibility(int panelID)
    {
        m_menuTutorial.SetActive(panelID == 0);

        // ID��0�w��őS�Ĕ�\���ɂ���
        for (int i = 0; i < m_panelTutrials.Count; i++)
        {
            bool isMyID = i + 1 == panelID;
            m_panelTutrials[i].SetActive(isMyID);
        }
    }

    /// <summary>
    /// �`���[�g���A���̃p�l�������{�^��
    /// </summary>
    public void OnClosePanelTutrialButton()
    {
        TogglePanelTutorialVisibility(0);
        m_windowTutrial.SetActive(true);
    }

    public void ShowPanelError(string error)
    {
        m_textError.text = error;
        m_uiPanelError.SetActive(true);
    }

    public void ShowPanelConfirmationGuest(string text,SignalGuestLogBar logBar)
    {
        m_uiPanelConfirmation.SetActive(true);
        m_textConfirmation.text = text;

        // ���O�̍폜�C�x���g��Yes�{�^���ɐݒ肷��
        m_buttonConfirmation.onClick.RemoveAllListeners();
        m_buttonConfirmation.onClick.AddListener(() => logBar.OnDestroyButton());
        m_buttonConfirmation.onClick.AddListener(() => HidePanelConfirmation());
    }

    public void ShowPanelConfirmationHost(string text, SignalHostLogBar logBar)
    {
        m_uiPanelConfirmation.SetActive(true);
        m_textConfirmation.text = text;

        // ���O�̍폜�C�x���g��Yes�{�^���ɐݒ肷��
        m_buttonConfirmation.onClick.RemoveAllListeners();
        m_buttonConfirmation.onClick.AddListener(() => logBar.OnDestroyButton());
        m_buttonConfirmation.onClick.AddListener(() => HidePanelConfirmation());
    }

    public void HidePanelConfirmation()
    {
        m_uiPanelConfirmation.SetActive(false);
    }
}
