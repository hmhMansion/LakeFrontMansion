using UnityEngine;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// Scene 전환 조건을 정의하는 인터페이스
    /// 이 인터페이스를 구현하여 다양한 조건을 만들 수 있습니다.
    /// </summary>
    public interface ISceneTransitionCondition
    {
        /// <summary>
        /// 조건이 만족되었는지 확인
        /// </summary>
        /// <returns>조건 만족 시 true, 아니면 false</returns>
        bool IsSatisfied();

        /// <summary>
        /// 조건 설명 (UI에 표시하거나 디버깅용)
        /// </summary>
        /// <returns>조건에 대한 설명</returns>
        string GetDescription();
    }
}
