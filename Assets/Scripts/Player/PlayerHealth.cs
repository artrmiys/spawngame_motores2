using UnityEngine;
using UnityEngine.Events;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] int maxHP = 5;

    public int MaxHP => maxHP;
    public int CurrentHP { get; private set; }
    public UnityEvent<int, int> onHealthChanged = new UnityEvent<int, int>();   // current, max
    public UnityEvent           onDied = new UnityEvent();

    HitFlash _hitFlash;
    AutoAttack _autoAttack;

    void Awake()
    {
        if (onHealthChanged == null)
            onHealthChanged = new UnityEvent<int, int>();
        if (onDied == null)
            onDied = new UnityEvent();

        CurrentHP = maxHP;
        _hitFlash = GetComponent<HitFlash>();
        if (_hitFlash == null) _hitFlash = gameObject.AddComponent<HitFlash>();
        _autoAttack = GetComponent<AutoAttack>();
    }

    void Start() => onHealthChanged?.Invoke(CurrentHP, maxHP);

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;
        if (CurrentHP <= 0) return;
        CurrentHP = Mathf.Max(0, CurrentHP - amount);
        onHealthChanged?.Invoke(CurrentHP, maxHP);
        _autoAttack?.ResetWeaponLevel();

        // Visual feedback for hit
        _hitFlash?.Flash();
        ScreenShake.Shake(0.2f, 0.25f);
        DamagePopup.Show(transform.position, amount, new Color(1f, 0.3f, 0.3f));

        if (CurrentHP == 0)
        {
            ParticleBurst.Burst(transform.position, new Color(0.3f, 0.6f, 1f), 16, 2.5f, 0.7f);
            onDied?.Invoke();
        }
    }

    public void Heal(int amount)
    {
        if (CurrentHP <= 0) return;
        CurrentHP = Mathf.Min(maxHP, CurrentHP + amount);
        onHealthChanged?.Invoke(CurrentHP, maxHP);

        // Visual feedback for heal
        DamagePopup.Show(transform.position, amount, new Color(0.4f, 1f, 0.5f));
        ParticleBurst.Burst(transform.position, new Color(0.4f, 1f, 0.5f), 6, 1.5f, 0.4f);
    }
}
