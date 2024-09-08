using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Key : MonoBehaviour
{
    UiController m_uiController;
    [SerializeField] bool isMove = true;   // ìÆÇ≠Ç©Ç«Ç§Ç©

    // Start is called before the first frame update
    void Start()
    {
        m_uiController = GameObject.Find("UiController").GetComponent<UiController>();

        if (isMove)
        {
            transform.DOMove(transform.position + Vector3.up * 2f, 1.5f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;
        if (collision.gameObject.tag == "Ghost") return;

        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 10 || collision.gameObject.layer == 11)
        {
            // åÆÇí«â¡ÇµÅAé©êgÇîjä¸Ç∑ÇÈ
            m_uiController.UpdateKeyUI(1);
            Destroy(this.transform.gameObject);
        }
    }
}
