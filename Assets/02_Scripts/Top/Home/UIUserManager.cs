using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UIUserManager : MonoBehaviour
{
    [SerializeField] GameObject m_textEmpty;

    #region ���[�U�[���
    [SerializeField] StageButtonController m_stageButtonController;
    [SerializeField] Text m_textUserName;                 // ���[�U�[��
    [SerializeField] InputField m_inputUserName;          // ���[�U�[�����͗�
    [SerializeField] List<Text> m_textAchievementTitle;   // �̍���
    [SerializeField] Text m_textStageID;                  // �X�e�[�WID
    [SerializeField] Text m_textScore;                    // �X�e�[�WID
    [SerializeField] List<Image> m_icons;                 // �A�C�R��
    [SerializeField] List<Sprite> m_texIcons;             // �A�C�R���摜
    #endregion

    #region �ҏW���[�h�Ŏg�p����I�u�W�F�N�g
    [SerializeField] List<Sprite> m_texTabs;    // �^�u�̉摜 [1:�A�N�e�B�u�ȉ摜,0:��A�N�e�B�u�ȉ摜]
    #region �v���t�B�[��
    [SerializeField] GameObject m_editProfileWindow;        // �v���t�B�[���̕ҏW�E�B���h�E
    [SerializeField] Text m_errorTextProfile;
    #endregion
    #region �A�C�R��
    [SerializeField] GameObject m_editIconWindow;           // �A�C�R���p�̕ҏW�E�B���h�E
    [SerializeField] GameObject m_iconListParent;           // �A�C�R�����X�g
    [SerializeField] GameObject m_selectIconButtonPrefab;   // �A�C�R���{�^���̃v���t�@�u
    #endregion
    #region �t�H���[���X�g
    [SerializeField] GameObject m_followUserScloleView;         // �t�H���[���X�g
    [SerializeField] GameObject m_recommendedUserScloleView;    // �������߂̃��[�U�[���X�g
    [SerializeField] GameObject m_tabFollow;                    // �t�H���[���X�g��\������^�u
    [SerializeField] GameObject m_tabCandidate;                 // �������߂̃��[�U�[���X�g��\������^�u
    [SerializeField] GameObject m_profileFollowPrefab;          // �t�H���[���Ă��郆�[�U�[�̃v���t�B�[���v���t�@�u
    [SerializeField] GameObject m_profileRecommendedPrefab;     // �������߂̃��[�U�[�̃v���t�B�[���v���t�@�u
    [SerializeField] Text m_followCntText;                      // �t�H���[���Ă���l���������e�L�X�g
    [SerializeField] Text m_errorTextFollow;
    #endregion
    #region �����L���O
    [SerializeField] GameObject m_rankingScloleView;            // �S���[�U�[���ł̃����L���O�r���[
    [SerializeField] GameObject m_followRankingScloleView;      // �t�H���[���ł̃����L���O�r���[
    [SerializeField] GameObject m_tabRanking;                   // �S���[�U�[����\������^�u
    [SerializeField] GameObject m_tabFollowRanking;             // �t�H���[����\������^�u
    [SerializeField] GameObject m_profileRankingPrefab;         // �����L���O�ɕ\�����郆�[�U�[�v���t�B�[���v���t�@�u
    #endregion
    #endregion

    /// <summary>
    /// �v���t�B�[���̕ҏW���[�h
    /// </summary>
    public enum EDITMODE
    {
        PROFILE = 0,
        ICON,
        ACHIEVE
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
    enum RANKINF_MODE
    {
        USERS = 0,        // �S���[�U�[��
        FOLLOW            // �t�H���[��
    }

    void OnEnable()
    {
        m_inputUserName.enabled = false;

        if (TopManager.m_isClickTitle)
        {
            UpdateUserDataUI(false,null);
        }   
    }

    public void ResetErrorText()
    {
        m_errorTextProfile.text = "";
    }

    public void ToggleTextEmpty(string text,bool isVisibility)
    {
        m_textEmpty.SetActive(isVisibility);
        m_textEmpty.GetComponent<Text>().text = text;
    }

    /// <summary>
    /// ���[�U�[�����X�V����
    /// </summary>
    public void UpdateUserDataUI(bool isMoveTopUI,GameObject parent_top)
    {
        // ���[�U�[���擾����
        StartCoroutine(NetworkManager.Instance.GetUserData(
            result =>
            {
                // �{�^���𐶐�����
                m_stageButtonController.GenerateButtons(result.StageID);

                // �A�C�R�����X�V����
                foreach (Image img in m_icons)
                {
                    img.sprite = m_texIcons[result.IconID - 1];
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
        switch (mode)
        {
            case FOLLOW_LIST_MODE.FOLLOW:

                // �v���t�B�[���̃v���t�@�u�̊i�[����擾����
                GameObject contentFollow = m_followUserScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // ���݁A���݂���Â��v���t�B�[����S�č폜����
                foreach (Transform oldProfile in contentFollow.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // �t�H���[���X�g�擾����
                StartCoroutine(NetworkManager.Instance.GetFollowList(
                    result =>
                    {
                        ToggleTextEmpty("���[�U�[��������܂���ł����B", result.Length == 0);
                        m_followCntText.text = "" + result.Length;

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        foreach (ShowUserFollowResponse user in result)
                        {
                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_profileFollowPrefab, contentFollow.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject,user.UserID,
                                user.Name, user.AchievementTitle,user.StageID, user.TotalScore, 
                                m_texIcons[user.IconID - 1], user.IsAgreement);
                        }
                    }));
                break;
            case FOLLOW_LIST_MODE.RECOMMENDED:

                // �v���t�B�[���̃v���t�@�u�̊i�[����擾����
                GameObject contentRecommended = m_recommendedUserScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // ���݁A���݂���Â��v���t�B�[����S�č폜����
                foreach (Transform oldProfile in contentRecommended.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // �������߂̃��[�U�[���X�g�擾����
                StartCoroutine(NetworkManager.Instance.GetRecommendedUserList(
                    result =>
                    {
                        m_textEmpty.SetActive(result.Length == 0);
                        m_textEmpty.GetComponent<Text>().text = result.Length == 0 ? "���[�U�[��������܂���ł����B" : "";

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        foreach (ShowUserRecommendedResponse user in result)
                        {
                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_profileRecommendedPrefab, contentRecommended.transform);
                            profile.GetComponent<FollowingUserProfile>().UpdateProfile(transform.gameObject,user.UserID, 
                                user.Name, user.AchievementTitle,user.StageID, user.TotalScore, 
                                m_texIcons[user.IconID - 1], user.IsFollower);
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// �����L���O���X�V����
    /// </summary>
    void UpdateRankingUI(RANKINF_MODE mode)
    {
        switch (mode)
        {
            case RANKINF_MODE.USERS:

                // �v���t�B�[���̃v���t�@�u�̊i�[����擾����
                GameObject contentRanking = m_rankingScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // ���݁A���݂���Â��v���t�B�[����S�č폜����
                foreach (Transform oldProfile in contentRanking.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // �����L���O�擾����
                StartCoroutine(NetworkManager.Instance.GetRankingList(
                    result =>
                    {
                        ToggleTextEmpty("���[�U�[��������܂���ł����B", result.Length == 0);
                        m_followCntText.text = "" + result.Length;

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        int i = 0;
                        foreach (ShowUserProfileResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;

                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_profileRankingPrefab, contentRanking.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i+1, isMyData,
                                user.Name, user.AchievementTitle, user.StageID, user.TotalScore,
                                m_texIcons[user.IconID - 1], user.IsAgreement);
                            i++;
                        }
                    }));
                break;
            case RANKINF_MODE.FOLLOW:

                // �v���t�B�[���̃v���t�@�u�̊i�[����擾����
                GameObject contentRecommended = m_followRankingScloleView.transform.GetChild(0).transform.GetChild(0).gameObject;

                // ���݁A���݂���Â��v���t�B�[����S�č폜����
                foreach (Transform oldProfile in contentRecommended.transform)
                {
                    Destroy(oldProfile.gameObject);
                }

                // �t�H���[���ł̃����L���O�擾����
                StartCoroutine(NetworkManager.Instance.GetFollowRankingList(
                    result =>
                    {
                        ToggleTextEmpty("���[�U�[��������܂���ł����B", result.Length <= 1);
                        if (result.Length <= 1) return;

                        // �擾�����t�H���[���X�g�̏������Ɋe���[�U�[�̃v���t�B�[�����쐬����
                        int i = 0;
                        foreach (ShowUserProfileResponse user in result)
                        {
                            bool isMyData = NetworkManager.Instance.UserID == user.UserID ? true : false;
                            // �v���t�B�[���𐶐�����
                            GameObject profile = Instantiate(m_profileRankingPrefab, contentRecommended.transform);
                            profile.GetComponent<RankingUserProfile>().UpdateProfile(i+1, isMyData,
                                user.Name, user.AchievementTitle, user.StageID, user.TotalScore,
                                m_texIcons[user.IconID - 1], user.IsAgreement);
                            i++;
                        }
                    }));
                break;
        }
    }

    /// <summary>
    /// ���݂̕ҏW���[�h�ɂ����UI�̐e�I�u�W�F�N�g��\���E��\���ɂ���
    /// </summary>
    public void SetActiveParents(EDITMODE currentEditMode)
    {
        ResetErrorText();
        m_editProfileWindow.SetActive(currentEditMode == EDITMODE.PROFILE ? true : false);
        m_editIconWindow.SetActive(currentEditMode == EDITMODE.ICON ? true : false);
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
            // ���[�U�[�X�V����
            StartCoroutine(NetworkManager.Instance.UpdateUser(
                m_inputUserName.text,
                NetworkManager.Instance.AchievementID,
                NetworkManager.Instance.StageID,
                NetworkManager.Instance.IconID,
                result =>
                {
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
                // �������Ă���A�C�R���̂ݐ�������
                for (int i = 0; i < result.Length; i++)
                {
                    GameObject button = Instantiate(m_selectIconButtonPrefab, m_iconListParent.transform);
                    button.GetComponent<Image>().sprite = m_texIcons[result[i].Effect - 1];

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
        SEManager.Instance.PlayButtonSE();
        // ���[�U�[�X�V����
        StartCoroutine(NetworkManager.Instance.UpdateUser(
            NetworkManager.Instance.UserName,
            NetworkManager.Instance.AchievementID,
            NetworkManager.Instance.StageID,
            iconID,
            result =>
            {
                // �G���[�����Ԃ��Ă����ꍇ�̓��^�[��
                if (result != null) return;

                // �A�C�R�����X�V����
                foreach (Image img in m_icons)
                {
                    img.sprite = m_texIcons[iconID - 1];
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
                m_followUserScloleView.SetActive(true);
                m_recommendedUserScloleView.SetActive(false);
                m_tabFollow.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabCandidate.GetComponent<Image>().sprite = m_texTabs[0];

                // �t�H���[���Ă��郆�[�U�[�̏����擾����
                UpdateFollowListUI(FOLLOW_LIST_MODE.FOLLOW);
                break;
            case 1: // �������߂̃��[�U�[���X�g��\������
                m_followUserScloleView.SetActive(false);
                m_recommendedUserScloleView.SetActive(true);
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
        if (isActive)
        {
            // �t�H���[����
            StartCoroutine(NetworkManager.Instance.StoreUserFollow(
                user_id,
                result =>
                {
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
                m_rankingScloleView.SetActive(true);
                m_followRankingScloleView.SetActive(false);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[0];

                // �����L���O���X�g���擾
                UpdateRankingUI(RANKINF_MODE.USERS);
                break;
            case 1: // �t�H���[���̃����L���O��\������
                m_rankingScloleView.SetActive(false);
                m_followRankingScloleView.SetActive(true);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // �t�H���[���ł̃����L���O�擾
                UpdateRankingUI(RANKINF_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// �A�`�[�u�����g�̃��X�g��\������
    /// </summary>
    public void OnEditAchieveButton(int mode)
    {
        ToggleTextEmpty("", false);
        switch (mode)
        {
            case 0: // �z�X�g�̃��O���X�g��\��
                m_rankingScloleView.SetActive(true);
                m_followRankingScloleView.SetActive(false);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[1];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[0];

                break;
            case 1: // �Q�X�g�̃��O���X�g��\��
                m_rankingScloleView.SetActive(false);
                m_followRankingScloleView.SetActive(true);
                m_tabRanking.GetComponent<Image>().sprite = m_texTabs[0];
                m_tabFollowRanking.GetComponent<Image>().sprite = m_texTabs[1];

                // ��W�ꗗ���擾
                //UpdateRankingUI(RANKINF_MODE.FOLLOW);
                break;
        }
    }

    /// <summary>
    /// �A�`�[�u�����g��ύX����
    /// </summary>
    public void OnDoneAchieveButton()
    {

    }
}
