using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonController : MonoBehaviour
{
    [SerializeField] GameObject m_uiBtnPrefab;
    [SerializeField] TopManager m_manager;

    /// <summary>
    /// ボタンを生成する
    /// </summary>
    /// <param name="stageCnt"></param>
    public void GenerateButtons(int stageCnt)
    {
        // 現在のボタンを全て破棄する
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // ボタンを生成する
        for (int i = 0; i < stageCnt; i++)
        {
            // ボタンのパラメータ
            int id = new int();
            id = i + 1;

            // ボタン生成
            GameObject button = Instantiate(m_uiBtnPrefab, transform);
            button.GetComponent<Button>().onClick.AddListener(() => m_manager.OnSelectStageButton(id));
            button.GetComponent<Button>().onClick.AddListener(() => SEManager.Instance.PlayButtonSE());
            button.GetComponentInChildren<Text>().text = "" + (i + 1);
        }
    }
}
