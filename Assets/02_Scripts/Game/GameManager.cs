using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    // �X�e�[�W�̃v���t�@�u
    [SerializeField] GameObject[] m_stagePrefabs;
    // UI�R���g���[���[
    [SerializeField] UiController m_UiController;

    [SerializeField] 

    // ���q
    GameObject m_son;

    #region ���C���J�����̃A�j���[�V�����֌W
    [SerializeField] GameObject m_mainCamera;   // ���C���J����
    GameObject goal;                            // �S�[���n�_
    public bool m_isEndAnim;                    // �A�j���[�V�������I���������ǂ���
    #endregion

    private void Awake()
    {
        // �g�b�v��ʂ��\���ɂ���
        if (Singleton.Instance != null) Singleton.Instance.ChangeActive(false);

        // �X�e�[�W�𐶐�����
        if (TopManager.stageID != 0)
        {
            Instantiate(m_stagePrefabs[TopManager.stageID - 1], Vector3.zero, Quaternion.identity);
        }

        // �ǂ��\���ɂ���
        GameObject.Find("Wall_R").GetComponent<Renderer>().enabled = false;
        GameObject.Find("Wall_L").GetComponent<Renderer>().enabled = false;

        // �Q�[���I�u�W�F�N�g�擾
        m_son = GameObject.Find("Son");
        goal = GameObject.Find("Goal");
    }

    // Start is called before the first frame update
    void Start()
    {
#if !UNITY_EDITOR
        // ���C���J�����̏����n�_
        float posX = goal.transform.position.x;
        float posY = goal.transform.position.y < 0 ? 0 : goal.transform.position.y + 2; // +2�͒���
        m_mainCamera.transform.position = new Vector3(posX, posY, -10);

        // ���C���J�����̃A�j���[�V����
        var sequence = DOTween.Sequence();
        sequence.Append(m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 2f).SetEase(Ease.InOutSine).SetDelay(1f))
                .OnComplete(() => m_isEndAnim = true);
        sequence.Play();
#else
        m_isEndAnim = true;
#endif

        // �ŏI�X�e�[�W�̏ꍇ
        if (TopManager.stageID >= m_stagePrefabs.Length)
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
        // UI���Q�[���N���A�p�ɐݒ肷��
        m_UiController.SetGameClearUI();
    }

    /// <summary>
    /// ���g���C����
    /// </summary>
    public void OnRetryButton()
    {
        SceneManager.LoadScene("02_GameScene");
    }

    /// <summary>
    /// ���̃X�e�[�W�֑J�ڂ���
    /// </summary>
    public void OnNextStageButton()
    {
        TopManager.stageID++;   // �X�e�[�WID���X�V����

        SceneManager.LoadScene("02_GameScene");
    }

    /// <summary>
    /// �g�b�v��ʂ֑J�ڂ���
    /// </summary>
    public void OnTopButton()
    {
        SceneManager.LoadScene("01_TopScene");
    }

    /// <summary>
    /// �Q�[�����Z�b�g
    /// </summary>
    public void OnGameReset()
    {
        m_son.GetComponent<Son>().Reset();
    }
}
