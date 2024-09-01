using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cow : MonoBehaviour
{
    [SerializeField] LayerMask m_obstacleLayer;     // ��Q���̃��C���[�^�O
    public int m_direction = 1;
    SonController m_sonController;

    void Start()
    {
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // �v���C���[���G�ꂽ�ꍇ
        if(collision.gameObject.layer == 6)
        {
            // ���q�̃e�N�X�`�������ɐ؂�ւ���
            m_sonController.ChangeCowTexture(m_direction, transform.position);
            Destroy(transform.gameObject);
        }
    }
}
