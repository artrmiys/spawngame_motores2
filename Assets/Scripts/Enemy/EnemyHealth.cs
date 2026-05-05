using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 3;

    public bool  IsDead { get; private set; }
    public UnityEvent onDied = new UnityEvent();

    int _currentHP;
    HitFlash _hitFlash;

    void Awake()
    {
        if (onDied == null)
            onDied = new UnityEvent();

        _currentHP = maxHP;
        _hitFlash = GetComponent<HitFlash>();
        if (_hitFlash == null) _hitFlash = gameObject.AddComponent<HitFlash>();
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        _currentHP -= amount;

        // Visual feedback
        _hitFlash?.Flash();
        DamagePopup.Show(transform.position, amount, new Color(1f, 0.85f, 0.2f));

        if (_currentHP <= 0) Die();
    }

    public void SetMaxHealth(int value)
    {
        maxHP = Mathf.Max(1, value);
        _currentHP = Mathf.Clamp(_currentHP, 1, maxHP);
    }

    void Die()
    {
        IsDead = true;
        ParticleBurst.Burst(transform.position, new Color(1f, 0.3f, 0.3f), 10, 2f, 0.5f);
        onDied?.Invoke();
        WaveManager.Instance?.OnEnemyKilled();
        Destroy(gameObject, 0.05f);
    }
}
