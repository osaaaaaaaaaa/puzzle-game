using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    // �Q�[���}�l�[�W���[
    GameManager m_gameManager;
    // �J�E���^�[�p�̃e�L�X�g
    GameObject m_textCounter;
    // UI�R���g���[���[
    UiController m_uiController;

    // �J�E���^�[
    const int TIME_LIMIT_MAX = 3;
    int m_timer;

    bool isInit = false;

    private void Start()
    {
        m_timer = TIME_LIMIT_MAX;
    }

    private void Update()
    {
        if (!isInit) return;

        if (m_gameManager.m_isEndGame)
        {
            CancelCountDown();
        }
    }

    /// <summary>
    /// ���O���C���[��Son���C���[�̂ݐڐG������E���悤�ݒ�ς�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TopSceneDirector.Instance == null || m_gameManager.m_isEndGame || collision.gameObject.tag == "Ghost" 
            || collision.gameObject.layer != 6 && collision.gameObject.layer != 10
            || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;

        if(m_textCounter == null)
        {
            m_textCounter = GameObject.Find("TextCounter");
        }
        // �J�E���g�J�n
        m_textCounter.SetActive(true);
        InvokeRepeating("StartCountDown", 0, 1);

        // ���Z�b�g�{�^���𖳌��ɂ���
        m_uiController.SetInteractableButtonReset(false);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ghost" || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;
        if (m_gameManager.m_isEndGame) return;

        CancelCountDown();

        // ���Z�b�g�{�^����L���ɂ���
        m_uiController.SetInteractableButtonReset(true);
    }

    /// <summary>
    /// �J�E���^�[
    /// </summary>
    private void StartCountDown()
    {
        m_timer--;

        if (m_timer <= 0)
        {
            // �Q�[���N���A����
            m_gameManager.GameClear();

            // �J�E���g�L�����Z��
            CancelInvoke("StartCountDown");
            m_timer = TIME_LIMIT_MAX;
        }
        else
        {
            m_textCounter.GetComponent<Text>().text = "" + m_timer;
        }
    }

    /// <summary>
    /// �J�E���^�[���L�����Z������
    /// </summary>
    private void CancelCountDown()
    {
        // �J�E���g�L�����Z��
        m_textCounter.SetActive(false);
        CancelInvoke("StartCountDown");
        m_timer = TIME_LIMIT_MAX;
    }

    /// <summary>
    /// �����o�ϐ�����������
    /// </summary>
    public void InitMemberVariable()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_textCounter = GameObject.Find("TextCounter");
        m_uiController = GameObject.Find("UiController").GetComponent<UiController>();
        isInit = true;
    }
}
