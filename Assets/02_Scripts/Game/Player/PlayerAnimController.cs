using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimController : MonoBehaviour
{
    [SerializeField] GameObject m_normalSkinMom;    // ��e�̃f�t�H���g�X�L��
    [SerializeField] GameObject m_abnormalSkinMom;  // ��e�̏R��Ƃ��̃X�L��
    [SerializeField] GameObject m_normalSkinGrandpa;    // �c���̃f�t�H���g�X�L��
    [SerializeField] GameObject m_abnormalSkinGrandpa;  // �c���̑łƂ��̃X�L��

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
        PlayStandbyAnim();
    }

    /// <summary>
    /// �ʏ�X�L����Idle�A�j���[�V�������Đ�����
    /// </summary>
    public void PlayStandbyAnim()
    {
        // �e�N�X�`����؂�ւ��ăA�j���[�V�����Đ�
        if (TopManager.isUseItem)
        {
            // �A�C�e�����g�p���Ă���ꍇ�͑c�����Đ�
            ToggleSkinVisivility(m_normalSkinGrandpa);
            m_normalSkinGrandpa.GetComponent<Animator>().Play("IdleNormalAnim");
        }
        else
        {
            // ��e���Đ�
            ToggleSkinVisivility(m_normalSkinMom);
            m_normalSkinMom.GetComponent<Animator>().Play(m_standbyAnimName);
        }
    }

    /// <summary>
    /// �A�u�m�[�}���X�L����Idle�A�j���[�V�������Đ�����
    /// </summary>
    public void PlayIdleAnim()
    {
        // �e�N�X�`����؂�ւ��ăA�j���[�V�����Đ�
        if (TopManager.isUseItem)
        {
            // �A�C�e�����g�p���Ă���ꍇ�͑c�����Đ�
            ToggleSkinVisivility(m_abnormalSkinGrandpa);
            m_abnormalSkinGrandpa.GetComponent<Animator>().Play("IdleAbnormalAnim");
        }
        else
        {
            // ��e���Đ�
            ToggleSkinVisivility(m_abnormalSkinMom);
            m_abnormalSkinMom.GetComponent<Animator>().Play("Idle");
        }
    }

    /// <summary>
    /// �R��p���̃A�j���[�V�������Đ�����
    /// </summary>
    public void PlayReadyAnim()
    {
        // �e�N�X�`����؂�ւ���
        if (TopManager.isUseItem)
        {
            // �A�C�e�����g�p���Ă���ꍇ�͑c�����Đ�
            ToggleSkinVisivility(m_abnormalSkinGrandpa);
            m_abnormalSkinGrandpa.GetComponent<Animator>().Play("IdleAbnormalAnim");
        }
        else
        {
            // ��e�̍Đ�����A�j���[�V�����w��
            ToggleSkinVisivility(m_abnormalSkinMom);
            m_readyAnimNum = (int)Random.Range(1, 11);
            string animName = m_readyAnimNum >= m_readyAnimMax ? "Ready02" : "Ready01";

            // �A�j���[�V�������Đ�����
            m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
        }
    }

    /// <summary>
    /// �R��A�j���[�V�������Đ�����
    /// </summary>
    public void PlayKickAnim()
    {
        // �e�N�X�`����؂�ւ���
        if (TopManager.isUseItem)
        {
            // �A�C�e�����g�p���Ă���ꍇ�͑c�����Đ�
            ToggleSkinVisivility(m_abnormalSkinGrandpa);
            m_abnormalSkinGrandpa.GetComponent<Animator>().Play("ShotAnim");
        }
        else
        {
            // ��e�̍Đ�����A�j���[�V�����w��
            ToggleSkinVisivility(m_abnormalSkinMom);
            string animName = m_readyAnimNum >= m_readyAnimMax ? "kick02" : "kick01";

            // �A�j���[�V�������Đ�����
            m_abnormalSkinMom.GetComponent<Animator>().Play(animName);
        }
    }

    /// <summary>
    /// �X�L����\���E��\���ɂ���
    /// </summary>
    /// <param name="showSkin">�w�肵���X�L��������\������</param>
    public void ToggleSkinVisivility(GameObject showSkin)
    {
        m_normalSkinMom.SetActive(showSkin == m_normalSkinMom);
        m_abnormalSkinMom.SetActive(showSkin == m_abnormalSkinMom);
        m_normalSkinGrandpa.SetActive(showSkin == m_normalSkinGrandpa);
        m_abnormalSkinGrandpa.SetActive(showSkin == m_abnormalSkinGrandpa);
    }
}
