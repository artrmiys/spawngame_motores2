using System.Collections;
using UnityEngine;

// FSM: Patrol → Chase → Attack → Dead
[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    enum State { Patrol, Chase, Attack, Dead }

    [Header("Movement")]
    [SerializeField] float detectionRange = 5f;
    [SerializeField] float attackRange    = 0.8f;
    [SerializeField] float patrolRadius   = 3f;

    [Header("Combat")]
    [SerializeField] int   meleeDamage   = 1;
    [SerializeField] float attackCooldown = 1.2f;

    State      _state = State.Patrol;
    Rigidbody2D _rb;
    EnemyHealth _health;
    Transform   _player;

    float _baseSpeed = 3f;
    float _speedMultiplier = 1f;
    float _nextAttackTime;
    Vector2 _patrolTarget;

    void Awake()
    {
        _rb     = GetComponent<Rigidbody2D>();
        _health = GetComponent<EnemyHealth>();
        _health.onDied.AddListener(OnDied);
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SyncBaseSpeed();
        SetNewPatrolTarget();
    }

    void Update()
    {
        if (_state == State.Dead) return;

        SyncBaseSpeed();

        switch (_state)
        {
            case State.Patrol: TickPatrol(); break;
            case State.Chase:  TickChase();  break;
            case State.Attack: TickAttack(); break;
        }
    }

    // ── Patrol ──────────────────────────────────────────────────────────────
    void TickPatrol()
    {
        MoveTowards(_patrolTarget, CurrentSpeed * 0.6f);

        if (Vector2.Distance(transform.position, _patrolTarget) < 0.3f)
            SetNewPatrolTarget();

        if (_player != null && PlayerInRange(detectionRange))
            _state = State.Chase;
    }

    void SetNewPatrolTarget()
    {
        Vector2 origin = transform.position;
        _patrolTarget = origin + Random.insideUnitCircle * patrolRadius;
    }

    // ── Chase ────────────────────────────────────────────────────────────────
    void TickChase()
    {
        if (_player == null) { _state = State.Patrol; return; }

        MoveTowards(_player.position, CurrentSpeed);

        if (PlayerInRange(attackRange))
            _state = State.Attack;
        else if (!PlayerInRange(detectionRange * 1.5f))
            _state = State.Patrol;
    }

    // ── Attack ───────────────────────────────────────────────────────────────
    void TickAttack()
    {
        _rb.velocity = Vector2.zero;

        if (!PlayerInRange(attackRange)) { _state = State.Chase; return; }

        if (Time.time >= _nextAttackTime)
        {
            _nextAttackTime = Time.time + attackCooldown;
            _player?.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────
    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = ((Vector2)target - (Vector2)transform.position).normalized;
        _rb.velocity = dir * speed;
        if (dir != Vector2.zero) transform.up = dir;
    }

    bool PlayerInRange(float range) =>
        _player != null && Vector2.Distance(transform.position, _player.position) <= range;

    float CurrentSpeed => _baseSpeed * _speedMultiplier;

    void SyncBaseSpeed()
    {
        if (RemoteConfigManager.Instance != null && RemoteConfigManager.Instance.IsReady)
            _baseSpeed = RemoteConfigManager.Instance.EnemySpeed;
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    void OnDied() => _state = State.Dead;

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
