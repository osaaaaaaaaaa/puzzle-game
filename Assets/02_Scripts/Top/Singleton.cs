using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            // トップ画面の状態を保持する
            Instance = this;

            // シーン遷移しても破棄しないようにする
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // トップ画面を表示する
            Instance.ChangeActive(true);

            // シーン遷移して新しく生成される自身を破棄
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// トップ画面の表示・非表示を切り替える
    /// </summary>
    public void ChangeActive(bool _active)
    {
        var obj = Instance.transform.GetChild(0).gameObject;
        obj.SetActive(_active); // 表示切り替え処理
    }
}
