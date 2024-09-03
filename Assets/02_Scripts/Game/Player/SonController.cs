using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SonController : MonoBehaviour
{
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_son_run;
    [SerializeField] GameObject m_ride_cow;

    Vector3 m_offsetRun;    // ����e�N�X�`���̃I�t�Z�b�g

    /// <summary>
    /// �Q�[���J�n���̑��q�̃e�N�X�`���ݒ�
    /// </summary>
    public enum STARTSON_TEXTURE
    {
        SON = 0,
        SON_COW
    }

    public STARTSON_TEXTURE m_startSonTex = STARTSON_TEXTURE.SON;

    private void Start()
    {
        m_offsetRun = m_son_run.transform.position - m_son.transform.position;
    }

    /// <summary>
    /// ����e�N�X�`���ɐ؂�ւ���
    /// </summary>
    public void ChangeRunTexture(int direction,float posY)
    {
        // �ʏ�̃e�N�X�`���̑��q���A�N�e�B�u��Ԃ̏ꍇ
        if (m_son.activeSelf)
        {
            // ���W���C��
            m_son_run.transform.position = new Vector3(m_son.transform.position.x, posY - 0.8f, m_son.transform.position.z);    // -0.8f�͌���e�I�u�W�F�N�g�ɂ����Ƃ��̃��[�J�����W
        }
        // �������
        m_son_run.GetComponent<SonRun>().m_direction = direction;

        // �e�N�X�`����؂�ւ���
        m_son_run.SetActive(true);
        m_son.SetActive(false);
        m_ride_cow.SetActive(false);
    }

    /// <summary>
    /// ���̃e�N�X�`���ɐ؂�ւ���
    /// </summary>
    public void ChangeCowTexture(int direction, Vector3 cowPos)
    {
        // ���̃p�����[�^�ݒ�
        m_ride_cow.GetComponent<SonCow>().SetCowParam(direction,cowPos);

        // �e�N�X�`����؂�ւ���
        m_son_run.SetActive(false);
        m_son.SetActive(false);
        m_ride_cow.SetActive(true);
        m_ride_cow.GetComponent<SonCow>().InitState();
    }

    /// <summary>
    /// ���Z�b�g����
    /// </summary>
    public void ResetSon()
    {
        if (m_ride_cow.activeSelf)
        {
            m_son_run.SetActive(false);
            m_son.SetActive(false);
            m_ride_cow.GetComponent<SonCow>().ResetSonCow();
        }
        else
        {
            m_son_run.SetActive(false);
            m_son.SetActive(true);
            m_son.GetComponent<Son>().Reset();
        }
    }

    /// <summary>
    /// �����o�ϐ�����������
    /// </summary>
    public void InitMemberVariable()
    {
        switch (m_startSonTex)
        {
            case STARTSON_TEXTURE.SON:
                m_son_run.SetActive(false);
                m_ride_cow.SetActive(false);
                break;
            case STARTSON_TEXTURE.SON_COW:
                m_son_run.SetActive(false);
                m_son.SetActive(false);
                break;
        }
    }
}
