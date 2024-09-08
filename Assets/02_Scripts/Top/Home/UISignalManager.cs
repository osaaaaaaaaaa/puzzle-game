using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UISignalManager : MonoBehaviour
{
    #region ���[�U�[���
    [SerializeField] List<Sprite> m_texIcons;             // �A�C�R���摜
    #endregion

    #region �~��M��
    [SerializeField] List<Sprite> m_texTabs;                 // �^�u�̉摜 [1:�A�N�e�B�u�ȉ摜,0:��A�N�e�B�u�ȉ摜]
    [SerializeField] GameObject m_tabLog;                    // ���O��\������^�u
    [SerializeField] GameObject m_logMenuBtnParent;          // ���j���[�{�^���̐e�I�u�W�F�N�g
    [SerializeField] GameObject m_logScloleView;             // ���O��\������r���[
    [SerializeField] GameObject m_barHostLogPrefab;          // �z�X�g�̃��O�o�[
    [SerializeField] GameObject m_barGuestLogPrefab;         // �Q�X�g�̃��O�o�[
    [SerializeField] GameObject m_tabRecruiting;             // �~��M���̕�W���X�g��\������^�u
    [SerializeField] GameObject m_signalScloleView;          // �~��M���̕�W��\������r���[
    [SerializeField] GameObject m_signalPrefab;              // �~��M���̕�W�̃v���t�@�u
    #endregion

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
                        if (result == null) return;

                        foreach (ShowGuestLogResponse log in result)
                        {
                            // ���O�𐶐�����
                            GameObject logHost = Instantiate(m_barGuestLogPrefab, contentLog.transform);
                            logHost.GetComponent<SignalGuestLogBar>().UpdateLogBar(this,log.SignalID, log.ElapsedDay,
                                m_texIcons[log.IconID - 1], log.IsAgreement, log.HostName, log.StageID, log.GuestCnt, log.IsStageClear, log.IsRewarded);
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
                if (result == null) return;

                // �擾�����������Ɋe�~��M�����쐬����
                foreach (ShowRndSignalResponse signal in result)
                {
                    // �~��M���𐶐�����
                    GameObject signalBar = Instantiate(m_signalPrefab, content.transform);
                    signalBar.GetComponent<SignalBar>().UpdateSignalBar(signal.SignalID, signal.ElapsedDay,
                        m_texIcons[signal.IconID - 1], signal.IsAgreement, signal.HostName, signal.StageID, signal.GuestCnt);
                }
            }));
    }

    /// <summary>
    /// �~��M���̓��e��؂�ւ���
    /// </summary>
    /// <param name="mode">SIGNAL_LIST_MODE�Q��</param>
    public void OnSignalTabButton(int mode)
    {
        m_logScloleView.SetActive(false);
        m_signalScloleView.SetActive(false);
        m_logMenuBtnParent.SetActive(false);
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
        m_logMenuBtnParent.SetActive(false);
        m_logScloleView.SetActive(true);
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
}
