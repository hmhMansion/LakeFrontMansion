using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace LakeFrontMansion.GameManagement
{
    /// <summary>
    /// 각 층의 방 풀을 관리하고 랜덤 선택을 담당하는 매니저
    /// 각 층마다 하나씩 인스턴스 생성됨 (Floor1Manager, Floor2Manager, Floor3Manager)
    /// </summary>
    public class FloorManager : MonoBehaviour
    {
        [Header("층 정보")]
        [Tooltip("층 번호 (1, 2, 3)")]
        public int floorNumber = 3;

        [Header("방 풀 설정")]
        [Tooltip("이 층의 방 풀 (랜덤 선택 대상)")]
        public List<RoomData> roomPool = new List<RoomData>();

        [Header("디버그")]
        [Tooltip("마지막으로 선택된 방들")]
        public List<RoomData> lastSelectedRooms = new List<RoomData>();

        /// <summary>
        /// 방 풀에서 랜덤하게 count개 선택 (중복 없이)
        /// </summary>
        /// <param name="count">선택할 방 개수</param>
        /// <returns>선택된 방 리스트</returns>
        public List<RoomData> SelectRandomRooms(int count)
        {
            if (count > roomPool.Count)
            {
                Debug.LogError($"Floor {floorNumber}: 선택 개수({count})가 방 풀 크기({roomPool.Count})보다 큽니다!");
                return new List<RoomData>();
            }

            // 방 풀을 섞고 앞에서 count개 선택
            List<RoomData> shuffled = roomPool.OrderBy(x => Random.value).ToList();
            List<RoomData> selected = shuffled.Take(count).ToList();

            // 디버그용 저장
            lastSelectedRooms = selected;

            Debug.Log($"Floor {floorNumber}: {count}개 방 선택 완료");
            foreach (var room in selected)
            {
                Debug.Log($"  - {room}");
            }

            return selected;
        }

        /// <summary>
        /// 특정 인덱스의 방이 이상현상인지 체크
        /// </summary>
        public bool IsRoomAbnormal(int index)
        {
            if (index < 0 || index >= lastSelectedRooms.Count)
            {
                Debug.LogError($"Floor {floorNumber}: 유효하지 않은 인덱스 {index}");
                return false;
            }

            return lastSelectedRooms[index].isAbnormal;
        }

        /// <summary>
        /// 선택 초기화 (재선택 준비)
        /// </summary>
        public void ResetSelection()
        {
            lastSelectedRooms.Clear();
            Debug.Log($"Floor {floorNumber}: 방 선택 초기화");
        }

        /// <summary>
        /// 복도의 Door들에 선택된 방을 연결
        /// Door 이름 규칙: Door1, Door2, Door3, Door4
        /// </summary>
        /// <param name="baseRoomSceneName">1번 Door의 기준방 씬 이름 (예: F3_GuestRoom01)</param>
        public void AssignRoomsToDoors(string baseRoomSceneName)
        {
            if (lastSelectedRooms.Count == 0)
            {
                Debug.LogError($"Floor {floorNumber}: 선택된 방이 없습니다! SelectRandomRooms를 먼저 호출하세요.");
                return;
            }

            // 현재 씬의 모든 SceneTransitionTrigger 찾기
            LakeFrontMansion.SceneManagement.SceneTransitionTrigger[] allTriggers =
                FindObjectsByType<LakeFrontMansion.SceneManagement.SceneTransitionTrigger>(FindObjectsSortMode.None);

            Debug.Log($"Floor {floorNumber}: 총 {allTriggers.Length}개의 Door 발견");

            // Door1: 기준방 (항상 정상)
            AssignDoor(allTriggers, "Door1", baseRoomSceneName);

            // Door2, 3, 4: 선택된 방들
            for (int i = 0; i < lastSelectedRooms.Count && i < 3; i++)
            {
                string doorName = $"Door{i + 2}";
                string sceneName = lastSelectedRooms[i].sceneName;
                AssignDoor(allTriggers, doorName, sceneName);
            }

            Debug.Log($"Floor {floorNumber}: Door 연결 완료!");
        }

        /// <summary>
        /// 특정 이름의 Door를 찾아서 targetSceneName 설정
        /// </summary>
        private void AssignDoor(LakeFrontMansion.SceneManagement.SceneTransitionTrigger[] triggers, string doorName, string sceneName)
        {
            foreach (var trigger in triggers)
            {
                if (trigger.gameObject.name == doorName)
                {
                    trigger.targetSceneName = sceneName;
                    Debug.Log($"  - {doorName} → {sceneName}");
                    return;
                }
            }

            Debug.LogWarning($"Floor {floorNumber}: {doorName}을(를) 찾을 수 없습니다!");
        }

        /// <summary>
        /// Inspector에서 테스트용 버튼
        /// </summary>
        [ContextMenu("테스트: 3개 방 선택")]
        private void TestSelectRooms()
        {
            SelectRandomRooms(3);
        }

        /// <summary>
        /// Inspector에서 방 풀 초기화 버튼 (Floor3 전용)
        /// </summary>
        [ContextMenu("Floor3 방 풀 초기화")]
        private void InitializeFloor3RoomPool()
        {
            if (floorNumber != 3)
            {
                Debug.LogWarning("이 기능은 Floor3 전용입니다.");
                return;
            }

            roomPool.Clear();

            // F3_GuestRoom02~05를 방 풀에 추가 (F3_GuestRoom01은 기준방이므로 제외)
            roomPool.Add(new RoomData("GuestRoom02", "F3_GuestRoom02", false, "정상 방"));
            roomPool.Add(new RoomData("GuestRoom03", "F3_GuestRoom03", true, "침대 위치 이상"));
            roomPool.Add(new RoomData("GuestRoom04", "F3_GuestRoom04", true, "벽 색깔 이상"));
            roomPool.Add(new RoomData("GuestRoom05", "F3_GuestRoom05", false, "정상 방"));

            Debug.Log($"Floor3 방 풀 초기화 완료: {roomPool.Count}개");
        }
    }
}
