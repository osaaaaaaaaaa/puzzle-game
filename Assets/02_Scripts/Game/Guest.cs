using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Guest : MonoBehaviour
{
    #region SE
    AudioSource m_audioSouse;
    [SerializeField] AudioClip m_kickSE;
    [SerializeField] AudioClip m_cowSE;
    #endregion

    [SerializeField] Text m_textName;

    [SerializeField] SimulationController m_simulationController;

    // �Q�[���}�l�[�W���[
    GameManager m_gameManager;
    // �v���C���[
    Player m_player;
    // ���q�̃R���g���[���[
    SonController m_sonController;

    // �R���΂������ǂ���
    public bool m_isKicked;
    // �R��Ƃ��̃x�N�g��
    Vector3 VectorKick = Vector3.zero;

    private void Start()
    {
        m_isKicked = false;

        // �I�u�W�F�N�g�E�R���|�[�l���g���擾����
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        m_player = GameObject.Find("Player").GetComponent<Player>();
        m_sonController = GameObject.Find("SonController").GetComponent<SonController>();
        m_audioSouse = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (VectorKick == Vector3.zero || m_sonController == null) return;
        m_simulationController.Simulation(transform.position + m_sonController.GetSonOfset());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
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

            // ���q�̏ꍇ
            m_isKicked = true;
            DOKick(son);
        }
    }

    /// <summary>
    /// �R���΂�����
    /// </summary>
    void DOKick(GameObject son)
    {
        if (m_gameManager.m_isEndGame) return;
        Debug.Log(son.name);

        // ��e�̃A�j���[�V�������Đ�����
        GetComponent<MomAnimController>().PlayKickAnim();  // �R��A�j��

        // ���q���R���΂�����
        Debug.Log("�Q�X�g�̃x�N�g���F" + VectorKick.ToString());
        if (son.GetComponent<Son>())
        {
            son.GetComponent<Son>().ResetSon(transform.position);
            son.GetComponent<Son>().DOKick(VectorKick, true);
        }
        else
        {
            son.GetComponent<SonCow>().ResetSonCow(transform.position);
            son.GetComponent<SonCow>().DOKick(VectorKick);
            m_audioSouse.PlayOneShot(m_cowSE);
        }
        m_audioSouse.PlayOneShot(m_kickSE);
    }

    /// <summary>
    /// �v���C���[�̏�Ԃ����Z�b�g����
    /// </summary>
    public void ResetGuest()
    {
        // �ēx�R�邱�Ƃ��ł���悤�ɂ���
        m_isKicked = false;

        // ��e�̃A�j���[�V�������Đ�����
        GetComponent<MomAnimController>().PlayStandbyAnim();  // �ʏ�X�L����Idle�A�j��
    }

    public void InitMemberVariable(string name,Vector3 position,Vector3 vector)
    {
        m_textName.text = name;
        transform.position = position;
        VectorKick = vector;

        // �N���\�����̕`�����񂾂�����
        m_simulationController.vecKick = VectorKick;
    }
}
