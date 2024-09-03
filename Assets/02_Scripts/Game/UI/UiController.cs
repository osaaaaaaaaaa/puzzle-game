using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UiController : MonoBehaviour
{
    #region パネル
    [SerializeField] GameObject m_uiPanelGame;      // ゲームのUI
    [SerializeField] GameObject m_uiPanelTutorial;  // チュートリアルのUI
    [SerializeField] GameObject m_uiPanelHome;      // ホーム画面に戻るかの確認するUI
    [SerializeField] GameObject m_uiPanelResult;    // リザルトのUI
    [SerializeField] GameObject m_uiPanelGameOver;  // ゲームオーバーのUI
    #endregion

    #region ボタン
    [SerializeField] GameObject m_buttonReset;      // リセットボタン
    [SerializeField] GameObject m_buttonNextStage;  // 次のステージへ進むボタン
    [SerializeField] GameObject m_buttonZoomIn;     // ズームインボタン
    public GameObject ButtonZoomIn { get { return m_buttonZoomIn; }}
    [SerializeField] GameObject m_buttonZoomOut;    // ズームアウトボタン
    public GameObject ButtonZoomOut { get { return m_buttonZoomOut; } }
    #endregion

    #region ゲーム
    [SerializeField] Text m_textTimer;
    [SerializeField] GameObject m_textSubTimePrefab;
    [SerializeField] Text m_textStageID;
    #endregion

    #region リザルト
    [SerializeField] List<Image> m_medalContainers;
    [SerializeField] List<Sprite> m_texMedals;
    [SerializeField] Text m_textPlateResult;
    [SerializeField] Text m_textClearTime;
    [SerializeField] Text m_textScore;
    [SerializeField] Image m_imgRank;
    [SerializeField] List<Sprite> m_texRanks;
    #endregion

    #region 非アクティブにするUIシーンのオブジェクト
    [SerializeField] Image m_uiPanelImage;
    [SerializeField] GameObject m_uiCamera;
    [SerializeField] GameObject m_uiClearCamera;
    #endregion

    #region 鍵のUI
    [SerializeField] GameObject m_uiKeyParent;
    [SerializeField] GameObject m_uiKeyPrefab;
    #endregion

    // ゲームマネージャー
    [SerializeField] GameManager gameManager;

    private void Start()
    {
        m_textStageID.text = "ステージ " + TopManager.stageID;
        m_textTimer.text = "40:00";

        // 非アクティブにする
        m_uiCamera.SetActive(false);
        m_uiClearCamera.SetActive(false);
        m_uiPanelResult.SetActive(false);
        m_uiPanelGameOver.SetActive(false);

        // 無効化する
        m_buttonReset.GetComponent<Button>().interactable = false;
    }

    /// <summary>
    /// ゲームのパネルUIを表示切り替え
    /// </summary>
    /// <param name="isActive"></param>
    public void SetActiveGameUI(bool isActive)
    {
        m_uiPanelGame.SetActive(isActive);
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
    /// ゲームオーバー時のUIを表示する
    /// </summary>
    public void SetGameOverUI(bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        m_uiPanelGame.SetActive(false);
        m_uiPanelGameOver.SetActive(true);

        m_uiPanelGameOver.GetComponent<GameOverUI>().PlayAnim(this,isMedal1,isMedal2,time,score,isStageClear);
    }

    /// <summary>
    /// リザルトのUIを表示する
    /// </summary>
    public void SetResultUI(bool isMedal1, bool isMedal2, float time, int score, bool isStageClear)
    {
        // メダルのUIを更新する
        if (isMedal1) m_medalContainers[0].sprite = m_texMedals[0];
        if (isMedal2) m_medalContainers[1].sprite = m_texMedals[1];

        // クリアタイム表記
        string text = "" + Mathf.Floor(time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textClearTime.text = text.Insert(2, ":");

        // スコアを表記
        m_textScore.text = "" + score;

        // 評価を表記
        m_imgRank.color = new Color(1, 1, 1, 1);
        m_imgRank.sprite = TopManager.GetScoreRank(m_texRanks,score);

        // クリアしたかどうかで動的に設定
        m_textPlateResult.text = isStageClear ? "ステージクリア！" : "しっぱい...";
        // クリア済みのステージの場合はボタンを押せるようにする、そうでない場合は押せないようにする
        m_buttonNextStage.GetComponent<Button>().interactable = NetworkManager.Instance.StageResults.Count < TopManager.stageID ? 
            false : true;

        // パネルをリザルトに設定する
        m_uiPanelGameOver.SetActive(false);
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
    /// 鍵のUIを更新する
    /// </summary>
    public void UpdateKeyUI(int keyNum)
    {
        switch (keyNum)
        {
            case -1:    // 破棄する
                Destroy(m_uiKeyParent.transform.GetChild(0).gameObject);
                break;
            case 1:     // 追加する
                Instantiate(m_uiKeyPrefab, m_uiKeyParent.transform.position, Quaternion.Euler(0f, 0f, -45f), m_uiKeyParent.transform);
                break;
        }
    }

    /// <summary>
    /// 鍵の個数を取得する
    /// </summary>
    /// <returns></returns>
    public int GetKeyCount()
    {
        return m_uiKeyParent.transform.childCount;
    }

    /// <summary>
    /// タイマーテキストを更新
    /// </summary>
    public void UpdateTextTimer(float time)
    {
        string text = "" + Mathf.Floor(time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textTimer.text = text.Insert(2,":");
    }

    /// <summary>
    /// 減った時間を表示するテキストを生成する
    /// </summary>
    public void GenerateSubTimeText(int subTime)
    {
        GameObject subTimeObj = Instantiate(m_textSubTimePrefab, m_uiPanelGame.transform);
        subTimeObj.GetComponent<Text>().text = "-" + subTime;

        // 徐々に透明・移動して削除
        var sequence = DOTween.Sequence();
        sequence.Append(subTimeObj.transform.DOLocalMoveY(487, 1f).SetEase(Ease.Linear))
            .Join(subTimeObj.GetComponent<Text>().DOFade(0,1).OnComplete(() => { Destroy(subTimeObj.gameObject); }));
        sequence.Play();
    }

    /// <summary>
    /// ボタンにカーソルが入ったor抜けたときに処理
    /// </summary>
    public void EventPause(bool frag)
    {
        gameManager.m_isPause = frag;
    }
}
