using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField] List<Sprite> m_texTutorial;
    [SerializeField] Image m_imgTutorial;
    [SerializeField] Text m_textCurrentPage;
    int m_currentPage;

    private void OnEnable()
    {
        m_currentPage = 1;
        m_textCurrentPage.text = m_currentPage + "/" + m_texTutorial.Count;
        m_imgTutorial.sprite = m_texTutorial[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            m_currentPage = m_currentPage < m_texTutorial.Count ? m_currentPage + 1 : 1;
            m_textCurrentPage.text = m_currentPage + "/" + m_texTutorial.Count;
            m_imgTutorial.sprite = m_texTutorial[m_currentPage - 1];
        }
    }
}
