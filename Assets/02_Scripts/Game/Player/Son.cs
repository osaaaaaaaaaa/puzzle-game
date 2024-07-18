using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// �R���΂���鏈��
    /// </summary>
    /// <param name="dir">���p</param>
    /// <param name="power">�p���[</param>
    public void BeKicked(Vector3 dir, float power)
    {
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // �͂�ݒ�
        rb.AddForce(force, ForceMode2D.Impulse);  // �͂�������
    }

    public void Reset()
    {
        transform.position = startPos;
    }
}
