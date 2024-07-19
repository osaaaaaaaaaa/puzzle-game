using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    GameManager m_gameManager;

    private void Start()
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    /// <summary>
    /// ���O���C���[��Son���C���[�̂ݐڐG������E���悤�ݒ�ς�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �Q�[���N���A����
        m_gameManager.GameClear();
    }
}
