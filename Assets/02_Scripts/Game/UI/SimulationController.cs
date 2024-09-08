using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour
{
    Scene m_simulationScene;            // �V���~���[�V�����p�V�[��
    PhysicsScene2D m_physicsScene;      // �������Z�̍Đ�
    Transform m_obstacleParent;         // �X�e�[�W�̏�Q��

    [SerializeField] LineRenderer m_line;
    const int m_iMaxPhysicsFrame = 5;    // ���C����`�悷��t���[����

    #region ���q�֌W
    [SerializeField] GameObject m_somPrefab;           // �V���~���[�V�����ΏۃI�u�W�F�N�g
    [SerializeField] GameObject m_rideCowPrefab;       // �V���~���[�V�����ΏۃI�u�W�F�N�g
    GameObject m_son;             // ���q
    GameObject m_ride_cow;        // ���ɏ�������q
    public Vector3 vecKick;
    #endregion

    GameObject m_player;
    [SerializeField] bool m_isTargetGuest = false;

    private void Awake()
    {
        vecKick = Vector3.zero;

        // �I�u�W�F�N�g���擾����
        m_obstacleParent = GameObject.Find("ObstacleParent").transform;
        m_player = GameObject.Find("Player");
        m_son = GameObject.Find("Son");
        m_ride_cow = GameObject.Find("ride_cow");

        // �V���~���[�V�����Ŏg�p����V�[�����쐬����
        CreatePhysicsScene();
    }

    void Update()
    {
        if (m_isTargetGuest) return;

        // �܂��R���Ă��Ȃ� && �x�N�g����0�ȊO�̂Ƃ�
        if (!m_player.GetComponent<Player>().m_isKicked && vecKick != Vector3.zero)
        {
            // �ʏ�X�L���̑��q���A�N�e�B�u�̏ꍇ
            if (m_son.activeSelf)
            {
                Simulation(m_son.transform.position);
            }
            // ���ɏ�������q���A�N�e�B�u�̏ꍇ
            else if (m_ride_cow.activeSelf)
            {
                Simulation(m_ride_cow.transform.position);
            }
        }
        else
        {
            // �O���\���������Z�b�g
            m_line.GetComponent<LineRenderer>().positionCount = 0;
        }
    }

    /// <summary>
    /// �V���~���[�V�����Ŏg�p����V�[�����쐬����(���������V�[���Ƀu���b�N�ȂǏ�Q���ɂȂ���𐶐�����)
    /// </summary>
    void CreatePhysicsScene()
    {
        // �V�[�������ɑ��݂��邩�`�F�b�N
        Scene scene = SceneManager.GetSceneByName("Simulation");
        if (scene.IsValid())
        {
            m_simulationScene = scene;
            m_physicsScene = m_simulationScene.GetPhysicsScene2D();
            return;
        }

        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();

        // �e�I�u�W�F�N�g�𕡐�����
        var parent = Instantiate(m_obstacleParent.gameObject, m_obstacleParent.position, m_obstacleParent.rotation);

        // �V���~���[�V�����p�V�[���ɃI�u�W�F�N�g(�u���b�N�ȂǏ�Q���ɂȂ����)�𐶐�����
        foreach (Transform tf in parent.transform)
        {
            // �R���|�[�l���g�����݂���ꍇ
            if (tf.gameObject.GetComponent<Renderer>() != null)
            {
                // ��\���ɂ���
                tf.gameObject.GetComponent<Renderer>().enabled = false;
            }
        }

        // �e�I�u�W�F�N�g���w�肵���V�[���ֈړ�����
        SceneManager.MoveGameObjectToScene(parent, m_simulationScene);
    }

    /// <summary>
    /// �O���\������`�悷��
    /// </summary>
    /// <param name="_somPrefab">�V���~���[�V�����ΏۃI�u�W�F�N�g</param>
    /// <param name="_pos"></param>
    /// <param name="_velocity">�ړ���</param>
    public void Simulation(Vector2 _pos)
    {
        GameObject _somPrefab;
        // �ʏ�X�L���̑��q���A�N�e�B�u�̏ꍇ
        if (m_son.activeSelf)
        {
            _somPrefab = m_somPrefab;
        }
        // ���ɏ�������q���A�N�e�B�u�̏ꍇ
        else
        {
            _somPrefab = m_rideCowPrefab;
        }

        // ���q�̃S�[�X�g�쐬(��\���ɂ���)
        var ghost = Instantiate(_somPrefab, _pos, Quaternion.identity);
        ghost.tag = "Ghost";
        if (ghost.GetComponent<Renderer>() != null)
        {
            ghost.GetComponent<Renderer>().enabled = false;
        }

        // �I�u�W�F�N�g���V���~���[�V�����p�V�[���ֈړ�����
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);

        // �ʏ�X�L���̑��q���A�N�e�B�u�̏ꍇ
        if (m_son.activeSelf)
        {
            // ���V�[���Ɉړ������Ă���͂�������
            ghost.GetComponent<Son>().DOKick(vecKick, true);
        }
        // ���ɏ�������q���A�N�e�B�u�̏ꍇ
        else
        {
            // ���V�[���Ɉړ������Ă���͂�������
            ghost.GetComponent<SonCow>().DOKick(vecKick);
        }

        //---------------------------------------------------------
        // �w�肵���t���[�����̊Ԃłǂ̂��炢���������̋O�����쐬
        //---------------------------------------------------------
        m_line.positionCount = m_iMaxPhysicsFrame;  // �t���[�������̔z��𐶐�

        for (int i = 0; i < m_iMaxPhysicsFrame; i++)
        {
            // �S�[�X�g�̒��S���W���擾����
            var offsetCollider = ghost.GetComponent<BoxCollider2D>() != null ? ghost.GetComponent<BoxCollider2D>().offset : Vector2.zero;
            Vector3 pivot = m_son.activeSelf ? ghost.transform.position 
                : ghost.transform.position + new Vector3(offsetCollider.x, offsetCollider.y, 0f);

            // �`��J�n
            m_physicsScene.Simulate(Time.fixedDeltaTime);       // �������Z���w��b���i�߂�
            m_line.SetPosition(i, pivot);    // ���C���I�u�W�F�N�g�̍��W(���_�H)��ǉ�����
        }

        // �V���~���[�V�����I���ŃS�[�X�g��j��
        Destroy(ghost.gameObject);
    }
}