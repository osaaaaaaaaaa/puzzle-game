using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayData
{
    /// <summary>
    /// ‘§q‚Ìó‘Ô
    /// </summary>
    public enum TYPESON
    {
        DEFAULT,
        RUN,
        COW
    }
    public TYPESON TypeSon { get; set; }

    /// <summary>
    /// Œü‚«(1or-1)
    /// </summary>
    public float Dir { get; set; }

    /// <summary>
    /// d—Í
    /// </summary>
    public float Gravity { get; set; }

    /// <summary>
    /// À•W
    /// </summary>
    public Vector3 Pos { get; set; }

    /// <summary>
    /// ˆÚ“®—Ê
    /// </summary>
    public Vector3 Vel { get; set; }
}
