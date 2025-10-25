using UnityEngine;
using UnityEngine.SceneManagement;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// 문이나 특정 오브젝트에 붙여서 Scene 전환을 트리거하는 컴포넌트
    /// 플레이어가 충돌하거나 클릭하면 지정된 Scene으로 이동
    /// </summary>
    public class SceneTransitionTrigger : MonoBehaviour
    {
        [Header("Scene 설정")]
        [Tooltip("이동할 Scene 이름")]
        public string targetSceneName;

        [Header("전환 방식")]
        [Tooltip("클릭으로 전환할지 여부")]
        public bool useClick = true;

        [Tooltip("충돌(Trigger)로 전환할지 여부")]
        public bool useTrigger = false;

        [Header("플레이어 설정")]
        [Tooltip("플레이어 태그 (충돌 감지용)")]
        public string playerTag = "Player";

        [Header("UI 표시")]
        [Tooltip("플레이어가 가까이 있을 때 표시할 텍스트 (E키를 누르세요 등)")]
        public GameObject interactionUI;

        private bool playerNearby = false;

        private void Start()
        {
            // Trigger 방식을 사용한다면 Collider2D가 필요
            if (useTrigger && GetComponent<Collider2D>() == null)
            {
                Debug.LogWarning($"{gameObject.name}: Trigger 방식을 사용하려면 Collider2D 컴포넌트가 필요합니다.");
            }

            // UI 숨기기
            if (interactionUI != null)
            {
                interactionUI.SetActive(false);
            }
        }

        private void Update()
        {
            // 클릭 방식 + 플레이어가 근처에 있을 때
            if (useClick && playerNearby)
            {
                // E키나 마우스 클릭으로 전환
                if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
                {
                    TransitionToScene();
                }
            }
        }

        private void OnMouseDown()
        {
            // 마우스로 직접 클릭했을 때
            if (useClick && !useTrigger)
            {
                TransitionToScene();
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                playerNearby = true;

                // UI 표시
                if (interactionUI != null)
                {
                    interactionUI.SetActive(true);
                }

                // Trigger 방식이면 즉시 전환
                if (useTrigger && !useClick)
                {
                    TransitionToScene();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other.CompareTag(playerTag))
            {
                playerNearby = false;

                // UI 숨기기
                if (interactionUI != null)
                {
                    interactionUI.SetActive(false);
                }
            }
        }

        /// <summary>
        /// Scene 전환 실행
        /// </summary>
        private void TransitionToScene()
        {
            if (string.IsNullOrEmpty(targetSceneName))
            {
                Debug.LogError($"{gameObject.name}: 타겟 Scene 이름이 설정되지 않았습니다!");
                return;
            }

            Debug.Log($"Scene 전환: {SceneManager.GetActiveScene().name} -> {targetSceneName}");
            SceneLoader.Instance.LoadScene(targetSceneName);
        }

        // Gizmo로 위치 표시 (에디터에서만)
        private void OnDrawGizmos()
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireCube(transform.position, transform.localScale);
        }
    }
}
