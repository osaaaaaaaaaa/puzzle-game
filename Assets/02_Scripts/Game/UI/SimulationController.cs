using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SimulationController : MonoBehaviour
{
    Scene m_simulationScene;                        // �V���~���[�V�����p�V�[��
    PhysicsScene2D m_physicsScene;                  // �������Z�̍Đ�
    [SerializeField] Transform m_obstacleParent;    // �X�e�[�W�̏�Q��

    [SerializeField] LineRenderer m_line;
    const int m_iMaxPhysicsFrame = 8;    // ���C����`�悷��t���[����

    #region ���q�֌W
    [SerializeField] GameObject m_somPrefab;           // �V���~���[�V�����ΏۃI�u�W�F�N�g
    GameObject m_som;             // ���q
    public Vector3 m_kickDir;     // �ΏۃI�u�W�F�N�g�̃x�N�g��
    public float m_kickPower;     // �͂̑傫��
    #endregion

    GameObject m_player;

    void Start()
    {
        // ������
        m_kickDir = Vector3.zero;
        m_kickPower = 0;

        // �I�u�W�F�N�g���擾����
        m_som = GameObject.Find("Son");
        m_player = GameObject.Find("Player");

        // �V���~���[�V�����Ŏg�p����V�[�����쐬����
        CreatePhysicsScene();
    }

    void Update()
    {
        // �܂��R���Ă��Ȃ� && �ړ��x�N�g����0�ȊO�̂Ƃ�
        if (!m_player.GetComponent<Player>().m_isKicked && m_kickDir != Vector3.zero)
        {
            Simulation(m_somPrefab, m_som.transform.position, m_kickDir,m_kickPower);
        }
        else
        {
            // �O���\���������Z�b�g
            m_line.GetComponent<LineRenderer>().positionCount = 0;
        }
    }

    /// <summary>
    /// �V���~���[�V�����Ŏg�p����V�[�����쐬����
    /// </summary>
    void CreatePhysicsScene()
    {
        m_simulationScene = SceneManager.CreateScene("Simulation", new CreateSceneParameters(LocalPhysicsMode.Physics2D));
        m_physicsScene = m_simulationScene.GetPhysicsScene2D();

        // �V���~���[�V�����p�V�[���ɃI�u�W�F�N�g(�u���b�N�ȂǏ�Q���ɂȂ����)�𐶐�����
        foreach (Transform tf in m_obstacleParent)
        {
            var ghost = Instantiate(tf.gameObject, tf.position, tf.rotation);
            // ��\���ɂ���
            ghost.GetComponent<Renderer>().enabled = false;
            // �I�u�W�F�N�g���w�肵���V�[���ֈړ�����
            SceneManager.MoveGameObjectToScene(ghost, m_simulationScene);
        }
    }

    /// <summary>
    /// �O���\������`�悷��
    /// </summary>
    /// <param name="_somPrefab">�V���~���[�V�����ΏۃI�u�W�F�N�g</param>
    /// <param name="_pos"></param>
    /// <param name="_velocity">�ړ���</param>
    void Simulation(GameObject _somPrefab, Vector2 _pos, Vector3 _dir, float _power)
    {
        // ���q�̃S�[�X�g�쐬(��\���ɂ���)
        var ghost = Instantiate(_somPrefab, _pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        // �I�u�W�F�N�g���V���~���[�V�����p�V�[���ֈړ�����
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);

        //=====================================
        // ���V�[���Ɉړ������Ă���͂�������
        //=====================================
        var rb = ghost.GetComponent<Rigidbody2D>();
        rb.drag = ghost.GetComponent<Son>().m_dragNum;  // ��C��R��ݒ�
        rb.velocity = transform.forward * ghost.GetComponent<Son>().m_initialSpeed; // ���x��ݒ�
        Vector3 force = new Vector3(_dir.x * _power, _dir.y * _power);  // �͂�ݒ�

        // �͂�������
        rb.AddForce(force, ForceMode2D.Impulse);

        //---------------------------------------------------------
        // �w�肵���t���[�����̊Ԃłǂ̂��炢���������̋O�����쐬
        //---------------------------------------------------------
        m_line.positionCount = m_iMaxPhysicsFrame;  // �t���[�������̔z��𐶐�

        for (int i = 0; i < m_iMaxPhysicsFrame; i++)
        {
            m_physicsScene.Simulate(Time.fixedDeltaTime);       // �������Z���w��b���i�߂�
            m_line.SetPosition(i, ghost.transform.position);    // ���C���I�u�W�F�N�g�̍��W(���_�H)��ǉ�����
        }

        // �V���~���[�V�����I���ŃS�[�X�g��j��
        Destroy(ghost.gameObject);
    }
}