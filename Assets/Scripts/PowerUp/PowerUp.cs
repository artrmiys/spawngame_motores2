using UnityEngine;

public enum PowerUpType { Heal, SpeedBoost }

public class PowerUp : MonoBehaviour
{
    [SerializeField] PowerUpType type          = PowerUpType.Heal;
    [SerializeField] float       seekRadius    = 4f;
    [SerializeField] float       seekSpeed     = 2.5f;
    [SerializeField] int         healAmount    = 2;
    [SerializeField] float       boostDuration = 5f;

    Transform _player;
    bool      _seeking;

    void Start()
    {
        if (RemoteConfigManager.Instance != null && !RemoteConfigManager.Instance.PowerupEnabled)
        {
            Destroy(gameObject);
            return;
        }
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
    }

    void Update()
    {
        if (_player == null) return;

        float dist = Vector2.Distance(transform.position, _player.position);

        if (dist <= seekRadius)
            _seeking = true;

        if (_seeking)
        {
            transform.position = Vector2.MoveTowards(
                transform.position, _player.position, seekSpeed * Time.deltaTime);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (type == PowerUpType.Heal)
            other.GetComponent<PlayerHealth>()?.Heal(healAmount);
        else
            other.GetComponent<PlayerController>()?.ApplySpeedBoost(boostDuration);

        Destroy(gameObject);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, seekRadius);
    }
}
