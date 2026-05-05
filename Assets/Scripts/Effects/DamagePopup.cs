using System.Collections;
using TMPro;
using UnityEngine;

/// <summary>
/// Floating damage number with smooth float-up and fade-out animation.
/// Spawned dynamically when enemies/player take damage.
/// </summary>
public class DamagePopup : MonoBehaviour
{
    [SerializeField] private float floatSpeed = 1.5f;
    [SerializeField] private float lifetime = 0.8f;
    [SerializeField] private TMP_FontAsset font;

    private TextMeshPro text;
    private float elapsed;
    private Color startColor;
    private Vector3 startPos;
    private Vector3 randomDrift;

    public static void Show(Vector3 worldPos, int damage, Color color)
    {
        var go = new GameObject("DamagePopup");
        go.transform.position = worldPos + new Vector3(Random.Range(-0.3f, 0.3f), 0.5f, 0);
        var popup = go.AddComponent<DamagePopup>();
        popup.Initialize(damage, color);
    }

    public void Initialize(int damage, Color color)
    {
        text = gameObject.AddComponent<TextMeshPro>();
        text.text = damage.ToString();
        text.fontSize = 5;
        text.alignment = TextAlignmentOptions.Center;
        text.color = color;
        text.fontStyle = FontStyles.Bold;
        text.outlineWidth = 0.2f;
        text.outlineColor = Color.black;

        // Try to set the font; if not assigned, will fall back to default
        if (font != null)
            text.font = font;

        // Sort over everything
        var renderer = GetComponent<MeshRenderer>();
        if (renderer != null)
            renderer.sortingOrder = 100;

        startColor = color;
        startPos = transform.position;
        randomDrift = new Vector3(Random.Range(-0.3f, 0.3f), 0, 0);
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;

        // Smooth float upward with slight horizontal drift
        transform.position = startPos + Vector3.up * (floatSpeed * elapsed) + randomDrift * t;

        // Scale: pop in then shrink
        float scale = t < 0.2f ? Mathf.Lerp(0.5f, 1.2f, t / 0.2f) : Mathf.Lerp(1.2f, 0.8f, (t - 0.2f) / 0.8f);
        transform.localScale = Vector3.one * scale;

        // Fade out in second half
        if (text != null)
        {
            Color c = startColor;
            c.a = t < 0.5f ? 1f : Mathf.Lerp(1f, 0f, (t - 0.5f) / 0.5f);
            text.color = c;
        }

        if (elapsed >= lifetime)
            Destroy(gameObject);
    }
}
