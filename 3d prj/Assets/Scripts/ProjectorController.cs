using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// ─────────────────────────────────────────────────────────────
//  ProjectorController.cs
//
//  What this does:
//  ┌─────────────────────────────────────────────────────────┐
//  │  Player walks near projector → prompt appears           │
//  │  Press T → 3D beam fades in + Canvas slide fades in    │
//  │  Press T again → both fade out and disappear           │
//  │  Walk away → everything auto-closes                    │
//  └─────────────────────────────────────────────────────────┘
//
//  Attach to: ProjectorSystem (empty parent GameObject)
// ─────────────────────────────────────────────────────────────
public class ProjectorController : MonoBehaviour
{
    // ── 3D World ──────────────────────────────────────────────
    [Header("3D World Objects")]
    [Tooltip("The Cylinder GameObject used as the light beam.")]
    public GameObject lightBeam;

    [Tooltip("Spot Light on the projector pointing forward.")]
    public Light projectorSpotLight;

    [Tooltip("How bright the spotlight gets when ON.")]
    public float spotLightIntensity = 5f;

    [Tooltip("How long the beam/light fades in or out (seconds).")]
    public float beamFadeDuration = 0.45f;

    // ── Canvas UI ─────────────────────────────────────────────
    [Header("Canvas Slide UI")]
    [Tooltip("Root panel of the fullscreen slide. Has CanvasGroup.")]
    public GameObject projectorSlideUI;

    [Tooltip("The UI Image inside the panel. Drag your PNG Sprite here.")]
    public Image slideImage;

    [Tooltip("Optional title text at the top.")]
    public TextMeshProUGUI titleText;

    [Tooltip("Optional hint text at the bottom.")]
    public TextMeshProUGUI hintText;

    [Tooltip("How fast the Canvas UI fades in/out.")]
    public float uiFadeDuration = 0.4f;

    // ── Proximity Prompt ──────────────────────────────────────
    [Header("Proximity Prompt")]
    [Tooltip("Small panel shown when player walks near projector.")]
    public GameObject promptPanel;
    public TextMeshProUGUI promptText;

    // ── Runtime ───────────────────────────────────────────────
    private bool        _playerNear;
    private bool        _projectorOn;
    private CanvasGroup _slideCG;
    private Material    _beamMat;
    private Coroutine   _beamRoutine;
    private Coroutine   _uiRoutine;

    // ─────────────────────────────────────────────────────────
    //  Start
    // ─────────────────────────────────────────────────────────
    void Start()
    {
        // ── Canvas Group ──────────────────────────────────────
        if (projectorSlideUI != null)
        {
            _slideCG = projectorSlideUI.GetComponent<CanvasGroup>();
            if (_slideCG == null)
                _slideCG = projectorSlideUI.AddComponent<CanvasGroup>();
            _slideCG.alpha = 0f;
            projectorSlideUI.SetActive(false);
        }

        // ── Beam material instance ────────────────────────────
        if (lightBeam != null)
        {
            Renderer r = lightBeam.GetComponent<Renderer>();
            if (r != null) _beamMat = r.material; // instance, won't affect shared mat
            lightBeam.SetActive(false);
        }

        // ── Spot light off ────────────────────────────────────
        if (projectorSpotLight != null)
            projectorSpotLight.intensity = 0f;

        // ── Prompt hidden ─────────────────────────────────────
        if (promptPanel != null) promptPanel.SetActive(false);

        // ── Static text ───────────────────────────────────────
        if (titleText != null)
            titleText.text = "College Smart Card Access System";
        if (hintText  != null)
            hintText.text  = "Press  <b>T</b>  to toggle projector";
        if (promptText != null)
            promptText.text = "Press  <color=#FFD95A><b>T</b></color>  to toggle projector";
    }

    // ─────────────────────────────────────────────────────────
    //  Update — key input
    // ─────────────────────────────────────────────────────────
    void Update()
    {
        if (_playerNear && Input.GetKeyDown(KeyCode.T))
            ToggleProjector();
    }

    // ─────────────────────────────────────────────────────────
    //  Proximity Zone
    // ─────────────────────────────────────────────────────────
    void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Player>() == null) return;
        _playerNear = true;
        if (promptPanel != null) promptPanel.SetActive(true);
    }

    void OnTriggerExit(Collider other)
    {
        if (other.GetComponentInParent<Player>() == null) return;
        _playerNear = false;
        if (promptPanel != null) promptPanel.SetActive(false);

       
    }

    // ─────────────────────────────────────────────────────────
    //  Toggle
    // ─────────────────────────────────────────────────────────
    void ToggleProjector()
    {
        if (_projectorOn) TurnOff();
        else              TurnOn();
    }

    void TurnOn()
    {
        _projectorOn = true;

        // Start beam fade in
        if (_beamRoutine != null) StopCoroutine(_beamRoutine);
        _beamRoutine = StartCoroutine(FadeBeam(true));

        // Start UI fade in
        if (_uiRoutine != null) StopCoroutine(_uiRoutine);
        _uiRoutine = StartCoroutine(FadeSlideUI(true));
    }

    void TurnOff()
    {
        _projectorOn = false;

        if (_beamRoutine != null) StopCoroutine(_beamRoutine);
        _beamRoutine = StartCoroutine(FadeBeam(false));

        if (_uiRoutine != null) StopCoroutine(_uiRoutine);
        _uiRoutine = StartCoroutine(FadeSlideUI(false));
    }

    // ─────────────────────────────────────────────────────────
    //  Beam Fade  (3D cylinder + spotlight)
    // ─────────────────────────────────────────────────────────
    IEnumerator FadeBeam(bool on)
    {
        if (on && lightBeam != null)
            lightBeam.SetActive(true);

        float startAlpha = on ? 0f : GetBeamAlpha();
        float endAlpha   = on ? 0.22f : 0f;
        float startLight = projectorSpotLight != null ? projectorSpotLight.intensity : 0f;
        float endLight   = on ? spotLightIntensity : 0f;

        float t = 0f;
        while (t < beamFadeDuration)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / beamFadeDuration);

            SetBeamAlpha(Mathf.Lerp(startAlpha, endAlpha, p));

            if (projectorSpotLight != null)
                projectorSpotLight.intensity = Mathf.Lerp(startLight, endLight, p);

            yield return null;
        }

        SetBeamAlpha(endAlpha);
        if (projectorSpotLight != null)
            projectorSpotLight.intensity = endLight;

        // Deactivate beam mesh after fade out
        if (!on && lightBeam != null)
            lightBeam.SetActive(false);
    }

    void SetBeamAlpha(float a)
    {
        if (_beamMat == null) return;
        Color c = _beamMat.color;
        c.a = a;
        _beamMat.color = c;
    }

    float GetBeamAlpha()
    {
        if (_beamMat == null) return 0f;
        return _beamMat.color.a;
    }

    // ─────────────────────────────────────────────────────────
    //  Canvas Slide UI Fade
    // ─────────────────────────────────────────────────────────
    IEnumerator FadeSlideUI(bool on)
    {
        if (on && projectorSlideUI != null)
            projectorSlideUI.SetActive(true);

        float startA = on ? 0f : (_slideCG != null ? _slideCG.alpha : 1f);
        float endA   = on ? 1f : 0f;

        float t = 0f;
        while (t < uiFadeDuration)
        {
            t += Time.deltaTime;
            if (_slideCG != null)
                _slideCG.alpha = Mathf.Lerp(startA, endA, t / uiFadeDuration);
            yield return null;
        }

        if (_slideCG != null) _slideCG.alpha = endA;

        if (!on && projectorSlideUI != null)
            projectorSlideUI.SetActive(false);
    }
}
