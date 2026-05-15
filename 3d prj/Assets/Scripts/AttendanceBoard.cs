using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────
//  AttendanceBoard.cs
//  Attach to your wall attendance board object.
//  Add Box Collider (Is Trigger ON).
// ─────────────────────────────────────────────
public class AttendanceBoard : MonoBehaviour
{
    [Header("Prompt")]
    public GameObject      promptPanel;
    public TextMeshProUGUI promptText;

    [Header("Attendance UI")]
    public GameObject      attendancePanel;   // shown after marking
    public TextMeshProUGUI attendanceText;
    public float           displayDuration = 3f;
    public float           fadeDuration    = 0.4f;

    private bool        _playerNear;
    private bool        _marked;
    private CanvasGroup _attendanceCG;

    void Start()
    {
        if (promptPanel    != null) promptPanel.SetActive(false);
        if (attendancePanel != null)
        {
            _attendanceCG = attendancePanel.GetComponent<CanvasGroup>();
            if (_attendanceCG == null)
                _attendanceCG = attendancePanel.AddComponent<CanvasGroup>();
            _attendanceCG.alpha = 0f;
            attendancePanel.SetActive(false);
        }
        if (promptText != null)
            promptText.text = "Press  <color=#FFD95A><b>P</b></color>  to Mark Attendance";
    }

    void Update()
    {
        if (_playerNear && !_marked && Input.GetKeyDown(KeyCode.P))
            StartCoroutine(MarkAttendance());
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNear = true;
        if (!_marked && promptPanel != null) promptPanel.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNear = false;
        if (promptPanel != null) promptPanel.SetActive(false);
    }

    IEnumerator MarkAttendance()
    {
        _marked = true;
        if (promptPanel != null) promptPanel.SetActive(false);

        // Set attendance message
        if (attendanceText != null)
        {
            string time = GameClock.Instance != null
                ? GameClock.Instance.GetTimeString()
                : "10:00 AM";
            attendanceText.text =
                "✓  ATTENDANCE MARKED\n" +
                $"<size=70%>Recorded at {time}</size>";
        }

        // Fade in
        attendancePanel.SetActive(true);
        yield return StartCoroutine(FadeCG(0f, 1f));

        yield return new WaitForSeconds(displayDuration);

        // Fade out
        yield return StartCoroutine(FadeCG(1f, 0f));
        attendancePanel.SetActive(false);
    }

    IEnumerator FadeCG(float from, float to)
    {
        float t = 0f;
        _attendanceCG.alpha = from;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            _attendanceCG.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        _attendanceCG.alpha = to;
    }
}
