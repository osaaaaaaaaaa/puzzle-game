using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartArea : MonoBehaviour
{
    private void Start()
    {
        if (TopSceneDirector.Instance != null)
        {
            if (TopSceneDirector.Instance.PlayMode == TopSceneDirector.PLAYMODE.GUEST) return;
        }

        // 格納先
        List<Vector2> points = new List<Vector2>();

        var lineRenderer = GetComponent<LineRenderer>();
        var transform = lineRenderer.transform;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            // LineRendererの頂点座標をローカル座標に変換して取得する
            Vector3 point = transform.InverseTransformPoint(transform.TransformPoint(lineRenderer.GetPosition(i)));
            points.Add(point);
        }

        // EdgeCollider2Dのポイントを設定
        GetComponent<EdgeCollider2D>().SetPoints(points);
        // PolygonCollider2Dのパスを設定
        GetComponent<PolygonCollider2D>().SetPath(0, points);
    }
}
