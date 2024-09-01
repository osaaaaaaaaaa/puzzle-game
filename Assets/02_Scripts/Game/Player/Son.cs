using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    GameObject m_player;
    Rigidbody2D m_rb;
    Vector3 m_offset;      // 母親とのオフセット

    // 初速度
    public float m_initialSpeed = 50f;
    // 空気抵抗
    public float m_dragNum = 3f;
    // 重量スケール
    public float m_gravityScale = 10f;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        m_rb = GetComponent<Rigidbody2D>();
        m_offset = transform.position - m_player.transform.position;

        // リセットする
        Reset();

        // 空気抵抗を設定する
        m_rb.drag = m_dragNum;
    }

    /// <summary>
    /// 蹴り飛ばされる処理
    /// </summary>
    /// <param name="dir">方角</param>
    /// <param name="power">パワー</param>
    public void DOKick(Vector3 dir, float power, bool isSetSpeed)
    {
        var rb = GetComponent<Rigidbody2D>();

        // 力を設定
        Vector3 force = new Vector3(dir.x * power, dir.y * power);

        // 重力を設定する
        rb.gravityScale = m_gravityScale;

        if (isSetSpeed)
        {
            // 初速度を設定する
            rb.velocity = transform.forward * m_initialSpeed;
            // 力を加える
            rb.AddForce(force, ForceMode2D.Impulse);
        }
        else
        {
            // 力を加える
            rb.AddForce(force, ForceMode2D.Force);
        }
    }

    /// <summary>
    /// リセット処理
    /// </summary>
    public void Reset()
    {
        m_rb.gravityScale = 0;
        transform.position = m_player.transform.position + m_offset;
    }
}
