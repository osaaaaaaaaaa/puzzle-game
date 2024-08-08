using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Wood : MonoBehaviour
{
    [SerializeField] Vector2 m_endPos;  // 移動先
    [SerializeField] float m_endTime;   // 何秒かけて移動するか

    // Start is called before the first frame update
    void Start()
    {
        transform.DOMove(m_endPos, m_endTime).SetEase(Ease.InOutSine).SetLoops(-1,LoopType.Yoyo);
    }
}
