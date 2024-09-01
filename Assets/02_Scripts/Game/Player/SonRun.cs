using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonRun : MonoBehaviour
{
    [SerializeField] LayerMask m_obstacleLayer;     // 障害物のレイヤータグ
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

        // 壁に衝突した場合
        if (IsWall())
        {
            m_direction *= -1;  // 移動方向を反転させる
            m_rb.velocity = Vector3.zero;
        }

        // テクスチャの向きを設定する
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * m_direction, transform.localScale.y, transform.localScale.z);

        // 移動処理
        m_rb.velocity = new Vector2(m_speed * m_direction, m_rb.velocity.y); ;
    }

    /// <summary>
    /// 判定の先に壁があるかどうか
    /// </summary>
    /// <returns></returns>
    private bool IsWall()
    {
        Vector3 startVec = Vector3.zero, endVec = Vector3.zero;
        Vector3 startVec2 = Vector3.zero, endVec2 = Vector3.zero;

        startVec = transform.position + transform.right * 0.2f * transform.localScale.x + transform.up * m_lineStart;    // 始点
        endVec = startVec + transform.up * transform.localScale.y * m_lineEnd;    // 終点

        Debug.DrawLine(startVec, endVec, Color.red);   // 描写する

        return Physics2D.Linecast(startVec, endVec, m_obstacleLayer);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ゴールに触れた場合、停止する
        if (collision.gameObject.CompareTag("Goal"))
        {
            m_isStop = true;
            GetComponent<Animator>().Play("IdleAnimation");
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // ゴールから離れた場合、再び走り出す
        if (collision.gameObject.CompareTag("Goal"))
        {
            m_isStop = false;
            GetComponent<Animator>().Play("RunAnimation");
        }
    }
}
