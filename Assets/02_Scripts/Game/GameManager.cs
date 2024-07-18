using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    #region ���E�̕�
    [SerializeField] GameObject m_Wall_L;
    [SerializeField] GameObject m_Wall_R;
    #endregion

    [SerializeField] UiController m_UiController;

    // Start is called before the first frame update
    void Start()
    {
        // �ǂ��\���ɂ���
        m_Wall_L.GetComponent<Renderer>().enabled = false;
        m_Wall_R.GetComponent<Renderer>().enabled = false;
    }

    /// <summary>
    /// �Q�[���N���A����
    /// </summary>
    public void GameClear()
    {
        // UI���Q�[���N���A�p�ɐݒ肷��
        m_UiController.SetGameClearUI();
    }

    /// <summary>
    /// ���g���C����
    /// </summary>
    public void OnRetryButton()
    {
        SceneManager.LoadScene("02_GameScene");
    }
}
