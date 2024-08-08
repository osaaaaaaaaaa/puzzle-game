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

    // �J�E���^�[
    int m_timer;

    private void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_textCounter = GameObject.Find("TextCounter");
        m_timer = 3;
    }

    /// <summary>
    /// ���O���C���[��Son���C���[�̂ݐڐG������E���悤�ݒ�ς�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (m_gameManager.m_isStageClear) return;

        // �J�E���g�J�n
        m_textCounter.SetActive(true);
        InvokeRepeating("StartCountDown", 0, 1);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (m_gameManager.m_isStageClear) return;

        // �J�E���g�L�����Z��
        m_textCounter.SetActive(false);
        CancelInvoke("StartCountDown");
        m_timer = 3;
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
            m_timer = 3;
        }
        else
        {
            m_textCounter.GetComponent<Text>().text = "" + m_timer;
        }
    }
}
