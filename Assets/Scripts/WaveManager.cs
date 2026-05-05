using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float      spawnRadius  = 9f;
    [SerializeField] Vector2    playAreaHalfExtents = new Vector2(5.1f, 11.1f);
    [SerializeField] float      spawnEdgeInset = 0.35f;
    [SerializeField] float      minPlayerSpawnDistance = 4f;
    [SerializeField] int        baseEnemies  = 5;
    [SerializeField] int        levelWaveCount = 5;
    [SerializeField] string     levelTitle = "Level";
    [SerializeField] bool       showSymbolDoor = true;

    [Header("Enemy variants")]
    [SerializeField] int        fastEnemyEvery = 4;
    [SerializeField] float      fastEnemySpeedMultiplier = 1.75f;
    [SerializeField] int        fastEnemyMaxHp = 1;
    [SerializeField] float      fastEnemyScaleMultiplier = 1.2f;
    [SerializeField] Color      fastEnemyColor = new Color(1f, 0.75f, 0.15f);

    int   _totalWaves;
    int   _currentWave;
    int   _enemiesAlive;
    int   _enemiesLeftToSpawn;
    float _spawnInterval;
    bool  _waveActive;
    bool  _doorPassed = false;
    int   _spawnSequence;
    Transform _player;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    IEnumerator Start()
    {
        // Wait for RemoteConfig
        yield return new WaitUntil(() =>
            RemoteConfigManager.Instance == null || RemoteConfigManager.Instance.IsReady);

        _totalWaves    = levelWaveCount;
        _spawnInterval = RemoteConfigManager.Instance?.SpawnInterval ?? 2f;
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;

        UIManager.Instance?.SetWave(0, _totalWaves);
        UIManager.Instance?.ShowMessage(levelTitle, 1.5f);
        yield return new WaitForSeconds(1f);
        StartCoroutine(StartNextWave());
    }

    IEnumerator StartNextWave()
    {
        _currentWave++;
        if (_currentWave > _totalWaves)
        {
            GameManager.Instance?.Win();
            yield break;
        }

        int count = baseEnemies + (_currentWave - 1) * 2;
        _enemiesLeftToSpawn = count;
        _enemiesAlive       = count;
        _waveActive         = true;

        UIManager.Instance?.SetWave(_currentWave, _totalWaves);
        UIManager.Instance?.ShowMessage($"Wave {_currentWave}");

        while (_enemiesLeftToSpawn > 0)
        {
            SpawnEnemy();
            _enemiesLeftToSpawn--;
            yield return new WaitForSeconds(_spawnInterval);
        }
    }

    void SpawnEnemy()
    {
        if (enemyPrefab == null)
            return;

        Vector3 pos = GetSpawnPosition();
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        _spawnSequence++;

        if (fastEnemyEvery > 0 && _spawnSequence % fastEnemyEvery == 0)
            ConfigureFastEnemy(enemy);
    }

    Vector3 GetSpawnPosition()
    {
        if (playAreaHalfExtents.x <= 0f || playAreaHalfExtents.y <= 0f)
            return Random.insideUnitCircle.normalized * spawnRadius;

        Vector3 candidate = Vector3.zero;
        for (int i = 0; i < 12; i++)
        {
            candidate = GetEdgeSpawnPosition();
            if (_player == null || Vector2.Distance(candidate, _player.position) >= minPlayerSpawnDistance)
                return candidate;
        }

        return candidate;
    }

    Vector3 GetEdgeSpawnPosition()
    {
        float xLimit = Mathf.Max(0.5f, playAreaHalfExtents.x - spawnEdgeInset);
        float yLimit = Mathf.Max(0.5f, playAreaHalfExtents.y - spawnEdgeInset);

        switch (Random.Range(0, 4))
        {
            case 0: return new Vector3(Random.Range(-xLimit, xLimit), yLimit, 0f);
            case 1: return new Vector3(Random.Range(-xLimit, xLimit), -yLimit, 0f);
            case 2: return new Vector3(-xLimit, Random.Range(-yLimit, yLimit), 0f);
            default: return new Vector3(xLimit, Random.Range(-yLimit, yLimit), 0f);
        }
    }

    void ConfigureFastEnemy(GameObject enemy)
    {
        if (enemy == null)
            return;

        enemy.name = "FastFragileEnemy";
        enemy.transform.localScale *= fastEnemyScaleMultiplier;
        enemy.GetComponent<EnemyAI>()?.SetSpeedMultiplier(fastEnemySpeedMultiplier);
        enemy.GetComponent<EnemyHealth>()?.SetMaxHealth(fastEnemyMaxHp);

        var renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = fastEnemyColor;

        ParticleBurst.Burst(enemy.transform.position, fastEnemyColor, 6, 1.1f, 0.35f);
    }

    public void OnEnemyKilled()
    {
        _enemiesAlive--;
        if (_enemiesAlive <= 0 && _waveActive)
        {
            _waveActive = false;
            StartCoroutine(WaveEndDelay());
        }
    }

    IEnumerator WaveEndDelay()
    {
        UIManager.Instance?.ShowMessage("Wave cleared!");
        yield return new WaitForSeconds(2f);

        if (showSymbolDoor && !_doorPassed && _currentWave < _totalWaves)
        {
            yield return StartCoroutine(ShowSymbolDoorSafely());
        }

        StartCoroutine(StartNextWave());
    }

    IEnumerator ShowSymbolDoorSafely()
    {
        // Use reflection to avoid .csproj compile issues
        // This allows Symbol Door to work even if .csproj is outdated

        System.Type doorType = System.Type.GetType("SymbolDoor");
        if (doorType == null)
        {
            Debug.LogWarning("[WaveManager] SymbolDoor not compiled yet");
            yield break;
        }

        GameObject doorObj = GameObject.Find("SymbolDoor");
        if (doorObj == null)
        {
            Debug.LogWarning("[WaveManager] SymbolDoor GameObject not in scene");
            yield break;
        }

        var doorComponent = doorObj.GetComponent(doorType);
        if (doorComponent == null)
        {
            Debug.LogWarning("[WaveManager] SymbolDoor component missing");
            yield break;
        }

        Time.timeScale = 0f;

        // Prepare puzzle data
        var symbols = new List<string> { "☀️", "🌙", "🕐" };
        var meanings = new List<string> { "день", "ночь", "время" };
        var pairs = new Dictionary<string, string>
        {
            { "☀️", "день" },
            { "🌙", "ночь" },
            { "🕐", "время" }
        };

        // Call ShowPuzzle via reflection
        var showMethod = doorType.GetMethod("ShowPuzzle", new System.Type[]
        {
            typeof(List<string>),
            typeof(List<string>),
            typeof(Dictionary<string, string>)
        });

        if (showMethod != null)
        {
            showMethod.Invoke(doorComponent, new object[] { symbols, meanings, pairs });
        }

        // Wait for door to finish
        yield return new WaitUntil(() => !doorObj.activeSelf);

        Time.timeScale = 1f;
        _doorPassed = true;
        UIManager.Instance?.ShowMessage("Door passed!", 1f);
    }
}
