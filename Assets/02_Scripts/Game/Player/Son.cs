using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    GameObject m_player;
    Rigidbody2D m_rb;
    Vector3 m_offset;      // ��e�Ƃ̃I�t�Z�b�g

    // �����x
    public float m_initialSpeed = 50f;
    // ��C��R
    public float m_dragNum = 3f;
    // �d�ʃX�P�[��
    public float m_gravityScale = 10f;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        m_rb = GetComponent<Rigidbody2D>();
        m_offset = transform.position - m_player.transform.position;

        // ���Z�b�g����
        Reset();

        // ��C��R��ݒ肷��
        m_rb.drag = m_dragNum;
    }

    /// <summary>
    /// �R���΂���鏈��
    /// </summary>
    /// <param name="dir">���p</param>
    /// <param name="power">�p���[</param>
    public void DOKick(Vector3 dir, float power, bool isSetSpeed)
    {
        var rb = GetComponent<Rigidbody2D>();

        // �͂�ݒ�
        Vector3 force = new Vector3(dir.x * power, dir.y * power);

        // �d�͂�ݒ肷��
        rb.gravityScale = m_gravityScale;

        if (isSetSpeed)
        {
            // �����x��ݒ肷��
            rb.velocity = transform.forward * m_initialSpeed;
            // �͂�������
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            // �͂�������
            rb.AddForce(force, ForceMode2D.Force);
        }
    }

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    public void Reset()
    {
        m_rb.gravityScale = 0;
        transform.position = m_player.transform.position + m_offset;
    }
}
