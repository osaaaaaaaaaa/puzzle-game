using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    GameObject m_player;
    PhysicsMaterial2D m_material;  // 物理マテリアル
    Rigidbody2D m_rb;
    Vector2 m_startPos;
    Vector3 m_offsetStartPos;      // 母親とのオフセット

    // 初速度
    public float m_initialSpeed = 50f;
    // 空気抵抗
    public float m_dragNum = 3f;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");
        m_startPos = transform.position;
        m_rb = GetComponent<Rigidbody2D>();
        m_material = m_rb.sharedMaterial;
        m_offsetStartPos = transform.position - m_player.transform.position;

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
    public void BeKicked(Vector3 dir, float power)
    {
        // 速度を設定する
        m_rb.velocity = transform.forward * m_initialSpeed;

        m_rb.sharedMaterial = m_material;   // 物理マテリアルセット
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // 力を設定
        m_rb.AddForce(force, ForceMode2D.Impulse);  // 力を加える
    }

    /// <summary>
    /// リセット処理
    /// </summary>
    public void Reset()
    {
        m_rb.sharedMaterial = null;   // 物理マテリアルを外す
        transform.position = m_player.transform.position + m_offsetStartPos;
    }
}
