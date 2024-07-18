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

    [SerializeField] Transform m_stageButtonParent;

    [SerializeField] GameObject obj;

    bool isClickTitle;  // タイトルをクリックしたかどうか

    /// <summary>
    /// 選択したステージID
    /// </summary>
    public static int stageID { get; set; }

    void Start()
    {
        isClickTitle = false;
        m_ui_start.GetComponent<CanvasGroup>().DOFade(0.0f, 1).SetEase(Ease.InCubic).SetLoops(-1, LoopType.Yoyo);

        SetEventButton();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && isClickTitle == false)
        {
            m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x - 1980f, 0, 0), 0.5f).SetEase(Ease.Linear);
            isClickTitle = true;
        }
    }

    void SetEventButton()
    {
        int i = 0;
        // 子オブジェクトを取得して関数と引数を割り当てる
        Button[] buttons = m_stageButtonParent.GetComponentsInChildren<Button>();
        foreach(Button button in buttons)
        {
            i++;
            int data = new int();
            data = i;
            Color color = new Color(Random.value, Random.value, Random.value, 1.0f);
            button.onClick.AddListener(() => Test(color,data));
        }
    }

    void Test(Color _color,int id)
    {
        Debug.Log(_color + ", id =" + id);
        obj.GetComponent<SpriteRenderer>().color = _color;
    }

    /// <summary>
    /// ステージ選択のボタン
    /// </summary>
    public void OnSelectStageButton(int id)
    {
        stageID = id;
        Debug.Log("私は：" + stageID);

        // ゲームシーンに遷移する

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
