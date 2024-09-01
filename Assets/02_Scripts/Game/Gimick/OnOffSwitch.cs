using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffSwitch : MonoBehaviour
{
    // 自信がONスイッチであれば、OFFスイッチを全て格納する
    // 自信がOFFスイッチであれば、ONスイッチを全て格納する
    [SerializeField] List<GameObject> m_reverseSwitchList;

    //public void SetInactivateReverseSwitch()
    //{
    //    foreach(GameObject switchObj in m_reverseSwitchList)
    //    {
    //        switchObj.S
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (collision.gameObject.tag == "Ghost" || collision.gameObject.layer != 6 && collision.gameObject.layer != 10) return;

    //    // 鍵を追加し、自身を破棄する
    //    m_uiController.UpdateKeyUI(1);
    //    Destroy(this.transform.gameObject);
    //}
}
