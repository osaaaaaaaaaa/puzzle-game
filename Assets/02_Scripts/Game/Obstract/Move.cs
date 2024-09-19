using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Move : MonoBehaviour
{
    [SerializeField] Vector2 m_endPos;  // 移動先
    [SerializeField] float m_endTime;   // 何秒かけて移動するか

    // Start is called before the first frame update
    void Start()
    {
        transform.DOLocalMove(m_endPos, m_endTime).SetEase(Ease.Linear).SetLoops(-1,LoopType.Yoyo);
    }
}
