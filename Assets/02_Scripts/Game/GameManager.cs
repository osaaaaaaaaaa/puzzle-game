using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AddressableAssets;

public class GameManager : MonoBehaviour
{
    // �|�[�Y�����ǂ���
    public bool m_isPause;

    // UI�R���g���[���[
    [SerializeField] UiController m_UiController;
    // SE�}�l�[�W���[
    [SerializeField] SEManager m_seManager;
    // �v���C���[
    GameObject m_player;
    // �J�����R���g���[��
    CameraController m_cameraController;

    #region �Q�[���N���A���̉��o
    [SerializeField] GameObject m_stageClearEffect;
    Vector3 m_offsetEffectL = new Vector3(-10.43f, -7.48999f,5);
    Vector3 m_offsetEffectR = new Vector3(10.39f, -7.48999f,5);
    #endregion

    #region ���C���J�����̃A�j���[�V�����֌W
    GameObject m_mainCamera;   // ���C���J����
    GameObject goal;           // �S�[���n�_
    public bool m_isEndAnim;   // �A�j���[�V�������I���������ǂ���
    #endregion

    // �Q�[�����N���A�������ǂ���
    public bool m_isStageClear;

    private void Awake()
    {
        m_isEndAnim = false;

#if UNITY_EDITOR
        // �e��X�N���v�g�̃����o�ϐ�����������
        GameObject.Find("CameraController").GetComponent<CameraController>().
            InitMemberVariable(m_UiController.ButtonZoomIn, m_UiController.ButtonZoomOut);
        GameObject.Find("ride_cow").GetComponent<SonCow>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();
#endif

        // �g�b�v��ʂ��\���ɂ���
        if (Singleton.Instance != null) Singleton.Instance.ChangeActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        // �A�Z�b�g�o���h�����g�p����ꍇ�AStart���\�b�h�̌^��IEnumerator�ɕύX���邱��
        // �r���h����Ƃ��́A�S�̂�!�����邱��
#if !UNITY_EDITOR
        // �Q�[���V�[����ǂݍ��ނ܂őҋ@����
        var op = Addressables.LoadSceneAsync(TopManager.stageID + "_GameScene", LoadSceneMode.Additive);
        yield return op;

        // �e��X�N���v�g�̃����o�ϐ�����������
        GameObject.Find("CameraController").GetComponent<CameraController>().
            InitMemberVariable(m_UiController.ButtonZoomIn, m_UiController.ButtonZoomOut);
        GameObject.Find("ride_cow").GetComponent<SonCow>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();
#endif

        // �ǂ��\���ɂ���
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_T").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // �Q�[���I�u�W�F�N�g�擾
        m_mainCamera = GameObject.Find("MainCamera_Game");
        m_player = GameObject.Find("Player");
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        goal = GameObject.Find("Goal");

        // �t���OOFF
        m_isPause = false;
        m_isStageClear = false;

#if !UNITY_EDITOR
        // �J�����̏����n�_���擾
        Vector3 startPos = m_mainCamera.transform.position;

        // �J�������A�j���[�V��������J�n���W��ݒ�
        float posX = goal.transform.position.x;
        float posY = goal.transform.position.y;
        m_mainCamera.transform.position = new Vector3(posX, posY, -10);

        // ���C���J�����̃A�j���[�V����
        var sequence = DOTween.Sequence();
        sequence.Append(m_mainCamera.transform.DOMove(startPos, 2f).SetEase(Ease.InOutSine).SetDelay(1f))
                .OnComplete(StartGame) ;
        sequence.Play();
#else
        StartGame();
#endif

        // �ŏI�X�e�[�W�̏ꍇ
        if (TopManager.stageID >= TopManager.stageMax)
        {
            // ���̃X�e�[�W�֑J�ڂ���{�^���𖳌�������
            m_UiController.DisableNextStageButton();
        }
    }

    /// <summary>
    /// �Q�[���J�n����
    /// </summary>
    public void StartGame()
    {
        m_cameraController.ZoomOut(1f);

        m_UiController.GetComponent<UiController>().SetActiveGameUI(true);

        m_isEndAnim = true;
    }

    /// <summary>
    /// �Q�[���N���A����
    /// </summary>
    public void GameClear()
    {
        bool isUpdateStageID = NetworkManager.Instance.StageID < TopManager.stageMax && NetworkManager.Instance.StageID == TopManager.stageID;

        // �ŐV�̃X�e�[�W���N���A�����ꍇ
        if (isUpdateStageID)
        {
            // ���[�U�[�X�V����
            StartCoroutine(NetworkManager.Instance.UpdateUser(
                NetworkManager.Instance.UserName,
                NetworkManager.Instance.AchievementID,
                NetworkManager.Instance.StageID + 1,
                NetworkManager.Instance.IconID,
                result =>
                {
                    // �X�e�[�W���N���A�������Ƃɂ���
                    m_isStageClear = true;

                    // UI���Q�[���N���A�p�ɐݒ肷��
                    m_UiController.SetGameClearUI();

                    // ���N���A���o
                    PlayStageClearEffect();
                }));
        }
        else
        {
            // �X�e�[�W���N���A�������Ƃɂ���
            m_isStageClear = true;

            // UI���Q�[���N���A�p�ɐݒ肷��
            m_UiController.SetGameClearUI();

            // ���N���A���o
            PlayStageClearEffect();
        }
    }

    /// <summary>
    /// �X�e�[�W�N���A�̉��o
    /// </summary>
    void PlayStageClearEffect()
    {
        // �G�t�F�N�g�𐶐�����
        GameObject effectL = Instantiate(m_stageClearEffect, m_mainCamera.transform);
        effectL.transform.localPosition = m_offsetEffectL;
        GameObject effectR = Instantiate(m_stageClearEffect, m_mainCamera.transform);
        effectR.transform.localPosition = m_offsetEffectR;
        effectR.transform.localScale = new Vector3(-effectR.transform.localScale.x, effectR.transform.localScale.y, effectR.transform.localScale.z);

        if(m_cameraController.m_cameraMode == CameraController.CAMERAMODE.ZOOMOUT)
        {
            // ���݂̃J�������Y�[���A�E�g��Ԃ̏ꍇ
            effectL.transform.localScale *= 2;
            effectL.transform.localPosition *= 2;
            effectR.transform.localScale *= 2;
            effectR.transform.localPosition *= 2;
        }

        m_seManager.PlayStageClearSE();
    }

    /// <summary>
    /// ���g���C����
    /// </summary>
    public void OnRetryButton()
    {
#if !UNITY_EDITOR
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
#endif
    }

    /// <summary>
    /// ���̃X�e�[�W�֑J�ڂ���
    /// </summary>
    public void OnNextStageButton()
    {
        TopManager.stageID++;   // �X�e�[�WID���X�V����

#if !UNITY_EDITOR
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
#endif
    }

    /// <summary>
    /// �g�b�v��ʂ֑J�ڂ���
    /// </summary>
    public void OnTopButton()
    {
        Initiate.Fade("01_TopScene", Color.black, 1.0f);
    }

    /// <summary>
    /// �Q�[�����Z�b�g
    /// </summary>
    public void OnGameReset()
    {
        m_player.GetComponent<Player>().ResetPlayer();
    }
}
