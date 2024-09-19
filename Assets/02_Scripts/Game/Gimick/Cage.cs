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
        // 鍵を取得している場合、当たり判定を無くす
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
        if (TopSceneDirector.Instance == null || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;
        if (collision.gameObject.tag == "Ghost" || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;

        // 鍵を所持している場合
        if (m_uiController.GetKeyCount() > 0)
        {
            // 鍵を一つ破棄し、自身を破棄する
            m_uiController.UpdateKeyUI(-1);
            Destroy(this.transform.gameObject);
        }
    }
}
