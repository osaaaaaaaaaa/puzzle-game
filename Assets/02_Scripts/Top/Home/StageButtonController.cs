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
        // 子オブジェクトを取得して関数と引数を割り当てる
        Button[] buttons = GetComponentsInChildren<Button>();
        for (int i = 0; i < buttons.Length; i++)
        {
            int id = new int();
            id = i + 1;
            buttons[i].onClick.AddListener(() => manager.OnSelectStageButton(id));
        }
    }
}
