using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEngine.AddressableAssets;
using Newtonsoft.Json;

public class TopManager : MonoBehaviour
{
    [SerializeField] UIUserManager m_uiUserManager;
    [SerializeField] UISignalManager m_uiSignalManager;

    [SerializeField] GameObject m_parent_top;
    [SerializeField] GameObject m_ui_startTextParent;
    [SerializeField] Image m_panelImage;                    // ��\���ɂ���p�l���̃C���[�W
    [SerializeField] Text m_uiUserName;                     // ���[�U�[��
    [SerializeField] AssetDownLoader m_assetDownLoader;     // �A�Z�b�g�_�E�����[�_�[
    [SerializeField] GameObject m_boxStage;                 // �X�e�[�W�V�[���ɓ���O�̃E�C���h�E
    [SerializeField] Image m_imgSignalButton;               // �~��M���̃{�^���̉摜
    [SerializeField] GameObject m_panelSignalButtonError;   // �~��M���{�^�����������Ƃ��ɕ\�������E�C���h�E
    bool isOnStageButton;   //�X�e�[�W�V�[���ɑJ�ڂ���{�^�����N���b�N�������ǂ���

    // �V�X�e����ʂ̃p�l�����X�g
    [SerializeField] List<GameObject> m_sys_panelList;
    // �V�X�e���{�^���̘A��
    public enum SYSTEM
    {
        PROFILE = 0,
        MAILBOX,
        FOLLOWBOX,
        RANKING,
        D_SIGNAL,
        ACHIEVEMENT
    }

    /// <summary>
    /// �^�C�g�����N���b�N�������ǂ���
    /// </summary>
    public static bool m_isClickTitle { get; private set; } = false;

    /// <summary>
    /// �ő�X�e�[�W��
    /// </summary>
    public static int stageMax { get; set; }

    /// <summary>
    /// �I�������X�e�[�WID
    /// </summary>
    public static int stageID { get; set; }

    /// <summary>
    /// �A�C�e�����g�p���Ă��邩�ǂ���
    /// </summary>
    public static bool isUseItem { get; set; }

    /// <summary>
    /// �A�C�R���f�U�C���̃��X�g
    /// </summary>
    [SerializeField] List<Sprite> m_texIcons;
    public static List<Sprite> TexIcons { get; private set; }

    public enum ScoreRank
    {
        S = 9999,
        A = 1200,
        B = 800,
        C = 500,
        X = 0,
    }

    private void OnEnable()
    {
       
        isOnStageButton = false;
        isUseItem = false;
    }

    void Start()
    {
        stageID = 0;
        m_panelImage.enabled = false;
        TexIcons = m_texIcons;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_isClickTitle)
        {
            m_isClickTitle = true;

#if !UNITY_EDITOR
            DOTween.Kill(m_ui_startTextParent.transform);
            m_ui_startTextParent.SetActive(false);

            // �A�Z�b�g�o���h�����X�V�\���ǂ����`�F�b�N
            m_assetDownLoader.StartCoroutine(m_assetDownLoader.checkCatalog());
#else
            StoreUser();
#endif
        }
    }

    /// <summary>
    /// ���[�U�[�o�^����
    /// </summary>
    public void StoreUser()
    {
        // ���[�U�[�f�[�^���ۑ�����Ă��Ȃ��ꍇ
        if (!NetworkManager.Instance.LoadUserData())
        {
            // ���[�U�[�o�^����
            StartCoroutine(NetworkManager.Instance.StoreUser(
                Guid.NewGuid().ToString(),
                result =>
                {
                    if (result) OnClickTitleWindow();
                }));    // �o�^������̏���
        }
        else
        {
            // �����A�C�e�����擾����
            StartCoroutine(NetworkManager.Instance.GetUserItem(
                3,
                result => { }
                ));

            // ��������W���̋~��M�����擾����
            StartCoroutine(NetworkManager.Instance.GetDistressSignalList(
                result => { }
                ));

            // ���[�U�[�����X�V����
            StartCoroutine(NetworkManager.Instance.GetUserData(
                result => 
                {
                    // �X�e�[�W�̃��U���g�����擾����
                    StartCoroutine(NetworkManager.Instance.GetStageResults(
                        result =>
                        {
                            OnClickTitleWindow();
                        }));
                }
                ));
        }
    }

    /// <summary>
    /// �X�e�[�W�I���̃{�^��
    /// </summary>
    public void OnSelectStageButton(int id)
    {
        if (isOnStageButton) return;

        stageID = id;

        // �X�e�[�W�ɓ���O�̃E�C���h�E��\������
        ShowStageResultResponse resultData = NetworkManager.Instance.StageResults.Count < stageID ? null : NetworkManager.Instance.StageResults[stageID - 1];
        m_boxStage.SetActive(true);
        m_boxStage.GetComponent<StageBox>().InitStatus(resultData);
    }

    /// <summary>
    /// �X�e�[�W�V�[���ɑJ�ڂ���
    /// </summary>
    public void OnPlayStageButton(TopSceneDirector.PLAYMODE playMode,int signalID, int id, bool isStageClear)
    {
        if (isOnStageButton) return;

        stageID = id == 0 ? stageID : id;   // �\���ŗV�ԏꍇ(id=0)�͍X�V���Ȃ�
        TopSceneDirector.Instance.SetPlayMode(playMode, signalID, isStageClear);
        isOnStageButton = true;
        isUseItem = m_boxStage.GetComponent<StageBox>().m_isUseItem;
        m_boxStage.GetComponent<StageBox>().OnCloseButton();
#if !UNITY_EDITOR
        // �Q�[��UI�V�[���ɑJ�ڂ���
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        // �Q�[���V�[���ɑJ�ڂ���
        Initiate.Fade(stageID + "_GameScene", Color.black, 1.0f);
#endif
    }

    /// <summary>
    /// �^�C�g����ʂ���z�[����ʂֈړ�����
    /// </summary>
    void OnClickTitleWindow()
    {
        if (isOnStageButton) return;

        m_uiUserManager.UpdateUserDataUI(true, m_parent_top);

        // �~��M���{�^���̃J���[�ύX
        m_imgSignalButton.color = NetworkManager.Instance.IsDistressSignalEnabled ? new Color(1f, 1f, 1f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);
    }

    /// <summary>
    /// �z�[����ʂ���^�C�g����ʂ֖߂�
    /// </summary>
    public void OnBackButtonHome()
    {
        if (isOnStageButton) return;

        m_ui_startTextParent.SetActive(true);

        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(()=> { m_isClickTitle = false; });
    }

    /// <summary>
    /// �z�[����ʂ���V�X�e�����(�v���t�B�[���A���[���{�b�N�X�Ȃ�)�ֈړ�����
    /// </summary>
    /// <param name="systemNum">SYSTEM�i�V�X�e���{�^���̘A�ԁj�Q��</param>
    public void OnButtonSystemPanel(int systemNum)
    {
        if (isOnStageButton) return;

        // �S�ẴV�X�e����ʂ��\���ɂ���
        foreach (GameObject item in m_sys_panelList)
        {
            item.SetActive(false);
        }

        // �\������
        m_sys_panelList[systemNum].SetActive(true);     // �I�������V�X�e�����
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, -1080, 0), 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// �V�X�e����ʂ���z�[����ʂ֖߂�
    /// </summary>
    public void OnBackButtonSystemPanel()
    {
        if (isOnStageButton) return;

        // �~��M���{�^���̃J���[�ύX
        m_imgSignalButton.color = NetworkManager.Instance.IsDistressSignalEnabled ? new Color(1f, 1f, 1f, 1f) : new Color(0.7f, 0.7f, 0.7f, 1f);

        m_uiUserManager.ResetErrorText();
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, 0, 0), 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// �~��M���{�^������
    /// </summary>
    public void OnDistressSignalButton()
    {
        if (!NetworkManager.Instance.IsDistressSignalEnabled)
        {
            TogglePanelDistressErrorVisibility(true);
            return;
        }

        OnButtonSystemPanel((int)SYSTEM.D_SIGNAL);

        if (TopSceneDirector.Instance != null && TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.SOLO && !NetworkManager.Instance.IsDistressSignalTutrial)
        {
            NetworkManager.Instance.TutrialViewed();        // �`���[�g���A�����������Ƃɂ���
            m_uiSignalManager.ToggleWindowVisibility(1);    // �`���[�g���A����ʂ�\������
        }
        else
        {
            // �`���[�g���A����ʂ��������Ƃ�����ꍇ�͋~��M���̉�ʂ�\������
            m_uiSignalManager.ToggleWindowVisibility(0);
        }
    }

    public void TogglePanelDistressErrorVisibility(bool isVisibility)
    {
        m_panelSignalButtonError.SetActive(isVisibility);
    }

    /// <summary>
    /// �����N���擾
    /// </summary>
    /// <returns></returns>
    public static Sprite GetScoreRank(List<Sprite> spriteRanks, int score)
    {
        // �Ăяo����X����s���� , spriteRanks�͏ォ��S~X�̏��Ŋi�[����Ă���
        int i = spriteRanks.Count - 1;
        foreach (var value in Enum.GetValues(typeof(TopManager.ScoreRank)))
        {
            if ((int)value > score)
            {
                return spriteRanks[i];
            }
            i--;
        }

        // �ǂ�ɂ����Ă͂܂�Ȃ������ꍇ�͍Œ�l�̃����N
        return spriteRanks[spriteRanks.Count - 1];
    }
}
