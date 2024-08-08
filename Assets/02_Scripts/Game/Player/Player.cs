using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region ���q�֌W
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_son;
    float m_son_defaultGravityScale;
    #endregion

    #region �v���C���[�֌W
    const float m_dragSpeed = 100f;    // �h���b�O�X�s�[�h
    Vector2 m_offsetPlayer;            // ��e�Ƒ��q�Ƃ̃I�t�Z�b�g
    float m_mom_defaultGravityScale;   // ��e�̏d��
    public bool m_canDragPlayer;       // �v���C���[���h���b�O�ł��邩�ǂ���
    #endregion

    // �Q�[���}�l�[�W���[
    GameManager m_gameManager;
    // UI�R���g���[���[
    UiController uiController;
    // �J�����R���g���[���[
    CameraController m_cameraController;

    // �V���������������
    public GameObject m_arrow;
    // �R���΂������ǂ���
    public bool m_isKicked;
    // �R���΂��Ƃ��ɏ�Z����l
    public int m_mulPower = 10;

    private void Start()
    {
        m_canDragPlayer = false;
        m_isKicked = false;

        // �d�ʃX�P�[�����擾����
        m_mom_defaultGravityScale = GetComponent<Rigidbody2D>().gravityScale;
        m_son_defaultGravityScale = m_son.GetComponent<Rigidbody2D>().gravityScale;

        // �I�u�W�F�N�g�E�R���|�[�l���g���擾����
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiController = GameObject.Find("UiController").GetComponent<UiController>();
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();

        // �I�t�Z�b�g���擾����
        m_offsetPlayer = new Vector2(transform.position.x - m_son.transform.position.x, transform.position.y - m_son.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // �G�f�B�^���ꎞ��~����
#endif

        // �Q�[���J�n���Ă��Ȃ� || �|�[�Y���̏ꍇ
        if (!m_gameManager.m_isEndAnim || m_gameManager.m_isPause) return;

        // �v���C���[���h���b�O���Ă���ꍇ
        if (m_canDragPlayer)
        {
            // ��e�̃h���b�O����
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (worldPos - transform.position).normalized;
            transform.GetComponent<Rigidbody2D>().velocity = dir * m_dragSpeed;
        }

        // �R���΂����Ƃ��J������Ǐ]������
        if (m_isKicked)
        {
            // �J���������q��Ǐ]����
            m_cameraController.Follow();
        }
        // �R���΂��O�̏ꍇ
        else
        {
            // ���q���Œ肷��
            m_son.transform.position = new Vector2(transform.position.x - m_offsetPlayer.x, transform.position.y - m_offsetPlayer.y);
        }

        // ��ʃ^�b�`����&&���ݖ��𐶐��ł���ꍇ
        if (Input.GetMouseButtonDown(0) && m_isKicked == false)
        {
            // �^�b�`�����ꏊ��Ray���΂�
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast(worldPos, Vector2.zero);

            if (hit2d)
            {
                GameObject targetObj = hit2d.collider.gameObject;
                if (targetObj.tag == "StartArea")
                {// �X�^�[�g�G���A�Əd�Ȃ��Ă����ꍇ

                    // ���𐶐�
                    m_arrow = Instantiate(m_prefabArrow, m_son.transform.position, Quaternion.identity);

                    // �J�������Y�[���A�E�g������
                    m_cameraController.ZoomoOut();

                    // ���Z�b�g�{�^�����\���ɂ���
                    uiController.SetActiveButtonReset(false);
                }
                else if (targetObj.tag == "Player")
                {// �v���C���[�^�O�����R���C�_�[�̏ꍇ

                    // �h���b�O���鏀��
                    m_canDragPlayer = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    m_son.GetComponent<Rigidbody2D>().gravityScale = 0;
                }
            }
        }

        // �w�𗣂���
        if (Input.GetMouseButtonUp(0))
        {
            // �h���b�O�����̃p�����[�^��������
            m_canDragPlayer = false;
            GetComponent<Rigidbody2D>().gravityScale = m_mom_defaultGravityScale;
            m_son.GetComponent<Rigidbody2D>().gravityScale = m_son_defaultGravityScale;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_son.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            // ���Z�b�g�{�^����\������
            uiController.SetActiveButtonReset(true);

            // ��󂪑��݂���ꍇ(�R������𖞂������ꍇ) && �܂��R���΂��Ă��Ȃ��ꍇ
            if (m_arrow != null && m_isKicked == false)
            {
                // �R�邱�Ƃ��\�ȏꍇ�ATween�I����Ɋ֐���callback����
                if (m_arrow.GetComponent<Arrow>().isKick)
                {
                    // �R���΂������Ƃɂ���
                    m_isKicked = true;

                    // �R���΂��������w��b����Ɏ��s����
                    Invoke("DOKick", 0.2f);
                }
                else
                {
                    // ��e�̃A�j���[�V�������Đ�����
                    GetComponent<MomAnimController>().PlayStandbyAnim();  // �ʏ�X�L����Idle�A�j��

                    // ������������j������
                    Destroy(m_arrow);
                    m_arrow = null;
                }
            }
        }
    }

    /// <summary>
    /// �R���΂�����
    /// </summary>
    void DOKick()
    {
        // �R��Ƃ��̃p�����[�^�擾(�����_�����g�p���邽��new�ŃA�h���X�ύX)
        Vector3 dir = new Vector3();
        dir = m_arrow.GetComponent<Arrow>().dir.normalized;
        float power = new float();
        power = m_arrow.GetComponent<Arrow>().dis * m_mulPower;

        // ��e�̃A�j���[�V�������Đ�����
        GetComponent<MomAnimController>().PlayKickAnim();  // �R��A�j��

        // ���Z�b�g�{�^����L��������
        uiController.SetInteractableButtonReset(true);

        // ���q���R���΂�����
        Debug.Log("���p�F" + dir + " , �p���[�F" + power);
        m_son.GetComponent<Son>().BeKicked(dir, power);

        // ������������j������
        Destroy(m_arrow);
        m_arrow = null;
    }

    /// <summary>
    /// �v���C���[�̏�Ԃ����Z�b�g����
    /// </summary>
    public void ResetPlayer()
    {
        // ���q�����Z�b�g����
        m_son.GetComponent<Son>().Reset();

        // �ēx�R�邱�Ƃ��ł���悤�ɂ���
        m_isKicked = false;

        // �J�������Y�[���C������
        m_cameraController.ZoomIn();

        // ��e�̃A�j���[�V�������Đ�����
        GetComponent<MomAnimController>().PlayStandbyAnim();  // �ʏ�X�L����Idle�A�j��
    }
}
