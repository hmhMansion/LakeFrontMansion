using UnityEngine;
using UnityEngine.SceneManagement;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// Scene 로딩을 담당하는 매니저 클래스
    /// </summary>
    public class SceneLoader : MonoBehaviour
    {
        private static SceneLoader instance;

        public static SceneLoader Instance
        {
            get
            {
                if (instance == null)
                {
                    GameObject go = new GameObject("SceneLoader");
                    instance = go.AddComponent<SceneLoader>();
                    DontDestroyOnLoad(go);
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Scene 이름으로 로드
        /// </summary>
        public void LoadScene(string sceneName)
        {
            Debug.Log($"Loading scene: {sceneName}");
            SceneManager.LoadScene(sceneName);
        }

        /// <summary>
        /// Scene 인덱스로 로드
        /// </summary>
        public void LoadScene(int sceneIndex)
        {
            Debug.Log($"Loading scene index: {sceneIndex}");
            SceneManager.LoadScene(sceneIndex);
        }

        /// <summary>
        /// 비동기 로딩 (로딩 바 표시 가능)
        /// </summary>
        public void LoadSceneAsync(string sceneName)
        {
            StartCoroutine(LoadSceneAsyncCoroutine(sceneName));
        }

        private System.Collections.IEnumerator LoadSceneAsyncCoroutine(string sceneName)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName);

            while (!asyncLoad.isDone)
            {
                float progress = Mathf.Clamp01(asyncLoad.progress / 0.9f);
                Debug.Log($"Loading progress: {progress * 100}%");
                yield return null;
            }
        }

        /// <summary>
        /// 게임 종료
        /// </summary>
        public void QuitGame()
        {
            Debug.Log("Quitting game...");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
