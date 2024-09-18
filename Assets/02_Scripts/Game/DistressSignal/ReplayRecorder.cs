using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
    [SerializeField] LoadingContainer m_loading;
    [SerializeField] GameManager m_gameManager;

    List<ReplayData> m_replayDatas;
    GameObject m_son;
    GameObject m_son_run;
    GameObject m_ride_cow;

    // �f�[�^��ۑ�����Ԋu
    const float saveInterval = 0.05f;

    // ���v���C�̍X�V�������I���������ǂ���
    public bool IsUpdateReplayData { get; private set; }

    void OnEnable()
    {
        m_replayDatas = new List<ReplayData>();

        // ���q�̃I�u�W�F�N�g���擾����
        var sonController = GameObject.Find("SonController").GetComponent<SonController>();
        m_son = sonController.Son;
        m_son_run = sonController.SonRun;
        m_ride_cow = sonController.SonRideCow;

        // �L�^�J�n
        StartCoroutine(SaveCoroutine());
    }

    IEnumerator SaveCoroutine()
    {
        // �Q�[�����I������܂�(���悻10~30�b)�L�^����
        while (!m_gameManager.m_isEndGame)
        {
            // �|�[�Y���̏ꍇ
            if (m_gameManager.m_isPause)
            {
                yield return null;
                continue;
            };

            ReplayData replayData = new ReplayData();
            GameObject targetSon = null;

            // ���ݕ\������Ă��鑧�q��T��
            if (m_son.activeSelf)
            {
                targetSon = m_son;
                replayData.TypeSon = ReplayData.TYPESON.DEFAULT;
            }
            else if (m_son_run.activeSelf)
            {
                targetSon = m_son_run;
                replayData.TypeSon = ReplayData.TYPESON.RUN;
            }
            else if (m_ride_cow.activeSelf)
            {
                targetSon = m_ride_cow;
                replayData.TypeSon = ReplayData.TYPESON.COW;
            }
            else
            {
                replayData = null;
                m_replayDatas.Add(replayData);
                yield return new WaitForSeconds(saveInterval);
                continue;
            }

            // ���v���C�f�[�^�ɕK�v�Ȃ��̂������i�[
            replayData.Dir = targetSon.transform.localScale.x > 0 ? 1 : -1;
            replayData.Gravity = targetSon.GetComponent<Rigidbody2D>().gravityScale;
            replayData.Pos = targetSon.transform.position;
            replayData.Vel = targetSon.GetComponent<Rigidbody2D>().velocity;

            // ���X�g�ɒǉ�����
            m_replayDatas.Add(replayData);

            yield return new WaitForSeconds(saveInterval);
        }

        Debug.Log("�L�^�����F" + m_replayDatas.Count);

        m_loading.ToggleLoadingUIVisibility(1);

        // ���v���C���X�V����
        StartCoroutine(NetworkManager.Instance.UpdateReplayData(
            TopSceneDirector.Instance.DistressSignalID,
            m_replayDatas,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
            }));

        IsUpdateReplayData = true;

        if (m_gameManager.m_isExitGame)
        {
            // �z�X�g���r���ޏo�����ꍇ
            Initiate.Fade("01_TopScene", Color.black, 1.0f);
        }
    }
}
