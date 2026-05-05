using UnityEngine;

public enum PowerUpType { Heal, SpeedBoost, RapidFire, SpreadShot, BlastShot, PowerSnack }

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
            case PowerUpType.PowerSnack:
                player.GetComponent<AutoAttack>()?.UpgradeWeaponLevel();
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

        float scale = type == PowerUpType.BlastShot || type == PowerUpType.PowerSnack ? 0.45f : 0.35f;
        transform.localScale = Vector3.one * scale;

        var visual = GetComponent<PowerUpFoodVisual>();
        if (visual == null)
            visual = gameObject.AddComponent<PowerUpFoodVisual>();
        visual.Apply(type, GetColor());
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
            PowerUpType.PowerSnack => new Color(1f, 0.55f, 0.12f),
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
            PowerUpType.PowerSnack => "Power snack",
            _ => "Power up"
        };
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, seekRadius);
    }
}

[DisallowMultipleComponent]
public class PowerUpFoodVisual : MonoBehaviour
{
    const string VisualRootName = "PowerSnackVisual";
    const string StripeName = "SnackStripe";
    const string SparkName = "SnackSpark";
    const string BiteName = "SnackBite";
    const string LeafName = "SnackLeaf";

    [SerializeField] float pulseAmount = 0.08f;
    [SerializeField] float pulseSpeed = 4f;
    [SerializeField] float spinSpeed = 70f;

    Vector3 _baseScale;
    Transform _visualRoot;

    void Awake()
    {
        _baseScale = transform.localScale;
    }

    public void Apply(PowerUpType type, Color accentColor)
    {
        _baseScale = transform.localScale;

        EnsureVisuals();
        SetRendererColor(gameObject, GetBodyColor(type, accentColor));
        SetRendererColor(_visualRoot.Find(StripeName)?.gameObject, GetStripeColor(type));
        SetRendererColor(_visualRoot.Find(SparkName)?.gameObject, accentColor);
        SetRendererColor(_visualRoot.Find(BiteName)?.gameObject, new Color(0.05f, 0.04f, 0.05f, 1f));
        SetRendererColor(_visualRoot.Find(LeafName)?.gameObject, new Color(0.2f, 0.9f, 0.35f, 1f));
    }

    void Update()
    {
        if (_baseScale == Vector3.zero)
            _baseScale = transform.localScale;

        float pulse = 1f + Mathf.Sin(Time.time * pulseSpeed) * pulseAmount;
        transform.localScale = _baseScale * pulse;

        if (_visualRoot != null)
            _visualRoot.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }

    void EnsureVisuals()
    {
        if (_visualRoot == null)
        {
            Transform existing = transform.Find(VisualRootName);
            _visualRoot = existing != null ? existing : new GameObject(VisualRootName).transform;
            _visualRoot.SetParent(transform, false);
        }

        CreateQuad(StripeName, new Vector3(0f, -0.05f, -0.14f), new Vector3(0.9f, 0.2f, 1f), 0f);
        CreateSphere(SparkName, new Vector3(-0.2f, 0.18f, -0.16f), Vector3.one * 0.18f);
        CreateSphere(BiteName, new Vector3(0.27f, 0.2f, -0.17f), Vector3.one * 0.22f);
        CreateQuad(LeafName, new Vector3(0.06f, 0.44f, -0.16f), new Vector3(0.28f, 0.1f, 1f), 28f);
    }

    GameObject CreateQuad(string name, Vector3 localPosition, Vector3 localScale, float zRotation)
    {
        Transform existing = _visualRoot.Find(name);
        GameObject go = existing != null ? existing.gameObject : GameObject.CreatePrimitive(PrimitiveType.Quad);
        go.name = name;
        go.transform.SetParent(_visualRoot, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.Euler(0f, 0f, zRotation);
        go.transform.localScale = localScale;
        RemoveCollider(go);
        return go;
    }

    GameObject CreateSphere(string name, Vector3 localPosition, Vector3 localScale)
    {
        Transform existing = _visualRoot.Find(name);
        GameObject go = existing != null ? existing.gameObject : GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.name = name;
        go.transform.SetParent(_visualRoot, false);
        go.transform.localPosition = localPosition;
        go.transform.localRotation = Quaternion.identity;
        go.transform.localScale = localScale;
        RemoveCollider(go);
        return go;
    }

    static void RemoveCollider(GameObject go)
    {
        var collider3D = go.GetComponent<Collider>();
        if (collider3D != null)
            Destroy(collider3D);
    }

    static void SetRendererColor(GameObject go, Color color)
    {
        if (go == null)
            return;

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = color;
    }

    static Color GetBodyColor(PowerUpType type, Color accentColor)
    {
        return type == PowerUpType.PowerSnack
            ? new Color(1f, 0.44f, 0.12f, 1f)
            : Color.Lerp(accentColor, Color.white, 0.18f);
    }

    static Color GetStripeColor(PowerUpType type)
    {
        return type == PowerUpType.PowerSnack
            ? new Color(1f, 0.92f, 0.48f, 1f)
            : new Color(1f, 1f, 1f, 0.85f);
    }
}
