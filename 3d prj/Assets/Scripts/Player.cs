using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────
//  Player.cs
//  Handles ALL card pickup / drop / UI logic.
//  Card.cs is data-only — no overlap.
// ─────────────────────────────────────────────
public class Player : MonoBehaviour
{
    [Header("Hold Point")]
    public Transform holdPoint;

    [Header("Card Inspection UI")]
    public GameObject      cardInfoPanel;
    public Image           cardImageUI;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI deptText;
    public TextMeshProUGUI idText;
    public TextMeshProUGUI cardTypeText;

    [Header("Pickup Prompt UI")]
    public GameObject      pickupPrompt;
    public TextMeshProUGUI pickupPromptText;

    [Header("Drop Settings")]
    public float dropForce = 2f;

    [Header("Fade Settings")]
    public float fadeDuration = 0.25f;

    [HideInInspector] public Card heldCard;

    private Card        _nearbyCard;
    private bool        _uiVisible;
    private CanvasGroup _panelCG;
    private Coroutine   _fadeRoutine;

    // ─────────────────────────────────────────
    //  Start
    // ─────────────────────────────────────────
    void Start()
    {
        _panelCG = cardInfoPanel.GetComponent<CanvasGroup>();
        if (_panelCG == null)
            _panelCG = cardInfoPanel.AddComponent<CanvasGroup>();

        cardInfoPanel.SetActive(false);
        pickupPrompt.SetActive(false);

        if (pickupPromptText != null)
            pickupPromptText.text = "Press  E  to Pick Up";
    }

    // ─────────────────────────────────────────
    //  Update
    // ─────────────────────────────────────────
    void Update()
    {
        // E — pick up
        if (Input.GetKeyDown(KeyCode.E) && _nearbyCard != null && heldCard == null)
            PickCard(_nearbyCard);

        // F — inspect toggle
        if (Input.GetKeyDown(KeyCode.F))
            ToggleCardUI();

        // G — drop
        if (Input.GetKeyDown(KeyCode.G) && heldCard != null)
            DropCard();
    }

    // ─────────────────────────────────────────
    //  Pickup
    // ─────────────────────────────────────────
    void PickCard(Card card)
    {
        heldCard        = card;
        heldCard.isHeld = true;   // ← tells ScannerZone this card is being carried
        _nearbyCard     = null;

        card.transform.SetParent(holdPoint);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        Collider col = card.GetComponent<Collider>();
        if (col != null) col.enabled = false;

        pickupPrompt.SetActive(false);
    }

    // ─────────────────────────────────────────
    //  Drop
    // ─────────────────────────────────────────
    void DropCard()
    {
            Debug.Log("DROP CALLED");
        if (heldCard == null) return;
        if (_uiVisible) CloseCardUI();

        Card card   = heldCard;
        heldCard    = null;
        card.isHeld = false;   // ← mark as no longer held

        card.transform.SetParent(null);

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = false; rb.useGravity = true; rb.AddForce(Vector3.down * 1f, ForceMode.Impulse); }

        Collider col = card.GetComponent<Collider>();
        if (col != null) col.enabled = true;

    }

    // ─────────────────────────────────────────
    //  Card Info UI
    // ─────────────────────────────────────────
    void ToggleCardUI()
    {
        if (heldCard == null) return;
        if (_uiVisible) CloseCardUI(); else OpenCardUI();
    }

    void OpenCardUI()
    {
        if (cardImageUI  != null) cardImageUI.sprite   = heldCard.cardImage;
        if (nameText     != null) nameText.text         = heldCard.cardHolderName;
        if (deptText     != null) deptText.text         = heldCard.department;
        if (idText       != null) idText.text           = heldCard.cardID;
        if (cardTypeText != null) cardTypeText.text     = heldCard.cardType;

        cardInfoPanel.SetActive(true);
        _uiVisible = true;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeCG(_panelCG, 0f, 1f));
    }

    void CloseCardUI()
    {
        _uiVisible = false;
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeAndHide());
    }

    IEnumerator FadeCG(CanvasGroup cg, float from, float to)
    {
        float t = 0f; cg.alpha = from;
        while (t < fadeDuration) { t += Time.deltaTime; cg.alpha = Mathf.Lerp(from, to, t / fadeDuration); yield return null; }
        cg.alpha = to;
    }

    IEnumerator FadeAndHide()
    {
        yield return StartCoroutine(FadeCG(_panelCG, 1f, 0f));
        cardInfoPanel.SetActive(false);
    }

    // ─────────────────────────────────────────
    //  Proximity Detection
    // ─────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Card")) return;
        Card card = other.GetComponent<Card>();
        if (card == null || card == heldCard) return;
        _nearbyCard = card;
        if (heldCard == null) pickupPrompt.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Card")) return;
        Card card = other.GetComponent<Card>();
        if (card == null || card != _nearbyCard) return;
        _nearbyCard = null;
        pickupPrompt.SetActive(false);
    }
}
