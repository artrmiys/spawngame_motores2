using UnityEngine;

public class PowerUpSpawner : MonoBehaviour
{
    [SerializeField] GameObject healPrefab;
    [SerializeField] GameObject speedPrefab;
    [SerializeField] float      spawnInterval = 15f;
    [SerializeField] float      spawnRadius   = 7f;

    void Start() => InvokeRepeating(nameof(SpawnRandom), spawnInterval, spawnInterval);

    void SpawnRandom()
    {
        if (RemoteConfigManager.Instance != null && !RemoteConfigManager.Instance.PowerupEnabled)
            return;

        Vector2 pos = Random.insideUnitCircle * spawnRadius;
        GameObject prefab = Random.value > 0.5f ? healPrefab : speedPrefab;
        if (prefab != null)
            Instantiate(prefab, pos, Quaternion.identity);
    }
}
