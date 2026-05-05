using UnityEngine;

public enum PowerUpType { Heal, SpeedBoost, RapidFire, SpreadShot, BlastShot }

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
        ApplyVisuals();

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

        ApplyToPlayer(other.gameObject);

        Destroy(gameObject);
    }

    public void Configure(PowerUpType newType)
    {
        type = newType;
        ApplyVisuals();
    }

    void ApplyToPlayer(GameObject player)
    {
        switch (type)
        {
            case PowerUpType.Heal:
                player.GetComponent<PlayerHealth>()?.Heal(healAmount);
                UIManager.Instance?.ShowMessage("Heal", 0.9f);
                break;
            case PowerUpType.SpeedBoost:
                player.GetComponent<PlayerController>()?.ApplySpeedBoost(boostDuration);
                UIManager.Instance?.ShowMessage("Speed boost", 0.9f);
                break;
            case PowerUpType.RapidFire:
            case PowerUpType.SpreadShot:
            case PowerUpType.BlastShot:
                player.GetComponent<AutoAttack>()?.ApplyWeaponPowerUp(type);
                UIManager.Instance?.ShowMessage(GetPickupLabel(), 1.1f);
                break;
        }

        ParticleBurst.Burst(transform.position, GetColor(), 8, 1.3f, 0.35f);
    }

    void ApplyVisuals()
    {
        var renderer = GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = GetColor();

        float scale = type == PowerUpType.BlastShot ? 0.45f : 0.35f;
        transform.localScale = Vector3.one * scale;
    }

    Color GetColor()
    {
        return type switch
        {
            PowerUpType.Heal => new Color(0.2f, 1f, 0.35f),
            PowerUpType.SpeedBoost => new Color(0.25f, 0.8f, 1f),
            PowerUpType.RapidFire => new Color(1f, 0.85f, 0.2f),
            PowerUpType.SpreadShot => new Color(0.95f, 0.35f, 1f),
            PowerUpType.BlastShot => new Color(1f, 0.45f, 0.1f),
            _ => Color.white
        };
    }

    string GetPickupLabel()
    {
        return type switch
        {
            PowerUpType.RapidFire => "Weapon: rapid fire",
            PowerUpType.SpreadShot => "Weapon: spread shot",
            PowerUpType.BlastShot => "Weapon: blast shot",
            _ => "Power up"
        };
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, seekRadius);
    }
}
