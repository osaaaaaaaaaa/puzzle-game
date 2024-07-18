using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Son : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 startPos;

    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// 蹴り飛ばされる処理
    /// </summary>
    /// <param name="dir">方角</param>
    /// <param name="power">パワー</param>
    public void BeKicked(Vector3 dir, float power)
    {
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // 力を設定
        rb.AddForce(force, ForceMode2D.Impulse);  // 力を加える
    }

    public void Reset()
    {
        transform.position = startPos;
    }
}
