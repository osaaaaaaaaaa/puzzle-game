using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MoveCircle : MonoBehaviour
{
    [SerializeField] float m_radius;
    [SerializeField] float m_speed;
    Vector3 m_startPos;

    private void Start()
    {
        m_startPos = transform.position;
    }

    private void Update()
    {
        float sin = Mathf.Sin(Time.time * m_speed) * m_radius;
        float cos = Mathf.Cos(Time.time * m_speed) * m_radius;
        transform.position = new Vector3(m_startPos.x + cos, m_startPos.y + sin, 0);   // keepPos.x * 0.5fは調整
    }
}
