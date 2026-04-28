using System.Collections;
using UnityEngine;
using TMPro;

// ─────────────────────────────────────────────
//  GateController.cs
//  Attach to the Gate GameObject (or a manager empty).
// ─────────────────────────────────────────────
public class GateController : MonoBehaviour
{
    // ── Inspector Fields ──────────────────────
    [Header("Gate")]
    [Tooltip("The cube / mesh that acts as the door barrier.")]
    public GameObject door;

    [Header("Popup UI")]
    public GameObject        popup;
    public TextMeshProUGUI   popupText;

    [Tooltip("Color applied to popup text on access granted.")]
    public Color grantedColor = new Color(0.2f, 0.9f, 0.3f);  // green
    [Tooltip("Color applied to popup text on denial.")]
    public Color deniedColor  = new Color(0.95f, 0.2f, 0.2f); // red

    [Header("Timing")]
    public float popupDuration = 1.5f;

    // ── Runtime State ─────────────────────────
    private bool _gateOpen;
    private Coroutine _hideRoutine;

    // ─────────────────────────────────────────
    //  Public API (called by ScannerZone)
    // ─────────────────────────────────────────

    public void GrantAccess()
    {
        if (_gateOpen) return; // already open, no duplicate messages

        OpenGate();
        ShowPopup("ACCESS GRANTED", grantedColor);
    }

    public void DenyAccess(string reason)
    {
        ShowPopup(reason, deniedColor);
    }

    // ─────────────────────────────────────────
    //  Gate
    // ─────────────────────────────────────────

    void OpenGate()
    {
        _gateOpen = true;

        if (door == null) return;

        // Hide visually
        MeshRenderer mr = door.GetComponent<MeshRenderer>();
        if (mr != null) mr.enabled = false;

        // Disable collision so player can walk through
        Collider col = door.GetComponent<Collider>();
        if (col != null) col.enabled = false;
    }

    // ─────────────────────────────────────────
    //  Popup
    // ─────────────────────────────────────────

    void ShowPopup(string message, Color color)
    {
        if (popup == null || popupText == null) return;

        popupText.text  = message;
        popupText.color = color;
        popup.SetActive(true);

        // Cancel any previous hide timer
        if (_hideRoutine != null)
            StopCoroutine(_hideRoutine);

        _hideRoutine = StartCoroutine(HideAfterDelay(popupDuration));
    }

    IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        popup.SetActive(false);
    }
}
