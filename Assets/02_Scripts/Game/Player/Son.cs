using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    GameObject m_player;
    PhysicsMaterial2D m_material;  // �����}�e���A��
    Rigidbody2D m_rb;
    Vector2 m_startPos;
    Vector3 m_offsetStartPos;      // ��e�Ƃ̃I�t�Z�b�g

    // �����x
    public float m_initialSpeed = 50f;
    // ��C��R
    public float m_dragNum = 3f;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        m_startPos = transform.position;
        m_rb = GetComponent<Rigidbody2D>();
        m_material = m_rb.sharedMaterial;
        m_offsetStartPos = transform.position - m_player.transform.position;

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
    public void BeKicked(Vector3 dir, float power)
    {
        // ���x��ݒ肷��
        m_rb.velocity = transform.forward * m_initialSpeed;

        m_rb.sharedMaterial = m_material;   // �����}�e���A���Z�b�g
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // �͂�ݒ�
        m_rb.AddForce(force, ForceMode2D.Impulse);  // �͂�������
    }

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    public void Reset()
    {
        m_rb.sharedMaterial = null;   // �����}�e���A�����O��
        transform.position = m_player.transform.position + m_offsetStartPos;
    }
}
