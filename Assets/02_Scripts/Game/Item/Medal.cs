using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Medal : MonoBehaviour
{
    [SerializeField] int m_medalID;
    GameManager m_gameManager;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Ghost") return;
        
        if(collision.gameObject.layer == 6 || collision.gameObject.layer == 10 || collision.gameObject.layer == 11)
        {
            m_gameManager.UpdateMedalFrag(m_medalID);
            Destroy(transform.gameObject);
        }
    }

    /// <summary>
    /// ƒƒ“ƒo•Ï”‰Šú‰»ˆ—
    /// </summary>
    public void InitMemberVariable(bool isAcquired)
    {
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        if (isAcquired)
        {
            GetComponent<SpriteRenderer>().color = new Color(0.76f, 0.76f, 0.76f, 0.59f);
        }
    }
}
