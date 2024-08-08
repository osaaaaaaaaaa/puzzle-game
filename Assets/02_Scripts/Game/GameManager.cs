using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // �|�[�Y�����ǂ���
    public bool m_isPause;

    // UI�R���g���[���[
    [SerializeField] UiController m_UiController;
    // �v���C���[
    GameObject m_player;

    #region ���C���J�����̃A�j���[�V�����֌W
    GameObject m_mainCamera;   // ���C���J����
    GameObject goal;           // �S�[���n�_
    public bool m_isEndAnim;   // �A�j���[�V�������I���������ǂ���
    #endregion

    // �Q�[�����N���A�������ǂ���
    public bool m_isStageClear;

    private void Awake()
    {
        // �g�b�v��ʂ��\���ɂ���
        if (Singleton.Instance != null) Singleton.Instance.ChangeActive(false);

        // �ǂ��\���ɂ���
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_T").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // �Q�[���I�u�W�F�N�g�擾
        m_mainCamera = GameObject.Find("MainCamera_Game");
        m_player = GameObject.Find("Player");
        goal = GameObject.Find("Goal");

        // �t���OOFF
        m_isPause = false;
        m_isStageClear = false;
    }

    // Start is called before the first frame update
    void Start()
    {
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
                .OnComplete(() => m_isEndAnim = true);
        sequence.Play();
#else
        m_isEndAnim = true;
#endif

        // �ŏI�X�e�[�W�̏ꍇ
        if (TopManager.stageID >= TopManager.stageMax)
        {
            // ���̃X�e�[�W�֑J�ڂ���{�^���𖳌�������
            m_UiController.DisableNextStageButton();
        }
    }

    /// <summary>
    /// �Q�[���N���A����
    /// </summary>
    public void GameClear()
    {
        // �X�e�[�W���N���A�������Ƃɂ���
        m_isStageClear = true;

        // UI���Q�[���N���A�p�ɐݒ肷��
        m_UiController.SetGameClearUI();
    }

    /// <summary>
    /// ���g���C����
    /// </summary>
    public void OnRetryButton()
    {
        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
    }

    /// <summary>
    /// ���̃X�e�[�W�֑J�ڂ���
    /// </summary>
    public void OnNextStageButton()
    {
        TopManager.stageID++;   // �X�e�[�WID���X�V����

        Initiate.Fade(TopManager.stageID + "_GameScene", Color.black, 1.0f);
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
