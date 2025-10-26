using UnityEngine;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// 일정 시간이 경과했는지 체크하는 조건
    /// 씬에 들어온 후 일정 시간이 지나야 나갈 수 있게 할 때 사용
    /// </summary>
    public class TimeCondition : MonoBehaviour, ISceneTransitionCondition
    {
        [Header("시간 설정")]
        [Tooltip("필요한 대기 시간 (초)")]
        public float requiredTime = 10f;

        [Tooltip("시작 시 자동으로 타이머 시작")]
        public bool startOnAwake = true;

        [Header("진행 상황")]
        [Tooltip("현재 경과 시간 (읽기 전용)")]
        [SerializeField]
        private float elapsedTime = 0f;

        private bool isRunning = false;

        private void Start()
        {
            if (startOnAwake)
            {
                StartTimer();
            }
        }

        private void Update()
        {
            if (isRunning)
            {
                elapsedTime += Time.deltaTime;
            }
        }

        public bool IsSatisfied()
        {
            return elapsedTime >= requiredTime;
        }

        public string GetDescription()
        {
            float remaining = Mathf.Max(0, requiredTime - elapsedTime);
            return $"대기 시간: {remaining:F1}초 남음";
        }

        /// <summary>
        /// 타이머 시작
        /// </summary>
        public void StartTimer()
        {
            isRunning = true;
            Debug.Log($"타이머 시작: {requiredTime}초");
        }

        /// <summary>
        /// 타이머 일시정지
        /// </summary>
        public void PauseTimer()
        {
            isRunning = false;
        }

        /// <summary>
        /// 타이머 리셋
        /// </summary>
        public void ResetTimer()
        {
            elapsedTime = 0f;
            isRunning = false;
        }

        /// <summary>
        /// 현재 진행률 반환
        /// </summary>
        /// <returns>0.0 ~ 1.0 사이의 값</returns>
        public float GetProgress()
        {
            if (requiredTime == 0) return 1f;
            return Mathf.Clamp01(elapsedTime / requiredTime);
        }
    }
}
