using UnityEngine;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// 단순 bool 값으로 조건을 체크하는 컴포넌트
    /// 다른 스크립트에서 conditionMet 값을 true로 변경하면 조건 충족
    /// </summary>
    public class SimpleBoolCondition : MonoBehaviour, ISceneTransitionCondition
    {
        [Header("조건 설정")]
        [Tooltip("조건 만족 여부")]
        public bool conditionMet = false;

        [Tooltip("조건 설명")]
        public string description = "특정 조건 만족";

        public bool IsSatisfied()
        {
            return conditionMet;
        }

        public string GetDescription()
        {
            return description;
        }

        /// <summary>
        /// 조건을 만족시킵니다
        /// </summary>
        public void SetConditionMet()
        {
            conditionMet = true;
            Debug.Log($"{description} 조건이 충족되었습니다!");
        }

        /// <summary>
        /// 조건을 리셋합니다
        /// </summary>
        public void ResetCondition()
        {
            conditionMet = false;
        }
    }
}
