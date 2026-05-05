using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] GameObject healPrefab;
    [SerializeField] GameObject speedPrefab;
    [SerializeField] GameObject rapidFirePrefab;
    [SerializeField] GameObject spreadShotPrefab;
    [SerializeField] GameObject blastShotPrefab;
    [SerializeField] float      spawnInterval = 15f;
    [SerializeField] Vector2    spawnHalfExtents = new Vector2(4.7f, 10.7f);

    void Start() => InvokeRepeating(nameof(SpawnRandom), spawnInterval, spawnInterval);

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
        if (roll < 0.18f) return PowerUpType.Heal;
        if (roll < 0.34f) return PowerUpType.SpeedBoost;
        if (roll < 0.60f) return PowerUpType.RapidFire;
        if (roll < 0.83f) return PowerUpType.SpreadShot;
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
            _ => null
        };

        if (selected != null)
            return selected;

        return speedPrefab != null ? speedPrefab : healPrefab;
    }
}
