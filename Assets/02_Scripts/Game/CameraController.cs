using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
    // ���C���J����
    [SerializeField] GameObject m_mainCamera;

    #region �J�����̃p�����[�^
    Vector3 m_startCameraPos;       // �J�����̏����ʒu
    [SerializeField] float m_cameraSizeMin;  // ����̍ŏ��T�C�Y
    [SerializeField] float m_cameraSizeMax;  // ����̍ő�T�C�Y
    [SerializeField] Vector3 m_camera_movePos;       // �J�������ړ�������W
    #endregion  

    #region ���q�֌W
    GameObject m_son;
    Vector3 m_offsetSon;                        // �J�����Ƒ��q�Ƃ̈�苗��
    const float m_posY_Max = 3f;                // ���q����ʊO�ɏo�����ɂȂ鍂��
    #endregion

    // �v���C���[
    Player m_player;

    // �Y�[���C���A�E�g�{�^��
    GameObject m_buttonZoomIn;
    GameObject m_buttonZoomOut;

    // �J�����̃��[�h
    enum CAMERAMODE
    {
        ZOOMIN = 0,
        ZOOMOUT,
    }

    CAMERAMODE m_cameraMode;

    // �J�����̒ǔ����\���ǂ���
    bool m_isFollow;

    // Start is called before the first frame update
    void Start()
    {
        // �I�u�W�F�N�g���擾����
        m_son = GameObject.Find("Son");

        // ���q�̃|�W�V�����ƃJ�����̃I�t�Z�b�g
        var tmp_offset = m_mainCamera.transform.position - m_son.transform.position;
        m_offsetSon = new Vector3(tmp_offset.x, m_mainCamera.transform.position.y, m_mainCamera.transform.position.z);

        // �J�����̏����ʒu���擾
        m_startCameraPos = m_mainCamera.transform.position;

        // ���[�h�ύX
        m_cameraMode = CAMERAMODE.ZOOMIN;
        // �ǔ�OFF
        m_isFollow = false;

        // �Y�[���C���A�E�g�{�^���ɃC�x���g��ݒ肷��
        m_buttonZoomIn = GameObject.Find("ButtonZoomIn");
        m_buttonZoomIn.GetComponent<Button>().onClick.AddListener(() => ZoomIn());
        m_buttonZoomOut = GameObject.Find("ButtonZoomOut");
        m_buttonZoomOut.GetComponent<Button>().onClick.AddListener(() => ZoomoOut());

        // �v���C���[���擾����
        m_player = GameObject.Find("Player").GetComponent<Player>();
    }

    /// <summary>
    /// �Y�[���A�E�g����
    /// </summary>
    public void ZoomoOut()
    {
        // ���[�h�ύX
        m_cameraMode = CAMERAMODE.ZOOMOUT;

        // Tween�j������
        DOTween.Kill(m_mainCamera.transform);

        //Sequence����
        var sequence = DOTween.Sequence();
        //Tween���Ȃ���
        sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMax, 0.3f))
                .Join(m_mainCamera.transform.DOMove(m_camera_movePos, 0.3f));

        // Tween�Đ�����
        sequence.Play();

        // �Y�[���C���A�E�g�{�^���̕\���؂�ւ�
        m_buttonZoomIn.SetActive(true);
        m_buttonZoomOut.SetActive(false);
    }

    /// <summary>
    /// �Y�[���C������
    /// </summary>
    public void ZoomIn()
    {
        // �J�����̒ǔ�OFF
        m_isFollow = false;

        // ���[�h�ύX
        m_cameraMode = CAMERAMODE.ZOOMIN;

        // Tween�j������
        DOTween.Kill(m_mainCamera.transform);

        //Sequence����
        var sequence = DOTween.Sequence();

        // �v���C���[���R��O�̏ꍇ
        if (!m_player.m_isKicked)
        {
            //Tween���Ȃ���
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 1f))
                    .Join(m_mainCamera.transform.DOMove(m_startCameraPos, 1f)
                    .OnComplete(() => { m_isFollow = true; }));
        }
        // �R���΂�����̏ꍇ
        else
        {
            // Tween�I����ɒǔ�������
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 0.3f))
                    .Join(m_mainCamera.transform.DOMove(new Vector3(m_son.transform.position.x + m_offsetSon.x, m_offsetSon.y, -10f), 0.3f)
                    .OnComplete(()=> { m_isFollow = true; }));
        }

        // Tween�Đ�����
        sequence.Play();

        // �Y�[���C���A�E�g�{�^���̕\���؂�ւ�
        m_buttonZoomIn.SetActive(false);
        m_buttonZoomOut.SetActive(true);
    }

    /// <summary>
    /// ���q��Ǐ]����
    /// </summary>
    public void Follow()
    {
        // �J�����̃��[�h���Y�[���C���ɂȂ��Ă���ꍇ
        if (m_cameraMode == CAMERAMODE.ZOOMIN && m_isFollow)
        {        
            // ���q�Ƃ̃I�t�Z�b�g��ݒ�
            Vector3 offset = m_son.transform.position.y < m_posY_Max ? m_offsetSon : new Vector3(m_offsetSon.x, m_son.transform.position.y - m_posY_Max, m_offsetSon.z);

            // �J�����̎���ƒǏ]��ݒ�
            m_mainCamera.transform.position = new Vector3(m_son.transform.position.x + offset.x, offset.y, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = m_cameraSizeMin;

            // �J�������͈͊O�ɂł�̂�j�~����
            if (m_mainCamera.transform.position.x <= m_startCameraPos.x)
            {
                m_mainCamera.transform.position = m_startCameraPos;
            }
        }
    }
}
