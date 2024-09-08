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

        // �i�[��
        List<Vector2> points = new List<Vector2>();

        var lineRenderer = GetComponent<LineRenderer>();
        var transform = lineRenderer.transform;
        for (int i = 0; i < lineRenderer.positionCount; i++)
        {
            // LineRenderer�̒��_���W�����[�J�����W�ɕϊ����Ď擾����
            Vector3 point = transform.InverseTransformPoint(transform.TransformPoint(lineRenderer.GetPosition(i)));
            points.Add(point);
        }

        // EdgeCollider2D�̃|�C���g��ݒ�
        GetComponent<EdgeCollider2D>().SetPoints(points);
        // PolygonCollider2D�̃p�X��ݒ�
        GetComponent<PolygonCollider2D>().SetPath(0, points);
    }
}
