using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MomAnimController : MonoBehaviour
{
    [SerializeField] GameObject m_normalSkinMom;    // �f�t�H���g�X�L��
    [SerializeField] GameObject m_abnormalSkinMom;  // �R��Ƃ��̃X�L��

    // �ҋ@�A�j���[�V�����̖��O
    string m_standbyAnimName;
    // �ʂ̃A�j���[�V�������Đ������Œ�l
    const int m_readyAnimMax =  6;
    // �R��A�j���[�V�����̗���
    int m_readyAnimNum;

    public enum DEFAULT_STANDBYANIM
    {
        IDLE01 = 1,
        IDLE02
    }

    public DEFAULT_STANDBYANIM m_default_animpattern;

    // Start is called before the first frame update
    void Start()
    {
        // �ҋ@��Ԃ̃A�j���[�V�����Đ�
        switch (m_default_animpattern)
        {
            case DEFAULT_STANDBYANIM.IDLE01:
                m_standbyAnimName = "Idle01";
                break;
            case DEFAULT_STANDBYANIM.IDLE02:
                m_standbyAnimName = "Idle02";
                break;
        }

        // �ҋ@�A�j���[�V�������Đ�����
        m_normalSkinMom.GetComponent<Animator>().Play(m_standbyAnimName);
    }

    /// <summary>
    /// �ʏ�X�L����Idle�A�j���[�V�������Đ�����
    /// </summary>
    public void PlayStandbyAnim()
    {
        // �e�N�X�`����؂�ւ���
        m_normalSkinMom.SetActive(true);
        m_abnormalSkinMom.SetActive(false);

        // �A�j���[�V�������Đ�����
        m_normalSkinMom.GetComponent<Animator>().Play(m_standbyAnimName);
    }

    /// <summary>
    /// �A�u�m�[�}���X�L����Idle�A�j���[�V�������Đ�����
    /// </summary>
    public void PlayIdleAnim()
    {
        // �e�N�X�`����؂�ւ���
        m_normalSkinMom.SetActive(false);
        m_abnormalSkinMom.SetActive(true);

        // �A�j���[�V�������Đ�����
        m_abnormalSkinMom.GetComponent<Animator>().Play("Idle");
    }

    /// <summary>
    /// �R��p���̃A�j���[�V�������Đ�����
    /// </summary>
    public void PlayReadyAnim()
    {
        // �e�N�X�`����؂�ւ���
        m_normalSkinMom.SetActive(false);
        m_abnormalSkinMom.SetActive(true);

        // �A�j���[�V�����w��
        m_readyAnimNum = (int)Random.Range(1, 11);
        string animName = m_readyAnimNum >= m_readyAnimMax ? "Ready02" : "Ready01";

        // �A�j���[�V�������Đ�����
        m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
    }

    /// <summary>
    /// �R��A�j���[�V�������Đ�����
    /// </summary>
    public void PlayKickAnim()
    {
        // �e�N�X�`����؂�ւ���
        m_normalSkinMom.SetActive(false);
        m_abnormalSkinMom.SetActive(true);

        // �A�j���[�V�����w��
        string animName = m_readyAnimNum >= m_readyAnimMax ? "kick02" : "kick01";

        // �A�j���[�V�������Đ�����
        m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
    }
}
