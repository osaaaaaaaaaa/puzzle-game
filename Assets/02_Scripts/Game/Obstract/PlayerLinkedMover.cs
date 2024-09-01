using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLinkedMover : MonoBehaviour
{
    [SerializeField] bool m_isMoveX;    // X座標に移動するかどうか
    [SerializeField] bool m_isMoveY;    // Y座標に移動するかどうか
    [SerializeField] bool m_isInverseX;  // X座標を反比例させるかどうか
    [SerializeField] bool m_isInverseY;  // Y座標を反比例させるかどうか
    [SerializeField] float m_mulNumX = 1f;    // 乗算する値
    [SerializeField] float m_mulNumY = 1f;    // 乗算する値
    [SerializeField] Vector2 m_minPos = Vector2.zero;
    [SerializeField] Vector2 m_maxPos = Vector2.zero;

    // プレイヤー
    GameObject m_player;
    // プレイヤーの初期位置
    Vector3 m_player_startPos;
    // 自身の初期位置
    Vector3 m_my_startPos;

    // Start is called before the first frame update
    void Start()
    {
        m_player = GameObject.Find("Player");

        // 親と子の初期位置を取得する
        m_player_startPos = m_player.transform.position;
        m_my_startPos = transform.position;
    }

    void Update()
    {
        // プレイヤーの移動量を計算
        Vector3 playerMovement = (m_player.transform.position - m_player_startPos);
        playerMovement = new Vector3(playerMovement.x * m_mulNumX, playerMovement.y * m_mulNumY, playerMovement.z);

        if(m_isMoveX && m_isMoveY)
        {
            // XY座標を反比例させて移動
            if (m_isInverseX && m_isInverseY)
            {
                transform.position = m_my_startPos - playerMovement;
            }
            // X座標を反比例させて移動
            else if (m_isInverseX)
            {
                transform.position = new Vector3(m_my_startPos.x - playerMovement.x, m_my_startPos.y + playerMovement.y, transform.position.z);
            }
            // Y座標を反比例させて移動
            else if (m_isInverseY)
            {
                transform.position = new Vector3(m_my_startPos.x + playerMovement.x, m_my_startPos.y - playerMovement.y, transform.position.z);
            }
            // 比例させて移動
            else
            {
                transform.position = m_my_startPos + playerMovement;
            }
        }
        // X座標をリンクさせる場合
        else if (m_isMoveX)
        {
            // 反比例させて移動
            if (m_isInverseX)
            {
                transform.position = new Vector3(m_my_startPos.x - playerMovement.x, m_my_startPos.y, transform.position.z);
            }
            // 比例させて移動
            else
            {
                transform.position = new Vector3(m_my_startPos.x + playerMovement.x, m_my_startPos.y, transform.position.z);
            }
        }
        // Y座標をリンクさせる場合
        else if (m_isMoveY)
        {
            // 反比例させて移動
            if (m_isInverseY)
            {
                transform.position = new Vector3(m_my_startPos.x, m_my_startPos.y - playerMovement.y, transform.position.z);
            }
            // 比例させて移動
            else
            {
                transform.position = new Vector3(m_my_startPos.x, m_my_startPos.y + playerMovement.y, transform.position.z);
            }
        }

        // X座標の最小値を指定している&&現在の値が最小値以下の場合
        if(m_minPos.x != 0 && transform.localPosition.x <= m_minPos.x)
        {
            transform.localPosition = new Vector3(m_minPos.x, transform.localPosition.y, transform.localPosition.z);
        }
        // Y座標の最小値を指定している&&現在の値が最小値以下の場合
        if (m_minPos.y != 0 && transform.localPosition.y <= m_minPos.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, m_minPos.y, transform.localPosition.z);
        }
        // X座標の最大値を指定している&&現在の値が最大値以上の場合
        if (m_maxPos.x != 0 && transform.localPosition.x >= m_maxPos.x)
        {
            transform.localPosition = new Vector3(m_maxPos.x, transform.localPosition.y, transform.localPosition.z);
        }
        // Y座標の最大値を指定している&&現在の値が最大値以上の場合
        if (m_maxPos.y != 0 && transform.localPosition.y >= m_maxPos.y)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, m_maxPos.y, transform.localPosition.z);
        }
    }
}
