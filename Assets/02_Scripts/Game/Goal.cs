using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager manager;

    /// <summary>
    /// ���O���C���[��Son���C���[�̂ݐڐG������E���悤�ݒ�ς�
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �Q�[���N���A����
        manager.GameClear();
    }
}
