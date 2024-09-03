using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using System;

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

    #region �X�e�[�W�̐i�����
    bool m_isMedal1;
    bool m_isMedal2;
    float m_gameTimer;
    public bool m_isEndGame { get; private set; }
    #endregion

    #region �Q�[���N���A���̉��o
    [SerializeField] GameObject m_stageClearEffect;
    Vector3 m_offsetEffectL = new Vector3(-10.43f, -7.48999f,5);
    Vector3 m_offsetEffectR = new Vector3(10.39f, -7.48999f,5);
    #endregion

    #region ���C���J�����̃A�j���[�V�����֌W
    GameObject m_mainCamera;   // ���C���J����
    public bool m_isEndAnim;   // �A�j���[�V�������I���������ǂ���
    #endregion

    private void Awake()
    {
        m_isEndAnim = false;
        m_isEndGame = false;
        m_isMedal1 = false;
        m_isMedal2 = false;
        m_gameTimer = 40;

#if UNITY_EDITOR
        // �e��X�N���v�g�̃����o�ϐ�����������
        GameObject.Find("CameraController").GetComponent<CameraController>().
            InitMemberVariable(m_UiController.ButtonZoomIn, m_UiController.ButtonZoomOut);
        GameObject.Find("ride_cow").GetComponent<SonCow>().InitMemberVariable();
        GameObject.Find("Son").GetComponent<Son>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();

        // �O�񃁃_�����擾���Ă���ꍇ�͕\����ύX���� �A ���U���g�����݂��Ȃ��ꍇ�͕K��false
        bool isMedal1 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ? 
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal1; 
        bool isMedal2 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ? 
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal2;
        GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(isMedal1);
        GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(isMedal2);
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
        GameObject.Find("Son").GetComponent<Son>().InitMemberVariable();
        GameObject.Find("Goal").GetComponent<Goal>().InitMemberVariable();
        GameObject.Find("SonController").GetComponent<SonController>().InitMemberVariable();

        // �O�񃁃_�����擾���Ă���ꍇ�͕\����ύX���� �A ���U���g�����݂��Ȃ��ꍇ�͕K��false
        bool isMedal1 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ? 
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal1; 
        bool isMedal2 = NetworkManager.Instance.StageResults.Count < TopManager.stageID ? 
            false : NetworkManager.Instance.StageResults[TopManager.stageID - 1].IsMedal2;
        GameObject.Find("Medal1").GetComponent<Medal>().InitMemberVariable(isMedal1);
        GameObject.Find("Medal2").GetComponent<Medal>().InitMemberVariable(isMedal2);
#endif

        // �ǂ��\���ɂ���
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_T").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // �Q�[���I�u�W�F�N�g�擾
        m_mainCamera = GameObject.Find("MainCamera_Game");
        m_player = GameObject.Find("Player");
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        // �t���OOFF
        m_isPause = false;
        m_isEndGame = false;

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

    private void Update()
    {
        // �J�E���g�_�E��
        if (m_isEndAnim && !m_isEndGame)
        {
            m_gameTimer -= Time.deltaTime;
            m_gameTimer = m_gameTimer <= 0 ? 0 : m_gameTimer;
            m_isEndGame = m_gameTimer <= 0 ? true : false;

            // �^�C�}�[�e�L�X�g���X�V
            m_UiController.UpdateTextTimer(m_gameTimer);

            if (m_isEndGame) GameOver();
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
    /// �W�v�����X�R�A�擾
    /// </summary>
    /// <returns></returns>
    int GetResultScore(float time)
    {
        // �X�R�A�W�v
        int medalCnt = m_isMedal1 ? 1 : 0;
        medalCnt += m_isMedal2 ? 1 : 0;
        int score = (int)(time * 15) + (medalCnt * 500);
        score = score <= 0 ? 0 : score;
        return score;
    }

    /// <summary>
    /// �Q�[���I�[�o�[����
    /// </summary>
    public void GameOver()
    {
        m_seManager.PlayGameOverSE();
        m_UiController.SetGameOverUI(m_isMedal1, m_isMedal2, m_gameTimer, GetResultScore(m_gameTimer), false);
    }

    /// <summary>
    /// �Q�[���N���A����
    /// </summary>
    public void GameClear()
    {
        float time = m_gameTimer;
        int score = GetResultScore(time);

        // ���݂̃X�e�[�W������ȉ������ŐV�̃X�e�[�W���N���A�������ǂ���
        bool isUpdateStageID = NetworkManager.Instance.StageID < TopManager.stageMax && NetworkManager.Instance.StageID == TopManager.stageID;

        // ���_�������l������||�n�C�X�R�A�����������ǂ���
        bool isUpdateResult = false;
        if (!isUpdateStageID)
        {
            ShowStageResultResponse currentResult = NetworkManager.Instance.StageResults[TopManager.stageID - 1];
            isUpdateResult = !currentResult.IsMedal1 && m_isMedal1
                || !currentResult.IsMedal2 && m_isMedal2
                || currentResult.Score < score;
        }

        // ���U���g���X�V����K�v������ꍇ
        if (isUpdateStageID || isUpdateResult)
        {
            // �X�e�[�W�N���A����
            StartCoroutine(NetworkManager.Instance.UpdateStageClear(
                isUpdateStageID,
                new ShowStageResultResponse { 
                    StageID = TopManager.stageID, 
                    IsMedal1 = m_isMedal1, 
                    IsMedal2 = m_isMedal2, 
                    Time = time,
                    Score = score 
                },
                result =>
                {
                    // �X�e�[�W���N���A�������Ƃɂ���
                    m_isEndGame = true;

                    // UI���Q�[���N���A�p�ɐݒ肷��
                    m_UiController.SetResultUI(m_isMedal1,m_isMedal2, m_gameTimer, score, true);

                    // ���N���A���o
                    PlayStageClearEffect();
                }));
        }
        else
        {
            // �X�e�[�W���N���A�������Ƃɂ���
            m_isEndGame = true;

            // UI���Q�[���N���A�p�ɐݒ肷��
            m_UiController.SetResultUI(m_isMedal1, m_isMedal2,m_gameTimer, score,true);

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
        m_gameTimer -= 2f;
        m_UiController.GetComponent<UiController>().GenerateSubTimeText(2);
        m_player.GetComponent<Player>().ResetPlayer();
    }

    public void UpdateMedalFrag(int medarID)
    {
        switch (medarID)
        {
            case 1:
                m_isMedal1 = true;
                break;
            case 2:
                m_isMedal2 = true;
                break;
        }
    }
}
