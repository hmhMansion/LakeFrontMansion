using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace LakeFrontMansion.GameManagement
{
    /// <summary>
    /// 게임 전체를 관리하는 싱글톤 매니저
    /// - 각 층의 FloorManager 관리
    /// - 선택된 방을 Door에 적용
    /// - 정답 검증 및 층 이동/초기화
    /// </summary>
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindFirstObjectByType<GameManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("GameManager");
                        _instance = go.AddComponent<GameManager>();
                    }
                }
                return _instance;
            }
        }
        #endregion

        [Header("게임 상태")]
        [Tooltip("현재 층 (1, 2, 3)")]
        public int currentFloor = 3;

        [Header("층별 FloorManager")]
        [Tooltip("1층 FloorManager (나중에 추가)")]
        public FloorManager floor1Manager;

        [Tooltip("2층 FloorManager (나중에 추가)")]
        public FloorManager floor2Manager;

        [Tooltip("3층 FloorManager")]
        public FloorManager floor3Manager;

        [Header("현재 게임 상태")]
        [Tooltip("현재 층에서 선택된 방들 (1번방 제외, 2~4번방)")]
        public List<RoomData> currentSelectedRooms = new List<RoomData>();

        [Tooltip("정답 마스킹 (O=정상, X=이상)")]
        public string answerMask = "";

        private void Awake()
        {
            // 싱글톤 패턴
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            // 씬 로드 이벤트 구독
            SceneManager.sceneLoaded += OnSceneLoaded;

            Debug.Log("GameManager 초기화 완료");
        }

        private void OnDestroy()
        {
            // 이벤트 구독 해제
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void Start()
        {
            InitializeFloorManagers();
            // Start에서는 Door 연결 안 함 (Tutorial 씬이므로)
        }

        /// <summary>
        /// 씬이 로드될 때마다 호출
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Debug.Log($"씬 로드됨: {scene.name}");

            // 복도 씬이 로드되면 Door 연결
            if (scene.name == $"F{currentFloor}_Corridor")
            {
                Debug.Log($"복도 씬 감지! Door 연결 시작");

                // 방 선택
                StartCurrentFloor();

                // Door 연결 (약간의 지연 후 - 씬 오브젝트가 완전히 로드되도록)
                Invoke(nameof(AssignDoorsToCurrentFloor), 0.1f);
            }
        }

        /// <summary>
        /// 현재 층의 Door들에 방 연결
        /// </summary>
        private void AssignDoorsToCurrentFloor()
        {
            FloorManager currentManager = GetCurrentFloorManager();
            if (currentManager == null)
            {
                Debug.LogError($"Floor {currentFloor}의 FloorManager가 없습니다!");
                return;
            }

            // 기준방 씬 이름
            string baseRoomScene = $"F{currentFloor}_GuestRoom01";

            currentManager.AssignRoomsToDoors(baseRoomScene);
        }

        /// <summary>
        /// FloorManager들 초기화 (자식 오브젝트로 생성)
        /// </summary>
        private void InitializeFloorManagers()
        {
            // Floor3Manager 생성 (현재는 3층만)
            if (floor3Manager == null)
            {
                GameObject floor3Obj = new GameObject("Floor3Manager");
                floor3Obj.transform.SetParent(transform);
                floor3Manager = floor3Obj.AddComponent<FloorManager>();
                floor3Manager.floorNumber = 3;

                // 방 풀 초기화
                floor3Manager.roomPool = new List<RoomData>
                {
                    new RoomData("GuestRoom02", "F3_GuestRoom02", false, "정상 방"),
                    new RoomData("GuestRoom03", "F3_GuestRoom03", true, "침대 위치 이상"),
                    new RoomData("GuestRoom04", "F3_GuestRoom04", true, "벽 색깔 이상"),
                    new RoomData("GuestRoom05", "F3_GuestRoom05", false, "정상 방")
                };

                Debug.Log("Floor3Manager 생성 및 초기화 완료");
            }

            // TODO: Floor1Manager, Floor2Manager도 나중에 추가
        }

        /// <summary>
        /// 현재 층 시작 (방 선택 및 정답 생성)
        /// </summary>
        public void StartCurrentFloor()
        {
            Debug.Log($"=== Floor {currentFloor} 시작 ===");

            FloorManager currentManager = GetCurrentFloorManager();
            if (currentManager == null)
            {
                Debug.LogError($"Floor {currentFloor}의 FloorManager가 없습니다!");
                return;
            }

            // 3개 방 랜덤 선택 (2, 3, 4번방)
            currentSelectedRooms = currentManager.SelectRandomRooms(3);

            // 정답 마스킹 생성
            GenerateAnswerMask();

            Debug.Log($"Floor {currentFloor} 준비 완료. 정답: {answerMask}");
        }

        /// <summary>
        /// 정답 마스킹 생성 (1번방은 항상 O, 2~4번방은 선택된 방의 이상 여부)
        /// </summary>
        private void GenerateAnswerMask()
        {
            // 1번방은 항상 정상(O)
            answerMask = "O";

            // 2~4번방 (currentSelectedRooms의 순서대로)
            foreach (var room in currentSelectedRooms)
            {
                answerMask += room.isAbnormal ? "X" : "O";
            }

            Debug.Log($"정답 마스킹 생성: {answerMask}");
        }

        /// <summary>
        /// 플레이어가 제출한 정답 검증
        /// </summary>
        /// <param name="playerAnswer">플레이어 답 (예: "OXOX")</param>
        /// <returns>정답 여부</returns>
        public bool ValidateAnswer(string playerAnswer)
        {
            bool isCorrect = playerAnswer == answerMask;

            if (isCorrect)
            {
                Debug.Log($"정답! 플레이어: {playerAnswer}, 정답: {answerMask}");
            }
            else
            {
                Debug.Log($"오답! 플레이어: {playerAnswer}, 정답: {answerMask}");
            }

            return isCorrect;
        }

        /// <summary>
        /// 다음 층으로 이동
        /// </summary>
        public void MoveToNextFloor()
        {
            if (currentFloor <= 1)
            {
                Debug.Log("게임 클리어! 모든 층을 완료했습니다!");
                // TODO: 게임 클리어 처리
                return;
            }

            currentFloor--;
            Debug.Log($"다음 층으로 이동: Floor {currentFloor}");

            StartCurrentFloor();

            // TODO: 실제 씬 전환 (복도로 이동)
        }

        /// <summary>
        /// 현재 층 초기화 (오답 시)
        /// </summary>
        public void ResetCurrentFloor()
        {
            Debug.Log($"Floor {currentFloor} 초기화 (재도전)");

            FloorManager currentManager = GetCurrentFloorManager();
            if (currentManager != null)
            {
                currentManager.ResetSelection();
            }

            StartCurrentFloor();

            // TODO: 복도로 다시 보내기
        }

        /// <summary>
        /// 현재 층의 FloorManager 반환
        /// </summary>
        private FloorManager GetCurrentFloorManager()
        {
            switch (currentFloor)
            {
                case 1:
                    return floor1Manager;
                case 2:
                    return floor2Manager;
                case 3:
                    return floor3Manager;
                default:
                    Debug.LogError($"유효하지 않은 층: {currentFloor}");
                    return null;
            }
        }

        /// <summary>
        /// 선택된 방을 Door에 적용하는 메서드 (다음 단계에서 구현)
        /// </summary>
        public void ApplyRoomsToDoors()
        {
            // TODO: 다음 단계에서 구현
            Debug.Log("ApplyRoomsToDoors() - 아직 미구현");
        }

        #region Inspector 테스트 메서드
        [ContextMenu("테스트: 정답 제출 (정답)")]
        private void TestCorrectAnswer()
        {
            bool result = ValidateAnswer(answerMask);
            if (result)
            {
                MoveToNextFloor();
            }
        }

        [ContextMenu("테스트: 정답 제출 (오답)")]
        private void TestWrongAnswer()
        {
            // 임의의 오답 생성
            string wrongAnswer = answerMask == "OOOO" ? "OXXX" : "OOOO";
            bool result = ValidateAnswer(wrongAnswer);
            if (!result)
            {
                ResetCurrentFloor();
            }
        }

        [ContextMenu("테스트: 현재 층 재시작")]
        private void TestRestartFloor()
        {
            StartCurrentFloor();
        }
        #endregion
    }
}
