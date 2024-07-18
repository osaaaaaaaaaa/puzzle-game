using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    [SerializeField] GameManager manager;

    /// <summary>
    /// 除外レイヤーでSonレイヤーのみ接触判定を拾うよう設定済み
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ゲームクリア処理
        manager.GameClear();
    }
}
