using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────
//  ChairSit.cs
//  Attach to EACH chair GameObject.
//  Add a Box Collider (Is Trigger ON) to the chair.
//
//  First sit  → fades black, time jumps to 10 AM, fades back
//  Second sit → fades black, shows end screen "Thanks for playing"
// ─────────────────────────────────────────────
public class ChairSit : MonoBehaviour
{
    [Header("Sit Point")]
    [Tooltip("Empty child Transform — player teleports here when sitting.")]
    public Transform sitPoint;

    [Header("Fade")]
    public Image    fadeOverlay;      // Fullscreen black Image on Canvas
    public float    fadeDuration = 1f;

    [Header("First Sit — Time Jump")]
    public int jumpToHour   = 10;
    public int jumpToMinute = 0;

    [Header("Second Sit — End Screen")]
    public GameObject endScreenPanel;  // "Thanks for playing" panel

    [Header("Prompt")]
    public GameObject      seatPrompt;
    public TextMeshProUGUI seatPromptText;

    // ── Runtime ───────────────────────────────
    private static int  _sitCount     = 0;   // shared across all chairs
    private bool        _playerNear;
    private bool        _isSitting;
    private GameObject  _player;

    void Start()
    {
        if (seatPrompt     != null) seatPrompt.SetActive(false);
        if (endScreenPanel != null) endScreenPanel.SetActive(false);
        if (fadeOverlay    != null)
        {
            fadeOverlay.color = Color.clear;
            fadeOverlay.gameObject.SetActive(false);
        }
        if (seatPromptText != null)
            seatPromptText.text = "Press  <b>F</b>  to Sit";
    }

    void Update()
    {
        if (_playerNear && !_isSitting && Input.GetKeyDown(KeyCode.F))
            StartCoroutine(SitRoutine());

        // Press F again to stand up
        if (_isSitting && Input.GetKeyDown(KeyCode.F))
            StandUp();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNear = true;
        _player     = other.gameObject;
        if (seatPrompt != null) seatPrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNear = false;
        if (seatPrompt != null) seatPrompt.SetActive(false);
    }

    // ─────────────────────────────────────────
    IEnumerator SitRoutine()
    {
        _isSitting = true;
        if (seatPrompt != null) seatPrompt.SetActive(false);

        // Disable player movement while sitting
        SetPlayerMovement(false);

        // Teleport player to sit point
        if (_player != null && sitPoint != null)
        {
            _player.transform.position = sitPoint.position;
            _player.transform.rotation = sitPoint.rotation;
        }

        // Fade to black
        yield return StartCoroutine(Fade(0f, 1f));

        _sitCount++;

        if (_sitCount == 1)
        {
            // First sit — jump time to 10 AM
            if (GameClock.Instance != null)
                GameClock.Instance.SetTime(jumpToHour, jumpToMinute);

            yield return new WaitForSeconds(1.5f);

            // Fade back in
            yield return StartCoroutine(Fade(1f, 0f));

            // Re-enable movement so player can stand and walk
            SetPlayerMovement(true);
            _isSitting = false;

            if (seatPrompt != null) seatPrompt.SetActive(true);
            if (seatPromptText != null) seatPromptText.text = "Press  <b>F</b>  to Stand";
        }
        else
        {
            // Second sit — show end screen
            if (endScreenPanel != null)
                endScreenPanel.SetActive(true);

            // Stay faded — game ends here
            // Player stays sitting, movement stays off
        }
    }

    void StandUp()
    {
        _isSitting = false;
        SetPlayerMovement(true);
        if (seatPrompt     != null) seatPrompt.SetActive(false);
        if (seatPromptText != null) seatPromptText.text = "Press  <b>F</b>  to Sit";
    }

    IEnumerator Fade(float from, float to)
    {
        if (fadeOverlay == null) yield break;
        fadeOverlay.gameObject.SetActive(true);

        float t = 0f;
        Color c = fadeOverlay.color;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            c.a = Mathf.Lerp(from, to, t / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }
        c.a = to;
        fadeOverlay.color = c;

        if (to == 0f) fadeOverlay.gameObject.SetActive(false);
    }

    void SetPlayerMovement(bool enabled)
    {
        // Works with Unity's StarterAssets ThirdPersonController
        if (_player == null) return;

        var tpc = _player.GetComponentInParent<UnityEngine.InputSystem.PlayerInput>();
        if (tpc != null) tpc.enabled = enabled;

        // Also try CharacterController
        var cc = _player.GetComponentInParent<CharacterController>();
        if (cc != null) cc.enabled = enabled;
    }
}
