using UnityEngine;

namespace LakeFrontMansion.Environment
{
    /// <summary>
    /// 가구/장식품 오브젝트
    /// 플레이어가 관통하지 못하도록 충돌 처리
    /// </summary>
    public class Furniture : MonoBehaviour
    {
        [Header("가구 설정")]
        [Tooltip("가구 종류 (침대, 책상, 의자 등)")]
        public string furnitureType = "Furniture";

        [Tooltip("상호작용 가능 여부")]
        public bool isInteractable = false;

        [Header("시각 효과")]
        [Tooltip("에디터에서 표시할 색상")]
        public Color gizmoColor = Color.blue;

        private void Awake()
        {
            // Collider2D가 있는지 확인
            if (GetComponent<Collider2D>() == null)
            {
                Debug.LogWarning($"{gameObject.name}: Collider2D 컴포넌트가 필요합니다!");
            }

            // Collider2D의 Is Trigger가 false인지 확인
            Collider2D col = GetComponent<Collider2D>();
            if (col != null && col.isTrigger)
            {
                Debug.LogWarning($"{gameObject.name}: 가구는 Is Trigger가 false여야 합니다!");
            }
        }

        // 에디터에서 가구 표시
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
        }

        /// <summary>
        /// 상호작용 가능한 가구라면 이 메서드가 호출됨
        /// </summary>
        public void Interact()
        {
            if (isInteractable)
            {
                Debug.Log($"{furnitureType} 상호작용!");
                // 여기에 상호작용 로직 추가
                // 예: 서랍 열기, 아이템 획득 등
            }
        }
    }
}
