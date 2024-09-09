using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReplayPlayer : MonoBehaviour
{
    [SerializeField] GameObject m_son;
    [SerializeField] GameObject m_sonRun;
    [SerializeField] GameObject m_sonCow;

    // データを再生する間隔
    const float saveInterval = 0.05f;

    private void Start()
    {
        m_son.SetActive(false);
        m_sonRun.SetActive(false);
        m_sonCow.SetActive(false);
    }

    /// <summary>
    /// リプレイ処理
    /// </summary>
    public IEnumerator ReplayCoroutine(GameManager gameManager,List<ReplayData> replayDatas)
    {
        foreach (var data in replayDatas)
        {
            if (data == null)
            {
                yield return new WaitForSeconds(saveInterval);
                continue;
            }

            GameObject targetSon = null;

            // 動かす息子を取得する
            switch (data.TypeSon)
            {
                case ReplayData.TYPESON.DEFAULT:
                    targetSon = m_son;
                    break;
                case ReplayData.TYPESON.RUN:
                    targetSon = m_sonRun;
                    break;
                case ReplayData.TYPESON.COW:
                    targetSon = m_sonCow;
                    break;
            }

            // 関係ない他の息子のオブジェクトを非表示処理
            ToggleSonObjVisibility(targetSon);

            // パラメータを設定する
            var scale = targetSon.transform.localScale;
            targetSon.transform.localScale = new Vector3(Mathf.Abs(scale.x) * data.Dir, scale.y, scale.z);
            targetSon.GetComponent<Rigidbody2D>().gravityScale = data.Gravity;
            targetSon.transform.position = data.Pos;
            targetSon.GetComponent<Rigidbody2D>().velocity = data.Vel;

            yield return new WaitForSeconds(saveInterval);
        }

        gameManager.IsReplayEnd = true;
        ResetRB();
    }

    /// <summary>
    /// 息子のオブジェクトを表示・非表示処理
    /// </summary>
    /// <param name="currentSon">現在動かす息子のオブジェクト</param>
    void ToggleSonObjVisibility(GameObject currentSon)
    {
        m_son.SetActive(m_son == currentSon);
        m_sonRun.SetActive(m_sonRun == currentSon);
        m_sonCow.SetActive(m_sonCow == currentSon);
    }

    /// <summary>
    /// 息子のRigidbody2Dをリセットする
    /// </summary>
    void ResetRB()
    {
        m_son.GetComponent<Rigidbody2D>().gravityScale = 0;
        m_son.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        m_sonRun.GetComponent<Rigidbody2D>().gravityScale = 0;
        m_sonRun.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
        m_sonCow.GetComponent<Rigidbody2D>().gravityScale = 0;
        m_sonCow.GetComponent<Rigidbody2D>().velocity = Vector3.zero;
    }
}
