using UnityEngine;

public class AutoAttack : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] float      attackRange    = 6f;
    [SerializeField] float      fireRate       = 1f;
    [SerializeField] Transform  firePoint;

    float _nextFireTime;

    void Update()
    {
        Transform target = FindNearest();
        if (target == null) return;

        Vector2 dir = (target.position - transform.position).normalized;

        if (Time.time >= _nextFireTime)
        {
            _nextFireTime = Time.time + 1f / fireRate;
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
        GameObject proj = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);
        proj.GetComponent<Projectile>()?.Init(dir);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
