using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    [SerializeField] GameObject m_mom;
    PhysicsMaterial2D m_material;  // �����}�e���A��
    Rigidbody2D m_rb;
    Vector2 m_startPos;
    Vector3 m_offsetStartPos;      // ��e�Ƃ̃I�t�Z�b�g

    // Start is called before the first frame update
    void Start()
    {
        m_startPos = transform.position;
        m_rb = GetComponent<Rigidbody2D>();
        m_material = m_rb.sharedMaterial;
        m_offsetStartPos = transform.position - m_mom.transform.position;

        Reset();
    }

    /// <summary>
    /// �R���΂���鏈��
    /// </summary>
    /// <param name="dir">���p</param>
    /// <param name="power">�p���[</param>
    public void BeKicked(Vector3 dir, float power)
    {
        m_rb.sharedMaterial = m_material;   // �����}�e���A���Z�b�g
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // �͂�ݒ�
        m_rb.AddForce(force, ForceMode2D.Impulse);  // �͂�������
    }

    public void Reset()
    {
        m_rb.sharedMaterial = null;   // �����}�e���A�����O��
        transform.position = m_mom.transform.position + m_offsetStartPos;
    }
}
