using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

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

        [Header("대화 설정")]
        [Tooltip("E 버튼을 처음 눌렀을 때 표시할 Dialogue Box (설정 시 대화 후 씬 전환)")]
        public GameObject dialogueBox;

        [Header("조건 설정")]
        [Tooltip("조건 없이 무조건 전환 가능하게 할지 여부")]
        public bool alwaysAllowTransition = true;

        [Tooltip("전환을 위해 만족해야 하는 조건들 (비어있으면 조건 없음)")]
        public MonoBehaviour[] transitionConditions;

        private bool playerNearby = false;
        private bool isDialogueShown = false;
        private ISceneTransitionCondition[] conditionComponents;

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

            // Dialogue Box 숨기기
            if (dialogueBox != null)
            {
                dialogueBox.SetActive(false);
            }

            // 조건 컴포넌트들을 캐싱
            if (transitionConditions != null && transitionConditions.Length > 0)
            {
                conditionComponents = transitionConditions
                    .Where(c => c != null && c is ISceneTransitionCondition)
                    .Cast<ISceneTransitionCondition>()
                    .ToArray();

                if (conditionComponents.Length != transitionConditions.Length)
                {
                    Debug.LogWarning($"{gameObject.name}: 일부 조건 컴포넌트가 ISceneTransitionCondition을 구현하지 않습니다.");
                }
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
                    // Dialogue Box가 설정되어 있고 아직 표시하지 않았다면
                    if (dialogueBox != null && !isDialogueShown)
                    {
                        ShowDialogue();
                    }
                    else
                    {
                        // Dialogue Box가 없거나 이미 표시했다면 Scene 전환
                        TransitionToScene();
                    }
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

                // Dialogue Box 숨기기 및 상태 초기화
                if (dialogueBox != null)
                {
                    dialogueBox.SetActive(false);
                    isDialogueShown = false;
                }
            }
        }

        /// <summary>
        /// 모든 조건이 만족되었는지 확인
        /// </summary>
        /// <returns>조건을 만족하거나 조건 체크를 무시할 경우 true</returns>
        private bool CheckConditions()
        {
            // 무조건 허용 옵션이 켜져있으면 바로 true
            if (alwaysAllowTransition)
            {
                return true;
            }

            // 조건이 없으면 true
            if (conditionComponents == null || conditionComponents.Length == 0)
            {
                return true;
            }

            // 모든 조건을 체크
            foreach (var condition in conditionComponents)
            {
                if (!condition.IsSatisfied())
                {
                    Debug.Log($"{gameObject.name}: 조건 미충족 - {condition.GetDescription()}");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Dialogue Box 표시
        /// </summary>
        private void ShowDialogue()
        {
            if (dialogueBox != null)
            {
                dialogueBox.SetActive(true);
                isDialogueShown = true;
                Debug.Log($"{gameObject.name}: Dialogue Box 표시 (E를 다시 눌러 씬 전환)");
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

            // 조건 체크
            if (!CheckConditions())
            {
                Debug.Log($"{gameObject.name}: Scene 전환 조건이 만족되지 않았습니다.");
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
