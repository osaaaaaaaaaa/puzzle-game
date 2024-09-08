using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiController : MonoBehaviour
{
    #region �p�l��
    [SerializeField] GameObject m_uiPanelGame;      // �Q�[����UI
    [SerializeField] GameObject m_uiPanelTutorial;  // �`���[�g���A����UI
    [SerializeField] GameObject m_uiPanelHome;      // �z�[����ʂɖ߂邩�̊m�F����UI
    [SerializeField] GameObject m_uiPanelResult;    // ���U���g��UI
    [SerializeField] GameObject m_uiPanelGameOver;  // �Q�[���I�[�o�[��UI
    #endregion

    #region �{�^��
    [SerializeField] GameObject m_buttonReset;      // ���Z�b�g�{�^��
    [SerializeField] GameObject m_buttonNextStage;  // ���̃X�e�[�W�֐i�ރ{�^��
    [SerializeField] GameObject m_buttonZoomIn;     // �Y�[���C���{�^��
    public GameObject ButtonZoomIn { get { return m_buttonZoomIn; }}
    [SerializeField] GameObject m_buttonZoomOut;    // �Y�[���A�E�g�{�^��
    public GameObject ButtonZoomOut { get { return m_buttonZoomOut; } }
    #endregion

    #region �Q�X�g�֌W
    [SerializeField] GameObject m_buttonGuest;         // �Q�X�g�̃v���t�B�[����\������{�^��
    [SerializeField] GameObject m_uiPanelGuests;       // �Q�X�g��UI�p�l��
    [SerializeField] GameObject m_guestScrollContent;  // �v���t�@�u�̊i�[��
    [SerializeField] GameObject m_profileGuestPrefab;  // �Q�X�g�̃v���t�B�[���v���t�@�u
    [SerializeField] List<Sprite> m_texIcons;          // �A�C�R���摜
    [SerializeField] GameObject m_buttonEditDone;      // �Q�X�g�̔z�u����{�^��
    [SerializeField] GameObject m_buttonEdit;          // �Q�X�g�̕ҏW�J�n�{�^��
    #endregion

    #region �Q�[��
    [SerializeField] Text m_textTimer;
    [SerializeField] GameObject m_textSubTimePrefab;
    [SerializeField] Text m_textStageID;
    #endregion

    #region ���U���g
    [SerializeField] List<Image> m_medalContainers;
    [SerializeField] List<Sprite> m_texMedals;
    [SerializeField] Text m_textPlateResult;
    [SerializeField] Text m_textClearTime;
    [SerializeField] Text m_textScore;
    [SerializeField] Image m_imgRank;
    [SerializeField] List<Sprite> m_texRanks;
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
    [SerializeField] GameManager m_gameManager;

    private void Start()
    {
        InitGuestUI();

        m_textStageID.text = "�X�e�[�W " + TopManager.stageID;
        m_textTimer.text = "40:00";

        // ��A�N�e�B�u�ɂ���
        m_uiCamera.SetActive(false);
        m_uiClearCamera.SetActive(false);
        m_uiPanelResult.SetActive(false);
        m_uiPanelGameOver.SetActive(false);

        // ����������
        m_buttonReset.GetComponent<Button>().interactable = false;
    }

    void InitGuestUI()
    {
        if (TopSceneDirector.Instance == null)
        {
            m_buttonGuest.SetActive(false);
            m_buttonEditDone.SetActive(false);
            m_buttonEdit.SetActive(false);
            return;
        }

        bool isActive = TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST;
        m_buttonGuest.SetActive(isActive);
        m_buttonEditDone.SetActive(isActive);
        m_buttonEdit.SetActive(isActive);

        if (!isActive) return;

        // �Q�[�����[�h��ҏW�������[�h�ɕύX����
        m_gameManager.UpdateGameMode(GameManager.GAMEMODE.EditDone);
        m_buttonEdit.GetComponent<Button>().interactable = true;
        m_buttonEditDone.GetComponent<Button>().interactable = false;
        m_buttonReset.GetComponent<Button>().interactable = false;

        // �Q�X�g�̃v���t�B�[���擾����
        StartCoroutine(NetworkManager.Instance.GetSignalUserProfile(
            TopSceneDirector.Instance.DistressSignalID,
            result =>
            {
                if (result == null) return;

                // �v���t�B�[������
                // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                foreach (ShowUserProfileResponse user in result)
                {
                    // �v���t�B�[���𐶐�����
                    GameObject profile = Instantiate(m_profileGuestPrefab, m_guestScrollContent.transform);
                    profile.GetComponent<GuestProfile>().UpdateProfile(user.UserID, user.Name,
                        user.AchievementTitle, user.StageID, user.TotalScore,
                        m_texIcons[user.IconID - 1], user.IsAgreement);
                }
            }));
    }

    /// <summary>
    /// �Q�X�g�̕ҏW�J�n�{�^��
    /// </summary>
    public void OnGuestEditButton()
    {
        m_gameManager.UpdateGameMode(GameManager.GAMEMODE.Edit);
        m_buttonEdit.GetComponent<Button>().interactable = false;
        m_buttonEditDone.GetComponent<Button>().interactable = true;
        m_buttonReset.GetComponent<Button>().interactable = true;
    }

    /// <summary>
    /// �Q�X�g�̔z�u����{�^������
    /// </summary>
    public void OnGuestEditDoneButton()
    {
        m_gameManager.UpdateGameMode(GameManager.GAMEMODE.EditDone);
        m_gameManager.OnGameReset();

        m_buttonEdit.GetComponent<Button>().interactable = true;
        m_buttonEditDone.GetComponent<Button>().interactable = false;
        m_buttonReset.GetComponent<Button>().interactable = false;

        // �Q�X�g�̔z�u���X�V����
        var requestData = m_gameManager.GetGuestEditData();
        StartCoroutine(NetworkManager.Instance.UpdateSignalGuest(
            requestData.SignalID,
            requestData.Pos,
            requestData.Vector,
            result =>
            {
                if (!result) return;
                Debug.Log("�Q�X�g�̔z�u�����X�V�����I");
            }));
    }

    /// <summary>
    /// �~��M���ɎQ�����Ă��郆�[�U�[�̃v���t�B�[���\���E��\��
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveGuestProfile(bool isActive)
    {
        if (isActive) SEManager.Instance.PlayButtonSE();
        if (!isActive) SEManager.Instance.PlayCanselSE();

        m_uiPanelGuests.SetActive(isActive);
        m_uiPanelGame.SetActive(!isActive);
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
        m_gameManager.m_isPause = !isActive ? false : true;
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
        m_gameManager.m_isPause = !isActive ? false : true;
    }

    /// <summary>
    /// �Q�[���I�[�o�[����UI��\������
    /// </summary>
    public void SetGameOverUI(bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        m_uiPanelTutorial.SetActive(false);
        m_uiPanelGuests.SetActive(false);
        m_uiPanelHome.SetActive(false);
        m_uiPanelGame.SetActive(false);
        m_uiPanelGameOver.SetActive(true);

        m_uiPanelGameOver.GetComponent<GameOverUI>().PlayAnim(this,isMedal1,isMedal2,time,score,isStageClear);
    }

    /// <summary>
    /// ���U���g��UI��\������
    /// </summary>
    public void SetResultUI(bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        // ���_����UI���X�V����
        if (isMedal1) m_medalContainers[0].sprite = m_texMedals[0];
        if (isMedal2) m_medalContainers[1].sprite = m_texMedals[1];

        // �N���A�^�C���\�L
        string text = "" + Mathf.Floor(time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textClearTime.text = text.Insert(2, ":");

        // �X�R�A��\�L
        m_textScore.text = "" + score;

        // �]����\�L
        m_imgRank.color = new Color(1, 1, 1, 1);
        m_imgRank.sprite = TopManager.GetScoreRank(m_texRanks,score);

        // �N���A�������ǂ����œ��I�ɐݒ�
        m_textPlateResult.text = isStageClear ? "�X�e�[�W�N���A�I" : "�����ς�...";
        // �N���A�ς݂̃X�e�[�W�̏ꍇ�̓{�^����������悤�ɂ���A�����łȂ��ꍇ�͉����Ȃ��悤�ɂ���
        m_buttonNextStage.GetComponent<Button>().interactable = NetworkManager.Instance.StageResults.Count < TopManager.stageID ? 
            false : true;

        // �p�l�������U���g�ɐݒ肷��
        m_uiPanelTutorial.SetActive(false);
        m_uiPanelGuests.SetActive(false);
        m_uiPanelHome.SetActive(false);
        m_uiPanelGameOver.SetActive(false);
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
    /// �^�C�}�[�e�L�X�g���X�V
    /// </summary>
    public void UpdateTextTimer(float time)
    {
        string text = "" + Mathf.Floor(time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textTimer.text = text.Insert(2,":");
    }

    /// <summary>
    /// ���������Ԃ�\������e�L�X�g�𐶐�����
    /// </summary>
    public void GenerateSubTimeText(int subTime)
    {
        GameObject subTimeObj = Instantiate(m_textSubTimePrefab, m_uiPanelGame.transform);
        subTimeObj.GetComponent<Text>().text = "-" + subTime;

        // ���X�ɓ����E�ړ����č폜
        var sequence = DOTween.Sequence();
        sequence.Append(subTimeObj.transform.DOLocalMoveY(487, 1f).SetEase(Ease.Linear))
            .Join(subTimeObj.GetComponent<Text>().DOFade(0,1).OnComplete(() => { Destroy(subTimeObj.gameObject); }));
        sequence.Play();
    }

    /// <summary>
    /// �{�^���ɃJ�[�\����������or�������Ƃ��ɏ���
    /// </summary>
    public void EventPause(bool frag)
    {
        m_gameManager.m_isPause = frag;
    }
}
