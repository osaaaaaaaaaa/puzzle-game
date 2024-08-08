using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TopManager : MonoBehaviour
{
    [SerializeField] GameObject m_selectStageButtonParent;
    [SerializeField] GameObject m_parent_top;
    [SerializeField] GameObject m_ui_start;
    [SerializeField] Image m_panelImage;        // ��\���ɂ���p�l���̃C���[�W

    bool isClickTitle;  // �^�C�g�����N���b�N�������ǂ���

    // �V�X�e����ʂ̃p�l�����X�g
    [SerializeField] List<GameObject> m_sys_panelList;
    // �V�X�e����ʂ̘A��
    public enum SYSTEM
    {
        PROFILE = 0,
        MAILBOX,
    }

    /// <summary>
    /// �ő�X�e�[�W��
    /// </summary>
    public static int stageMax { get; set; }

    /// <summary>
    /// �I�������X�e�[�WID
    /// </summary>
    public static int stageID { get; set; }

    void Start()
    {
        stageID = 0;
        isClickTitle = false;
        m_panelImage.enabled = false;
        m_ui_start.GetComponent<CanvasGroup>().DOFade(0.0f, 1).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isClickTitle == false)
        {
            // �\������UI���z�[���ֈړ�����
            m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x - 1980f, 0, 0), 0.5f).SetEase(Ease.Linear);
            isClickTitle = true;
        }
    }

    /// <summary>
    /// �X�e�[�W�I���̃{�^��
    /// </summary>
    public void OnSelectStageButton(int id)
    {
        stageID = id;
        // �Q�[���V�[���ɑJ�ڂ���
        Initiate.Fade(stageID + "_GameScene", Color.black, 1.0f);
    }

    /// <summary>
    /// �z�[����ʂ���^�C�g����ʂ֖߂�
    /// </summary>
    public void OnBackButtonHome()
    {
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(()=> { isClickTitle = false; });
    }

    /// <summary>
    /// �V�X�e�����(�v���t�B�[���A���[���{�b�N�X�Ȃ�)��\������
    /// </summary>
    public void OnButtonSystemPanel(int systemNum)
    {
        // �S�ẴV�X�e����ʂ��\���ɂ���
        foreach(GameObject item in m_sys_panelList)
        {
            item.SetActive(false);
        }

        // �\������
        m_sys_panelList[systemNum].SetActive(true);     // �I�������V�X�e�����
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, -1080, 0), 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// �v���t�B�[������z�[����ʂ֖߂�
    /// </summary>
    public void OnBackButtonSystemPanel()
    {
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, 0, 0), 0.5f).SetEase(Ease.Linear);
    }
}
