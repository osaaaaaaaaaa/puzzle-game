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
    /// R‚è”ò‚Î‚³‚ê‚éˆ—
    /// </summary>
    /// <param name="dir">•ûŠp</param>
    /// <param name="power">ƒpƒ[</param>
    public void BeKicked(Vector3 dir, float power)
    {
        Vector3 force = new Vector3(dir.x * power, dir.y * power);  // —Í‚ğİ’è
        rb.AddForce(force, ForceMode2D.Impulse);  // —Í‚ğ‰Á‚¦‚é
    }

    public void Reset()
    {
        transform.position = startPos;
    }
}
