using UnityEngine;
using System.Linq;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// 특정 태그를 가진 오브젝트의 개수를 체크하는 조건
    /// 예: 모든 적을 처치했는지 확인 (Enemy 태그를 가진 오브젝트가 0개인지)
    /// </summary>
    public class ObjectCountCondition : MonoBehaviour, ISceneTransitionCondition
    {
        [Header("카운트 설정")]
        [Tooltip("체크할 오브젝트의 태그")]
        public string targetTag = "Enemy";

        [Tooltip("조건을 만족하기 위한 오브젝트 개수")]
        public int requiredCount = 0;

        [Tooltip("개수 비교 방식")]
        public ComparisonType comparisonType = ComparisonType.Equal;

        public enum ComparisonType
        {
            Equal,              // 정확히 같음
            LessThan,           // 미만
            LessThanOrEqual,    // 이하
            GreaterThan,        // 초과
            GreaterThanOrEqual  // 이상
        }

        public bool IsSatisfied()
        {
            int currentCount = GetCurrentCount();

            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    return currentCount == requiredCount;
                case ComparisonType.LessThan:
                    return currentCount < requiredCount;
                case ComparisonType.LessThanOrEqual:
                    return currentCount <= requiredCount;
                case ComparisonType.GreaterThan:
                    return currentCount > requiredCount;
                case ComparisonType.GreaterThanOrEqual:
                    return currentCount >= requiredCount;
                default:
                    return false;
            }
        }

        public string GetDescription()
        {
            int currentCount = GetCurrentCount();
            string comparison = GetComparisonSymbol();
            return $"{targetTag} 오브젝트 개수: {currentCount} {comparison} {requiredCount}";
        }

        /// <summary>
        /// 현재 타겟 태그를 가진 오브젝트 개수를 반환
        /// </summary>
        private int GetCurrentCount()
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(targetTag);
            return objects != null ? objects.Length : 0;
        }

        /// <summary>
        /// 비교 방식에 따른 심볼을 반환
        /// </summary>
        private string GetComparisonSymbol()
        {
            switch (comparisonType)
            {
                case ComparisonType.Equal:
                    return "==";
                case ComparisonType.LessThan:
                    return "<";
                case ComparisonType.LessThanOrEqual:
                    return "<=";
                case ComparisonType.GreaterThan:
                    return ">";
                case ComparisonType.GreaterThanOrEqual:
                    return ">=";
                default:
                    return "?";
            }
        }

        // 에디터에서 확인용
        private void OnValidate()
        {
            // 태그가 유효한지 체크 (에디터에서만)
            #if UNITY_EDITOR
            try
            {
                UnityEditorInternal.InternalEditorUtility.tags
                    .FirstOrDefault(t => t == targetTag);
            }
            catch
            {
                // 태그가 존재하지 않는 경우 무시
            }
            #endif
        }
    }
}
