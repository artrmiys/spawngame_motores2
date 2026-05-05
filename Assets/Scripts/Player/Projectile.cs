using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] float speed  = 8f;
    [SerializeField] int   damage = 1;
    [SerializeField] float lifetime = 3f;

    Rigidbody2D _rb;

    void Awake() => _rb = GetComponent<Rigidbody2D>();

    public void Init(Vector2 direction)
    {
        _rb.velocity = direction * speed;
        Destroy(gameObject, lifetime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Enemy")) return;
        other.GetComponent<EnemyHealth>()?.TakeDamage(damage);
        Destroy(gameObject);
    }
}
