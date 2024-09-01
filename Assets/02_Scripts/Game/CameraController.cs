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
    GameObject m_son_run;
    Vector3 m_offsetSon;           // �J�����Ƒ��q�Ƃ̈�苗��
    Vector3 m_tmpSonPos;           // ���q���Ō�ɂ������W
    const float m_posY_Max = 3f;   // ���q����ʊO�ɏo�����ɂȂ鍂��
    #endregion

    // �v���C���[
    GameObject m_player;

    // �Y�[���C���A�E�g�{�^��
    GameObject m_buttonZoomIn;
    GameObject m_buttonZoomOut;

    // �J�����̃��[�h
    public enum CAMERAMODE
    {
        ZOOMIN = 0,
        ZOOMOUT,
    }

    public CAMERAMODE m_cameraMode;

    // �J�����̒ǔ����\���ǂ���
    bool m_isFollow;

    private void Awake()
    {
        // �I�u�W�F�N�g���擾����
        m_son = GameObject.Find("Son");
        m_son_run = GameObject.Find("son_run");
        m_player = GameObject.Find("Player");

        // ���q�̃|�W�V�����ƃJ�����̃I�t�Z�b�g
        var tmp_offset = m_mainCamera.transform.position - m_son.transform.position;
        m_offsetSon = new Vector3(tmp_offset.x, tmp_offset.y, m_mainCamera.transform.position.z);

        // �J�����̏����ʒu���擾
        m_startCameraPos = m_mainCamera.transform.position;

        // ���[�h�ύX
        m_cameraMode = CAMERAMODE.ZOOMIN;
        // �ǔ�OFF
        m_isFollow = false;
    }

    private void Update()
    {
        if (!(!m_son.activeSelf && !m_son_run.activeSelf))
        {
            // ���q���Ō�ɂ������W���擾����
            m_tmpSonPos = m_son.activeSelf ? m_son.transform.position : m_son_run.transform.position;
        }
    }

    /// <summary>
    /// �Y�[���A�E�g����
    /// </summary>
    public void ZoomOut(float time)
    {
        // ���[�h�ύX
        m_cameraMode = CAMERAMODE.ZOOMOUT;

        // Tween�j������
        DOTween.Kill(m_mainCamera.transform);

        //Sequence����
        var sequence = DOTween.Sequence();
        //Tween���Ȃ���
        sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMax, time))
                .Join(m_mainCamera.transform.DOMove(m_camera_movePos, time));

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

        // �Y�[���C������Ώۂ̍��W��ݒ肷��
        Vector3 target;
        if (!m_son.activeSelf && !m_son_run.activeSelf)
        {
            target = m_tmpSonPos;
        }
        else
        {
            // �Y�[���C������Ώۂ̍��W��ݒ肷��
            target = m_son.activeSelf ? m_son.transform.position : m_son_run.transform.position;
            m_tmpSonPos = target;
        }

        // Tween�j������
        DOTween.Kill(m_mainCamera.transform);
        //Sequence����
        var sequence = DOTween.Sequence();

        // �v���C���[���R��O�̏ꍇ
        if (!m_player.GetComponent<Player>().m_isKicked)
        {
            // �Y�[���C��������W��ݒ�
            Vector3 zoomin = m_player.transform.position.y < m_startCameraPos.y ?
                m_startCameraPos : new Vector3(m_startCameraPos.x, m_player.transform.position.y, m_startCameraPos.z);

            //Tween���Ȃ���
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 0.3f))
                    .Join(m_mainCamera.transform.DOMove(zoomin, 0.3f)
                    .OnComplete(() => { m_isFollow = true; }));
        }
        // �R���΂�����̏ꍇ
        else
        {
            // ���q�Ƃ̃I�t�Z�b�g��ݒ�
            Vector3 setPos = target.y < m_posY_Max ?
                new Vector3(target.x + m_offsetSon.x, m_startCameraPos.y, m_offsetSon.z) :
                new Vector3(target.x + m_offsetSon.x, target.y - m_posY_Max, m_offsetSon.z);

            // �J�������͈͊O�ɂł�̂�j�~����
            if (setPos.x <= m_startCameraPos.x)
            {
                setPos = new Vector3(m_startCameraPos.x, setPos.y, m_startCameraPos.z);
            }

            // Tween�I����ɒǔ�������
            sequence.Join(m_mainCamera.GetComponent<Camera>().DOOrthoSize(m_cameraSizeMin, 0.05f))
                    .Join(m_mainCamera.transform.DOMove(new Vector3(setPos.x, setPos.y, -10f), 0.05f)
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
        if (!m_son.activeSelf && !m_son_run.activeSelf) return;

        // �J�����̃��[�h���Y�[���C���ɂȂ��Ă���ꍇ
        if (m_cameraMode == CAMERAMODE.ZOOMIN && m_isFollow)
        {
            // �Y�[���C������Ώۂ̍��W��ݒ肷��
            Vector3 target;
            if (!m_son.activeSelf && !m_son_run.activeSelf)
            {
                target = m_tmpSonPos;
            }
            else
            {
                // �Y�[���C������Ώۂ̍��W��ݒ肷��
                target = m_son.activeSelf ? m_son.transform.position : m_son_run.transform.position;
            }

            // ���q�Ƃ̃I�t�Z�b�g��ݒ�
            Vector3 offset = target.y < m_posY_Max ? 
                new Vector3(target.x + m_offsetSon.x, m_startCameraPos.y, m_offsetSon.z) : 
                new Vector3(target.x + m_offsetSon.x, target.y - m_posY_Max, m_offsetSon.z);

            // �J�����̎���ƒǏ]��ݒ�
            m_mainCamera.transform.position = new Vector3(offset.x, offset.y, -10f);
            m_mainCamera.GetComponent<Camera>().orthographicSize = m_cameraSizeMin;

            // �J�������͈͊O�ɂł�̂�j�~����
            if (m_mainCamera.transform.position.x <= m_startCameraPos.x)
            {
                m_mainCamera.transform.position = new Vector3(m_startCameraPos.x, m_mainCamera.transform.position.y, m_startCameraPos.z);
            }
        }
    }

    /// <summary>
    /// �����o�ϐ�����������
    /// </summary>
    public void InitMemberVariable(GameObject zoomInBtn, GameObject zoomOutBtn)
    {
        // �Y�[���C���A�E�g�{�^���ɃC�x���g��ݒ肷��
        m_buttonZoomIn = zoomInBtn;
        m_buttonZoomIn.GetComponent<Button>().onClick.AddListener(() => ZoomIn());
        m_buttonZoomOut = zoomOutBtn;
        m_buttonZoomOut.GetComponent<Button>().onClick.AddListener(() => ZoomOut(0.3f));

        // �Q�[���̃p�l��UI���\���ɂ���
        GameObject.Find("UiController").GetComponent<UiController>().SetActiveGameUI(false);
    }
}
