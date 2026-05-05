using System.Collections;
using UnityEngine;

/// <summary>
/// Briefly flashes the sprite/material white when hit.
/// Attach to enemies and player for impact feedback.
/// </summary>
public class HitFlash : MonoBehaviour
{
    [SerializeField] private Color flashColor = Color.white;
    [SerializeField] private float flashDuration = 0.1f;

    private Renderer rend;
    private Color originalColor;
    private Coroutine flashRoutine;

    void Awake()
    {
        rend = GetComponent<Renderer>();
        if (rend != null && rend.material != null)
            originalColor = rend.material.color;
    }

    public void Flash()
    {
        if (rend == null) return;
        if (flashRoutine != null) StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        rend.material.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        if (rend != null)
            rend.material.color = originalColor;
    }
}
