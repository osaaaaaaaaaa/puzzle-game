using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
    [SerializeField] LayerMask m_obstacleLayer;     // 障害物のレイヤータグ
    public int m_direction = 1;
    SonController m_sonController;

    void Start()
    {
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // プレイヤーが触れた場合
        if(collision.gameObject.layer == 6)
        {
            // 息子のテクスチャを牛に切り替える
            m_sonController.ChangeCowTexture(m_direction, transform.position);
            Destroy(transform.gameObject);
        }
    }
}
