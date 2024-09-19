using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    GameObject m_player;
    Rigidbody2D m_rb;
    public Vector3 m_offset { get; private set; }      // 母親とのオフセット

    // 初速度
    float m_initialSpeed = 50f;
    // 空気抵抗
    float m_dragNum = 3f;
    // 重量スケール
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
    /// ゲストに蹴り飛ばされる処理
    /// </summary>
    public void DOKick(Vector3 vecKick, bool isSetSpeed)
    {
        var rb = GetComponent<Rigidbody2D>();

        // 力を設定
        Vector3 force = vecKick;

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
    public void ResetSon()
    {
        m_rb.gravityScale = 0;
        transform.position = m_player.transform.position + m_offset;
    }

    /// <summary>
    /// ゲストによるリセット処理
    /// </summary>
    public void ResetSon(Vector3 startPos)
    {
        m_rb.gravityScale = 0;
        transform.position = startPos + m_offset;
    }

    /// <summary>
    /// メンバ変数初期化処理
    /// </summary>
    public void InitMemberVariable()
    {
        m_player = GameObject.Find("Player");
        m_rb = GetComponent<Rigidbody2D>();
        m_offset = transform.position - m_player.transform.position;

        // リセットする
        ResetSon();

        // 空気抵抗を設定する
        m_rb.drag = m_dragNum;
    }
}
