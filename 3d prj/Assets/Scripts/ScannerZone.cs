using System.Collections;
using UnityEngine;

// ─────────────────────────────────────────────
//  ScannerZone.cs
//  Attach to an Empty GameObject with a Box Collider (Is Trigger = TRUE).
//  Recommended collider size: X=4, Y=3, Z=4
// ─────────────────────────────────────────────
public class ScannerZone : MonoBehaviour
{
    // ── Inspector Fields ──────────────────────
    [Header("References")]
    public GateController gate;

    [Header("Scan Animation")]
    [Tooltip("A quad / plane with a glowing material that sweeps up and down.")]
    public Transform scanBeam;
    [Tooltip("How many seconds the beam sweep animation takes.")]
    public float scanAnimDuration = 0.6f;
    [Tooltip("Local Y position at the bottom of the sweep.")]
    public float beamYMin = 0f;
    [Tooltip("Local Y position at the top of the sweep.")]
    public float beamYMax = 2f;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip   scanGrantedClip;   // Pleasant beep
    public AudioClip   scanDeniedClip;    // Error beep
    public AudioClip   scanBeepClip;      // Neutral scan beep

    // ── Runtime State ─────────────────────────
    private Player _nearbyPlayer;
    private bool   _isScanning;

    // ─────────────────────────────────────────
    //  Unity Callbacks
    // ─────────────────────────────────────────

    void Start()
    {
        // Hide beam until a scan is triggered
        if (scanBeam != null)
            scanBeam.gameObject.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && _nearbyPlayer != null && !_isScanning)
        {
            StartCoroutine(ScanRoutine());
        }
    }

    // ─────────────────────────────────────────
    //  Trigger Detection
    // ─────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        // GetComponentInParent also catches the exact object
        Player player = other.GetComponentInParent<Player>();
        if (player != null)
            _nearbyPlayer = player;
    }

    void OnTriggerExit(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player != null && player == _nearbyPlayer)
            _nearbyPlayer = null;
    }

    // ─────────────────────────────────────────
    //  Scan Sequence (Coroutine)
    // ─────────────────────────────────────────

    IEnumerator ScanRoutine()
    {
        _isScanning = true;

        // ① Play neutral beep & show animated beam
        PlayClip(scanBeepClip);
        yield return StartCoroutine(AnimateScanBeam());

        // ② Evaluate card
        EvaluateCard();

        _isScanning = false;
    }

    /// <summary>Sweeps the scan beam quad from bottom to top then hides it.</summary>
    IEnumerator AnimateScanBeam()
    {
        if (scanBeam == null) yield break;

        scanBeam.gameObject.SetActive(true);

        float elapsed = 0f;
        Vector3 startPos = scanBeam.localPosition;
        startPos.y = beamYMin;

        while (elapsed < scanAnimDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / scanAnimDuration);

            Vector3 pos = scanBeam.localPosition;
            pos.y = Mathf.Lerp(beamYMin, beamYMax, t);
            scanBeam.localPosition = pos;

            yield return null;
        }

        scanBeam.gameObject.SetActive(false);

        // Reset beam position for next scan
        Vector3 resetPos = scanBeam.localPosition;
        resetPos.y = beamYMin;
        scanBeam.localPosition = resetPos;
    }

    // ─────────────────────────────────────────
    //  Card Evaluation
    // ─────────────────────────────────────────

    void EvaluateCard()
    {
        // No card in hand
        if (_nearbyPlayer.heldCard == null)
        {
            PlayClip(scanDeniedClip);
            gate.DenyAccess("NO CARD");
            return;
        }

        Card.CardType type = _nearbyPlayer.heldCard.cardType;

        bool isValid = type == Card.CardType.StudentID ||
                       type == Card.CardType.StaffCard;

        if (isValid)
        {
            PlayClip(scanGrantedClip);
            gate.GrantAccess();
        }
        else
        {
            PlayClip(scanDeniedClip);
            gate.DenyAccess("INVALID CARD");
        }
    }

    // ─────────────────────────────────────────
    //  Audio Helper
    // ─────────────────────────────────────────

    void PlayClip(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        audioSource.PlayOneShot(clip);
    }
}
