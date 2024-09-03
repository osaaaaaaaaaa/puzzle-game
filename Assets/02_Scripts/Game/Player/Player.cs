using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region ���q�֌W
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_sonController;
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_ride_cow;
    public float m_sonOffsetX;    // ���q�Ƃ̃I�t�Z�b�g
    public float m_sonCowOffsetX; // ���q�Ƃ̃I�t�Z�b�g
    #endregion

    #region �v���C���[�֌W
    const float m_dragSpeed = 100f;    // �h���b�O�X�s�[�h
    public bool m_canDragPlayer;       // �v���C���[���h���b�O�ł��邩�ǂ���
    #endregion

    #region SE
    AudioSource m_audioSouse;
    [SerializeField] AudioClip m_kickSE;
    [SerializeField] AudioClip m_cowSE;
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
    public int m_mulPower = 50;

    private void Start()
    {
        m_canDragPlayer = false;
        m_isKicked = false;

        // �I�u�W�F�N�g�E�R���|�[�l���g���擾����
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        uiController = GameObject.Find("UiController").GetComponent<UiController>();
        m_cameraController = GameObject.Find("CameraController").GetComponent<CameraController>();
        m_audioSouse = GetComponent<AudioSource>();

        m_sonOffsetX = (transform.position - m_son.transform.position).x;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // �G�f�B�^���ꎞ��~����
#endif

        // �Q�[���J�n���Ă��Ȃ� || �|�[�Y���̏ꍇ
        if (!m_gameManager.m_isEndAnim || m_gameManager.m_isPause || m_gameManager.m_isEndGame) return;

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
            m_son.GetComponent<Son>().Reset();
            m_ride_cow.GetComponent<SonCow>().ResetSonCow();

            if(m_arrow != null)
            {
                // ���̌������X�V����
                if (m_ride_cow.activeSelf)
                {
                    var cowDir = m_arrow.GetComponent<Arrow>().dir.x < 0 ? -1 : 1;
                    m_ride_cow.GetComponent<SonCow>().m_direction = cowDir;
                }
            }
        }

        // ��ʃ^�b�`����&&���ݖ��𐶐��ł���ꍇ
        if (Input.GetMouseButtonDown(0) && m_isKicked == false)
        {
            // �^�b�`�����ꏊ��Ray���΂�
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            RaycastHit2D hit2d = Physics2D.Raycast(worldPos, Vector2.zero);

            // �v���C���[�^�O�����R���C�_�[�̏ꍇ
            if (hit2d)
            {
                if (hit2d.collider.gameObject.tag == "Player")
                {
                    // �h���b�O���鏀��
                    m_canDragPlayer = true;
                }
            }

            // �h���b�O��Ԃł͂Ȃ��ꍇ
            if(!m_canDragPlayer)
            {
                Vector3 pivotSon;
                if (m_son.activeSelf)
                {
                    pivotSon = m_son.transform.position;
                }
                else
                {
                    pivotSon = m_ride_cow.GetComponent<SonCow>().GetPivotPos();
                }

                // ���𐶐�
                m_arrow = Instantiate(m_prefabArrow, pivotSon, Quaternion.identity);

                // ���Z�b�g�{�^�����\���ɂ���
                uiController.SetActiveButtonReset(false);
            }
        }

        // �w�𗣂���
        if (Input.GetMouseButtonUp(0))
        {
            // �h���b�O�����̃p�����[�^��������
            m_canDragPlayer = false;
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
        if (m_gameManager.m_isEndGame) return;

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

        if (m_son.activeSelf)
        {
            m_son.GetComponent<Son>().DOKick(dir, power, true);
        }
        else
        {
            m_ride_cow.GetComponent<SonCow>().DOKick(dir, power);
            m_audioSouse.PlayOneShot(m_cowSE);
        }
        m_audioSouse.PlayOneShot(m_kickSE);

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
        m_sonController.GetComponent<SonController>().ResetSon();

        // �ēx�R�邱�Ƃ��ł���悤�ɂ���
        m_isKicked = false;
        m_canDragPlayer = false;

        // �J�������Y�[���C���E�Y�[���A�E�g����
        switch (m_cameraController.m_cameraMode)
        {
            case CameraController.CAMERAMODE.ZOOMIN:
                m_cameraController.ZoomIn();
                break;
            case CameraController.CAMERAMODE.ZOOMOUT:
                m_cameraController.ZoomOut(0.3f);
                break;
        }

        // ��e�̃A�j���[�V�������Đ�����
        GetComponent<MomAnimController>().PlayStandbyAnim();  // �ʏ�X�L����Idle�A�j��
    }
}
