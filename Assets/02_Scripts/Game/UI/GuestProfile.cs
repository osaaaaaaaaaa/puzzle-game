using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GuestProfile : MonoBehaviour
{
    [SerializeField] Text m_textUserName;                 // ユーザー名
    [SerializeField] Text m_textAchievementTitle;         // 称号名
    [SerializeField] Text m_textStageID;                  // ステージID
    [SerializeField] Text m_textScore;                    // スコア
    [SerializeField] Image m_icon;                        // アイコン
    [SerializeField] GameObject m_hertUI;                 // 相互フォロー,フォロワーを示すUI
    [SerializeField] Button m_buttonDestroy;              // ゲスト削除ボタン
    [SerializeField] Button m_buttonToggle;               // ゲストのオブジェクトを表示・非表示するボタン
    [SerializeField] GameObject m_window;                 // 確認ウインドウ
    [SerializeField] List<Sprite> m_spriteToggle;         // [0:非アクティブ画像 , 1:アクティブ画像]
    GameObject m_guestObj;
    int m_userID;

    /// <summary>
    /// プロフィール情報を更新する
    /// </summary>
    public void UpdateProfile(GameObject guestObj,int user_id,string name,string achievementTitle,int stageID,int score,Sprite icon,bool isAgreement)
    {
        // オブジェクトが存在しない場合は押せないようにする
        if (guestObj == null) m_buttonToggle.interactable = false;
        m_guestObj = guestObj;
        m_userID = user_id;
        m_textUserName.text = name;
        m_textAchievementTitle.text = achievementTitle;
        m_textStageID.text = "" + stageID;
        m_textScore.text = "" + score;
        m_icon.sprite = icon;
        m_hertUI.SetActive(isAgreement);

        // 自身がゲストの場合は破棄ボタンを押せないようにする
        if(TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) m_buttonDestroy.interactable = false;
    }

    /// <summary>
    /// ゲスト削除処理
    /// </summary>
    public void OnGuestDestroyButton()
    {
        SEManager.Instance.PlayButtonSE();

        // ゲスト削除処理
        StartCoroutine(NetworkManager.Instance.DestroySignalGuest(
            TopSceneDirector.Instance.DistressSignalID,
            m_userID,
            result =>
            {
                if (!result) return;
                if (m_guestObj != null) Destroy(m_guestObj);
                Destroy(gameObject);
            }));
    }

    /// <summary>
    /// 確認ウインドウの表示・非表示
    /// </summary>
    public void OnToggleWindowVisibility(bool isVisible)
    {
        if(isVisible) SEManager.Instance.PlayButtonSE();
        if(!isVisible) SEManager.Instance.PlayCanselSE();
        m_window.SetActive(isVisible);
    }

    /// <summary>
    /// ゲストを表示・非表示処理
    /// </summary>
    public void OnToggleButtonVisibility()
    {
        // 表示・非表示を切り替える
        m_guestObj.SetActive(!m_guestObj.activeSelf);
        m_buttonToggle.GetComponent<Image>().sprite = !m_guestObj.activeSelf ? m_spriteToggle[0] : m_spriteToggle[1];

        if (m_guestObj.activeSelf) SEManager.Instance.PlayButtonSE();
        if (!m_guestObj.activeSelf) SEManager.Instance.PlayCanselSE();

        // ゲストの起動予測を表示・非表示処理
        m_guestObj.GetComponent<Guest>().ToggleLineVisibility(m_guestObj.activeSelf);
    }
}
