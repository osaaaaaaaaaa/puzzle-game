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
    [SerializeField] int m_iMaxPhysicsFrame;    // ���C����`�悷��t���[����

    #region ���q�֌W
    [SerializeField] Transform m_tfSonPos;      // �V���~���[�V�����J�n�n�_
    [SerializeField] GameObject m_sonPrefab;    // �V���~���[�V�����Ώۃv���t�@�u
    public Vector2 m_sonVelocity;               // �ΏۃI�u�W�F�N�g�̈ړ��x�N�g��
    #endregion

    void Start()
    {
        m_sonVelocity = Vector2.zero;

        // �V���~���[�V�����Ŏg�p����V�[�����쐬����
        CreatePhysicsScene();
    }

    void Update()
    {
        // �܂��R���Ă��Ȃ� && �ړ��x�N�g����0�ȊO�̂Ƃ�
        if (!GetComponent<Player>().m_isKicked && m_sonVelocity != Vector2.zero)
        {
            Simulation(m_sonPrefab, m_tfSonPos.position, m_sonVelocity);
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
    /// <param name="_ballPrefab">�V���~���[�V�����ΏۃI�u�W�F�N�g</param>
    /// <param name="_pos"></param>
    /// <param name="_velocity">�ړ���</param>
    void Simulation(GameObject _ballPrefab, Vector2 _pos, Vector2 _velocity)
    {
        // ���q�̃S�[�X�g�쐬(��\���ɂ���)
        var ghost = Instantiate(_ballPrefab, _pos, Quaternion.identity);
        ghost.GetComponent<Renderer>().enabled = false;
        // �I�u�W�F�N�g���V���~���[�V�����p�V�[���ֈړ�����
        SceneManager.MoveGameObjectToScene(ghost.gameObject, m_simulationScene);
        ghost.GetComponent<Rigidbody2D>().AddForce(_velocity, ForceMode2D.Impulse);     // ���V���~���[�V�����V�[���Ɉړ������Ă���͂�������

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