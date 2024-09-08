using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Arrow : MonoBehaviour
{
    // �n�_
    Vector3 startMousePos;

    // �n�_�Ƃ̋����̍ő�l
    const float disMax = 2.5f;
    // �����l
    const float startSizeX = 4f;
    const float startSizeY = 1f;

    // �v���C���[�I�u�W�F�N�g
    GameObject m_player;
    // �O���\����
    GameObject m_lineGuide;

    #region �p�����[�^
    public bool isKick;    // �R�邱�Ƃ��\���ǂ���
    public float dis;      // �R��Ƃ��̗͂̑傫��
    public Vector3 dir;    // �R��Ƃ��̕��p
    #endregion

    private void Start()
    {
        // �I�u�W�F�N�g����������
        m_lineGuide = GameObject.Find("LineController");
        m_player = GameObject.Find("Player");

        // �`��off
        transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        // �n�_��ݒ�
        startMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        isKick = false;
    }

    private void Update()
    {
        if(Input.GetMouseButtonUp(0))
        {
            // ���̏���������
            transform.localScale = new Vector3(startSizeX, startSizeY, 0);
            // �`��off
            transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        }
        if (Input.GetMouseButton(0))
        {
            // �}�E�X�̃��[���h���W���擾����
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // �n�_�Ƃ̋���
            dis = Vector2.Distance(startMousePos, mousePos);

            // ���������������ȉ��ŏR�邱�Ƃ��ł��Ȃ��ꍇ
            if (dis < 0.4f)
            {
                // ��e�̃A�j���[�V�������Đ�����
                m_player.GetComponent<MomAnimController>().PlayIdleAnim();  // �A�u�m�[�}���X�L����Idle�A�j��

                // �`��off
                transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                // isKick��false
                isKick = false;
                // �V���~���[�V�����̕`��OFF
                m_lineGuide.GetComponent<SimulationController>().vecKick = Vector3.zero;
                return;
            }

            // �R�邱�Ƃ��\�ȋ����܂ň����������ꍇ(1�x�������ɓ���Ȃ�)
            if(isKick == false)
            {
                // ��e�̃A�j���[�V�������Đ�����
                m_player.GetComponent<MomAnimController>().PlayReadyAnim();  // �R��p���̃A�j��
            }

            // isKick��true
            isKick = true;

            // �`��on
            transform.gameObject.GetComponent<SpriteRenderer>().enabled = true;

            //-------------------------------------
            // �����ɉ����Ė��̑傫���𒲐�����
            //-------------------------------------

            // dis�̍ő�l�A�ŏ��l�𒴂����ꍇ
            dis = dis > disMax ? disMax : dis;
            dis = dis < 0f ? 0f : dis;

            transform.localScale = new Vector3(startSizeX - dis, startSizeY + dis);

            //-------------------------------------
            // �����J�[�\���̂�������Ɍ�������
            //-------------------------------------

            // ���������������v�Z
            dir = (startMousePos - mousePos);

            // �����Ō������������ɉ�]
            transform.rotation = Quaternion.FromToRotation(Vector3.up, new Vector3(dir.x, dir.y, 0f));

            //--------------------------------------------
            // �O���\������`�悷��p�����[�^��ݒ肷��
            //--------------------------------------------
            m_lineGuide.GetComponent<SimulationController>().enabled = true;
            m_lineGuide.GetComponent<SimulationController>().vecKick = dir.normalized * dis * m_player.GetComponent<Player>().m_mulPower;
        }
    }
}
