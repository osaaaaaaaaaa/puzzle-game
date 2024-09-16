using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIUserManager : MonoBehaviour
{
    [SerializeField] GameObject m_textEmpty;
    [SerializeField] LoadingContainer m_loading;

    #region ���[�U�[���
    [SerializeField] StageButtonController m_stageButtonController;
    [SerializeField] Text m_textUserName;                 // ���[�U�[��
    [SerializeField] InputField m_inputUserName;          // ���[�U�[�����͗�
    [SerializeField] List<Text> m_textAchievementTitle;   // �̍���
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textScore;                    // �X�e�[�WID
    [SerializeField] List<Image> m_icons;                 // �A�C�R��
    #endregion

    #region �ҏW���[�h�Ŏg�p����I�u�W�F�N�g
    [SerializeField] List<Sprite> m_texTabs;    // �^�u�̉摜 [1:�A�N�e�B�u�ȉ摜,0:��A�N�e�B�u�ȉ摜]
    #region �v���t�B�[��
    [SerializeField] GameObject m_editProfileWindow;        // �v���t�B�[���̕ҏW�E�B���h�E
    [SerializeField] Text m_errorTextProfile;
    #endregion
    #region �A�C�R��
    [SerializeField] GameObject m_editIconWindow;           // �A�C�R���p�̕ҏW�E�B���h�E
    [SerializeField] GameObject m_iconListParent;           // �A�C�R���{�^���̊i�[��
    [SerializeField] GameObject m_selectIconButtonPrefab;   // �A�C�R���{�^���̃v���t�@�u
    #endregion
    #region �̍�
    [SerializeField] GameObject m_editTitleWindow;           // �̍��p�̕ҏW�E�B���h�E
    [SerializeField] GameObject m_contentTitle;              // �̍��{�^���̊i�[��
    [SerializeField] GameObject m_selectTitleButtonPrefab;   // �̍��{�^���̃v���t�@�u
    #endregion
    #region �t�H���[���X�g
    [SerializeField] GameObject m_followUserScrollView;         // �t�H���[���X�g
    [SerializeField] GameObject m_recommendedUserScrollView;    // �������߂̃��[�U�[���X�g
    [SerializeField] GameObject m_tabFollow;                    // �t�H���[���X�g��\������^�u
    [SerializeField] GameObject m_tabCandidate;                 // �������߂̃��[�U�[���X�g��\������^�u
    [SerializeField] GameObject m_profileFollowPrefab;          // �t�H���[���Ă��郆�[�U�[�̃v���t�B�[���v���t�@�u
    [SerializeField] GameObject m_profileRecommendedPrefab;     // �������߂̃��[�U�[�̃v���t�B�[���v���t�@�u
    [SerializeField] Text m_followMaxCnt;                       // �t�H���[�ő�l���̃e�L�X�g
    [SerializeField] Text m_followCntText;                      // �t�H���[���Ă���l���������e�L�X�g
    [SerializeField] Text m_errorTextFollow;
    #endregion
    #region �����L���O
    [SerializeField] GameObject m_rankingScrollView;                // �S���[�U�[���ł̃����L���O�r���[
    [SerializeField] GameObject m_followRankingScrollView;          // �t�H���[���ł̃����L���O�r���[
    [SerializeField] GameObject m_tabRanking;                       // �S���[�U�[����\������^�u
    [SerializeField] GameObject m_tabFollowRanking;                 // �t�H���[����\������^�u
    [SerializeField] GameObject m_buttonRankingPrefab;              // �����L���O�̃v���t�@�u
    [SerializeField] GameObject m_followRankingPrefab;              // �t�H���[�������L���O�̃v���t�@�u
    [SerializeField] PanelRankingUserFollow m_rankingUserFollow;    // �����L���O�̃��[�U�[���t�H���[�E�t�H���[��������p�l��
    #endregion
    #region �A�`�[�u�����g�ꗗ
    [SerializeField] Text m_textTotalPoint;                 // ���v�����|�C���g
    [SerializeField] GameObject m_taskScrollView;           // �^�X�N�̃r���[
    [SerializeField] GameObject m_rewardScrollView;         // ��V�̃r���[
    [SerializeField] GameObject m_tabTask;                  // �^�X�N�ꗗ��\������^�u
    [SerializeField] GameObject m_tabReward;                // ��V�ꗗ��\������^�u
    [SerializeField] GameObject m_taskBarPrefab;            // �^�X�N�̃v���t�@�u
    [SerializeField] GameObject m_rewardBarPrefab;          // ��V�̃v���t�@�u
    [SerializeField] PanelItemDetails m_panelItemDetails;   // �A�C�e���ڍ׃p�l���R���|�[�l���g
    #endregion
    #region ���[���{�b�N�X
    [SerializeField] Sprite m_spriteReceivedMail;           // ���[���J���ς݂�UI
    [SerializeField] GameObject m_textMailEmpty;            // ��̂Ƃ��ɕ\������
    [SerializeField] GameObject m_mailScrollView;           // �v���t�@�u�̊i�[��
    [SerializeField] GameObject m_mailPrefab;               // ���[���̃v���t�@�u
    [SerializeField] MailContent m_mailContent;
    #endregion
    #endregion

    [SerializeField] GameObject m_uiMailUnread;             // ���J���̃��[�������邩�ǂ�����UI
    [SerializeField] GameObject m_uiRewardUnclaimed;        // ���󂯎��̃A�`�[�u�����g��V�����邩�ǂ�����UI

    /// <summary>
    /// �v���t�B�[���̕ҏW���[�h
    /// </summary>
    public enum EDITMODE
    {
        PROFILE = 0,
        ICON,
        TITLE
    }

    /// <summary>
    /// �t�H���[���X�g�̕\�����[�h
    /// </summary>
    enum FOLLOW_LIST_MODE
    {
        FOLLOW = 0,       // �t�H���[���X�g
        RECOMMENDED       // �������߂̃��[�U�[
    }

    /// <summary>
    /// �����L���O�̕\�����[�h
    /// </summary>
    enum RANKING_MODE
    {
        USERS = 0,        // �S���[�U�[��
        FOLLOW            // �t�H���[��
    }

    void OnEnable()
    {
        m_inputUserName.enabled = false;

        if (TopManager.m_isClickTitle)
        {
            UpdateUserDataUI(false, null);
        }

        if (NetworkManager.Instance == null || NetworkManager.Instance.UserID == 0) return;
        CheckRewardUnclaimed();
    }

    /// <summary>
    /// ���󂯎��̕�V�����邩�ǂ����`�F�b�N
    /// </summary>
    public void CheckRewardUnclaimed()
    {
        // ���J���̎�M���[�������邩�ǂ���
        StartCoroutine(NetworkManager.Instance.GetUserMailList(
            result =>
            {
                if (result == null || result.Length == 0)
                {
                    m_uiMailUnread.SetActive(false);
                    return;
                }

                foreach (ShowUserMailResponse mail in result)
                {
                    if (!mail.IsReceived)
                    {
                        m_uiMailUnread.SetActive(true);
                        return;
                    }
                    m_uiMailUnread.SetActive(false);
                }
            }));

        // ���󂯎��̃A�`�[�u�����g��V�����邩�ǂ���
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            6,
            result =>
            {
                int totalPoint = 0;
                if (result == null || result.Length == 0)
                {
                    m_uiRewardUnclaimed.SetActive(false);
                    return;
                }
                totalPoint = result[0].Amount;
                StartCoroutine(NetworkManager.Instance.GetAchievementList(
                    result =>
                    {
                        foreach (ShowAchievementResponse achieve in result)
                        {
                            if (achieve.Type == 3 && achieve.IsReceivedItem && achieve.AchievedVal <= totalPoint)
                            {
                                m_uiRewardUnclaimed.SetActive(true);
                                return;
                            }

                            m_uiRewardUnclaimed.SetActive(false);
                        }
                    }));

            }));
    }

    public void HideMailUnclaimedUI()
    {
        m_uiMailUnread.SetActive(false);
    }

    public void HideRewardUnclaimedUI()
    {
        m_uiRewardUnclaimed.SetActive(false);
    }

    public void ResetErrorText()
    {
        m_errorTextProfile.text = "";
    }

    public void ToggleTextEmpty(string text, bool isVisibility)
    {
        m_textEmpty.SetActive(isVisibility);
        m_textEmpty.GetComponent<Text>().text = text;
    }

    GameObject DestroyScrollContentChildren(GameObject scroll)
    {
        GameObject content = scroll.transform.GetChild(0).transform.GetChild(0).gameObject;

        // ���ݑ��݂���Â��q�I�u�W�F�N�g��S�č폜����
        foreach (Transform oldProfile in content.transform)
        {
            Destroy(oldProfile.gameObject);
        }

        return content;
    }

    void ChangeHierarchyOrder(List<GameObject> targetList)
    {
        foreach(var targeet in targetList)
        {
            targeet.transform.SetAsLastSibling();
        }
    }

    /// <summary>
    /// ���[�U�[�����X�V����
    /// </summary>
    public void UpdateUserDataUI(bool isMoveTopUI, GameObject parent_top)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        // ���[�U�[���擾����
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // �{�^���𐶐�����
                m_stageButtonController.GenerateButtons(result.StageID);

                // �A�C�R�����X�V����
                foreach (Image img in m_icons)
                {
                    img.sprite = TopManager.TexIcons[result.IconID - 1];
                }

                // ���[�U�[�����X�V����
                m_inputUserName.enabled = true;
                m_inputUserName.text = result.Name;
                m_textUserName.text = result.Name;
                m_inputUserName.enabled = false;

                // �A�`�[�u�����g�̃^�C�g���X�V����
                foreach (Text text in m_textAchievementTitle)
                {
                    text.text = result.AchievementTitle;
                }

                // �X�e�[�WID���X�V����
                m_textStageID.text = "" + result.StageID;

                // �X�R�A���X�V����
                m_textScore.text = "" + result.TotalScore;

                if (isMoveTopUI)
                {
                    // �\������UI���z�[���ֈړ�����
                    parent_top.transform.DOLocalMove(new Vector3(parent_top.transform.localPosition.x - 1980f, 0, 0), 0.5f).SetEase(Ease.Linear);
                }
            }));
    }

    /// <summary>
    /// �t�H���[���X�g���X�V����
    /// </summary>
    void UpdateFollowListUI(FOLLOW_LIST_MODE mode)
    {
        m_loading.ToggleLoadingUIVisibility(2);


        // �t�H���[�ł���ő吔���擾
        StartCoroutine(NetworkManager.Instance.GetConstant(
            2,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                if (result == null)
                {
                    m_followMaxCnt.text = "/30";
                    return;
                }
                m_followMaxCnt.text = "/" + result.Constant;
            }
            ));

        switch (mode)
        {
            case FOLLOW_LIST_MODE.FOLLOW:

                // �Â��v���t�B�[����S�č폜�A�i�[����擾����
                GameObject contentFollow = DestroyScrollContentChildren(m_followUserScrollView);

                // �t�H���[���X�g�擾����
                StartCoroutine(NetworkManager.Instance.GetFollowList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("���[�U�[��������܂���ł����B", result.Length == 0);
                        m_followCntText.text = "" + result.Length;

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        foreach (ShowUserFollowResponse user in result)
                        {
                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_profileFollowPrefab, contentFollow.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject, user.UserID,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsAgreement);
                        }
                    }));
                break;
            case FOLLOW_LIST_MODE.RECOMMENDED:

                // �Â��v���t�B�[����S�č폜�A�i�[����擾����
                GameObject contentRecommended = DestroyScrollContentChildren(m_recommendedUserScrollView);

                // �������߂̃��[�U�[���X�g�擾����
                StartCoroutine(NetworkManager.Instance.GetRecommendedUserList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        m_textEmpty.SetActive(result.Length == 0);
                        m_textEmpty.GetComponent<Text>().text = result.Length == 0 ? "���[�U�[��������܂���ł����B" : "";

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        foreach (ShowUserRecommendedResponse user in result)
                        {
                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_profileRecommendedPrefab, contentRecommended.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject, user.UserID,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsFollower);
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// �����L���O���X�V����
    /// </summary>
    void UpdateRankingUI(RANKING_MODE mode)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        switch (mode)
        {
            case RANKING_MODE.USERS:

                // �Â��v���t�B�[����S�č폜�A�i�[����擾����
                GameObject contentRanking = DestroyScrollContentChildren(m_rankingScrollView);

                // �����L���O�擾����
                StartCoroutine(NetworkManager.Instance.GetRankingList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("���[�U�[��������܂���ł����B", result.Length == 0);
                        m_followCntText.text = "" + result.Length;

                        m_rankingUserFollow.IniRankingtUserData(result);

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        int i = 0;
                        foreach (ShowRankingResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;

                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_buttonRankingPrefab, contentRanking.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i + 1, isMyData,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsAgreement);

                            // �t�H���[�E�t�H���[�����C�x���g��ݒ肷��
                            int index = new int();
                            index = i;
                            profile.GetComponent<Button>().onClick.AddListener(() => 
                            {
                                SEManager.Instance.PlayButtonSE();
                                m_rankingUserFollow.SetPanelContent(user.UserID, user.Name, index); 
                            });
                            i++;
                        }
                    }));
                break;
            case RANKING_MODE.FOLLOW:

                // �Â��v���t�B�[����S�č폜�A�i�[����擾����
                GameObject contentRankingFollow = DestroyScrollContentChildren(m_followRankingScrollView);

                // �t�H���[���ł̃����L���O�擾����
                StartCoroutine(NetworkManager.Instance.GetFollowRankingList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("���[�U�[��������܂���ł����B", result.Length <= 1);
                        if (result.Length <= 1) return;

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        int i = 0;
                        foreach (ShowUserProfileResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;
                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_followRankingPrefab, contentRankingFollow.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i + 1, isMyData,
                                user.Name, user.Title, user.StageID, user.TotalScore,
                                TopManager.TexIcons[user.IconID - 1], user.IsAgreement);
                            i++;
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// �A�`�[�u�����g�ꗗ���X�V����(�A�`�[�u�����g�ꗗ�\���{�^������Ă�)
    /// </summary>
    public void UpdateAchievementUI()
    {
        m_loading.ToggleLoadingUIVisibility(1);
        m_textTotalPoint.text = "pt";

        // �Â��A�`�[�u�����g��S�č폜�A�i�[����擾����
        GameObject contentTask = DestroyScrollContentChildren(m_taskScrollView);
        GameObject contentReward = DestroyScrollContentChildren(m_rewardScrollView);

        // ���v�|�C���g���擾����
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            6,
            result =>
            {
                int totalPoint = 0;
                if (result.Length != 0) totalPoint = result[0].Amount;
                m_textTotalPoint.text = totalPoint + "pt";

                // �A�`�[�u�����g�ꗗ�擾����
                StartCoroutine(NetworkManager.Instance.GetAchievementList(
                    result =>
                    {
                        m_loading.ToggleLoadingUIVisibility(-1);

                        ToggleTextEmpty("�ʐM�G���[���������܂����B", result.Length == 0);

                        List<GameObject> receivedAchieve = new List<GameObject>();
                        foreach (ShowAchievementResponse achieve in result)
                        {
                            GameObject barAchieve;
                            if (achieve.Type == 1 || achieve.Type == 2)
                            {
                                // �^�X�N�ꗗ�𐶐�����
                                barAchieve = Instantiate(m_taskBarPrefab, contentTask.transform);
                                barAchieve.GetComponent<TaskBar>().UpdateTask(achieve.Text, achieve.AchievedVal, achieve.ProgressVal,
                                    achieve.RewardItem.Amount, achieve.IsReceivedItem);
                            }
                            else
                            {
                                // ��V�ꗗ�𐶐�����
                                barAchieve = Instantiate(m_rewardBarPrefab, contentReward.transform);
                                barAchieve.GetComponent<RewardBar>().UpdateReward(m_panelItemDetails, achieve.Type,
                                    achieve.RewardItem, achieve.AchievedVal, totalPoint, achieve.IsReceivedItem);
                            }

                            if (achieve.IsReceivedItem) receivedAchieve.Add(barAchieve);
                        }

                        // ��V���ς݂̃I�u�W�F�N�g�̃q�G�����L�[����ύX����
                        ChangeHierarchyOrder(receivedAchieve);
                    }));

            }));
    }

    /// <summary>
    /// ���[���ꗗ���X�V����(���[���̃V�X�e���{�^������Ă�)
    /// </summary>
    public void UpdateMailUI()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        ToggleTextEmpty("", false);
        m_textMailEmpty.SetActive(false);

        // �Â����[����S�č폜�A�i�[����擾����
        GameObject content = DestroyScrollContentChildren(m_mailScrollView);

        // ��M���[�����擾����
        StartCoroutine(NetworkManager.Instance.GetUserMailList(
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                if (result.Length == 0)
                {
                    m_textMailEmpty.SetActive(true);
                    return;
                }

                foreach (ShowUserMailResponse mail in result)
                {
                    // ���[���ꗗ(�Z���N�g�{�^��)�𐶐�����
                    GameObject mailButton = Instantiate(m_mailPrefab, content.transform);
                    if (mail.IsReceived) mailButton.GetComponent<Image>().sprite = m_spriteReceivedMail;
                    mailButton.transform.GetChild(0).GetComponent<Text>().text = mail.Title;
                    mailButton.GetComponent<Button>().onClick.AddListener(() => 
                    {
                        SEManager.Instance.PlayButtonSE();
                        m_mailContent.SetMailContent(mailButton,mail.MailID,mail.Title, mail.CreatedAt, mail.ElapsedDay,mail.Text, mail.IsReceived); 
                    });
                }
            }));
    }

    /// <summary>
    /// ���݂̕ҏW���[�h�ɂ����UI�̐e�I�u�W�F�N�g��\���E��\���ɂ���
    /// </summary>
    public void SetActiveParents(EDITMODE currentEditMode)
    {
        ResetErrorText();
        m_editProfileWindow.SetActive(currentEditMode == EDITMODE.PROFILE ? true : false);
        m_editIconWindow.SetActive(currentEditMode == EDITMODE.ICON ? true : false);
        m_editTitleWindow.SetActive(currentEditMode == EDITMODE.TITLE ? true : false);
    }

    /// <summary>
    /// �ҏW��ʂ����
    /// </summary>
    public void OnCloseEditWindowButton()
    {
        SetActiveParents(EDITMODE.PROFILE);
    }

    /// <summary>
    /// ���O���͗��Ƀt�H�[�J�X�𓖂Ă�
    /// </summary>
    public void OnEditUserNameButton()
    {
        m_inputUserName.enabled = true;
        m_inputUserName.Select();

        // �󕶎��ɂ���
        m_inputUserName.text = "";
    }

    /// <summary>
    /// ���O�ύX
    /// </summary>
    public void OnDoneUserNameButton()
    {
        if (m_inputUserName.text != "")
        {
            m_loading.ToggleLoadingUIVisibility(1);

            // ���[�U�[�X�V����
            StartCoroutine(NetworkManager.Instance.UpdateUser(
                m_inputUserName.text,
                NetworkManager.Instance.TitleID,
                NetworkManager.Instance.StageID,
                NetworkManager.Instance.IconID,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    if (result == null)
                    {
                        // ����ɏ������ł����ꍇ�A���[�U�[�����X�V����
                        m_inputUserName.text = NetworkManager.Instance.UserName;
                        m_textUserName.text = NetworkManager.Instance.UserName;
                        m_inputUserName.enabled = false;
                        ResetErrorText();
                        return;
                    }

                    // �G���[�����Ԃ��Ă����ꍇ
                    m_errorTextProfile.text = result.Error;

                    // ���[�U�[�������ɖ߂�
                    m_inputUserName.text = NetworkManager.Instance.UserName;
                    m_textUserName.text = NetworkManager.Instance.UserName;
                    m_inputUserName.enabled = false;
                }));
        }
        else
        {
            // ���[�U�[�������ɖ߂�
            m_inputUserName.text = NetworkManager.Instance.UserName;
            m_textUserName.text = NetworkManager.Instance.UserName;
            m_inputUserName.enabled = false;
        }
    }

    /// <summary>
    /// �A�C�R���̃��X�g��\������
    /// </summary>
    public void OnEditIconButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SetActiveParents(EDITMODE.ICON);

        // ���ݑ��݂���A�C�R���̑I���{�^����S�Ĕj������
        foreach (Transform child in m_iconListParent.transform)
        {
            Destroy(child.gameObject);
        }

        // �������Ă���A�C�R�������擾����
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            1,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // �������Ă���A�C�R���̂ݐ�������
                for (int i = 0; i < result.Length; i++)
                {
                    GameObject button = Instantiate(m_selectIconButtonPrefab, m_iconListParent.transform);
                    button.GetComponent<Image>().sprite = TopManager.TexIcons[result[i].Effect - 1];

                    // �A�C�R���ύX�C�x���g��ǉ�����
                    int iconID = new int();   // �A�h���X�X�V
                    iconID = result[i].Effect;
                    button.GetComponent<Button>().onClick.AddListener(() => OnDoneIconButton(iconID));
                }
            }));
    }

    /// <summary>
    /// �A�C�R����ύX����
    /// </summary>
    public void OnDoneIconButton(int iconID)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SEManager.Instance.PlayButtonSE();
        // ���[�U�[�X�V����
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            NetworkManager.Instance.UserName,
            NetworkManager.Instance.TitleID,
            NetworkManager.Instance.StageID,
            iconID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // �G���[�����Ԃ��Ă����ꍇ�̓��^�[��
                if (result != null) return;

                // �A�C�R�����X�V����
                foreach (Image img in m_icons)
                {
                    img.sprite = TopManager.TexIcons[iconID - 1];
                }

                OnCloseEditWindowButton();
            }));
    }

    /// <summary>
    /// �̍��̃��X�g��\������
    /// </summary>
    public void OnEditTitleButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SetActiveParents(EDITMODE.TITLE);

        // ���ݑ��݂���A�C�R���̑I���{�^����S�Ĕj������
        foreach (Transform child in m_contentTitle.transform)
        {
            Destroy(child.gameObject);
        }

        // �������Ă���̍������擾����
        StartCoroutine(NetworkManager.Instance.GetUserItem(
            2,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // �̍����������鍀�ڂ��쐬
                GameObject buttonRelease = Instantiate(m_selectTitleButtonPrefab, m_contentTitle.transform);
                buttonRelease.transform.GetChild(0).GetComponent<Text>().text = "�~";
                buttonRelease.GetComponent<Button>().onClick.AddListener(() => OnDoneTitleButton(0, ""));

                // �������Ă���̍��̂ݐ�������
                for (int i = 0; i < result.Length; i++)
                {
                    GameObject button = Instantiate(m_selectTitleButtonPrefab, m_contentTitle.transform);
                    button.transform.GetChild(0).GetComponent<Text>().text = result[i].Name;

                    // �̍��ύX�C�x���g��ǉ�����
                    int titleID = new int();   // �A�h���X�X�V
                    string name = result[i].Name; ;
                    titleID = result[i].ItemID;
                    button.GetComponent<Button>().onClick.AddListener(() => OnDoneTitleButton(titleID, name));
                }
            }));
    }

    /// <summary>
    /// �̍���ύX����
    /// </summary>
    public void OnDoneTitleButton(int titleID,string title)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        SEManager.Instance.PlayButtonSE();
        // ���[�U�[�X�V����
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            NetworkManager.Instance.UserName,
            titleID,
            NetworkManager.Instance.StageID,
            NetworkManager.Instance.IconID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);

                // �G���[�����Ԃ��Ă����ꍇ�̓��^�[��
                if (result != null) return;

                // �̍����X�V����
                foreach (Text text in m_textAchievementTitle)
                {
                    text.text = title;
                }

                OnCloseEditWindowButton();
            }));
    }

    /// <summary>
    /// �t�H���[���X�g�̓��e��؂�ւ���
    /// </summary>
    /// <param name="mode">FOLLOW_LIST_MODE�Q��</param>
    public void OnFollowTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        m_errorTextFollow.text = "";

        switch (mode)
        {
            case 0: // �t�H���[���X�g��\������
                m_followUserScrollView.SetActive(true);
                m_recommendedUserScrollView.SetActive(false);
                m_tabFollow.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabCandidate.GetComponent<Image>().sprite = m_texTabs[0];

                // �t�H���[���Ă��郆�[�U�[�̏����擾����
                UpdateFollowListUI(FOLLOW_LIST_MODE.FOLLOW);
                break;
            case 1: // �������߂̃��[�U�[���X�g��\������
                m_followUserScrollView.SetActive(false);
                m_recommendedUserScrollView.SetActive(true);
                m_tabFollow.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabCandidate.GetComponent<Image>().sprite = m_texTabs[1];

                // �������߂̃��[�U�[���X�g�擾
                UpdateFollowListUI(FOLLOW_LIST_MODE.RECOMMENDED);
                break;
        }
    }

    /// <summary>
    /// �t�H���[�E�t�H���[��������
    /// </summary>
    public void ActionFollow(bool isActive,int user_id,GameObject btnObj)
    {
        m_loading.ToggleLoadingUIVisibility(1);

        if (isActive)
        {
            // �t�H���[����
            StartCoroutine(NetworkManager.Instance.StoreUserFollow(
                user_id,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    if (result != null)
                    {
                        // �G���[����\������
                        m_errorTextFollow.text = result.Error;
                    }
                    else
                    {
                        m_errorTextFollow.text = "";

                        // ���������ꍇ�̓e�L�X�g���X�V����
                        m_followCntText.text = "" + (int.Parse(m_followCntText.text) + 1);
                        btnObj.GetComponent<FollowActionButton>().Invert();
                    }
                }));
        }
        else
        {
            // �t�H���[��������
            StartCoroutine(NetworkManager.Instance.DestroyUserFollow(
                user_id,
                result =>
                {
                    m_loading.ToggleLoadingUIVisibility(-1);

                    if (!result) return;

                    m_errorTextFollow.text = "";
                    m_followCntText.text = "" + (int.Parse(m_followCntText.text) - 1);
                    btnObj.GetComponent<FollowActionButton>().Invert();
                }));
        }
    }

    /// <summary>
    /// �����L���O�̓��e��؂�ւ���
    /// </summary>
    /// <param name="mode">RANKINF_MODE�Q��</param>
    public void OnRankingTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        switch (mode)
        {
            case 0: // �S���[�U�[���̃����L���O��\������
                m_rankingScrollView.SetActive(true);
                m_followRankingScrollView.SetActive(false);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[0];

                // �����L���O���X�g���擾
                UpdateRankingUI(RANKING_MODE.USERS);
                break;
            case 1: // �t�H���[���̃����L���O��\������
                m_rankingScrollView.SetActive(false);
                m_followRankingScrollView.SetActive(true);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // �t�H���[���ł̃����L���O�擾
                UpdateRankingUI(RANKING_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// �A�`�[�u�����g�ꗗ�̓��e��؂�ւ���
    /// </summary>
    /// <param name="mode">ACHIEVEMENT_MODE�Q��</param>
    public void OnAchievementTabButton(int mode)
    {
        ToggleTextEmpty("", false);
        switch (mode)
        {
            case 0: // �^�X�N�ꗗ��\������
                m_taskScrollView.SetActive(true);
                m_rewardScrollView.SetActive(false);
                m_tabTask.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabReward.GetComponent<Image>().sprite = m_texTabs[0];
                break;
            case 1: // ��V�ꗗ��\������
                m_taskScrollView.SetActive(false);
                m_rewardScrollView.SetActive(true);
                m_tabTask.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabReward.GetComponent<Image>().sprite = m_texTabs[1];
                break;
        }
    }
}
