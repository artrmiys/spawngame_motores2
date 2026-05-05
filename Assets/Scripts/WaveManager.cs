using System.Collections;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager Instance { get; private set; }

    [SerializeField] GameObject enemyPrefab;
    [SerializeField] float spawnRadius = 8f;
    [SerializeField] Vector2 playAreaHalfExtents = new Vector2(4.35f, 8.35f);
    [SerializeField] float spawnEdgeInset = 0.85f;
    [SerializeField] float minPlayerSpawnDistance = 3.5f;
    [SerializeField] float maxSpawnInterval = 0.76f;
    [SerializeField] float firstWaveDelay = 0.35f;
    [SerializeField] float waveEndDelay = 0.75f;
    [SerializeField] int baseEnemies = 5;
    [SerializeField] int levelWaveCount = 5;
    [SerializeField] string levelTitle = "Level";
    [SerializeField] bool showSymbolDoor = true;

    [Header("Enemy variants")]
    [SerializeField] int fastEnemyEvery = 4;
    [SerializeField] float fastEnemySpeedMultiplier = 1.75f;
    [SerializeField] int fastEnemyMaxHp = 1;
    [SerializeField] float fastEnemyScaleMultiplier = 1.2f;
    [SerializeField] Color fastEnemyColor = new Color(1f, 0.75f, 0.15f);

    int _totalWaves;
    int _currentWave;
    int _enemiesAlive;
    int _enemiesLeftToSpawn;
    float _spawnInterval;
    bool _waveActive;
    bool _doorPassed;
    int _spawnSequence;
    Transform _player;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    IEnumerator Start()
    {
        yield return new WaitUntil(() =>
            RemoteConfigManager.Instance == null || RemoteConfigManager.Instance.IsReady);

        _totalWaves = levelWaveCount;
        _spawnInterval = GetSpawnInterval();
        _player = GameObject.FindGameObjectWithTag("Player")?.transform;
        ArenaVisualDirector.EnsureInScene(playAreaHalfExtents, levelTitle);

        UIManager.Instance?.SetWave(0, _totalWaves);
        UIManager.Instance?.ShowMessage(levelTitle, 1.5f);
        yield return new WaitForSeconds(firstWaveDelay);
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
        _enemiesAlive = count;
        _waveActive = true;

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

        Vector3 pos = ClampToPlayArea(GetSpawnPosition());
        GameObject enemy = Instantiate(enemyPrefab, pos, Quaternion.identity);
        _spawnSequence++;

        if (fastEnemyEvery > 0 && _spawnSequence % fastEnemyEvery == 0)
            ConfigureFastEnemy(enemy);
    }

    Vector3 GetSpawnPosition()
    {
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
        Vector2 limits = GetVisibleSpawnHalfExtents();
        float xLimit = Mathf.Max(0.5f, limits.x);
        float yLimit = Mathf.Max(0.5f, limits.y);

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
        enemy.GetComponent<EnemyAI>()?.SetAggressiveChaser(true);
        enemy.GetComponent<EnemyHealth>()?.SetMaxHealth(fastEnemyMaxHp);

        var renderer = enemy.GetComponent<Renderer>();
        if (renderer != null)
            renderer.material.color = fastEnemyColor;

        ParticleBurst.Burst(enemy.transform.position, fastEnemyColor, 6, 1.1f, 0.35f);
    }

    float GetSpawnInterval()
    {
        float configured = RemoteConfigManager.Instance?.SpawnInterval ?? maxSpawnInterval;
        return Mathf.Clamp(Mathf.Min(configured, maxSpawnInterval), 0.15f, 5f);
    }

    Vector2 GetVisibleSpawnHalfExtents()
    {
        Vector2 limits = new Vector2(
            Mathf.Max(0.5f, playAreaHalfExtents.x - spawnEdgeInset),
            Mathf.Max(0.5f, playAreaHalfExtents.y - spawnEdgeInset));
        float legacyRadiusLimit = Mathf.Max(0.5f, spawnRadius - spawnEdgeInset);
        limits.x = Mathf.Min(limits.x, legacyRadiusLimit);
        limits.y = Mathf.Min(limits.y, legacyRadiusLimit);

        Camera cam = Camera.main;
        if (cam != null && cam.orthographic)
        {
            float visibleY = cam.orthographicSize - spawnEdgeInset;
            float visibleX = cam.orthographicSize * cam.aspect - spawnEdgeInset;
            limits.x = Mathf.Min(limits.x, Mathf.Max(0.5f, visibleX));
            limits.y = Mathf.Min(limits.y, Mathf.Max(0.5f, visibleY));
        }

        return limits;
    }

    Vector3 ClampToPlayArea(Vector3 position)
    {
        Vector2 limits = GetVisibleSpawnHalfExtents();
        position.x = Mathf.Clamp(position.x, -limits.x, limits.x);
        position.y = Mathf.Clamp(position.y, -limits.y, limits.y);
        position.z = 0f;
        return position;
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
        yield return new WaitForSeconds(waveEndDelay);

        if (showSymbolDoor && !_doorPassed && _currentWave < _totalWaves)
            yield return StartCoroutine(ShowSymbolDoorSafely());

        StartCoroutine(StartNextWave());
    }

    IEnumerator ShowSymbolDoorSafely()
    {
        Time.timeScale = 0f;

        SymbolDoor door = GetOrCreateSymbolDoor();
        var puzzle = SymbolDoorPuzzleBuilder.CreateDefault().Build();
        door.ShowPuzzle(puzzle.Symbols, puzzle.Meanings, puzzle.CorrectPairs);

        yield return new WaitUntil(() => door == null || !door.gameObject.activeSelf);

        Time.timeScale = 1f;
        _doorPassed = true;
        UIManager.Instance?.ShowMessage("Door passed!", 1f);
    }

    SymbolDoor GetOrCreateSymbolDoor()
    {
        SymbolDoor door = FindObjectOfType<SymbolDoor>(true);
        if (door != null)
        {
            door.gameObject.SetActive(true);
            return door;
        }

        if (FindObjectOfType<SymbolDoorEventManager>(true) == null)
            new GameObject("SymbolDoorEventManager").AddComponent<SymbolDoorEventManager>();

        return new GameObject("SymbolDoor").AddComponent<SymbolDoor>();
    }
}
