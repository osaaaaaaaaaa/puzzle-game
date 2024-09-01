using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;
using UnityEngine.AddressableAssets;

public class TopManager : MonoBehaviour
{
    [SerializeField] GameObject m_parent_top;
    [SerializeField] GameObject m_ui_startTextParent;
    [SerializeField] Image m_panelImage;            // 非表示にするパネルのイメージ
    [SerializeField] Text m_uiUserName;             // ユーザー名
    [SerializeField] AssetDownLoader m_assetDownLoader;  // アセットダウンローダー

    // システム画面のパネルリスト
    [SerializeField] List<GameObject> m_sys_panelList;
    // システムボタンの連番
    public enum SYSTEM
    {
        PROFILE = 0,
        MAILBOX,
        FOLLOWBOX
    }

    /// <summary>
    /// タイトルをクリックしたかどうか
    /// </summary>
    public static bool m_isClickTitle { get; private set; } = false;

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
        m_panelImage.enabled = false;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !m_isClickTitle)
        {
            m_isClickTitle = true;

#if !UNITY_EDITOR
            DOTween.Kill(m_ui_startTextParent.transform);
            m_ui_startTextParent.SetActive(false);

            // アセットバンドルが更新可能かどうかチェック
            m_assetDownLoader.StartCoroutine(m_assetDownLoader.checkCatalog());
#else
            StoreUser();
#endif
        }
    }

    /// <summary>
    /// ユーザー登録処理
    /// </summary>
    public void StoreUser()
    {
        // ユーザーデータが保存されていない場合
        if (!NetworkManager.Instance.LoadUserData())
        {
            // ユーザー登録処理
            StartCoroutine(NetworkManager.Instance.StoreUser(
                Guid.NewGuid().ToString(),
                result =>
                {
                    if (result) OnClickTitleWindow();
                }));    // 登録処理後の処理
        }
        else
        {
            OnClickTitleWindow();
        }
    }

    /// <summary>
    /// ステージ選択のボタン
    /// </summary>
    public void OnSelectStageButton(int id)
    {
        stageID = id;

#if !UNITY_EDITOR
        // ゲームUIシーンに遷移する
        Initiate.Fade("02_UIScene", Color.black, 1.0f);
#else
        // ゲームシーンに遷移する
        Initiate.Fade(stageID + "_GameScene", Color.black, 1.0f);
#endif
    }

    /// <summary>
    /// タイトル画面からホーム画面へ移動する
    /// </summary>
    void OnClickTitleWindow()
    {
        GetComponent<UserController>().UpdateUserDataUI(true, m_parent_top);
    }

    /// <summary>
    /// ホーム画面からタイトル画面へ戻る
    /// </summary>
    public void OnBackButtonHome()
    {
        m_ui_startTextParent.SetActive(true);

        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x + 1980f, 0, 0), 0.5f).SetEase(Ease.Linear)
            .OnComplete(()=> { m_isClickTitle = false; });
    }

    /// <summary>
    /// ホーム画面からシステム画面(プロフィール、メールボックスなど)へ移動する
    /// </summary>
    /// <param name="systemNum">SYSTEM（システムボタンの連番）参照</param>
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
    /// システム画面からホーム画面へ戻る
    /// </summary>
    public void OnBackButtonSystemPanel()
    {
        GetComponent<UserController>().ResetErrorText();
        m_parent_top.transform.DOLocalMove(new Vector3(m_parent_top.transform.localPosition.x, 0, 0), 0.5f).SetEase(Ease.Linear);
    }
}
