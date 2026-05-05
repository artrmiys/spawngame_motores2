using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    const float FasterSpawnMultiplier = 0.75f;

    [SerializeField] GameObject healPrefab;
    [SerializeField] GameObject speedPrefab;
    [SerializeField] GameObject rapidFirePrefab;
    [SerializeField] GameObject spreadShotPrefab;
    [SerializeField] GameObject blastShotPrefab;
    [SerializeField] GameObject powerSnackPrefab;
    [SerializeField] float      spawnInterval = 8f;
    [SerializeField] Vector2    spawnHalfExtents = new Vector2(4.1f, 8.1f);

    void Start()
    {
        spawnInterval = GetEffectiveSpawnInterval(spawnInterval);
        InvokeRepeating(nameof(SpawnRandom), spawnInterval, spawnInterval);
    }

    public static float GetEffectiveSpawnInterval(float configuredInterval)
    {
        return Mathf.Clamp(configuredInterval * FasterSpawnMultiplier, 2.25f, 12f);
    }

    void SpawnRandom()
    {
        if (RemoteConfigManager.Instance != null && !RemoteConfigManager.Instance.PowerupEnabled)
            return;

        PowerUpType type = PickType();
        Vector2 pos = GetSpawnPosition();
        GameObject prefab = GetPrefab(type);
        if (prefab != null)
        {
            GameObject instance = Instantiate(prefab, pos, Quaternion.identity);
            instance.GetComponent<PowerUp>()?.Configure(type);
        }
    }

    PowerUpType PickType()
    {
        float roll = Random.value;
        if (roll < 0.234f) return PowerUpType.Heal;
        if (roll < 0.363f) return PowerUpType.SpeedBoost;
        if (roll < 0.636f) return PowerUpType.PowerSnack;
        if (roll < 0.773f) return PowerUpType.RapidFire;
        if (roll < 0.902f) return PowerUpType.SpreadShot;
        return PowerUpType.BlastShot;
    }

    Vector2 GetSpawnPosition()
    {
        float x = Random.Range(-spawnHalfExtents.x, spawnHalfExtents.x);
        float y = Random.Range(-spawnHalfExtents.y, spawnHalfExtents.y);
        return new Vector2(x, y);
    }

    GameObject GetPrefab(PowerUpType type)
    {
        GameObject selected = type switch
        {
            PowerUpType.Heal => healPrefab,
            PowerUpType.SpeedBoost => speedPrefab,
            PowerUpType.RapidFire => rapidFirePrefab,
            PowerUpType.SpreadShot => spreadShotPrefab,
            PowerUpType.BlastShot => blastShotPrefab,
            PowerUpType.PowerSnack => powerSnackPrefab,
            _ => null
        };

        if (selected != null)
            return selected;

        return speedPrefab != null ? speedPrefab : healPrefab;
    }
}
