using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelRankingUserFollow : MonoBehaviour
{
    [SerializeField] UIUserManager m_uiUserManager;
    [SerializeField] GameObject m_content;
    [SerializeField] Text m_text;
    [SerializeField] Button m_btnYes;
    LoadingContainer m_loading;
    ShowRankingResponse[] m_rankingUsers;

    public void IniRankingtUserData(ShowRankingResponse[] rankingUsers)
    {
        m_rankingUsers = rankingUsers;
    }

    public void SetPanelContent(int user_id,string name,int index)
    {
        m_loading = GameObject.Find("LoadingContainer").GetComponent<LoadingContainer>();
        bool isFollow = m_rankingUsers[index].IsFollow;
        m_content.SetActive(true);
        m_text.text = !isFollow ? name + "���t�H���[���܂����H" : name + "���t�H���[�������܂����H";
        m_btnYes.onClick.RemoveAllListeners();
        m_btnYes.onClick.AddListener(() => {
            if (!isFollow) OnStoreUserFollow(user_id);
            if (isFollow) OnDestroyUserFollow(user_id);
            m_rankingUsers[index].IsFollow = !isFollow;
        });
    }

    public void OnStoreUserFollow(int user_id)
    {
        SEManager.Instance.PlayButtonSE();
        m_loading.ToggleLoadingUIVisibility(1);
        // �t�H���[����
        StartCoroutine(NetworkManager.Instance.StoreUserFollow(
            user_id,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (result != null)
                {
                    return;
                }
                OnHideContent();
            }));
    }

    public void OnDestroyUserFollow(int user_id)
    {
        SEManager.Instance.PlayButtonSE();
        m_loading.ToggleLoadingUIVisibility(1);
        // �t�H���[��������
        StartCoroutine(NetworkManager.Instance.DestroyUserFollow(
            user_id,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (!result) return;
                OnHideContent();
            }));
    }

    public void OnHideContent()
    {
        m_content.SetActive(false);
    }
}
