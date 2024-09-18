using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardBar : MonoBehaviour
{
    [SerializeField] Text m_textAchievedValue;            // �B�������l
    [SerializeField] Text m_textItemDescription;          // �A�C�e������
    [SerializeField] GameObject m_btnIconItem;            // �A�C�e���̃A�C�R���{�^��
    [SerializeField] List<Sprite> m_texItemIcons;         // �A�C�e���A�C�R���̉摜
    [SerializeField] GameObject m_textReceived;           // ���ς݂��ǂ����̃e�L�X�g
    [SerializeField] GameObject m_btnReward;              // ��V�󂯎��{�^��
    LoadingContainer m_loading;
    PanelItemDetails m_panelItemDetails;
    Sprite m_spriteItem;
    int m_achievementID;
    string m_itemDescription;

    public void UpdateReward(PanelItemDetails panelItemDetails, int achievementID, ShowUserItemResponse itemData, int achievedValue, int progressVal, bool isReceived)
    {
        // �����o�ϐ��擾
        m_loading = GameObject.Find("LoadingContainer").GetComponent<LoadingContainer>();
        m_panelItemDetails = panelItemDetails;
        m_spriteItem = itemData.Type == 1 ? TopManager.TexIcons[itemData.Effect - 1] : m_texItemIcons[itemData.Type - 1];
        m_achievementID = achievementID;
        m_itemDescription = itemData.Type == 2 ? itemData.Name : itemData.Description;

        // �p�����[�^�ݒ�
        m_textAchievedValue.text = achievedValue + "pt";
        m_textItemDescription.text = itemData.Description;
        m_btnIconItem.GetComponent<Image>().sprite = m_texItemIcons[itemData.Type - 1];
        m_btnIconItem.GetComponent<Button>().onClick.AddListener(() => {
            SEManager.Instance.PlayButtonSE();
            panelItemDetails.SetPanelContent("�A�C�e���ڍ�", m_itemDescription, m_spriteItem); 
        });

        if (!isReceived && achievedValue <= progressVal)
        {
            // ���󂯎�聕���󂯎���ꍇ
            m_btnReward.SetActive(true);
            return;
        }
        else if (isReceived)
        {
            // ���ς݂̏ꍇ
            m_textReceived.SetActive(true);
        }

        m_btnReward.SetActive(false);
    }

    public void OnGetRewardButton()
    {
        m_loading.ToggleLoadingUIVisibility(1);

        // �A�`�[�u�����g��V�󂯎�菈��
        StartCoroutine(NetworkManager.Instance.ReceiveRewardAchievement(
            m_achievementID,
            result =>
            {
                m_loading.ToggleLoadingUIVisibility(-1);
                if (!result) return;

                m_btnReward.SetActive(false);
                m_textReceived.SetActive(true);
                transform.SetAsLastSibling();
                m_panelItemDetails.SetPanelContent("�A�C�e���l��", m_itemDescription, m_spriteItem);
            }));
    }
}
