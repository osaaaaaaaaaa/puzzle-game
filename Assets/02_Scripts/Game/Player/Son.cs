using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    [SerializeField] GameObject m_mom;
    PhysicsMaterial2D m_material;  // 物理マテリアル
    Rigidbody2D m_rb;
    Vector2 m_startPos;
    Vector3 m_offsetStartPos;      // 母親とのオフセット

    // Start is called before the first frame update
    void Start()
    {
        m_startPos = transform.position;
        m_rb = GetComponent<Rigidbody2D>();
        m_material = m_rb.sharedMaterial;
        m_offsetStartPos = transform.position - m_mom.transform.position;

        Reset();
    }

    /// <summary>
    /// 蹴り飛ばされる処理
    /// </summary>
    /// <param name="dir">方角</param>
    /// <param name="power">パワー</param>
    public void BeKicked(Vector3 dir, float power)
    {
        m_rb.sharedMaterial = m_material;   // 物理マテリアルセット
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // 力を設定
        m_rb.AddForce(force, ForceMode2D.Impulse);  // 力を加える
    }

    public void Reset()
    {
        m_rb.sharedMaterial = null;   // 物理マテリアルを外す
        transform.position = m_mom.transform.position + m_offsetStartPos;
    }
}
