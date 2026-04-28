using UnityEngine;

// ─────────────────────────────────────────────
//  Card.cs
//  Attach to every card GameObject in the scene.
//  Tag the GameObject as "Card".
// ─────────────────────────────────────────────
public class Card : MonoBehaviour
{
    // ── Inspector Fields ──────────────────────
    [Header("Card Identity")]
    public CardType cardType;
    public Sprite   cardSprite;
    public string   personName;
    public string   department;
    public string   idNumber;

    [Header("Highlight Settings")]
    [Tooltip("Assign an Outline component (from UI Effects or custom shader).")]
    public Renderer[] cardRenderers;          // all MeshRenderers on the card
    public Color  highlightColor = Color.yellow;
    public float  highlightEmissionIntensity = 1.2f;

    // ── Private State ─────────────────────────
    private bool _highlighted;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    // ── Enum ──────────────────────────────────
    public enum CardType
    {
        StudentID,
        StaffCard,
        CEPCard,
        Unknown
    }

    // ─────────────────────────────────────────
    //  Highlight / De-highlight
    // ─────────────────────────────────────────
    public void SetHighlight(bool on)
    {
        if (_highlighted == on) return;
        _highlighted = on;

        foreach (Renderer r in cardRenderers)
        {
            // Works with Standard shader (emission must be enabled in material)
            r.material.SetColor(EmissionColor,
                on ? highlightColor * highlightEmissionIntensity : Color.black);
        }
    }
}
