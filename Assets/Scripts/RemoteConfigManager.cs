using UnityEngine;

public class RemoteConfigManager : MonoBehaviour
{
    public static RemoteConfigManager Instance { get; private set; }

    [Header("Tuning")]
    [SerializeField] int waveCount = 5;
    [SerializeField] bool powerupEnabled = true;
    [SerializeField] float enemySpeed = 3.35f;
    [SerializeField] float playerSpeed = 5f;
    [SerializeField] float spawnInterval = 0.76f;

    public int WaveCount => waveCount;
    public bool PowerupEnabled => powerupEnabled;
    public float EnemySpeed => enemySpeed;
    public float PlayerSpeed => playerSpeed;
    public float SpawnInterval => spawnInterval;
    public bool IsReady { get; private set; }

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        IsReady = true;
    }
}
