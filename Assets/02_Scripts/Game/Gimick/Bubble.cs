using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    [SerializeField] ColorData.COLOR_TYPE m_coloerType;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Son>())
        {
            var son = collision.GetComponent<Son>();

            if (son.m_coloerType == ColorData.COLOR_TYPE.RED && m_coloerType == ColorData.COLOR_TYPE.BLUE) son.ChangeColorType(ColorData.COLOR_TYPE.PURPLE);
            if (son.m_coloerType == ColorData.COLOR_TYPE.BLUE && m_coloerType == ColorData.COLOR_TYPE.RED) son.ChangeColorType(ColorData.COLOR_TYPE.PURPLE);
            if (son.m_coloerType == ColorData.COLOR_TYPE.DEFAULT) son.ChangeColorType(m_coloerType);

            if (collision.tag != "Ghost") Destroy(this.gameObject);
        }
    }
}
