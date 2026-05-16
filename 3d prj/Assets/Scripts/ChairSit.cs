using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────
//  ChairSit.cs — Fixed version
//  1. Press H near chair → player sinks into chair
//  2. Screen fades to black slowly
//  3. Time jumps to 10 AM
//  4. Screen fades back in slowly
//  5. Player can now press F to stand up and walk
// ─────────────────────────────────────────────
public class ChairSit : MonoBehaviour
{
    [Header("Sit Point")]
    public Transform sitPoint;

    [Header("Fade")]
    public Image fadeOverlay;
    public float fadeDuration = 3f;    // how slow the fade is

    [Header("Time Jump")]
    public int jumpToHour   = 10;
    public int jumpToMinute = 0;

    [Header("Prompt")]
    public GameObject      seatPrompt;
    public TextMeshProUGUI seatPromptText;

    // ── Runtime ───────────────────────────────
    private bool       _playerNear;
    private bool       _isSitting;
    private bool       _hasSat;          // only do time jump once
    private GameObject _player;

    // ─────────────────────────────────────────
    void Start()
    {
        if (seatPrompt != null)  seatPrompt.SetActive(false);

        // Make sure fade overlay is fullscreen and hidden
        if (fadeOverlay != null)
        {
            fadeOverlay.color = Color.clear;
            fadeOverlay.gameObject.SetActive(false);

            // Force it to be fullscreen
            RectTransform rt = fadeOverlay.GetComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        if (seatPromptText != null)
            seatPromptText.text = "Press  H  to Sit";
    }

    // ─────────────────────────────────────────
    void Update()
    {
        if (_playerNear && !_isSitting && Input.GetKeyDown(KeyCode.H))
            StartCoroutine(SitRoutine());

        if (_isSitting && Input.GetKeyDown(KeyCode.H))
            StandUp();
    }

    // ─────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNear = true;
        _player = other.gameObject;
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

        // Lock player
        SetPlayerMovement(false);

        // Sink player into chair
        if (_player != null && sitPoint != null)
        {
            _player.transform.position = sitPoint.position + new Vector3(0, -0.8f, 0);
            _player.transform.rotation = sitPoint.rotation;
        }

        // Only do time jump ONCE
        if (!_hasSat)
        {
            _hasSat = true;

            // ── Fade to black slowly ──────────
            yield return StartCoroutine(Fade(0f, 1f));

            // Jump clock
            if (GameClock.Instance != null)
                GameClock.Instance.SetTime(jumpToHour, jumpToMinute);

            // Hold black for a moment
            yield return new WaitForSeconds(0.8f);

            // ── Fade back in slowly ───────────
            yield return StartCoroutine(Fade(1f, 0f));
        }

        // Show stand up prompt
        if (seatPrompt     != null) seatPrompt.SetActive(true);
        if (seatPromptText != null) seatPromptText.text = "Press  H  to Stand Up";
    }

    // ─────────────────────────────────────────
    void StandUp()
    {
        _isSitting = false;
        SetPlayerMovement(true);

        if (_player != null && sitPoint != null)
            _player.transform.position = sitPoint.position + new Vector3(0, 0.5f, 1f);

        if (seatPrompt     != null) seatPrompt.SetActive(false);
        if (seatPromptText != null) seatPromptText.text = "Press  H  to Sit";
    }

    // ─────────────────────────────────────────
    IEnumerator Fade(float from, float to)
    {
        if (fadeOverlay == null) yield break;

        fadeOverlay.gameObject.SetActive(true);
        float t = 0f;
        Color c = fadeOverlay.color;

        while (t < fadeDuration)
        {
            t   += Time.deltaTime;
            c.a  = Mathf.Lerp(from, to, t / fadeDuration);
            fadeOverlay.color = c;
            yield return null;
        }

        c.a = to;
        fadeOverlay.color = c;

        if (to == 0f)
            fadeOverlay.gameObject.SetActive(false);
    }

    // ─────────────────────────────────────────
    void SetPlayerMovement(bool enabled)
    {
        if (_player == null) return;

        var input = _player.GetComponentInParent<UnityEngine.InputSystem.PlayerInput>();
        if (input != null) input.enabled = enabled;

        var cc = _player.GetComponentInParent<CharacterController>();
        if (cc != null) cc.enabled = enabled;
    }
}
