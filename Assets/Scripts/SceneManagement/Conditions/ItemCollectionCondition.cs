using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// 특정 아이템들을 모두 수집했는지 체크하는 조건
    /// 각 아이템의 태그나 이름으로 체크할 수 있습니다
    /// </summary>
    public class ItemCollectionCondition : MonoBehaviour, ISceneTransitionCondition
    {
        [Header("수집 대상 설정")]
        [Tooltip("수집해야 하는 아이템 개수")]
        public int requiredItemCount = 3;

        [Tooltip("수집할 아이템의 태그 (비워두면 태그 체크 안함)")]
        public string itemTag = "";

        [Header("진행 상황")]
        [Tooltip("현재 수집한 아이템 개수 (읽기 전용)")]
        [SerializeField]
        private int collectedItemCount = 0;

        private HashSet<GameObject> collectedItems = new HashSet<GameObject>();

        public bool IsSatisfied()
        {
            return collectedItemCount >= requiredItemCount;
        }

        public string GetDescription()
        {
            return $"아이템 수집: {collectedItemCount}/{requiredItemCount}";
        }

        /// <summary>
        /// 아이템을 수집합니다
        /// </summary>
        /// <param name="item">수집할 아이템</param>
        public void CollectItem(GameObject item)
        {
            // 이미 수집한 아이템인지 체크
            if (collectedItems.Contains(item))
            {
                Debug.Log($"이미 수집한 아이템입니다: {item.name}");
                return;
            }

            // 태그 체크 (태그가 설정되어 있는 경우)
            if (!string.IsNullOrEmpty(itemTag) && !item.CompareTag(itemTag))
            {
                Debug.Log($"아이템 태그가 일치하지 않습니다: {item.tag} (필요: {itemTag})");
                return;
            }

            // 아이템 수집
            collectedItems.Add(item);
            collectedItemCount = collectedItems.Count;
            Debug.Log($"아이템 수집! ({collectedItemCount}/{requiredItemCount}): {item.name}");

            // 모든 아이템을 수집했는지 체크
            if (IsSatisfied())
            {
                Debug.Log("모든 아이템을 수집했습니다!");
            }
        }

        /// <summary>
        /// 수집 상태를 리셋합니다
        /// </summary>
        public void ResetCollection()
        {
            collectedItems.Clear();
            collectedItemCount = 0;
            Debug.Log("아이템 수집 상태가 리셋되었습니다.");
        }

        /// <summary>
        /// 현재 수집 진행률을 반환합니다
        /// </summary>
        /// <returns>0.0 ~ 1.0 사이의 값</returns>
        public float GetProgress()
        {
            if (requiredItemCount == 0) return 1f;
            return (float)collectedItemCount / requiredItemCount;
        }
    }
}
