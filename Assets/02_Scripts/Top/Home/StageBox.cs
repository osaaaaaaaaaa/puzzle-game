using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class StageBox : MonoBehaviour
{
    [SerializeField] List<Image> m_medalContainers;
    [SerializeField] List<Sprite> m_texMedals;
    [SerializeField] Sprite m_texMedalContainer;
    [SerializeField] Text m_textStageID;
    [SerializeField] Text m_textClearTime;
    [SerializeField] Text m_textScore;
    [SerializeField] Image m_imgRank;
    [SerializeField] List<Sprite> m_texRanks;
    [SerializeField] Button m_btnRecruiting;
    [SerializeField] Text m_textRecruiting;

    [SerializeField] TopManager managerTop;
    ShowDistressSignalResponse m_distressSignal;

    public void InitStatus(ShowStageResultResponse resultData)
    {
        if(resultData == null)
        {
            resultData = new ShowStageResultResponse
            {
                StageID = TopManager.stageID,
                IsMedal1 = false,
                IsMedal2 = false,
                Time = 0,
                Score = 0
            };
        }

        m_textStageID.text = "ステージ  " + TopManager.stageID;

        // メダルのUIを更新する
        if (resultData.IsMedal1) m_medalContainers[0].sprite = m_texMedals[0];
        if (resultData.IsMedal2) m_medalContainers[1].sprite = m_texMedals[1];

        // クリアタイム表記
        string text = "" + Mathf.Floor(resultData.Time * 100);
        text = text.Length == 3 ? "0" + text : text;
        text = text.Length == 2 ? "00" + text : text;
        text = text.Length == 1 ? "000" + text : text;
        m_textClearTime.text = "クリアタイム   " + text.Insert(2, ":");

        // スコアを表記
        m_textScore.text = "スコア   " + resultData.Score;

        // 評価を表記
        m_imgRank.color = new Color(1, 1, 1, 1);
        m_imgRank.sprite = TopManager.GetScoreRank(m_texRanks, resultData.Score);

        // 募集ボタンを編集 (募集済の場合=> textを募集中 & ボタンを押せなくする)
        m_distressSignal = NetworkManager.Instance.dSignalList.FirstOrDefault(item => item.StageID == TopManager.stageID);    // リストから検索して取得
        m_textRecruiting.text = m_distressSignal != null ? "募集中" : "募集する";
        m_btnRecruiting.interactable = m_distressSignal != null ? false : true;

        gameObject.SetActive(true);
    }

    public void OnCloseButton()
    {
        SEManager.Instance.PlayCanselSE();
        m_medalContainers[0].sprite = m_texMedalContainer;
        m_medalContainers[1].sprite = m_texMedalContainer;
        m_textClearTime.text = "クリアタイム";
        m_textScore.text = "スコア";
        m_imgRank.color = new Color(1, 1, 1, 0);
        m_imgRank.sprite = null;
        gameObject.SetActive(false);
    }

    public void OnRecruiting()
    {
        SEManager.Instance.PlayButtonSE();
        // 救難信号登録処理
        StartCoroutine(NetworkManager.Instance.StoreDistressSignal(
            TopManager.stageID,
            result =>
            {
                if (result == null) return;
                m_textRecruiting.text = "募集中";
                m_btnRecruiting.interactable = false;
                m_distressSignal = result;
            }));
    }

    public void OnTransitionButton()
    {
        // 募集中の場合はホストモード、募集していない場合はソロモード
        var mode = m_distressSignal != null ? TopSceneDirector.PLAYMODE.HOST : TopSceneDirector.PLAYMODE.SOLO;
        int signalID = m_distressSignal != null ? m_distressSignal.SignalID : 0;
        int stageID = m_distressSignal != null ? m_distressSignal.StageID : 0;
        managerTop.OnPlayStageButton(mode, signalID, stageID, false);
    }
}
