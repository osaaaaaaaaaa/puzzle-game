using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnOffSwitch : MonoBehaviour
{
    // ���M��ON�X�C�b�`�ł���΁AOFF�X�C�b�`��S�Ċi�[����
    // ���M��OFF�X�C�b�`�ł���΁AON�X�C�b�`��S�Ċi�[����
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

    //    // ����ǉ����A���g��j������
    //    m_uiController.UpdateKeyUI(1);
    //    Destroy(this.transform.gameObject);
    //}
}
