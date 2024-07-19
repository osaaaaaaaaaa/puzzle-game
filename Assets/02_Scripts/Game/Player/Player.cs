using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region ���q�ƃJ�����֌W
    GameObject m_mainCamera;
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_son;
    const float m_posY_Max = 3f;                // ���q����ʊO�ɏo�����ɂȂ鍂��
    #endregion

    #region �v���C���[�֌W
    [SerializeField] float m_dragSpeed;
    Vector2 m_dragOffsetSon;           // �h���b�O����Ƃ��̑��q�̃I�t�Z�b�g
    const float m_defaultGravity = 3f; // ��e�Ƒ��q�̃f�t�H���g�d��
    public bool m_canDragPlayer;       // �v���C���[���h���b�O�ł��邩�ǂ���
    #endregion

    // �Q�[���}�l�[�W���[
    GameManager m_gameManager;

    // �V���������������
    GameObject m_arrow;
    // �J�����Ƒ��q�Ƃ̈�苗��
    Vector3 m_offsetSon;
    // �R���΂������ǂ���
    public bool m_isKicked;

    private void Awake()
    {
        m_canDragPlayer = false;
        m_isKicked = false;

        m_mainCamera = GameObject.Find("Main Camera");
        m_gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();

        // ���q�̃|�W�V�����ƃJ�����̃I�t�Z�b�g
        var tmp_offset = m_mainCamera.transform.position - m_son.transform.position;
        m_offsetSon = new Vector3(tmp_offset.x, m_mainCamera.transform.position.y, m_mainCamera.transform.position.z);
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // �G�f�B�^���ꎞ��~����
#endif

        // �Q�[���J�n���Ă��Ȃ��ꍇ
        if (!m_gameManager.m_isEndAnim) return;

        // �v���C���[���h���b�O���Ă���ꍇ
        if (m_canDragPlayer)
        {
            // ��e�̃h���b�O����
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = (worldPos - transform.position).normalized;
            transform.GetComponent<Rigidbody2D>().velocity = dir * m_dragSpeed;

            // ���q�̃h���b�O����
            m_son.transform.position = new Vector2(transform.position.x - m_dragOffsetSon.x, transform.position.y - m_dragOffsetSon.y);
        }

        // �R���΂����Ƃ��J������Ǐ]������
        if (m_isKicked)
        {
            Vector3 offset = m_son.transform.position.y < m_posY_Max ? m_offsetSon : new Vector3(m_offsetSon.x, m_son.transform.position.y - m_posY_Max, m_offsetSon.z);

            m_mainCamera.transform.position = new Vector3(m_son.transform.position.x + offset.x, offset.y, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = 5f;

            if (m_mainCamera.transform.position.x <= 0.1f)
            {
                m_mainCamera.transform.position = new Vector3(0f, m_mainCamera.transform.position.y, -10f);
            }
        }

        // ���ݖ��𐶐��ł���ꍇ
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

                    // ���𐶐����V���~���[�V�����R���g���[���[��ݒ�
                    m_arrow = Instantiate(m_prefabArrow, m_son.transform.position, Quaternion.identity);
                    m_arrow.GetComponent<Arrow>().m_simulationController = GetComponent<SimulationController>();

                    // ���C���J�����̎���ƈʒu�𒲐�����
                    m_mainCamera.transform.DOKill();
                    m_mainCamera.GetComponent<Camera>().DOOrthoSize(8f, 0.3f);
                    m_mainCamera.transform.DOMove(new Vector3(5.4f, 3f, -10f), 0.3f);
                }
                else if (targetObj.tag == "Player")
                {
                    // �h���b�O���鏀��
                    m_canDragPlayer = true;
                    GetComponent<Rigidbody2D>().gravityScale = 0;
                    m_son.GetComponent<Rigidbody2D>().gravityScale = 0;

                    // �h���b�O����Ƃ��A��e���瑧�q�Ƃ̃I�t�Z�b�g
                    m_dragOffsetSon = new Vector2(transform.position.x - m_son.transform.position.x, transform.position.y - m_son.transform.position.y);
                }
            }
        }

        // �w�𗣂�������󂪑��݂���ꍇ
        if (Input.GetMouseButtonUp(0))
        {
            m_canDragPlayer = false;
            GetComponent<Rigidbody2D>().gravityScale = m_defaultGravity;
            m_son.GetComponent<Rigidbody2D>().gravityScale = m_defaultGravity;
            GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            m_son.GetComponent<Rigidbody2D>().velocity = Vector2.zero;

            if (m_arrow != null)
            {
                // ���C���J�����̎�������ɖ߂�
                m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 1f);

                // �R�邱�Ƃ��\�ȏꍇ�ATween�I����Ɋ֐���callback����
                if (m_arrow.GetComponent<Arrow>().isKick)
                {
                    // �R��Ƃ��̃p�����[�^�擾(�����_�����g�p���邽��new�ŃA�h���X�ύX)
                    Vector3 dir = new Vector3();
                    dir = m_arrow.GetComponent<Arrow>().dir.normalized;
                    float power = new float();
                    power = m_arrow.GetComponent<Arrow>().dis * 10;

                    // ���C���J�����̈ʒu�����ɖ߂�����A���q���R�鏈��
                    m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 1f)
                        .OnComplete(() =>
                        {
                            Debug.Log("���p�F" + dir + " , �p���[�F" + power);
                            m_isKicked = true;
                            m_son.GetComponent<Son>().BeKicked(dir, power);
                        });
                }
                else
                {
                    // �J�����̈ʒu�����Z�b�g����
                    m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 1f);
                }

                // ������������j������
                Destroy(m_arrow);
                m_arrow = null;

                // �V���~���[�V�����R���g���[���[�̈ړ��x�N�g�������Z�b�g
                GetComponent<SimulationController>().m_sonVelocity = Vector2.zero;

            }
        }
    }

    /// <summary>
    /// ���q���X�^�[�g�n�_�֖߂�
    /// </summary>
    public void OnRestart()
    {
        m_son.GetComponent<Son>().Reset();

        DOTween.Kill(m_mainCamera);
        m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 0.2f);
        m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 0.2f);
        m_isKicked = false;
    }
}
