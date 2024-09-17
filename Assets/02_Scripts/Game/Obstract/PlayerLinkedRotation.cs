using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLinkedRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 5f;  // ��]���x
    [SerializeField] bool m_isLinkedY;          // �v���C���[��Y���W�ƃ����N�����邩�ǂ���
    [SerializeField] bool m_isLinkedX;          // �v���C���[��X���W�ƃ����N�����邩�ǂ���

    // �v���C���[
    GameObject m_player;
    // �v���C���[�̏����ʒu
    Vector3 m_playerStartPos;
    // �O�t���[���̃v���C���[�̍��W
    Vector3 m_playerPreviousPosition;

    void Start()
    {
        m_player = GameObject.Find("Player");
        m_playerStartPos = m_player.transform.position;
        m_playerPreviousPosition = m_player.transform.position;
    }
    void Update()
    {
        if (m_playerPreviousPosition == m_player.transform.position) return;

        Vector3 movement = (m_player.transform.position - m_playerPreviousPosition);

        if(m_isLinkedY)
        {
            transform.Rotate(0f, 0f, movement.y * rotationSpeed);
        }
        else
        {
            transform.Rotate(0f, 0f, movement.x * rotationSpeed);
        }

        m_playerPreviousPosition = m_player.transform.position;
    }
}
