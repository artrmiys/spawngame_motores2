using System.Collections;
using UnityEngine;

/// <summary>
/// Camera shake effect for impactful events (player hit, boss death).
/// Singleton accessed via ScreenShake.Instance.
/// </summary>
public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }

    private Vector3 originalPosition;
    private Coroutine shakeRoutine;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        originalPosition = transform.localPosition;
    }

    public static void Shake(float duration = 0.2f, float magnitude = 0.3f)
    {
        if (Instance == null) return;
        if (Instance.shakeRoutine != null) Instance.StopCoroutine(Instance.shakeRoutine);
        Instance.shakeRoutine = Instance.StartCoroutine(Instance.ShakeRoutine(duration, magnitude));
    }

    private IEnumerator ShakeRoutine(float duration, float magnitude)
    {
        float elapsed = 0;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
