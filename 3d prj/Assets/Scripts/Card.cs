using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class Card : MonoBehaviour
{
    [Header("Card Data")]
    public string cardHolderName = "Alex Johnson";
    public string department     = "Engineering";
    public string cardID         = "EMP-2024-001";
    public string cardType       = "Staff";
    public Sprite cardImage;

    [Header("References")]
    public Transform       holdPoint;
    public GameObject      cardInfoPanel;
    public Image           cardImageUI;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI deptText;
    public TextMeshProUGUI idText;
    public TextMeshProUGUI cardTypeText;
    public GameObject      pickupPrompt;
    public TextMeshProUGUI pickupPromptText;

    private bool        _playerNearby;
    private bool        _isHeld;
    private Rigidbody   _rb;
    private Collider    _col;
    private CanvasGroup _panelCG;

    void Start()
    {
        _rb  = GetComponent<Rigidbody>();
        _col = GetComponent<Collider>();

        if (cardInfoPanel != null)
        {
            _panelCG = cardInfoPanel.GetComponent<CanvasGroup>();
            if (_panelCG == null)
                _panelCG = cardInfoPanel.AddComponent<CanvasGroup>();
            _panelCG.alpha = 0f;
            cardInfoPanel.SetActive(false);
        }

        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    void Update()
    {
        if (_playerNearby && !_isHeld && Input.GetKeyDown(KeyCode.E))
            PickUp();
        else if (_isHeld && Input.GetKeyDown(KeyCode.E))
            Drop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || _isHeld) return;
        _playerNearby = true;
        if (pickupPrompt != null)
        {
            pickupPrompt.SetActive(true);
            if (pickupPromptText != null)
                pickupPromptText.text = "Press E to Pick Up";
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        _playerNearby = false;
        if (pickupPrompt != null)
            pickupPrompt.SetActive(false);
    }

    void PickUp()
    {
        _isHeld = true;
        if (pickupPrompt != null) pickupPrompt.SetActive(false);

        if (_rb)  _rb.isKinematic  = true;
        if (_col) _col.enabled     = false;

        transform.SetParent(holdPoint);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        if (cardInfoPanel != null)
        {
            PopulatePanel();
            cardInfoPanel.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Fade(_panelCG, 0f, 1f, 0.5f));
        }
    }

    void Drop()
    {
        _isHeld = false;
        transform.SetParent(null);

        if (_rb)  _rb.isKinematic  = false;
        if (_col) _col.enabled     = true;

        if (cardInfoPanel != null)
            StartCoroutine(FadeAndHide(_panelCG, cardInfoPanel, 0.5f));
    }

    void PopulatePanel()
    {
        if (nameText     != null) nameText.text     = cardHolderName;
        if (deptText     != null) deptText.text     = department;
        if (idText       != null) idText.text       = cardID;
        if (cardTypeText != null) cardTypeText.text = cardType;
        if (cardImageUI  != null && cardImage != null)
            cardImageUI.sprite = cardImage;
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to, float duration)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < duration)
        {
            t        += Time.deltaTime;
            cg.alpha  = Mathf.Lerp(from, to, t / duration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FadeAndHide(CanvasGroup cg, GameObject panel, float duration)
    {
        yield return StartCoroutine(Fade(cg, 1f, 0f, duration));
        panel.SetActive(false);
    }
}