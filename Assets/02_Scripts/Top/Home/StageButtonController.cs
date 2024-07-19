using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButtonController : MonoBehaviour
{
    [SerializeField] TopManager manager;

    // Start is called before the first frame update
    void Start()
    {
        // �q�I�u�W�F�N�g���擾���Ċ֐��ƈ��������蓖�Ă�
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int id = new int();
            id = i + 1;
            buttons[i].onClick.AddListener(() => manager.OnSelectStageButton(id));
        }
    }
}
