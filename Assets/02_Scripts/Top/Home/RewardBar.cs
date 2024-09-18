using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardBar : MonoBehaviour
{
    [SerializeField] Text m_textAchievedValue;            // 達成条件値
    [SerializeField] Text m_textItemDescription;          // アイテム説明
    [SerializeField] GameObject m_btnIconItem;            // アイテムのアイコンボタン
    [SerializeField] List<Sprite> m_texItemIcons;         // アイテムアイコンの画像
    [SerializeField] GameObject m_textReceived;           // 受取済みかどうかのテキスト
    [SerializeField] GameObject m_btnReward;              // 報酬受け取りボタン
    LoadingContainer m_loading;
    PanelItemDetails m_panelItemDetails;
    Sprite m_spriteItem;
    int m_achievementID;
    string m_itemDescription;

    public void UpdateReward(PanelItemDetails panelItemDetails, int achievementID, ShowUserItemResponse itemData, int achievedValue, int progressVal, bool isReceived)
    {
        // メンバ変数取得
        m_loading = GameObject.Find("LoadingContainer").GetComponent<LoadingContainer>();
        m_panelItemDetails = panelItemDetails;
        m_spriteItem = itemData.Type == 1 ? TopManager.TexIcons[itemData.Effect - 1] : m_texItemIcons[itemData.Type - 1];
        m_achievementID = achievementID;
        m_itemDescription = itemData.Type == 2 ? itemData.Name : itemData.Description;

        // パラメータ設定
        m_textAchievedValue.text = achievedValue + "pt";
        m_textItemDescription.text = itemData.Description;
        m_btnIconItem.GetComponent<Image>().sprite = m_texItemIcons[itemData.Type - 1];
        m_btnIconItem.GetComponent<Button>().onClick.AddListener(() => {
            SEManager.Instance.PlayButtonSE();
            panelItemDetails.SetPanelContent("アイテム詳細", m_itemDescription, m_spriteItem); 
        });

        if (!isReceived && achievedValue <= progressVal)
        {
            // 見受け取り＆＆受け取れる場合
            m_btnReward.SetActive(true);
            return;
        }
        else if (isReceived)
        {
            // 受取済みの場合
            m_textReceived.SetActive(true);
        }

        m_btnReward.SetActive(false);
    }

    public void OnGetRewardButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        // アチーブメント報酬受け取り処理
        StartCoroutine(NetworkManager.Instance.ReceiveRewardAchievement(
            m_achievementID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (!result) return;

                m_btnReward.SetActive(false);
                m_textReceived.SetActive(true);
                transform.SetAsLastSibling();
                m_panelItemDetails.SetPanelContent("アイテム獲得", m_itemDescription, m_spriteItem);
            }));
    }
}
