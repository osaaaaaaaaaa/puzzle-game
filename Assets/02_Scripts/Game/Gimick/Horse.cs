using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Horse : MonoBehaviour
{
    #region SE
    AudioSource m_audioSouse;
    [SerializeField] AudioClip m_kickSE;
    [SerializeField] AudioClip m_cowSE;
    [SerializeField] AudioClip m_horseSE;
    #endregion

    // �v���C���[�̃V���~���[�V�����R���g���[���[
    [SerializeField] SimulationController m_playerSimulationController;

    // �v���C���[
    Player m_player;
    // ���q�̃R���g���[���[
    SonController m_sonController;

    // �R���΂������ǂ���
    public bool m_isKicked;
    // �R��Ƃ��̃x�N�g��
    Vector3 VectorKick = Vector3.zero;
    // ���q�Ƃ̃I�t�Z�b�g
    Vector3 m_offset;

    private void Start()
    {
        // �p�����[�^�ݒ�
        m_isKicked = false;
        m_audioSouse = GetComponent<AudioSource>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
        VectorKick = Vector3.zero;
        m_offset = Vector3.zero;
    }

    private void Update()
    {
        if (TopSceneDirector.Instance == null || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;

        if (!m_player.m_isKicked) ResetParam();
        if (!m_isKicked) VectorKick = m_playerSimulationController.vecKick;
        if (VectorKick == Vector3.zero || m_sonController == null) return;

        if (VectorKick.x <= 0) 
        {
            // �E�����̏ꍇ
            m_offset = new Vector3(-2.46f, 1.27f, 0f);
            transform.localScale = new Vector3(1f, 0.8f, 1f);
        }
        if (VectorKick.x >= 0)
        {
            // �������̏ꍇ
            m_offset = new Vector3(2.46f, 1.27f, 0f);
            transform.localScale = new Vector3(-1f, 0.8f, 1f);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (TopSceneDirector.Instance == null || TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;
        if (m_isKicked) return;
        if (collision.gameObject.layer == 6 || collision.gameObject.layer == 11)
        {
            // ���q�̃��C���[���A���q�ɏ���Ă鋍�̏ꍇ
            GameObject son = collision.gameObject;
            if (collision.gameObject.name == "son_run")
            {
                // �����Ă����Ԃ��猳�̃e�N�X�`���ɖ߂�
                m_sonController.ChangeDefaultTexture();
                son = m_sonController.Son;
            }

            m_isKicked = true;
            DOKick(son);
        }
    }

    /// <summary>
    /// �R���΂�����
    /// </summary>
    void DOKick(GameObject son)
    {
        // �R���΂��A�j���[�V�������Đ�����
        GetComponent<Animator>().Play("KickAnim");  // �R��A�j��

        // �v���C���[���R��Ƃ��̃x�N�g�����擾(�A�C�e���Ȃ��̋���)
        VectorKick = (VectorKick / m_player.m_mulPower) * 50f;

        // ���q���R���΂�����
        if (son.GetComponent<Son>())
        {
            son.GetComponent<Son>().ResetSon(transform.position);
            son.transform.position = transform.position + m_offset;
            son.GetComponent<Son>().DOKick(VectorKick, true);
        }
        else
        {
            son.GetComponent<SonCow>().ResetSonCow(transform.position);
            son.transform.position = transform.position + m_offset;
            son.GetComponent<SonCow>().DOKick(VectorKick);
            m_audioSouse.PlayOneShot(m_cowSE);
        }

        m_audioSouse.PlayOneShot(m_horseSE);
        m_audioSouse.PlayOneShot(m_kickSE);
    }

    /// <summary>
    /// ��Ԃ����Z�b�g����
    /// </summary>
    void ResetParam()
    {
        // �ēx�R�邱�Ƃ��ł���悤�ɂ���
        m_isKicked = false;

        // Idle�A�j���[�V�������Đ�����
        GetComponent<Animator>().Play("IdleAnimanim");
    }
}
