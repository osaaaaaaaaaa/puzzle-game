using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiController : MonoBehaviour
{
    #region パネル
    [SerializeField] GameObject m_uiPanelGame;      // ゲームのUI
    [SerializeField] GameObject m_uiPanelTutorial;  // チュートリアルのUI
    [SerializeField] GameObject m_uiPanelHome;      // ホーム画面に戻るかの確認するUI
    [SerializeField] GameObject m_uiPanelResult;    // リザルトのUI
    #endregion

    #region
    [SerializeField] GameObject m_buttonReset;      // リセットボタン
    [SerializeField] GameObject m_buttonNextStage;  // 次のステージへ進むボタン
    #endregion

    #region 非アクティブにするUIシーンのオブジェクト
    [SerializeField] Image m_uiPanelImage;
    [SerializeField] GameObject m_uiCamera;
    [SerializeField] GameObject m_uiClearCamera;
    #endregion

    // ゲームマネージャー
    [SerializeField] GameManager gameManager;

    private void Start()
    {
        // 非アクティブにする
        //m_uiPanelImage.enabled = false;
        m_uiCamera.SetActive(false);
        m_uiClearCamera.SetActive(false);
        m_uiPanelResult.SetActive(false);

        // 無効化する
        m_buttonReset.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// リセットボタンを表示・非表示
    /// </summary>
    /// <param name="isActive">表示・非表示</param>
    public void SetActiveButtonReset(bool isActive)
    {
        m_buttonReset.SetActive(isActive);
    }

    /// <summary>
    /// リセットボタンを無効・有効化にする
    /// </summary>
    /// <param name="isActive"></param>
    public void SetInteractableButtonReset(bool isActive)
    {
        m_buttonReset.GetComponent<Button>().interactable = isActive;
    }

    /// <summary>
    /// チュートリアル表示・非表示処理
    /// </summary>
    public void OnTutorialButton(bool isActive)
    {
        m_uiPanelTutorial.SetActive(isActive);
        m_uiPanelGame.SetActive(!isActive);

        // パネルを閉じる場合はポーズOFFにする
        gameManager.m_isPause = !isActive ? false : true;
    }

    /// <summary>
    /// ホーム画面に戻るかの確認UIを表示・非表示
    /// </summary>
    public void OnButtoneHome(bool isActive)
    {
        m_uiPanelHome.SetActive(isActive);

        // リザルト画面ではない場合
        if (!m_uiPanelResult.activeSelf)
        {
            m_uiPanelGame.SetActive(!isActive);
        }
        

        // パネルを閉じる場合はポーズOFFにする
        gameManager.m_isPause = !isActive ? false : true;
    }

    /// <summary>
    /// UIをゲームクリア用に設定する
    /// </summary>
    public void SetGameClearUI()
    {
        // パネルをリザルトに設定する
        m_uiPanelResult.SetActive(true);
        m_uiPanelGame.SetActive(false);
    }

    /// <summary>
    /// 次のステージに遷移するボタンを無効化する
    /// </summary>
    public void DisableNextStageButton()
    {
        m_buttonNextStage.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// ボタンにカーソルが入ったor抜けたときに処理
    /// </summary>
    public void EventPause()
    {
        gameManager.m_isPause = true;
    }
}
