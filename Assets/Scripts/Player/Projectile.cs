using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed  = 8f;
    [SerializeField] int   damage = 1;
    [SerializeField] float lifetime = 3f;

    Rigidbody2D _rb;
    float _explosionRadius;
    bool _hasHit;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        ActorVisualFx.Ensure(gameObject, ActorVisualFx.VisualRole.Projectile, new Color(1f, 0.9f, 0.25f));
    }

    public void Init(Vector2 direction)
    {
        _rb.velocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    public void Init(Vector2 direction, int projectileDamage, float explosionRadius)
    {
        damage = Mathf.Max(1, projectileDamage);
        _explosionRadius = Mathf.Max(0f, explosionRadius);

        if (_explosionRadius > 0f)
            transform.localScale *= 1.25f;

        Init(direction);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (_hasHit || !other.CompareTag("Enemy")) return;
        _hasHit = true;

        if (_explosionRadius > 0.05f)
            DamageExplosion();
        else
            other.GetComponent<EnemyHealth>()?.TakeDamage(damage);

        Destroy(gameObject);
    }

    void DamageExplosion()
    {
        var damaged = new HashSet<EnemyHealth>();
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, _explosionRadius);

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy"))
                continue;

            var enemy = hit.GetComponent<EnemyHealth>();
            if (enemy == null || !damaged.Add(enemy))
                continue;

            enemy.TakeDamage(damage);
        }

        ParticleBurst.Burst(transform.position, new Color(1f, 0.6f, 0.15f), 18, _explosionRadius * 1.8f, 0.45f);
        ScreenShake.Shake(0.12f, 0.18f);
    }

    void OnDrawGizmosSelected()
    {
        if (_explosionRadius <= 0.05f)
            return;

        Gizmos.color = new Color(1f, 0.6f, 0.15f, 0.5f);
        Gizmos.DrawWireSphere(transform.position, _explosionRadius);
    }
}
