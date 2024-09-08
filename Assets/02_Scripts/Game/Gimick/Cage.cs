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
        // �����擾���Ă���ꍇ�A�����蔻��𖳂���
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

        // �����������Ă���ꍇ
        if (m_uiController.GetKeyCount() > 0)
        {
            // ������j�����A���g��j������
            m_uiController.UpdateKeyUI(-1);
            Destroy(this.transform.gameObject);
        }
    }
}
