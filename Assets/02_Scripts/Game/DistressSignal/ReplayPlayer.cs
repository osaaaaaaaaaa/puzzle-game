using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : MonoBehaviour
{
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_sonRun;
    [SerializeField] GameObject m_sonCow;

    // �f�[�^���Đ�����Ԋu
    const float saveInterval = 0.05f;

    private void Start()
    {
        m_son.SetActive(false);
        m_sonRun.SetActive(false);
        m_sonCow.SetActive(false);
    }

    /// <summary>
    /// ���v���C����
    /// </summary>
    public IEnumerator ReplayCoroutine(GameManager gameManager,List<ReplayData> replayDatas)
    {
        foreach (var data in replayDatas)
        {
            if (data == null)
            {
                yield return new WaitForSeconds(saveInterval);
                continue;
            }

            GameObject targetSon = null;

            // ���������q���擾����
            switch (data.TypeSon)
            {
                case ReplayData.TYPESON.DEFAULT:
                    targetSon = m_son;
                    break;
                case ReplayData.TYPESON.RUN:
                    targetSon = m_sonRun;
                    break;
                case ReplayData.TYPESON.COW:
                    targetSon = m_sonCow;
                    break;
            }

            // �֌W�Ȃ����̑��q�̃I�u�W�F�N�g���\������
            ToggleSonObjVisibility(targetSon);

            // �p�����[�^��ݒ肷��
            var scale = targetSon.transform.localScale;
            targetSon.transform.localScale = new Vector3(Mathf.Abs(scale.x) * data.Dir, scale.y, scale.z);
            targetSon.GetComponent<Rigidbody2D>().gravityScale = data.Gravity;
            targetSon.transform.position = data.Pos;
            targetSon.GetComponent<Rigidbody2D>().velocity = data.Vel;

            yield return new WaitForSeconds(saveInterval);
        }

        gameManager.IsReplayEnd = true;
        ResetRB();
    }

    /// <summary>
    /// ���q�̃I�u�W�F�N�g��\���E��\������
    /// </summary>
    /// <param name="currentSon">���ݓ��������q�̃I�u�W�F�N�g</param>
    void ToggleSonObjVisibility(GameObject currentSon)
    {
        m_son.SetActive(m_son == currentSon);
        m_sonRun.SetActive(m_sonRun == currentSon);
        m_sonCow.SetActive(m_sonCow == currentSon);
    }

    /// <summary>
    /// ���q��Rigidbody2D�����Z�b�g����
    /// </summary>
    void ResetRB()
    {
        m_son.GetComponent<Rigidbody2D>().gravityScale = 0;
        m_son.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        m_sonRun.GetComponent<Rigidbody2D>().gravityScale = 0;
        m_sonRun.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        m_sonCow.GetComponent<Rigidbody2D>().gravityScale = 0;
        m_sonCow.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
}
