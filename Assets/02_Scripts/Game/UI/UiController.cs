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
    /// UIをゲームクリア用に設定する
    /// </summary>
    public void SetGameClearUI()
    {
        m_UiGame.SetActive(false);
        m_UiResult.SetActive(true);
    }

    /// <summary>
    /// 次のステージに遷移するボタンを無効化する
    /// </summary>
    public void DisableNextStageButton()
    {
        m_buttonNextStage.GetComponent<Button>().interactable = false;
    }
}
