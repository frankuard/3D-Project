using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class MainMenu : MonoBehaviour
{
    [Header("UI Elements")]
    public CanvasGroup    menuGroup;
    public Image          background;

    [Header("Buttons")]
    public Button playButton;
    public Button quitButton;

    [Header("Settings")]
    public float fadeInDuration  = 1.2f;
    public float fadeOutDuration = 0.6f;
    public int   gameSceneIndex  = 1;

    void Start()
    {
        // Fade in the whole menu on start
        if (menuGroup != null)
        {
            menuGroup.alpha = 0f;
            StartCoroutine(FadeIn());
        }

        // Wire up buttons
        if (playButton != null)
            playButton.onClick.AddListener(OnPlay);
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuit);
    }

    void OnPlay()
    {
        StartCoroutine(FadeOutAndLoad());
    }

    void OnQuit()
    {
        StartCoroutine(FadeOutAndQuit());
    }

    IEnumerator FadeIn()
    {
        float t = 0f;
        while (t < fadeInDuration)
        {
            t            += Time.deltaTime;
            menuGroup.alpha = Mathf.Lerp(0f, 1f, t / fadeInDuration);
            yield return null;
        }
        menuGroup.alpha = 1f;
    }

    IEnumerator FadeOutAndLoad()
    {
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t            += Time.deltaTime;
            menuGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        SceneManager.LoadScene(gameSceneIndex);
    }

    IEnumerator FadeOutAndQuit()
    {
        float t = 0f;
        while (t < fadeOutDuration)
        {
            t            += Time.deltaTime;
            menuGroup.alpha = Mathf.Lerp(1f, 0f, t / fadeOutDuration);
            yield return null;
        }
        Application.Quit();
        // This line only runs in Editor
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}