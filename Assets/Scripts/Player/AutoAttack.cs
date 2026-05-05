using UnityEngine;
using UnityEngine.Events;

public class AutoAttack : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float      attackRange    = 6f;
    [SerializeField] float      fireRate       = 1f;
    [SerializeField] Transform  firePoint;

    [Header("Weapon upgrades")]
    [SerializeField] int   maxWeaponLevel      = 3;
    [SerializeField] float rapidFireMultiplier = 1.8f;
    [SerializeField] int   spreadShotCount     = 3;
    [SerializeField] float spreadAngle         = 18f;
    [SerializeField] float blastRadius         = 1.45f;
    [SerializeField] int   projectileDamage    = 1;

    public int WeaponLevel { get; private set; }
    public UnityEvent<int> onWeaponLevelChanged = new UnityEvent<int>();

    float _nextFireTime;

    void Awake()
    {
        if (onWeaponLevelChanged == null)
            onWeaponLevelChanged = new UnityEvent<int>();
    }

    void Update()
    {
        Transform target = FindNearest();
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;

        if (Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + 1f / GetCurrentFireRate();
            Shoot(dir);
        }
    }

    Transform FindNearest()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, attackRange);
        Transform nearest = null;
        float minDist = float.MaxValue;

        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            float d = Vector2.Distance(transform.position, hit.transform.position);
            if (d < minDist) { minDist = d; nearest = hit.transform; }
        }
        return nearest;
    }

    void Shoot(Vector2 dir)
    {
        if (projectilePrefab == null)
            return;

        Transform spawnPoint = firePoint != null ? firePoint : transform;
        int shotCount = WeaponLevel >= 2 ? Mathf.Max(1, spreadShotCount) : 1;
        float totalSpread = shotCount > 1 ? spreadAngle * (shotCount - 1) : 0f;

        for (int i = 0; i < shotCount; i++)
        {
            float angle = shotCount == 1 ? 0f : -totalSpread * 0.5f + spreadAngle * i;
            Vector2 shotDir = Rotate(dir, angle);
            GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
            proj.GetComponent<Projectile>()?.Init(shotDir, projectileDamage, GetCurrentBlastRadius());
        }
    }

    public void ApplyWeaponPowerUp(PowerUpType powerUpType)
    {
        int requestedLevel = powerUpType switch
        {
            PowerUpType.RapidFire => 1,
            PowerUpType.SpreadShot => 2,
            PowerUpType.BlastShot => 3,
            _ => 0
        };

        if (requestedLevel <= 0)
            return;

        SetWeaponLevel(Mathf.Max(WeaponLevel, requestedLevel));
    }

    public void ResetWeaponLevel()
    {
        SetWeaponLevel(0);
    }

    void SetWeaponLevel(int level)
    {
        int clamped = Mathf.Clamp(level, 0, maxWeaponLevel);
        if (WeaponLevel == clamped)
            return;

        WeaponLevel = clamped;
        onWeaponLevelChanged?.Invoke(WeaponLevel);
    }

    float GetCurrentFireRate()
    {
        float baseRate = Mathf.Max(0.1f, fireRate);
        return WeaponLevel >= 1 ? baseRate * rapidFireMultiplier : baseRate;
    }

    float GetCurrentBlastRadius()
    {
        return WeaponLevel >= 3 ? blastRadius : 0f;
    }

    static Vector2 Rotate(Vector2 vector, float degrees)
    {
        float radians = degrees * Mathf.Deg2Rad;
        float sin = Mathf.Sin(radians);
        float cos = Mathf.Cos(radians);
        return new Vector2(
            vector.x * cos - vector.y * sin,
            vector.x * sin + vector.y * cos).normalized;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
