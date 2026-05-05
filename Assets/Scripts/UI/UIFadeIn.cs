using System.Collections;
using UnityEngine;

/// <summary>
/// Fades in a UI element with optional slide-up motion.
/// Used for menu transitions and panel reveals.
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class UIFadeIn : MonoBehaviour
{
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float delay = 0f;
    [SerializeField] private float slideOffset = 30f;
    [SerializeField] private bool playOnEnable = true;

    private CanvasGroup canvasGroup;
    private RectTransform rect;
    private Vector2 finalPosition;

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        finalPosition = rect.anchoredPosition;
    }

    void OnEnable()
    {
        if (playOnEnable) Play();
    }

    public void Play()
    {
        canvasGroup.alpha = 0;
        rect.anchoredPosition = finalPosition + Vector2.down * slideOffset;
        StopAllCoroutines();
        StartCoroutine(FadeRoutine());
    }

    private IEnumerator FadeRoutine()
    {
        if (delay > 0) yield return new WaitForSecondsRealtime(delay);

        float elapsed = 0;
        Vector2 startPos = rect.anchoredPosition;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / duration;
            t = 1f - (1f - t) * (1f - t); // EaseOutQuad
            canvasGroup.alpha = t;
            rect.anchoredPosition = Vector2.Lerp(startPos, finalPosition, t);
            yield return null;
        }
        canvasGroup.alpha = 1f;
        rect.anchoredPosition = finalPosition;
    }
}
