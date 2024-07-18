using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiController : MonoBehaviour
{
    [SerializeField] GameObject m_UiGame;
    [SerializeField] GameObject m_UiResult;

    /// <summary>
    /// UIをゲームクリア用に設定する
    /// </summary>
    public void SetGameClearUI()
    {
        m_UiGame.SetActive(false);
        m_UiResult.SetActive(true);
    }
}
