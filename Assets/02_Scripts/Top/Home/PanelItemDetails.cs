using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelItemDetails : MonoBehaviour
{
    [SerializeField] GameObject m_content;
    [SerializeField] Text m_textPlate;
    [SerializeField] Text m_textItemDescription;
    [SerializeField] Image m_imgItemIcon;

    public void SetPanelContent(string textPlate,string itemDescription,Sprite sprite)
    {
        m_textPlate.text = textPlate;
        m_textItemDescription.text = itemDescription;
        m_imgItemIcon.sprite = sprite;
        OnToggleVisibility(true);
    }

    public void OnToggleVisibility(bool isVisisbility)
    {
        m_content.SetActive(isVisisbility);
    }
}
