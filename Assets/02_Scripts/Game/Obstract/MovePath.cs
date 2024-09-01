using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MovePath : MonoBehaviour
{
    [SerializeField] Vector3[] m_rootPath;
    [SerializeField] float m_time;

    /// <summary>
    /// ���[�v����^�C�v
    /// </summary>
    public enum LOOPTYPE
    {
        YOYO = 0,
        Restart,
    }

    public LOOPTYPE m_loopType = LOOPTYPE.Restart;

    // Start is called before the first frame update
    void Start()
    {
        switch (m_loopType)
        {
            case LOOPTYPE.YOYO:
                transform.DOLocalPath(m_rootPath, m_time, PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
                break;
            case LOOPTYPE.Restart:
                transform.DOLocalPath(m_rootPath, m_time, PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart);
                break;
        }
    }

    /// <summary>
    /// ���X�^�[�g����(Looptype.Restart�̋��������������Ȃ邽�ߗp�ӂ����֐�)
    /// </summary>
    void DoRestart()
    {
        // �n�_�ƏI�_���C������
        m_rootPath[0] = transform.position;
        m_rootPath[m_rootPath.Length - 1] = transform.position;

        transform.DOPath(m_rootPath, m_time, PathType.CatmullRom).SetEase(Ease.Linear).OnComplete(() => { transform.DOPath(m_rootPath, m_time, PathType.CatmullRom).SetEase(Ease.Linear).SetLoops(-1, LoopType.Restart); });
    }
}
