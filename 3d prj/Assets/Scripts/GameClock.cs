using UnityEngine;
using TMPro;

// ─────────────────────────────────────────────
//  GameClock.cs
//  Attach to an empty GameObject called ClockManager.
//  Drag your clock TMP text into clockText field.
// ─────────────────────────────────────────────
public class GameClock : MonoBehaviour
{
    public static GameClock Instance;

    [Header("UI")]
    public TextMeshProUGUI clockText;

    [Header("Start Time")]
    public int startHour   = 7;
    public int startMinute = 50;

    private int _hour;
    private int _minute;

    void Awake() { Instance = this; }

    void Start()
    {
        _hour   = startHour;
        _minute = startMinute;
        UpdateDisplay();
    }

    // Call from any script to jump time e.g. GameClock.Instance.SetTime(10, 0)
    public void SetTime(int hour, int minute)
    {
        _hour   = hour;
        _minute = minute;
        UpdateDisplay();
    }

    void UpdateDisplay()
    {
        string period  = _hour >= 12 ? "PM" : "AM";
        int display    = _hour > 12 ? _hour - 12 : (_hour == 0 ? 12 : _hour);
        clockText.text = $"{display}:{_minute:00} {period}";
    }

    public string GetTimeString()
    {
        string period = _hour >= 12 ? "PM" : "AM";
        int display   = _hour > 12 ? _hour - 12 : (_hour == 0 ? 12 : _hour);
        return $"{display}:{_minute:00} {period}";
    }
}
