using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GateController : MonoBehaviour
{
    [Header("Gate")]
    public GameObject door;

    [Header("Popup UI")]
    public GameObject      popup;
    public TextMeshProUGUI popupText;
    public Image           popupBackground;

    [Header("Screen Flash")]
    public Image screenFlash;

    [Header("Timing")]
    public float popupDuration = 3f;
    public float fadeDuration  = 0.5f;
    public float flashDuration = 1.5f;

    private bool        _gateOpen;
    private Coroutine   _popupRoutine;
    private CanvasGroup _popupCG;

    private readonly Color _grantedBg    = new Color(0.05f, 0.22f, 0.10f, 0.95f);
    private readonly Color _grantedText  = new Color(0.50f, 1.00f, 0.60f, 1.00f);
    private readonly Color _grantedFlash = new Color(0.00f, 1.00f, 0.20f, 0.35f);

    private readonly Color _deniedBg    = new Color(0.22f, 0.05f, 0.05f, 0.95f);
    private readonly Color _deniedText  = new Color(1.00f, 0.40f, 0.40f, 1.00f);
    private readonly Color _deniedFlash = new Color(1.00f, 0.00f, 0.00f, 0.35f);

    void Start()
    {
        _popupCG = popup.GetComponent<CanvasGroup>();
        if (_popupCG == null)
            _popupCG = popup.AddComponent<CanvasGroup>();

        _popupCG.alpha = 0f;
        popup.SetActive(false);

        if (screenFlash != null)
        {
            screenFlash.color = Color.clear;
            screenFlash.gameObject.SetActive(false);
        }
    }

    public void GrantAccess()
    {
        if (_gateOpen) return;
        OpenGate();
        ShowPopup("✦  ACCESS GRANTED  ✦", _grantedText, _grantedBg);
        StartCoroutine(FlashScreen(_grantedFlash));
    }

    public void DenyAccess(string reason)
    {
        ShowPopup("✦  " + reason + "  ✦", _deniedText, _deniedBg);
        StartCoroutine(FlashScreen(_deniedFlash));
    }

    void OpenGate()
    {
        _gateOpen = true;
        if (door == null) return;

        var mr  = door.GetComponent<MeshRenderer>();
        var col = door.GetComponent<Collider>();
        if (mr  != null) mr.enabled  = false;
        if (col != null) col.enabled = false;
    }

    void ShowPopup(string message, Color textColor, Color bgColor)
    {
        if (popup == null || popupText == null) return;

        popupText.text  = message;
        popupText.color = textColor;
        if (popupBackground != null)
            popupBackground.color = bgColor;

        if (_popupRoutine != null)
            StopCoroutine(_popupRoutine);
        _popupRoutine = StartCoroutine(PopupSequence());
    }

    IEnumerator PopupSequence()
    {
        popup.SetActive(true);
        yield return StartCoroutine(Fade(_popupCG, 0f, 1f));
        yield return new WaitForSeconds(popupDuration);
        yield return StartCoroutine(Fade(_popupCG, 1f, 0f));
        popup.SetActive(false);
    }

    IEnumerator Fade(CanvasGroup cg, float from, float to)
    {
        float t = 0f;
        cg.alpha = from;
        while (t < fadeDuration)
        {
            t        += Time.deltaTime;
            cg.alpha  = Mathf.Lerp(from, to, t / fadeDuration);
            yield return null;
        }
        cg.alpha = to;
    }

    IEnumerator FlashScreen(Color flashColor)
    {
        if (screenFlash == null) yield break;

        screenFlash.gameObject.SetActive(true);
        screenFlash.color = flashColor;

        float t = 0f;
        while (t < flashDuration)
        {
            t += Time.deltaTime;
            screenFlash.color = Color.Lerp(flashColor, Color.clear, t / flashDuration);
            yield return null;
        }

        screenFlash.color = Color.clear;
        screenFlash.gameObject.SetActive(false);
    }
}