using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    // �n�_
    Vector3 startMousePos;

    // �n�_�Ƃ̋����̍ő�l
    const float disMax = 2.5f;
    // �����l
    const float startSizeX = 4f;
    const float startSizeY = 1f;

    #region �p�����[�^
    public bool isKick;    // �R�邱�Ƃ��\���ǂ���
    public float dis;      // �R��Ƃ��̗͂̑傫��
    public Vector3 dir;    // �R��Ƃ��̕��p
    #endregion

    // �V���~���[�V�����R���g���[���[
    public SimulationController m_simulationController;

    private void Start()
    {
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

            if (dis < 0.4f)
            {
                // �`��off
                transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                // isKick��false
                isKick = false;
                // �V���~���[�V�����̕`��OFF
                m_simulationController.m_sonVelocity = Vector2.zero;
                return;
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

            //-------------------------------------
            // �O���\������`�悷��
            //-------------------------------------
            float power = dis * 10;
            m_simulationController.enabled = true;
            m_simulationController.m_sonVelocity = new Vector2(dir.normalized.x * power, dir.normalized.y * power);
        }
    }
}
