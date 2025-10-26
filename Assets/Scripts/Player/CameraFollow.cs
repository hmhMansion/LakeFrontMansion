using UnityEngine;
using LakeFrontMansion.Environment;

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

        [Header("경계 제한 설정")]
        [Tooltip("방 경계 제한 활성화")]
        public bool enableBoundary = true;

        [Tooltip("경계에서 추가 여유 공간 (양수 = 경계 넘어서도 조금 더 갈 수 있음)")]
        public float boundaryBuffer = 0.5f;

        private RoomBoundary currentRoom;

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

            // 현재 씬에서 RoomBoundary 찾기 (없으면 경계 제한 없이 동작)
            currentRoom = FindObjectOfType<RoomBoundary>();

            if (currentRoom == null && enableBoundary)
            {
                Debug.LogWarning("RoomBoundary를 찾을 수 없습니다. 카메라 경계 제한이 비활성화됩니다.");
            }
        }

        private void LateUpdate()
        {
            if (targetCamera == null) return;

            // 목표 위치 계산
            Vector3 desiredPosition = transform.position + offset;

            // 방 경계가 있고 제한이 활성화되어 있으면 위치 제한
            if (enableBoundary && currentRoom != null)
            {
                desiredPosition = ClampToBoundary(desiredPosition);
            }

            // 부드러운 이동
            Vector3 smoothedPosition = Vector3.Lerp(
                targetCamera.transform.position,
                desiredPosition,
                smoothSpeed * Time.deltaTime
            );

            // 카메라 위치 업데이트
            targetCamera.transform.position = smoothedPosition;
        }

        /// <summary>
        /// 카메라 위치를 방 경계 내로 제한합니다
        /// </summary>
        private Vector3 ClampToBoundary(Vector3 position)
        {
            Bounds roomBounds = currentRoom.GetBounds();

            // 카메라가 보는 영역의 크기 계산
            float cameraHeight = targetCamera.orthographicSize * 2f;
            float cameraWidth = cameraHeight * targetCamera.aspect;

            // 카메라 이동 가능한 범위 계산 (방 경계 - 카메라 뷰포트의 절반 + 버퍼)
            float minX = roomBounds.min.x + (cameraWidth / 2f) - boundaryBuffer;
            float maxX = roomBounds.max.x - (cameraWidth / 2f) + boundaryBuffer;
            float minY = roomBounds.min.y + (cameraHeight / 2f) - boundaryBuffer;
            float maxY = roomBounds.max.y - (cameraHeight / 2f) + boundaryBuffer;

            // 방이 카메라보다 작은 경우 중앙으로 고정
            if (minX > maxX)
            {
                float centerX = (roomBounds.min.x + roomBounds.max.x) / 2f;
                minX = maxX = centerX;
            }

            if (minY > maxY)
            {
                float centerY = (roomBounds.min.y + roomBounds.max.y) / 2f;
                minY = maxY = centerY;
            }

            // 위치 제한
            Vector3 clampedPosition = position;
            clampedPosition.x = Mathf.Clamp(position.x, minX, maxX);
            clampedPosition.y = Mathf.Clamp(position.y, minY, maxY);

            return clampedPosition;
        }
    }
}
