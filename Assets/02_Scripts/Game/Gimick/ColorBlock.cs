using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorBlock : MonoBehaviour
{
    [SerializeField] BoxCollider2D m_collider2D;
    [SerializeField] ColorData.COLOR_TYPE m_coloerType;
    Son m_son;

    private void Start()
    {
        m_son = GameObject.Find("Son").GetComponent<Son>();
    }

    private void Update()
    {
        // カラーが一致する場合はコライダーOFF
        m_collider2D.enabled = !(m_son.m_coloerType == m_coloerType);
    }
}
