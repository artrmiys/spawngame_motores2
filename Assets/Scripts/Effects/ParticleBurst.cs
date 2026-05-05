using System.Collections;
using UnityEngine;

/// <summary>
/// Quick particle burst effect for hits, deaths, spawns.
/// Creates soft colored sprites that drift outward then fade.
/// </summary>
public class ParticleBurst : MonoBehaviour
{
    public static void Burst(Vector3 position, Color color, int count = 8, float spread = 1.5f, float lifetime = 0.5f)
    {
        var parent = new GameObject("ParticleBurst");
        parent.transform.position = position;

        for (int i = 0; i < count; i++)
        {
            Color particleColor = color;
            particleColor.a = Mathf.Min(0.9f, color.a);
            SpriteRenderer renderer = SoftVisualSprite.CreateRenderer("Particle", parent.transform, Vector3.zero,
                Vector2.one * Random.Range(0.12f, 0.19f), particleColor, 45);

            float angle = (360f / count) * i + Random.Range(-15f, 15f);
            float speed = Random.Range(spread * 0.5f, spread);
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            var mover = renderer.gameObject.AddComponent<ParticleMover>();
            mover.Initialize(dir * speed, lifetime, particleColor);
        }

        if (Application.isPlaying)
            Object.Destroy(parent, lifetime + 0.2f);
    }
}

public class ParticleMover : MonoBehaviour
{
    private Vector3 velocity;
    private float lifetime;
    private float elapsed;
    private SpriteRenderer rend;
    private Color startColor;
    private Vector3 startScale;

    public void Initialize(Vector3 vel, float life, Color color)
    {
        velocity = vel;
        lifetime = Mathf.Max(0.05f, life);
        startColor = color;
        startScale = transform.localScale;
        rend = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = Mathf.Clamp01(elapsed / lifetime);

        float drift = 1f - SoftVisualSprite.Smooth01(t);
        transform.position += velocity * drift * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 3.2f);

        transform.localScale = startScale * Mathf.Lerp(1f, 1.85f, SoftVisualSprite.EaseOut(t));

        if (rend != null)
        {
            Color c = startColor;
            c.a = startColor.a * (1f - SoftVisualSprite.Smooth01(t));
            rend.color = c;
        }

        if (t >= 1f && Application.isPlaying)
            Destroy(gameObject);
    }
}
