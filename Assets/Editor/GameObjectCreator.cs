using UnityEngine;
using UnityEditor;
using LakeFrontMansion.Player;
using LakeFrontMansion.SceneManagement;
using LakeFrontMansion.Environment;

namespace LakeFrontMansion.Editor
{
    /// <summary>
    /// Unity Editor 메뉴에 게임 오브젝트 생성 기능 추가
    /// GameObject > LakeFrontMansion 메뉴 확인
    /// </summary>
    public class GameObjectCreator
    {
        [MenuItem("GameObject/LakeFrontMansion/Create Player", false, 0)]
        public static void CreatePlayer()
        {
            // 플레이어 GameObject 생성
            GameObject player = new GameObject("Player");
            player.tag = "Player";

            // SpriteRenderer 추가 (임시 사각형)
            SpriteRenderer sr = player.AddComponent<SpriteRenderer>();
            sr.color = Color.green; // 초록색으로 구분
            sr.sprite = CreateSquareSprite(32, Color.white);
            sr.sortingOrder = 10; // 다른 오브젝트보다 위에 렌더링

            // Rigidbody2D 추가
            Rigidbody2D rb = player.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.gravityScale = 0f; // 2D 탑다운이므로 중력 없음
            rb.mass = 1f;
            rb.linearDamping = 0f;
            rb.angularDamping = 0.05f;
            rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            rb.sleepMode = RigidbodySleepMode2D.NeverSleep; // 항상 활성 상태
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; // 부드러운 움직임
            rb.constraints = RigidbodyConstraints2D.FreezeRotation; // 회전 방지

            // BoxCollider2D 추가
            BoxCollider2D collider = player.AddComponent<BoxCollider2D>();
            collider.size = new Vector2(0.8f, 0.8f); // 약간 작게 설정

            // PlayerMovement 스크립트 추가
            PlayerMovement movement = player.AddComponent<PlayerMovement>();
            movement.moveSpeed = 5f;

            // 카메라 따라다니기 스크립트 추가
            player.AddComponent<CameraFollow>();

            // Scene에 배치
            player.transform.position = Vector3.zero;

            // 선택 상태로 변경
            Selection.activeGameObject = player;

            Debug.Log("플레이어가 생성되었습니다! 초록색 사각형이 플레이어입니다.");
        }

        [MenuItem("GameObject/LakeFrontMansion/Create Door (Scene Transition)", false, 1)]
        public static void CreateDoor()
        {
            // 문 GameObject 생성
            GameObject door = new GameObject("Door");
            door.tag = "Interactable";

            // SpriteRenderer 추가 (임시 사각형)
            SpriteRenderer sr = door.AddComponent<SpriteRenderer>();
            sr.color = Color.yellow; // 노란색으로 구분
            sr.sprite = CreateSquareSprite(64, Color.white);
            sr.sortingOrder = 5;

            // BoxCollider2D 추가 (Trigger)
            BoxCollider2D collider = door.AddComponent<BoxCollider2D>();
            collider.isTrigger = true;
            collider.size = new Vector2(1.2f, 1.5f); // 문 크기

            // SceneTransitionTrigger 스크립트 추가
            SceneTransitionTrigger trigger = door.AddComponent<SceneTransitionTrigger>();
            trigger.targetSceneName = ""; // Inspector에서 설정 필요
            trigger.useClick = true;
            trigger.useTrigger = false;
            trigger.characterTag = "Character";

            // 상호작용 UI 생성
            GameObject uiObject = CreateInteractionUI(door.transform);
            trigger.interactionUI = uiObject;

            // Scene에 배치
            door.transform.position = new Vector3(3, 0, 0);

            // 선택 상태로 변경
            Selection.activeGameObject = door;

            Debug.Log("문이 생성되었습니다! 노란색 사각형이 문입니다. Inspector에서 Target Scene Name을 설정하세요.");
        }

        [MenuItem("GameObject/LakeFrontMansion/Create Room", false, 2)]
        public static void CreateRoomBackground()
        {
            // 방 오브젝트 생성
            GameObject room = new GameObject("Room");

            // SpriteRenderer 추가
            SpriteRenderer sr = room.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.9f, 0.9f, 0.9f);
            sr.sprite = CreateSquareSprite(512, Color.white);
            sr.sortingOrder = -10;

            // Rigidbody2D 추가 (Static)
            Rigidbody2D rb = room.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            // EdgeCollider2D 추가 (테두리만 막음, 내부는 비어있음)
            EdgeCollider2D edge = room.AddComponent<EdgeCollider2D>();

            // 사각형 테두리 경로 (닫힌 경로)
            float halfSize = 2.56f; // 512픽셀 / 100 ppu / 2
            Vector2[] points = new Vector2[]
            {
                new Vector2(-halfSize, -halfSize),  // 왼쪽 아래
                new Vector2(halfSize, -halfSize),   // 오른쪽 아래
                new Vector2(halfSize, halfSize),    // 오른쪽 위
                new Vector2(-halfSize, halfSize),   // 왼쪽 위
                new Vector2(-halfSize, -halfSize)   // 왼쪽 아래 (닫힘)
            };
            edge.points = points;

            // RoomBoundary 스크립트
            room.AddComponent<RoomBoundary>();

            room.transform.position = Vector3.zero;
            Selection.activeGameObject = room;

            Debug.Log("방이 생성되었습니다!\n" +
                      "1. Sprite를 방 배경 이미지로 교체하세요.\n" +
                      "2. EdgeCollider2D > Edit Collider로 경계를 배경에 맞게 조정하세요.\n" +
                      "3. 플레이어가 테두리를 넘지 못하고 방 안에서만 움직입니다.");
        }


        [MenuItem("GameObject/LakeFrontMansion/Create Furniture", false, 3)]
        public static void CreateFurniture()
        {
            // 가구 GameObject 생성
            GameObject furniture = new GameObject("Furniture");

            // SpriteRenderer 추가 (임시 사각형)
            SpriteRenderer sr = furniture.AddComponent<SpriteRenderer>();
            sr.color = new Color(0.6f, 0.4f, 0.2f); // 갈색
            sr.sprite = CreateSquareSprite(64, Color.white);
            sr.sortingOrder = 2;

            // Rigidbody2D 추가 (Static - 움직이지 않는 가구)
            Rigidbody2D rb = furniture.AddComponent<Rigidbody2D>();
            rb.bodyType = RigidbodyType2D.Static;

            // BoxCollider2D 추가 (Is Trigger = false, 물리적으로 막음)
            BoxCollider2D collider = furniture.AddComponent<BoxCollider2D>();
            collider.isTrigger = false; // 중요: 관통 방지

            // Furniture 스크립트 추가
            Furniture furnitureScript = furniture.AddComponent<Furniture>();
            furnitureScript.furnitureType = "Furniture";
            furnitureScript.isInteractable = false;

            // Scene에 배치
            furniture.transform.position = new Vector3(2, 2, 0);

            // 선택 상태로 변경
            Selection.activeGameObject = furniture;

            Debug.Log("가구가 생성되었습니다! 갈색 사각형이 가구입니다. 플레이어가 관통하지 못합니다.");
        }

        [MenuItem("GameObject/LakeFrontMansion/Create Build Settings Helper", false, 10)]
        public static void OpenBuildSettingsHelper()
        {
            EditorWindow.GetWindow<BuildSettingsHelperWindow>("Build Settings Helper");
        }

        /// <summary>
        /// 간단한 사각형 Sprite 생성
        /// </summary>
        private static Sprite CreateSquareSprite(int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point; // 픽셀 아트용
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        /// <summary>
        /// 간단한 원형 Sprite 생성
        /// </summary>
        private static Sprite CreateCircleSprite(int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            int radius = size / 2;
            Vector2 center = new Vector2(radius, radius);

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), center);
                    pixels[y * size + x] = distance <= radius ? color : Color.clear;
                }
            }

            texture.SetPixels(pixels);
            texture.filterMode = FilterMode.Point;
            texture.Apply();

            return Sprite.Create(texture, new Rect(0, 0, size, size), new Vector2(0.5f, 0.5f), size);
        }

        /// <summary>
        /// 상호작용 UI 생성 (E키를 누르세요)
        /// </summary>
        private static GameObject CreateInteractionUI(Transform parent)
        {
            // Canvas가 있는지 확인
            Canvas canvas = Object.FindFirstObjectByType<Canvas>();
            if (canvas == null)
            {
                // Canvas 생성
                GameObject canvasObj = new GameObject("Canvas");
                canvas = canvasObj.AddComponent<Canvas>();
                canvas.renderMode = RenderMode.ScreenSpaceOverlay;

                // Canvas Scale 명시적 설정 (0,0,0이면 UI가 안 보이는 문제 방지)
                canvasObj.transform.localScale = Vector3.one;

                // Canvas Scaler 설정 (다양한 해상도 대응)
                UnityEngine.UI.CanvasScaler scaler = canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
                scaler.uiScaleMode = UnityEngine.UI.CanvasScaler.ScaleMode.ScaleWithScreenSize;
                scaler.referenceResolution = new Vector2(1920, 1080);

                canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            // UI 텍스트 생성
            GameObject uiObject = new GameObject("InteractionUI");
            uiObject.transform.SetParent(parent);
            uiObject.transform.localPosition = new Vector3(0, 1, 0);

            // TextMesh 추가 (World Space)
            TextMesh textMesh = uiObject.AddComponent<TextMesh>();
            textMesh.text = "[E] 입장";
            textMesh.fontSize = 20;
            textMesh.color = Color.white;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.alignment = TextAlignment.Center;

            // 기본적으로 비활성화
            uiObject.SetActive(false);

            return uiObject;
        }
    }

    /// <summary>
    /// Build Settings에 Scene을 자동 추가하는 Helper Window
    /// </summary>
    public class BuildSettingsHelperWindow : EditorWindow
    {
        private Vector2 scrollPosition;

        private void OnGUI()
        {
            GUILayout.Label("Build Settings Helper", EditorStyles.boldLabel);
            GUILayout.Space(10);

            EditorGUILayout.HelpBox(
                "이 버튼을 누르면 Assets/Scenes 폴더의 모든 Scene을 Build Settings에 자동으로 추가합니다.",
                MessageType.Info
            );

            GUILayout.Space(10);

            if (GUILayout.Button("모든 Scene을 Build Settings에 추가", GUILayout.Height(40)))
            {
                AddAllScenesToBuildSettings();
            }

            GUILayout.Space(20);
            GUILayout.Label("현재 Build Settings:", EditorStyles.boldLabel);

            scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            if (scenes.Length == 0)
            {
                EditorGUILayout.HelpBox("Build Settings에 Scene이 없습니다.", MessageType.Warning);
            }
            else
            {
                for (int i = 0; i < scenes.Length; i++)
                {
                    EditorGUILayout.LabelField($"{i}: {scenes[i].path}");
                }
            }

            EditorGUILayout.EndScrollView();
        }

        private void AddAllScenesToBuildSettings()
        {
            // Assets/Scenes 폴더의 모든 .unity 파일 찾기
            string[] sceneGUIDs = AssetDatabase.FindAssets("t:Scene", new[] { "Assets/Scenes" });

            EditorBuildSettingsScene[] buildScenes = new EditorBuildSettingsScene[sceneGUIDs.Length];

            for (int i = 0; i < sceneGUIDs.Length; i++)
            {
                string scenePath = AssetDatabase.GUIDToAssetPath(sceneGUIDs[i]);
                buildScenes[i] = new EditorBuildSettingsScene(scenePath, true);
            }

            EditorBuildSettings.scenes = buildScenes;

            Debug.Log($"{buildScenes.Length}개의 Scene이 Build Settings에 추가되었습니다!");
            Repaint();
        }
    }
}
