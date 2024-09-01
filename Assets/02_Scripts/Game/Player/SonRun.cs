using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonRun : MonoBehaviour
{
    [SerializeField] LayerMask m_obstacleLayer;     // ��Q���̃��C���[�^�O
    Rigidbody2D m_rb;
    float m_speed = 7f;
    public int m_direction = 1;
    bool m_isStop;

    float m_lineStart = 0.2f;
    float m_lineEnd = 0.9f;

    void Start()
    {
        m_rb = GetComponent<Rigidbody2D>();
        m_isStop = false;
    }

    private void FixedUpdate()
    {
        if (m_isStop) return;

        // �ǂɏՓ˂����ꍇ
        if (IsWall())
        {
            m_direction *= -1;  // �ړ������𔽓]������
            m_rb.velocity = Vector3.zero;
        }

        // �e�N�X�`���̌�����ݒ肷��
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * m_direction, transform.localScale.y, transform.localScale.z);

        // �ړ�����
        m_rb.velocity = new Vector2(m_speed * m_direction, m_rb.velocity.y); ;
    }

    /// <summary>
    /// ����̐�ɕǂ����邩�ǂ���
    /// </summary>
    /// <returns></returns>
    private bool IsWall()
    {
        Vector3 startVec = Vector3.zero, endVec = Vector3.zero;
        Vector3 startVec2 = Vector3.zero, endVec2 = Vector3.zero;

        startVec = transform.position + transform.right * 0.2f * transform.localScale.x + transform.up * m_lineStart;    // �n�_
        endVec = startVec + transform.up * transform.localScale.y * m_lineEnd;    // �I�_

        Debug.DrawLine(startVec, endVec, Color.red);   // �`�ʂ���

        return Physics2D.Linecast(startVec, endVec, m_obstacleLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �S�[���ɐG�ꂽ�ꍇ�A��~����
        if (collision.gameObject.CompareTag("Goal"))
        {
            m_isStop = true;
            GetComponent<Animator>().Play("IdleAnimation");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �S�[�����痣�ꂽ�ꍇ�A�Ăё���o��
        if (collision.gameObject.CompareTag("Goal"))
        {
            m_isStop = false;
            GetComponent<Animator>().Play("RunAnimation");
        }
    }
}
