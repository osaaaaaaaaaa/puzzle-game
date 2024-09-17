using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLinkedRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 5f;  // 回転速度
    [SerializeField] bool m_isLinkedY;          // プレイヤーのY座標とリンクさせるかどうか
    [SerializeField] bool m_isLinkedX;          // プレイヤーのX座標とリンクさせるかどうか

    // プレイヤー
    GameObject m_player;
    // プレイヤーの初期位置
    Vector3 m_playerStartPos;
    // 前フレームのプレイヤーの座標
    Vector3 m_playerPreviousPosition;

    void Start()
    {
        m_player = GameObject.Find("Player");
        m_playerStartPos = m_player.transform.position;
        m_playerPreviousPosition = m_player.transform.position;
    }
    void Update()
    {
        if (m_playerPreviousPosition == m_player.transform.position) return;

        Vector3 movement = (m_player.transform.position - m_playerPreviousPosition);

        if(m_isLinkedY)
        {
            transform.Rotate(0f, 0f, movement.y * rotationSpeed);
        }
        else
        {
            transform.Rotate(0f, 0f, movement.x * rotationSpeed);
        }

        m_playerPreviousPosition = m_player.transform.position;
    }
}
