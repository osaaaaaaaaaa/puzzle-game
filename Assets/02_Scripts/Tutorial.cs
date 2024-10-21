using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<Sprite> m_texTutorial;
    [SerializeField] Image m_imgTutorial;
    [SerializeField] Text m_textCurrentPage;
    const int dragTutorialPageIndex = 2;
    int m_currentPage;
    int m_pageMax;

    private void OnEnable()
    {
        if(TopManager.stageID == 1)
        {
            m_texTutorial.RemoveAt(dragTutorialPageIndex);
        }

        m_currentPage = TopManager.stageID == 2 ? dragTutorialPageIndex + 1 : 1;
        m_pageMax = m_texTutorial.Count;
        m_textCurrentPage.text = m_currentPage + "/" + m_pageMax;
        m_imgTutorial.sprite = m_texTutorial[m_currentPage - 1];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_currentPage = m_currentPage < m_pageMax ? m_currentPage + 1 : 1;
            m_textCurrentPage.text = m_currentPage + "/" + m_pageMax;
            m_imgTutorial.sprite = m_texTutorial[m_currentPage - 1];
        }
    }
}
