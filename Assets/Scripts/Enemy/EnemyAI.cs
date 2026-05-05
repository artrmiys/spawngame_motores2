using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(EnemyHealth))]
public class EnemyAI : MonoBehaviour
{
    enum State { Chase, Attack, Dead }

    [Header("Movement")]
    [SerializeField] float attackRange = 0.8f;

    [Header("Combat")]
    [SerializeField] int meleeDamage = 1;
    [SerializeField] float attackCooldown = 1.2f;
    [SerializeField] float aggressiveAttackCooldownMultiplier = 0.65f;

    State _state = State.Chase;
    Rigidbody2D _rb;
    EnemyHealth _health;
    Transform _player;

    float _baseSpeed = 3f;
    float _speedMultiplier = 1f;
    float _nextAttackTime;
    bool _aggressiveChaser;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _health = GetComponent<EnemyHealth>();
        _health.onDied.AddListener(OnDied);
    }

    void Start()
    {
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        SyncBaseSpeed();
        EnsureVisuals();
    }

    void Update()
    {
        if (_state == State.Dead)
            return;

        if (_player == null)
            _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (_player == null)
        {
            _rb.velocity = Vector2.zero;
            return;
        }

        SyncBaseSpeed();

        if (PlayerInRange(attackRange))
            _state = State.Attack;
        else
            _state = State.Chase;

        if (_state == State.Attack)
            TickAttack();
        else
            TickChase();
    }

    void TickChase()
    {
        MoveTowards(_player.position, CurrentSpeed);
    }

    void TickAttack()
    {
        _rb.velocity = Vector2.zero;

        if (Time.time < _nextAttackTime)
            return;

        _nextAttackTime = Time.time + CurrentAttackCooldown;
        _player.GetComponent<PlayerHealth>()?.TakeDamage(meleeDamage);
    }

    void MoveTowards(Vector2 target, float speed)
    {
        Vector2 dir = (target - (Vector2)transform.position).normalized;
        _rb.velocity = dir * speed;
        if (dir != Vector2.zero)
            transform.up = dir;
    }

    bool PlayerInRange(float range) =>
        Vector2.Distance(transform.position, _player.position) <= range;

    float CurrentSpeed => _baseSpeed * _speedMultiplier;

    float CurrentAttackCooldown => _aggressiveChaser
        ? attackCooldown * aggressiveAttackCooldownMultiplier
        : attackCooldown;

    void SyncBaseSpeed()
    {
        if (RemoteConfigManager.Instance != null && RemoteConfigManager.Instance.IsReady)
            _baseSpeed = Mathf.Max(3.35f, RemoteConfigManager.Instance.EnemySpeed);
    }

    public void SetSpeedMultiplier(float multiplier)
    {
        _speedMultiplier = Mathf.Max(0.1f, multiplier);
    }

    public void SetAggressiveChaser(bool value)
    {
        _aggressiveChaser = value;
        _state = State.Chase;
        EnsureVisuals();
    }

    void EnsureVisuals()
    {
        var renderer = GetComponent<Renderer>();
        Color color = renderer != null ? renderer.material.color : new Color(1f, 0.25f, 0.25f);
        ActorVisualFx.VisualRole role = _aggressiveChaser
            ? ActorVisualFx.VisualRole.FastEnemy
            : ActorVisualFx.VisualRole.Enemy;
        ActorVisualFx.Ensure(gameObject, role, color);
    }

    void OnDied()
    {
        _state = State.Dead;
        _rb.velocity = Vector2.zero;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
