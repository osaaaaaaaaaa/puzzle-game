using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLinkedMover : MonoBehaviour
{
    [SerializeField] bool m_isMoveX;    // X���W�Ɉړ����邩�ǂ���
    [SerializeField] bool m_isMoveY;    // Y���W�Ɉړ����邩�ǂ���
    [SerializeField] bool m_isInverseX;  // X���W�𔽔�Ⴓ���邩�ǂ���
    [SerializeField] bool m_isInverseY;  // Y���W�𔽔�Ⴓ���邩�ǂ���
    [SerializeField] float m_mulNumX = 1f;    // ��Z����l
    [SerializeField] float m_mulNumY = 1f;    // ��Z����l
    [SerializeField] Vector2 m_minPos = Vector2.zero;
    [SerializeField] Vector2 m_maxPos = Vector2.zero;

    // �v���C���[
    GameObject m_player;
    // �v���C���[�̏����ʒu
    Vector3 m_player_startPos;
    // ���g�̏����ʒu
    Vector3 m_my_startPos;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");

        // �e�Ǝq�̏����ʒu���擾����
        m_player_startPos = m_player.transform.position;
        m_my_startPos = transform.position;
    }

    void Update()
    {
        // �v���C���[�̈ړ��ʂ��v�Z
        Vector3 playerMovement = (m_player.transform.position - m_player_startPos);
        playerMovement = new Vector3(playerMovement.x * m_mulNumX, playerMovement.y * m_mulNumY, playerMovement.z);

        if(m_isMoveX && m_isMoveY)
        {
            // XY���W�𔽔�Ⴓ���Ĉړ�
            if (m_isInverseX && m_isInverseY)
            {
                transform.position = m_my_startPos - playerMovement;
            }
            // X���W�𔽔�Ⴓ���Ĉړ�
            else if (m_isInverseX)
            {
                transform.position = new Vector3(m_my_startPos.x - playerMovement.x, m_my_startPos.y + playerMovement.y, transform.position.z);
            }
            // Y���W�𔽔�Ⴓ���Ĉړ�
            else if (m_isInverseY)
            {
                transform.position = new Vector3(m_my_startPos.x + playerMovement.x, m_my_startPos.y - playerMovement.y, transform.position.z);
            }
            // ��Ⴓ���Ĉړ�
            else
            {
                transform.position = m_my_startPos + playerMovement;
            }
        }
        // X���W�������N������ꍇ
        else if (m_isMoveX)
        {
            // ����Ⴓ���Ĉړ�
            if (m_isInverseX)
            {
                transform.position = new Vector3(m_my_startPos.x - playerMovement.x, m_my_startPos.y, transform.position.z);
            }
            // ��Ⴓ���Ĉړ�
            else
            {
                transform.position = new Vector3(m_my_startPos.x + playerMovement.x, m_my_startPos.y, transform.position.z);
            }
        }
        // Y���W�������N������ꍇ
        else if (m_isMoveY)
        {
            // ����Ⴓ���Ĉړ�
            if (m_isInverseY)
            {
                transform.position = new Vector3(m_my_startPos.x, m_my_startPos.y - playerMovement.y, transform.position.z);
            }
            // ��Ⴓ���Ĉړ�
            else
            {
                transform.position = new Vector3(m_my_startPos.x, m_my_startPos.y + playerMovement.y, transform.position.z);
            }
        }

        // X���W�̍ŏ��l���w�肵�Ă���&&���݂̒l���ŏ��l�ȉ��̏ꍇ
        if(m_minPos.x != 0 && transform.localPosition.x <= m_minPos.x)
        {
            transform.localPosition = new Vector3(m_minPos.x, transform.localPosition.y, transform.localPosition.z);
        }
        // Y���W�̍ŏ��l���w�肵�Ă���&&���݂̒l���ŏ��l�ȉ��̏ꍇ
        if (m_minPos.y != 0 && transform.localPosition.y <= m_minPos.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, m_minPos.y, transform.localPosition.z);
        }
        // X���W�̍ő�l���w�肵�Ă���&&���݂̒l���ő�l�ȏ�̏ꍇ
        if (m_maxPos.x != 0 && transform.localPosition.x >= m_maxPos.x)
        {
            transform.localPosition = new Vector3(m_maxPos.x, transform.localPosition.y, transform.localPosition.z);
        }
        // Y���W�̍ő�l���w�肵�Ă���&&���݂̒l���ő�l�ȏ�̏ꍇ
        if (m_maxPos.y != 0 && transform.localPosition.y >= m_maxPos.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, m_maxPos.y, transform.localPosition.z);
        }
    }
}
