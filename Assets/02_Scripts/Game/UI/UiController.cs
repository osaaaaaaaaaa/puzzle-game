using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    [SerializeField] GameObject m_UiGame;
    [SerializeField] GameObject m_UiResult;
    [SerializeField] GameObject m_buttonNextStage;

    /// <summary>
    /// UI���Q�[���N���A�p�ɐݒ肷��
    /// </summary>
    public void SetGameClearUI()
    {
        m_UiGame.SetActive(false);
        m_UiResult.SetActive(true);
    }

    /// <summary>
    /// ���̃X�e�[�W�ɑJ�ڂ���{�^���𖳌�������
    /// </summary>
    public void DisableNextStageButton()
    {
        m_buttonNextStage.GetComponent<Button>().interactable = false;
    }
}
