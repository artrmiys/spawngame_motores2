using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Visual line connecting symbol to meaning.
/// Renders with fade-in animation.
/// </summary>
public class SymbolConnectionLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private CanvasGroup canvasGroup;
    private Image lineImage;
    private RectTransform rectTransform;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        canvasGroup = GetComponent<CanvasGroup>();
        lineImage = GetComponent<Image>();
        rectTransform = GetComponent<RectTransform>();
    }

    public void ConnectPoints(Vector3 from, Vector3 to, float duration = 0.3f)
    {
        if (lineRenderer != null)
        {
            lineRenderer.SetPosition(0, from);
            lineRenderer.SetPosition(1, to);
        }

        if (canvasGroup != null)
            StartCoroutine(FadeInLine(duration));
    }

    public void Disconnect(float duration = 0.2f)
    {
        if (canvasGroup != null)
            StartCoroutine(FadeOutLine(duration));
        else
            Destroy(gameObject);
    }

    private IEnumerator FadeInLine(float duration)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;
        canvasGroup.alpha = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = SymbolDoorUIDesign.EaseOutQuad(t);
            yield return null;
        }

        canvasGroup.alpha = 1f;
    }

    private IEnumerator FadeOutLine(float duration)
    {
        if (canvasGroup == null) yield break;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            canvasGroup.alpha = 1f - t;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        Destroy(gameObject);
    }
}
