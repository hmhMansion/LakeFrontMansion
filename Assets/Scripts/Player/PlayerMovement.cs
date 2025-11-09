using UnityEngine;

namespace LakeFrontMansion.Player
{
    /// <summary>
    /// 2D 픽셀 게임용 플레이어 이동 컨트롤러
    /// WASD 또는 방향키로 8방향 이동
    /// </summary>
    [RequireComponent(typeof(Rigidbody2D))]
    public class PlayerMovement : MonoBehaviour
    {
        [Header("이동 설정")]
        [Tooltip("이동 속도")]
        public float moveSpeed = 5f;

        [Header("애니메이션 (선택사항)")]
        [Tooltip("Animator 컴포넌트 (없으면 비워두기)")]
        public Animator animator;

        private Rigidbody2D rb;
        private Vector2 movement;
        private Vector2 lastMovement;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            // Rigidbody2D 기본 설정
            rb.gravityScale = 0f; // 2D 탑다운이므로 중력 없음
            rb.freezeRotation = true; // 회전 방지

            // 초기 방향은 아래
            lastMovement = new Vector2(0, -1);
        }

        private void Update()
        {
            // 입력 받기
            movement.x = Input.GetAxisRaw("Horizontal"); // A/D 또는 ←/→
            movement.y = Input.GetAxisRaw("Vertical");   // W/S 또는 ↑/↓

            // 대각선 이동 시 속도 정규화
            if (movement.magnitude > 1f)
            {
                movement.Normalize();
            }

            // 마지막 이동 방향 저장 (움직이고 있을 때만)
            if (movement.magnitude > 0.01f)
            {
                lastMovement = movement;
            }

            // 애니메이션 파라미터 설정 (Animator가 있는 경우)
            if (animator != null)
            {
                animator.SetFloat("Horizontal", movement.x);
                animator.SetFloat("Vertical", movement.y);
                animator.SetFloat("Speed", movement.magnitude);
                animator.SetFloat("LastHorizontal", lastMovement.x);
                animator.SetFloat("LastVertical", lastMovement.y);
            }
        }

        private void FixedUpdate()
        {
            // 물리 기반 이동
            rb.linearVelocity = movement * moveSpeed;
        }

        /// <summary>
        /// 플레이어 이동 가능/불가능 설정
        /// </summary>
        public void SetMovementEnabled(bool enabled)
        {
            this.enabled = enabled;
            if (!enabled)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}
