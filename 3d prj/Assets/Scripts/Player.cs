using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────
//  Player.cs
//  Attach to the Player GameObject.
//  Requires a Collider set as Trigger on the player (pickup detection radius).
// ─────────────────────────────────────────────
public class Player : MonoBehaviour
{
    // ── Inspector Fields ──────────────────────
    [Header("Hold Point")]
    [Tooltip("Empty child Transform where the card will sit in the player's hand.")]
    public Transform holdPoint;

    [Header("Card Inspection UI")]
    public GameObject cardInfoPanel;   // The full-screen / overlay panel
    public Image      cardImageUI;     // UI Image inside the panel

    [Header("Pickup Prompt UI")]
    [Tooltip("WorldSpace or ScreenSpace panel showing 'Press E to Pick Up'.")]
    public GameObject pickupPrompt;

    [Header("Drop Settings")]
    [Tooltip("Force applied when dropping the card so it doesn't clip the player.")]
    public float dropForce = 2f;

    // ── Runtime State ─────────────────────────
    [HideInInspector] public Card heldCard;

    private Card _nearbyCard;
    private bool _uiVisible;

    // ─────────────────────────────────────────
    //  Unity Callbacks
    // ─────────────────────────────────────────
    void Start()
    {
        // Make sure UI elements are hidden at startup
        cardInfoPanel.SetActive(false);
        pickupPrompt.SetActive(false);
    }

    void Update()
    {
        HandlePickup();
        HandleInspect();
        HandleDrop();
    }

    // ─────────────────────────────────────────
    //  Input Handlers
    // ─────────────────────────────────────────

    /// <summary>E key – pick up the nearby card (if hands are empty).</summary>
    void HandlePickup()
    {
        if (Input.GetKeyDown(KeyCode.E) && _nearbyCard != null && heldCard == null)
        {
            PickCard(_nearbyCard);
        }
    }

    /// <summary>F key – toggle card image inspection panel.</summary>
    void HandleInspect()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            ToggleCardUI();
        }
    }

    /// <summary>G key – drop / throw away the currently held card.</summary>
    void HandleDrop()
    {
        if (Input.GetKeyDown(KeyCode.G) && heldCard != null)
        {
            DropCard();
        }
    }

    // ─────────────────────────────────────────
    //  Card Logic
    // ─────────────────────────────────────────

    void PickCard(Card card)
    {
        // Detach from any previous parent & re-parent to holdPoint
        card.transform.SetParent(holdPoint);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;

        // Disable physics while held
        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic  = true;
            rb.useGravity   = false;
        }

        // Turn off highlight once picked up
        card.SetHighlight(false);

        heldCard    = card;
        _nearbyCard = null;

        // Hide prompt since the card is no longer nearby & loose
        pickupPrompt.SetActive(false);
    }

    void DropCard()
    {
        if (heldCard == null) return;

        // Close inspect panel if open
        if (_uiVisible) CloseCardUI();

        Card card = heldCard;
        heldCard = null;

        // Detach from player
        card.transform.SetParent(null);

        // Re-enable physics
        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            // Toss gently forward
            rb.AddForce(transform.forward * dropForce, ForceMode.Impulse);
        }
    }

    void ToggleCardUI()
    {
        if (heldCard == null) return;

        _uiVisible = !_uiVisible;

        if (_uiVisible)
            OpenCardUI();
        else
            CloseCardUI();
    }

    void OpenCardUI()
    {
        cardImageUI.sprite = heldCard.cardSprite;
        cardInfoPanel.SetActive(true);
        _uiVisible = true;
    }

    void CloseCardUI()
    {
        cardInfoPanel.SetActive(false);
        _uiVisible = false;
    }

    // ─────────────────────────────────────────
    //  Trigger Detection (Card Proximity)
    // ─────────────────────────────────────────

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Card")) return;

        Card card = other.GetComponent<Card>();
        if (card == null || card == heldCard) return;

        _nearbyCard = card;
        _nearbyCard.SetHighlight(true);

        if (heldCard == null)
            pickupPrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Card")) return;

        Card card = other.GetComponent<Card>();
        if (card == null || card != _nearbyCard) return;

        _nearbyCard.SetHighlight(false);
        _nearbyCard = null;

        pickupPrompt.SetActive(false);
    }
}
