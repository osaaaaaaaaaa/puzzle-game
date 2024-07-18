using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : MonoBehaviour
{
    #region ���q�ƃJ�����֘A
    [SerializeField] GameObject m_mainCamera;
    [SerializeField] GameObject m_prefabArrow;
    [SerializeField] GameObject m_son;
    #endregion

    // �V���������������
    GameObject m_arrow;
    // �J�����Ƒ��q�Ƃ̈�苗��
    float m_fixedDis;
    // �R���΂������ǂ���
    public bool m_isKicked;

    #region �R��Ƃ��̃p�����[�^
    Vector3 m_keepDir;
    float m_keepPower;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        // ���q�ƃJ�����֘A�̃p�����[�^������
        m_fixedDis = m_mainCamera.transform.position.x - m_son.transform.position.x;
        m_isKicked = false;
    }

    // Update is called once per frame
    void Update()
    {
#if UNITY_EDITOR
        if(Input.GetKey(KeyCode.S)) UnityEditor.EditorApplication.isPaused = true;      // �G�f�B�^���ꎞ��~����
        if(Input.GetKey(KeyCode.P)) UnityEditor.EditorApplication.isPaused = false;     // �G�f�B�^���Đ�����
#endif

        // �R���΂����Ƃ��J������Ǐ]������
        if (m_isKicked)
        {
            m_mainCamera.transform.position = new Vector3(m_son.transform.position.x + m_fixedDis, 0, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = 5f;

            if (m_mainCamera.transform.position.x <= 0.1f)
            {
                m_mainCamera.transform.position = new Vector3(0f, 0f, -10f);
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
            }
        }

        // �w�𗣂�������󂪑��݂���ꍇ
        if (Input.GetMouseButtonUp(0) && m_arrow != null)
        {
            // ���C���J�����̎���ƈʒu�����ɖ߂�
            m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 1f);

            // �R�邱�Ƃ��\�ȏꍇ�ATween�I����Ɋ֐���callback����
            if (m_arrow.GetComponent<Arrow>().isKick)
            {
                m_keepDir = m_arrow.GetComponent<Arrow>().dir;
                m_keepPower = m_arrow.GetComponent<Arrow>().dis;
                m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 1f)
                    .OnComplete(() => CallBackSonMethod());
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

    /// <summary>
    /// son�̊֐����Ă�
    /// </summary>
    public void CallBackSonMethod()
    {
        Debug.Log("���p�F" + m_keepDir.normalized + " , �p���[�F" + m_keepPower * 10);
        m_isKicked = true;
        m_son.GetComponent<Son>().BeKicked(m_keepDir.normalized, m_keepPower * 10);
    }

    /// <summary>
    /// ���q���X�^�[�g�n�_�֖߂�
    /// </summary>
    public void OnRestart()
    {
        m_son.GetComponent<Son>().Reset();

        m_mainCamera.GetComponent<Camera>().DOOrthoSize(5f, 0.2f);
        m_mainCamera.transform.DOMove(new Vector3(0f, 0f, -10f), 0.2f);
        m_isKicked = false;
    }
}
