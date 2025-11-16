using UnityEngine;

namespace LakeFrontMansion.GameManagement
{
    /// <summary>
    /// 방 하나의 데이터를 표현하는 클래스
    /// </summary>
    [System.Serializable]
    public class RoomData
    {
        [Header("방 기본 정보")]
        [Tooltip("방 고유 ID (예: GuestRoom01)")]
        public string roomId;

        [Tooltip("씬 이름 (예: F3_GuestRoom01)")]
        public string sceneName;

        [Header("이상현상 설정")]
        [Tooltip("이 방이 이상현상이 있는 방인가?")]
        public bool isAbnormal;

        [Tooltip("이상현상 설명 (디버깅/개발용)")]
        public string abnormalDescription;

        public RoomData(string id, string scene, bool abnormal = false, string description = "")
        {
            roomId = id;
            sceneName = scene;
            isAbnormal = abnormal;
            abnormalDescription = description;
        }

        /// <summary>
        /// 디버그용 문자열 출력
        /// </summary>
        public override string ToString()
        {
            string status = isAbnormal ? "이상" : "정상";
            return $"[{roomId}] {sceneName} - {status}";
        }
    }
}
