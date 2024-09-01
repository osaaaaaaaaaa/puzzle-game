using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonController : MonoBehaviour
{
    [SerializeField] GameObject m_uiBtnPrefab;
    [SerializeField] TopManager m_manager;
    public int m_stageMax;

    private void Start()
    {
        TopManager.stageMax = m_stageMax;
    }

    /// <summary>
    /// �{�^���𐶐�����
    /// </summary>
    /// <param name="stageCnt"></param>
    public void GenerateButtons(int stageCnt)
    {
        // ���݂̃{�^����S�Ĕj������
        foreach(Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        // �{�^���𐶐�����
        for (int i = 0; i < stageCnt; i++)
        {
            // �{�^���̃p�����[�^
            int id = new int();
            id = i + 1;

            // �{�^������
            GameObject button = Instantiate(m_uiBtnPrefab, transform);
            button.GetComponent<Button>().onClick.AddListener(() => m_manager.OnSelectStageButton(id));
            button.GetComponentInChildren<Text>().text = "" + (i + 1);
        }
    }
}
