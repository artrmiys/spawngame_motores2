using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Animated button: hover scale, press feedback, gentle pulse for primary actions.
/// Add to Image components on Canvas Buttons.
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class UIAnimatedButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private float hoverScale = 1.06f;
    [SerializeField] private float pressScale = 0.94f;
    [SerializeField] private float animDuration = 0.12f;
    [SerializeField] private bool pulseWhenIdle = false;
    [SerializeField] private float pulseAmount = 0.04f;
    [SerializeField] private float pulseSpeed = 1.5f;

    private RectTransform rect;
    private Vector3 baseScale;
    private Coroutine scaleRoutine;
    private bool isHovered;
    private bool isPressed;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        baseScale = rect.localScale;
    }

    void Update()
    {
        if (pulseWhenIdle && !isHovered && !isPressed)
        {
            float pulse = 1f + Mathf.Sin(Time.unscaledTime * pulseSpeed) * pulseAmount;
            rect.localScale = baseScale * pulse;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        AnimateTo(baseScale * hoverScale);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;
        if (!isPressed) AnimateTo(baseScale);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        AnimateTo(baseScale * pressScale);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        AnimateTo(isHovered ? baseScale * hoverScale : baseScale);
    }

    private void AnimateTo(Vector3 target)
    {
        if (scaleRoutine != null) StopCoroutine(scaleRoutine);
        scaleRoutine = StartCoroutine(ScaleTo(target));
    }

    private IEnumerator ScaleTo(Vector3 target)
    {
        Vector3 from = rect.localScale;
        float elapsed = 0;
        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / animDuration;
            t = 1f - (1f - t) * (1f - t); // EaseOutQuad
            rect.localScale = Vector3.Lerp(from, target, t);
            yield return null;
        }
        rect.localScale = target;
    }
}
