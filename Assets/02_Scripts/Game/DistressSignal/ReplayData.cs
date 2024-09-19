using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayData
{
    /// <summary>
    /// 息子の状態
    /// </summary>
    public enum TYPESON
    {
        DEFAULT,
        RUN,
        COW
    }
    public TYPESON TypeSon { get; set; }

    /// <summary>
    /// 向き(1or-1)
    /// </summary>
    public float Dir { get; set; }

    /// <summary>
    /// 重力
    /// </summary>
    public float Gravity { get; set; }

    /// <summary>
    /// 座標
    /// </summary>
    public Vector3 Pos { get; set; }

    /// <summary>
    /// 移動量
    /// </summary>
    public Vector3 Vel { get; set; }
}
