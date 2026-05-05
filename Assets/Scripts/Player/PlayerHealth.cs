using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 5;

    public int MaxHP => maxHP;
    public int CurrentHP { get; private set; }
    public UnityEvent<int, int> onHealthChanged = new UnityEvent<int, int>();   // current, max
    public UnityEvent           onDied = new UnityEvent();

    void Awake()
    {
        if (onHealthChanged == null)
            onHealthChanged = new UnityEvent<int, int>();
        if (onDied == null)
            onDied = new UnityEvent();

        CurrentHP = maxHP;
    }

    void Start() => onHealthChanged?.Invoke(CurrentHP, maxHP);

    public void TakeDamage(int amount)
    {
        if (CurrentHP <= 0) return;
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        onHealthChanged?.Invoke(CurrentHP, maxHP);
        if (CurrentHP == 0) onDied?.Invoke();
    }

    public void Heal(int amount)
    {
        if (CurrentHP <= 0) return;
        CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
        onHealthChanged?.Invoke(CurrentHP, maxHP);
    }
}
