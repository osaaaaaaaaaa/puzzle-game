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
    [SerializeField] Image m_panelImage;        // 非表示にするパネルのイメージ

    bool isClickTitle;  // タイトルをクリックしたかどうか

    // システム画面のパネルリスト
    [SerializeField] List<GameObject> m_sys_panelList;
    // システム画面の連番
    public enum SYSTEM
    {
        PROFILE = 0,
        MAILBOX,
    }

    /// <summary>
    /// 最大ステージ数
    /// </summary>
    public static int stageMax { get; set; }

    /// <summary>
    /// 選択したステージID
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
            // 表示するUIをホームへ移動する
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
        Initiate.Fade(stageID + "_GameScene", Color.black, 1.0f);
    }

    /// <summary>
    /// ホーム画面からタイトル画面へ戻る
    /// </summary>
    public void OnBackButtonHome()
    {
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(()=> { isClickTitle = false; });
    }

    /// <summary>
    /// システム画面(プロフィール、メールボックスなど)を表示する
    /// </summary>
    public void OnButtonSystemPanel(int systemNum)
    {
        // 全てのシステム画面を非表示にする
        foreach(GameObject item in m_sys_panelList)
        {
            item.SetActive(false);
        }

        // 表示処理
        m_sys_panelList[systemNum].SetActive(true);     // 選択したシステム画面
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, -1080, 0), 0.5f).SetEase(Ease.Linear);
    }

    /// <summary>
    /// プロフィールからホーム画面へ戻る
    /// </summary>
    public void OnBackButtonSystemPanel()
    {
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, 0, 0), 0.5f).SetEase(Ease.Linear);
    }
}
