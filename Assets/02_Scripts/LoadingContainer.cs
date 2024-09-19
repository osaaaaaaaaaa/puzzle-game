using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingContainer : MonoBehaviour
{
    [SerializeField] GameObject m_loadingSet;
    int m_coroutineCnt;

    void Start()
    {
        m_loadingSet.SetActive(false);
        m_coroutineCnt = 0;
    }

    /// <summary>
    /// ローディングUIの表示・非表示処理
    /// </summary>
    public void ToggleLoadingUIVisibility(int allie)
    {
        m_coroutineCnt = (m_coroutineCnt + allie) < 0 ? 0 : (m_coroutineCnt + allie);
        m_loadingSet.SetActive(m_coroutineCnt > 0);
    }
}
