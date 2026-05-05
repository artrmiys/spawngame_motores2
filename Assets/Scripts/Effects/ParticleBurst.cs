using System.Collections;
using UnityEngine;

/// <summary>
/// Quick particle burst effect for hits, deaths, spawns.
/// Creates small colored squares that explode outward then fade.
/// </summary>
public class ParticleBurst : MonoBehaviour
{
    public static void Burst(Vector3 position, Color color, int count = 8, float spread = 1.5f, float lifetime = 0.5f)
    {
        var parent = new GameObject("ParticleBurst");
        parent.transform.position = position;

        for (int i = 0; i < count; i++)
        {
            var particle = GameObject.CreatePrimitive(PrimitiveType.Quad);
            particle.transform.SetParent(parent.transform, false);
            particle.transform.localScale = Vector3.one * 0.12f;

            // Remove default collider
            var collider = particle.GetComponent<Collider>();
            if (collider) Object.Destroy(collider);

            var renderer = particle.GetComponent<Renderer>();
            if (renderer) renderer.material.color = color;

            float angle = (360f / count) * i + Random.Range(-15f, 15f);
            float speed = Random.Range(spread * 0.5f, spread);
            Vector3 dir = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);

            var mover = particle.AddComponent<ParticleMover>();
            mover.Initialize(dir * speed, lifetime, color);
        }

        Object.Destroy(parent, lifetime + 0.1f);
    }
}

public class ParticleMover : MonoBehaviour
{
    private Vector3 velocity;
    private float lifetime;
    private float elapsed;
    private Renderer rend;
    private Color startColor;

    public void Initialize(Vector3 vel, float life, Color color)
    {
        velocity = vel;
        lifetime = life;
        startColor = color;
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;
        float t = elapsed / lifetime;

        // Move + decelerate
        transform.position += velocity * Time.deltaTime;
        velocity = Vector3.Lerp(velocity, Vector3.zero, Time.deltaTime * 4f);

        // Shrink
        transform.localScale = Vector3.one * 0.12f * Mathf.Lerp(1f, 0f, t);

        // Fade
        if (rend != null)
        {
            Color c = startColor;
            c.a = Mathf.Lerp(1f, 0f, t);
            rend.material.color = c;
        }
    }
}
