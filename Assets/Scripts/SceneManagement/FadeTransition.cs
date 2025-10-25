using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

namespace LakeFrontMansion.SceneManagement
{
    /// <summary>
    /// Scene 전환 시 페이드 효과를 제공하는 클래스
    /// Canvas > Image (검은색 전체화면)에 붙여서 사용
    /// </summary>
    [RequireComponent(typeof(Image))]
    public class FadeTransition : MonoBehaviour
    {
        private static FadeTransition instance;
        public static FadeTransition Instance => instance;

        [Header("페이드 설정")]
        [Tooltip("페이드 인/아웃 속도 (초)")]
        public float fadeDuration = 1f;

        private Image fadeImage;
        private bool isFading = false;

        private void Awake()
        {
            // 싱글톤 설정
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;
            DontDestroyOnLoad(gameObject);

            // Image 컴포넌트 가져오기
            fadeImage = GetComponent<Image>();

            // 시작 시 투명하게
            Color color = fadeImage.color;
            color.a = 0f;
            fadeImage.color = color;
        }

        /// <summary>
        /// 페이드 효과와 함께 Scene 로드
        /// </summary>
        public void LoadSceneWithFade(string sceneName)
        {
            if (!isFading)
            {
                StartCoroutine(FadeAndLoadScene(sceneName));
            }
        }

        /// <summary>
        /// 페이드 아웃 -> Scene 로드 -> 페이드 인
        /// </summary>
        private IEnumerator FadeAndLoadScene(string sceneName)
        {
            isFading = true;

            // 페이드 아웃 (화면 어둡게)
            yield return StartCoroutine(FadeOut());

            // Scene 로드
            SceneManager.LoadScene(sceneName);

            // 페이드 인 (화면 밝게)
            yield return StartCoroutine(FadeIn());

            isFading = false;
        }

        /// <summary>
        /// 화면을 어둡게 (알파 0 -> 1)
        /// </summary>
        public IEnumerator FadeOut()
        {
            float elapsedTime = 0f;
            Color color = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            color.a = 1f;
            fadeImage.color = color;
        }

        /// <summary>
        /// 화면을 밝게 (알파 1 -> 0)
        /// </summary>
        public IEnumerator FadeIn()
        {
            float elapsedTime = 0f;
            Color color = fadeImage.color;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                color.a = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
                fadeImage.color = color;
                yield return null;
            }

            color.a = 0f;
            fadeImage.color = color;
        }
    }
}
