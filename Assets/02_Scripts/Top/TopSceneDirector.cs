using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopSceneDirector : MonoBehaviour
{
    public static TopSceneDirector Instance;

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

    [SerializeField] GameObject topObjParent;

    public enum PLAYMODE
    {
        SOLO,
        HOST,
        GUEST
    }
    public PLAYMODE PlayMode { get; private set; }

    public int DistressSignalID { get; private set; }

    /// <summary>
    /// トップ画面の表示・非表示を切り替える
    /// </summary>
    public void ChangeActive(bool _active)
    {
        topObjParent.SetActive(_active); // 表示切り替え処理
    }

    public void SetPlayMode(PLAYMODE mode,int signalID)
    {
        PlayMode = mode;
        DistressSignalID = signalID;
    }
}
