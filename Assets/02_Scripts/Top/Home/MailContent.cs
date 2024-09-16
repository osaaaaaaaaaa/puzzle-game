using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MailContent : MonoBehaviour
{
    [SerializeField] LoadingContainer m_loading;

    [SerializeField] GameObject m_container;
    [SerializeField] Text m_textTitle;
    [SerializeField] Text m_textCreatedAt;
    [SerializeField] Text m_textElapsedDay;
    [SerializeField] Text m_textMail;
    [SerializeField] GameObject m_btnOK;
    [SerializeField] PanelItemDetails m_itemDetails;
    [SerializeField] Sprite m_spritePoint;
    [SerializeField] Sprite m_spriteReceivedMail;
    GameObject m_selectMailButton;
    int m_mailID;

    public void SetMailContent(GameObject mailButton,int mailID,string title,string createdAt,int elapsedDay, string text,bool isReceived)
    {
        m_selectMailButton = mailButton;
        m_mailID = mailID;
        m_container.SetActive(true);
        m_textTitle.text = title;
        m_textCreatedAt.text = createdAt;
        m_textElapsedDay.text = elapsedDay + "日後に自動削除";
        m_textMail.text = text;
        m_btnOK.SetActive(!isReceived);
    }

    public void OnUpdateUserMailButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);
        // 受信メール開封処理
        StartCoroutine(NetworkManager.Instance.UpdateUserMail(
            m_mailID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (result == null || result.Length == 0) return;

                // 取得アイテム表示
                m_selectMailButton.GetComponent<Image>().sprite = m_spriteReceivedMail;
                m_itemDetails.SetPanelContent("アイテム獲得", result[0].Description, m_spritePoint);
                m_btnOK.SetActive(false);

            }));
    }

    public void OnDestroyButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);
        // 受信メール削除処理
        StartCoroutine(NetworkManager.Instance.DestroyUserMail(
            m_mailID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (!result) return;
                Destroy(m_selectMailButton);
                m_container.SetActive(false);
            }));
    }
}
