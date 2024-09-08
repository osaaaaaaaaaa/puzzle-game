using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cage : MonoBehaviour
{
    UiController m_uiController;
    [SerializeField] BoxCollider2D m_collieder2d;

    // Start is called before the first frame update
    void Start()
    {
        m_uiController = GameObject.Find("UiController").GetComponent<UiController>();
    }

    private void Update()
    {
        // Œ®‚ğæ“¾‚µ‚Ä‚¢‚éê‡A“–‚½‚è”»’è‚ğ–³‚­‚·
        if(m_uiController.GetKeyCount() > 0)
        {
            m_collieder2d.enabled = false;
        }
        else
        {
            m_collieder2d.enabled = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;
        if (collision.gameObject.tag == "Ghost" || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;

        // Œ®‚ğŠ‚µ‚Ä‚¢‚éê‡
        if (m_uiController.GetKeyCount() > 0)
        {
            // Œ®‚ğˆê‚Â”jŠü‚µA©g‚ğ”jŠü‚·‚é
            m_uiController.UpdateKeyUI(-1);
            Destroy(this.transform.gameObject);
        }
    }
}
