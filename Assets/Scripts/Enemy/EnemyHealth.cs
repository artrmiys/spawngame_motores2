using UnityEngine;
using UnityEngine.Events;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 3;

    public bool  IsDead { get; private set; }
    public UnityEvent onDied = new UnityEvent();

    int _currentHP;

    void Awake()
    {
        if (onDied == null)
            onDied = new UnityEvent();

        _currentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (IsDead) return;
        _currentHP -= amount;
        if (_currentHP <= 0) Die();
    }

    void Die()
    {
        IsDead = true;
        onDied?.Invoke();
        WaveManager.Instance?.OnEnemyKilled();
        Destroy(gameObject, 0.1f);
    }
}
