using UnityEngine;

namespace LakeFrontMansion.Player
{
    /// <summary>
    /// 카메라가 플레이어를 부드럽게 따라다니도록 하는 스크립트
    /// 플레이어 오브젝트에 추가
    /// </summary>
    public class CameraFollow : MonoBehaviour
    {
        [Header("카메라 설정")]
        [Tooltip("따라갈 대상 (자동으로 Main Camera 찾음)")]
        public Camera targetCamera;

        [Tooltip("카메라 따라가는 속도 (높을수록 빠름)")]
        public float smoothSpeed = 5f;

        [Tooltip("카메라 오프셋 (Z는 -10 권장)")]
        public Vector3 offset = new Vector3(0, 0, -10);

        private void Start()
        {
            // Main Camera 자동 찾기
            if (targetCamera == null)
            {
                targetCamera = Camera.main;
            }

            if (targetCamera == null)
            {
                Debug.LogError("Main Camera를 찾을 수 없습니다!");
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null) return;

            // 목표 위치 계산
            Vector3 desiredPosition = transform.position + offset;

            // 부드러운 이동
            Vector3 smoothedPosition = Vector3.Lerp(
                targetCamera.transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

            // 카메라 위치 업데이트
            targetCamera.transform.position = smoothedPosition;
        }
    }
}
