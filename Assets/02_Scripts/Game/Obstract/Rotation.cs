using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotation : MonoBehaviour
{
    [SerializeField] float m_addAngle;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(0f, 0f, m_addAngle);
    }
}
