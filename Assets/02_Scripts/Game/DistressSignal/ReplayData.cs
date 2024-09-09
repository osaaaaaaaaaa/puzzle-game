using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReplayData
{
    /// <summary>
    /// ���q�̏��
    /// </summary>
    public enum TYPESON
    {
        DEFAULT,
        RUN,
        COW
    }
    public TYPESON TypeSon { get; set; }

    /// <summary>
    /// ����(1or-1)
    /// </summary>
    public float Dir { get; set; }

    /// <summary>
    /// �d��
    /// </summary>
    public float Gravity { get; set; }

    /// <summary>
    /// ���W
    /// </summary>
    public Vector3 Pos { get; set; }

    /// <summary>
    /// �ړ���
    /// </summary>
    public Vector3 Vel { get; set; }
}
