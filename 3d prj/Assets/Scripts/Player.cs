using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour
{
    [Header("Hold Point")]
    public Transform holdPoint;

    [Header("Card Inspection UI")]
    public GameObject  cardInfoPanel;
    public Image        cardImageUI;
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

    private Card       _nearbyCard;
    private bool       _uiVisible;
    private CanvasGroup _panelCG;
    private Coroutine  _fadeRoutine;

    void Start()
    {
        // Get or add CanvasGroup for fade
        _panelCG = cardInfoPanel.GetComponent<CanvasGroup>();
        if (_panelCG == null)
            _panelCG = cardInfoPanel.AddComponent<CanvasGroup>();

        cardInfoPanel.SetActive(false);
        pickupPrompt.SetActive(false);

        if (pickupPromptText != null)
            pickupPromptText.text = "Press  E  to Pick Up";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && _nearbyCard != null && heldCard == null)
            PickCard(_nearbyCard);

        if (Input.GetKeyDown(KeyCode.F))
            ToggleCardUI();

        if (Input.GetKeyDown(KeyCode.G) && heldCard != null)
            DropCard();
    }

    // ── Card Logic ────────────────────────────

    void PickCard(Card card)
    {
        card.transform.SetParent(holdPoint);
        card.transform.localPosition = Vector3.zero;
        card.transform.localRotation = Quaternion.identity;

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null) { rb.isKinematic = true; rb.useGravity = false; }

        card.SetHighlight(false);
        heldCard    = card;
        _nearbyCard = null;
        pickupPrompt.SetActive(false);
    }

    void DropCard()
    {
        if (heldCard == null) return;
        if (_uiVisible) CloseCardUI();

        Card card = heldCard;
        heldCard = null;
        card.transform.SetParent(null);

        Rigidbody rb = card.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity  = true;
            rb.AddForce(transform.forward * dropForce, ForceMode.Impulse);
        }
    }

    // ── Card UI ───────────────────────────────

    void ToggleCardUI()
    {
        if (heldCard == null) return;
        if (_uiVisible) CloseCardUI(); else OpenCardUI();
    }

    void OpenCardUI()
    {
        cardImageUI.sprite = heldCard.cardSprite;

        if (nameText     != null) nameText.text     = heldCard.personName;
        if (deptText     != null) deptText.text     = heldCard.department;
        if (idText       != null) idText.text       = heldCard.idNumber;
        if (cardTypeText != null) cardTypeText.text = heldCard.cardType.ToString();

        cardInfoPanel.SetActive(true);
        _uiVisible = true;

        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeCanvasGroup(_panelCG, 0f, 1f));
    }

    void CloseCardUI()
    {
        _uiVisible = false;
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        _fadeRoutine = StartCoroutine(FadeAndHide());
    }

    IEnumerator FadeCanvasGroup(CanvasGroup cg, float from, float to)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeAndHide()
    {
        yield return StartCoroutine(FadeCanvasGroup(_panelCG, 1f, 0f));
        cardInfoPanel.SetActive(false);
    }

    // ── Proximity Detection ───────────────────

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Card")) return;
        Card card = other.GetComponent<Card>();
        if (card == null || card == heldCard) return;
        _nearbyCard = card;
        _nearbyCard.SetHighlight(true);
        if (heldCard == null) pickupPrompt.SetActive(true);
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
