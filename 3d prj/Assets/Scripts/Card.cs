using UnityEngine;

// ─────────────────────────────────────────────
//  Card.cs  — DATA ONLY
//  All pickup / drop / UI is handled by Player.cs
//  ScannerZone uses IsHeld() to check if carried
// ─────────────────────────────────────────────
public class Card : MonoBehaviour
{
    [Header("Card Data")]
    public string cardHolderName = "Arjun Sharma";
    public string department     = "Computer Science";
    public string cardID         = "SCE-2024-001";
    public string cardType       = "Student";   // must match ScannerZone allowedCardTypes
    public Sprite cardImage;

    // Tracked by Player.cs — do not set manually
    [HideInInspector] public bool isHeld;

    public bool IsHeld() => isHeld;
}
