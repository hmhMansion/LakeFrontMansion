using UnityEngine;

namespace LakeFrontMansion.Environment
{
    /// <summary>
    /// 방의 경계(벽)를 설정하는 스크립트
    /// 플레이어가 방 밖으로 나가지 못하게 막음
    /// </summary>
    public class RoomBoundary : MonoBehaviour
    {
        [Header("경계 설정")]
        [Tooltip("경계선 색상 (에디터에서만 보임)")]
        public Color gizmoColor = Color.red;

        private void Awake()
        {
            // Collider2D가 있는지 확인
            if (GetComponent<Collider2D>() == null)
            {
                Debug.LogWarning($"{gameObject.name}: Collider2D 컴포넌트가 필요합니다!");
            }
        }

        // 에디터에서 경계선 표시
        private void OnDrawGizmos()
        {
            Gizmos.color = gizmoColor;

            BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
            if (boxCollider != null)
            {
                Gizmos.DrawWireCube(
                    transform.position + (Vector3)boxCollider.offset,
                    boxCollider.size
                );
            }

            PolygonCollider2D polygonCollider = GetComponent<PolygonCollider2D>();
            if (polygonCollider != null)
            {
                for (int i = 0; i < polygonCollider.pathCount; i++)
                {
                    Vector2[] points = polygonCollider.GetPath(i);
                    for (int j = 0; j < points.Length; j++)
                    {
                        Vector2 start = transform.TransformPoint(points[j]);
                        Vector2 end = transform.TransformPoint(points[(j + 1) % points.Length]);
                        Gizmos.DrawLine(start, end);
                    }
                }
            }
        }
    }
}
