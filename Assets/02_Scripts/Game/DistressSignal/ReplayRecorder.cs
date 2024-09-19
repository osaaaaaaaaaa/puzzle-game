using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ReplayRecorder : MonoBehaviour
{
    [SerializeField] LoadingContainer m_loading;
    [SerializeField] GameManager m_gameManager;

    List<ReplayData> m_replayDatas;
    GameObject m_son;
    GameObject m_son_run;
    GameObject m_ride_cow;

    // データを保存する間隔
    const float saveInterval = 0.05f;

    // リプレイの更新処理が終了したかどうか
    public bool IsUpdateReplayData { get; private set; }

    void OnEnable()
    {
        m_replayDatas = new List<ReplayData>();

        // 息子のオブジェクトを取得する
        var sonController = GameObject.Find("SonController").GetComponent<SonController>();
        m_son = sonController.Son;
        m_son_run = sonController.SonRun;
        m_ride_cow = sonController.SonRideCow;

        // 記録開始
        StartCoroutine(SaveCoroutine());
    }

    IEnumerator SaveCoroutine()
    {
        // ゲームが終了するまで(およそ10~30秒)記録する
        while (!m_gameManager.m_isEndGame)
        {
            // ポーズ中の場合
            if (m_gameManager.m_isPause)
            {
                yield return null;
                continue;
            };

            ReplayData replayData = new ReplayData();
            GameObject targetSon = null;

            // 現在表示されている息子を探す
            if (m_son.activeSelf)
            {
                targetSon = m_son;
                replayData.TypeSon = ReplayData.TYPESON.DEFAULT;
            }
            else if (m_son_run.activeSelf)
            {
                targetSon = m_son_run;
                replayData.TypeSon = ReplayData.TYPESON.RUN;
            }
            else if (m_ride_cow.activeSelf)
            {
                targetSon = m_ride_cow;
                replayData.TypeSon = ReplayData.TYPESON.COW;
            }
            else
            {
                replayData = null;
                m_replayDatas.Add(replayData);
                yield return new WaitForSeconds(saveInterval);
                continue;
            }

            // リプレイデータに必要なものを順次格納
            replayData.Dir = targetSon.transform.localScale.x > 0 ? 1 : -1;
            replayData.Gravity = targetSon.GetComponent<Rigidbody2D>().gravityScale;
            replayData.Pos = targetSon.transform.position;
            replayData.Vel = targetSon.GetComponent<Rigidbody2D>().velocity;

            // リストに追加する
            m_replayDatas.Add(replayData);

            yield return new WaitForSeconds(saveInterval);
        }

        Debug.Log("記録完了：" + m_replayDatas.Count);

        m_loading.ToggleLoadingUIVisibility(1);

        // リプレイ情報更新処理
        StartCoroutine(NetworkManager.Instance.UpdateReplayData(
            TopSceneDirector.Instance.DistressSignalID,
            m_replayDatas,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
            }));

        IsUpdateReplayData = true;

        if (m_gameManager.m_isExitGame)
        {
            // ホストが途中退出した場合
            Initiate.Fade("01_TopScene", Color.black, 1.0f);
        }
    }
}
