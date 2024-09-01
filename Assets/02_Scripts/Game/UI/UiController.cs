using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    #region �p�l��
    [SerializeField] GameObject m_uiPanelGame;      // �Q�[����UI
    [SerializeField] GameObject m_uiPanelTutorial;  // �`���[�g���A����UI
    [SerializeField] GameObject m_uiPanelHome;      // �z�[����ʂɖ߂邩�̊m�F����UI
    [SerializeField] GameObject m_uiPanelResult;    // ���U���g��UI
    #endregion

    #region �{�^��
    [SerializeField] GameObject m_buttonReset;      // ���Z�b�g�{�^��
    [SerializeField] GameObject m_buttonNextStage;  // ���̃X�e�[�W�֐i�ރ{�^��

    [SerializeField] GameObject m_buttonZoomIn;     // �Y�[���C���{�^��
    public GameObject ButtonZoomIn { get { return m_buttonZoomIn; }}
    [SerializeField] GameObject m_buttonZoomOut;    // �Y�[���A�E�g�{�^��
    public GameObject ButtonZoomOut { get { return m_buttonZoomOut; } }
    #endregion

    #region ��A�N�e�B�u�ɂ���UI�V�[���̃I�u�W�F�N�g
    [SerializeField] Image m_uiPanelImage;
    [SerializeField] GameObject m_uiCamera;
    [SerializeField] GameObject m_uiClearCamera;
    #endregion

    #region ����UI
    [SerializeField] GameObject m_uiKeyParent;
    [SerializeField] GameObject m_uiKeyPrefab;
    #endregion

    // �Q�[���}�l�[�W���[
    [SerializeField] GameManager gameManager;

    private void Start()
    {
        // ��A�N�e�B�u�ɂ���
        //m_uiPanelImage.enabled = false;
        m_uiCamera.SetActive(false);
        m_uiClearCamera.SetActive(false);
        m_uiPanelResult.SetActive(false);

        // ����������
        m_buttonReset.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// �Q�[���̃p�l��UI��\���؂�ւ�
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveGameUI(bool isActive)
    {
        m_uiPanelGame.SetActive(isActive);
    }

    /// <summary>
    /// ���Z�b�g�{�^����\���E��\��
    /// </summary>
    /// <param name="isActive">�\���E��\��</param>
    public void SetActiveButtonReset(bool isActive)
    {
        m_buttonReset.SetActive(isActive);
    }

    /// <summary>
    /// ���Z�b�g�{�^���𖳌��E�L�����ɂ���
    /// </summary>
    /// <param name="isActive"></param>
    public void SetInteractableButtonReset(bool isActive)
    {
        m_buttonReset.GetComponent<Button>().interactable = isActive;
    }

    /// <summary>
    /// �`���[�g���A���\���E��\������
    /// </summary>
    public void OnTutorialButton(bool isActive)
    {
        m_uiPanelTutorial.SetActive(isActive);
        m_uiPanelGame.SetActive(!isActive);

        // �p�l�������ꍇ�̓|�[�YOFF�ɂ���
        gameManager.m_isPause = !isActive ? false : true;
    }

    /// <summary>
    /// �z�[����ʂɖ߂邩�̊m�FUI��\���E��\��
    /// </summary>
    public void OnButtoneHome(bool isActive)
    {
        m_uiPanelHome.SetActive(isActive);

        // ���U���g��ʂł͂Ȃ��ꍇ
        if (!m_uiPanelResult.activeSelf)
        {
            m_uiPanelGame.SetActive(!isActive);
        }
        

        // �p�l�������ꍇ�̓|�[�YOFF�ɂ���
        gameManager.m_isPause = !isActive ? false : true;
    }

    /// <summary>
    /// UI���Q�[���N���A�p�ɐݒ肷��
    /// </summary>
    public void SetGameClearUI()
    {
        // �p�l�������U���g�ɐݒ肷��
        m_uiPanelResult.SetActive(true);
        m_uiPanelGame.SetActive(false);
    }

    /// <summary>
    /// ���̃X�e�[�W�ɑJ�ڂ���{�^���𖳌�������
    /// </summary>
    public void DisableNextStageButton()
    {
        m_buttonNextStage.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// ����UI���X�V����
    /// </summary>
    public void UpdateKeyUI(int keyNum)
    {
        switch (keyNum)
        {
            case -1:    // �j������
                Destroy(m_uiKeyParent.transform.GetChild(0).gameObject);
                break;
            case 1:     // �ǉ�����
                Instantiate(m_uiKeyPrefab, m_uiKeyParent.transform.position, Quaternion.Euler(0f, 0f, -45f), m_uiKeyParent.transform);
                break;
        }
    }

    /// <summary>
    /// ���̌����擾����
    /// </summary>
    /// <returns></returns>
    public int GetKeyCount()
    {
        return m_uiKeyParent.transform.childCount;
    }

    /// <summary>
    /// �{�^���ɃJ�[�\����������or�������Ƃ��ɏ���
    /// </summary>
    public void EventPause(bool frag)
    {
        gameManager.m_isPause = frag;
    }
}
