using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class TopManager : MonoBehaviour
{
    [SerializeField] GameObject m_parent_top;
    [SerializeField] GameObject m_ui_start;

    bool isClickTitle;  // タイトルをクリックしたかどうか

    /// <summary>
    /// 選択したステージID
    /// </summary>
    public static int stageID { get; set; }

    void Start()
    {
        stageID = 0;
        isClickTitle = false;
        m_ui_start.GetComponent<CanvasGroup>().DOFade(0.0f, 1).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isClickTitle == false)
        {
            m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x - 1980f, 0, 0), 0.5f).SetEase(Ease.Linear);
            isClickTitle = true;
        }
    }

    /// <summary>
    /// ステージ選択のボタン
    /// </summary>
    public void OnSelectStageButton(int id)
    {
        stageID = id;
        // ゲームシーンに遷移する
        SceneManager.LoadScene("02_GameScene");
    }

    /// <summary>
    /// タイトルを表示する
    /// </summary>
    public void OnBackButton()
    {
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(()=> { isClickTitle = false; });
    }
}
