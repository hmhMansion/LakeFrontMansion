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

        private Collider2D cachedCollider;

        private void Awake()
        {
            // Collider2D가 있는지 확인
            cachedCollider = GetComponent<Collider2D>();
            if (cachedCollider == null)
            {
                Debug.LogWarning($"{gameObject.name}: Collider2D 컴포넌트가 필요합니다!");
            }
        }

        /// <summary>
        /// 방의 경계 영역을 반환합니다
        /// </summary>
        public Bounds GetBounds()
        {
            if (cachedCollider == null)
            {
                cachedCollider = GetComponent<Collider2D>();
            }

            if (cachedCollider != null)
            {
                return cachedCollider.bounds;
            }

            // Collider가 없으면 기본 Bounds 반환
            Debug.LogWarning($"{gameObject.name}: Collider2D가 없어 기본 Bounds를 반환합니다.");
            return new Bounds(transform.position, Vector3.one * 100f);
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
