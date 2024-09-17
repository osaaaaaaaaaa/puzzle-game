using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    GameObject m_player;
    Rigidbody2D m_rb;
    public Vector3 m_offset { get; private set; }      // ��e�Ƃ̃I�t�Z�b�g

    // �����x
    float m_initialSpeed = 50f;
    // ��C��R
    float m_dragNum = 3f;
    // �d�ʃX�P�[��
    float m_gravityScale = 10f;

    public ColorData.COLOR_TYPE m_coloerType { get; private set; } = ColorData.COLOR_TYPE.DEFAULT;
    public void ChangeColorType(ColorData.COLOR_TYPE type)
    {
        m_coloerType = type;

        switch (type)
        {
            case ColorData.COLOR_TYPE.RED:
                GetComponent<SpriteRenderer>().color = new Color(1f,0.6f,0.8f,1f);
                break;
            case ColorData.COLOR_TYPE.BLUE:
                GetComponent<SpriteRenderer>().color = new Color(0.5f, 0.8f, 1f, 1f);
                break;
            case ColorData.COLOR_TYPE.PURPLE:
                GetComponent<SpriteRenderer>().color = new Color(0.9f, 0.5f, 1f, 1f);
                break;
        }
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
    /// �Q�X�g�ɏR���΂���鏈��
    /// </summary>
    public void DOKick(Vector3 vecKick, bool isSetSpeed)
    {
        var rb = GetComponent<Rigidbody2D>();

        // �͂�ݒ�
        Vector3 force = vecKick;

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
    public void ResetSon()
    {
        m_rb.gravityScale = 0;
        transform.position = m_player.transform.position + m_offset;
    }

    /// <summary>
    /// �Q�X�g�ɂ�郊�Z�b�g����
    /// </summary>
    public void ResetSon(Vector3 startPos)
    {
        m_rb.gravityScale = 0;
        transform.position = startPos + m_offset;
    }

    /// <summary>
    /// �����o�ϐ�����������
    /// </summary>
    public void InitMemberVariable()
    {
        m_player = GameObject.Find("Player");
        m_rb = GetComponent<Rigidbody2D>();
        m_offset = transform.position - m_player.transform.position;

        // ���Z�b�g����
        ResetSon();

        // ��C��R��ݒ肷��
        m_rb.drag = m_dragNum;
    }
}
