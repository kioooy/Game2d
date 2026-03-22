using System;
<<<<<<< Updated upstream
=======
using System.Collections;
>>>>>>> Stashed changes
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveInScene
{
    public List<WaveSegment> segments;
    public int GetTotalEnemies()
    {
        int total = 0;
        if (segments == null) return 0;
        foreach (var s in segments) total += s.count;
        return total;
    }
}

public class Spawner : MonoBehaviour
{
    public static event Action<int> OnWaveChanged;
<<<<<<< Updated upstream
<<<<<<< Updated upstream
=======
>>>>>>> Stashed changes

    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];
<<<<<<< Updated upstream
=======
    [SerializeField] private bool useInSceneWaves = true; // Toggle this to edit waves right in the Inspector!
    [SerializeField] private List<WaveInScene> inSceneWaves; // Edit your waves here!
    [SerializeField] private WaveData[] waves; 
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    
    private int TotalEnemiesInCurrentWave {
        get {
            if (useInSceneWaves && inSceneWaves != null && inSceneWaves.Count > _currentWaveIndex)
                return inSceneWaves[_currentWaveIndex].GetTotalEnemies();
            
            if (!useInSceneWaves && waves != null && waves.Length > _currentWaveIndex)
                return waves[_currentWaveIndex].GetTotalEnemies();
                
            return 0;
        }
    }

    private int TotalWavesCount {
        get {
            if (useInSceneWaves) return inSceneWaves?.Count ?? 0;
            return waves?.Length ?? 0;
        }
    }

    private WaveSegment GetCurrentSegment()
    {
        if (useInSceneWaves)
        {
            if (inSceneWaves == null || inSceneWaves.Count <= _currentWaveIndex || inSceneWaves[_currentWaveIndex].segments == null || inSceneWaves[_currentWaveIndex].segments.Count <= _currentSegmentIndex)
                return null;
            return inSceneWaves[_currentWaveIndex].segments[_currentSegmentIndex];
        }
        
        if (waves == null || waves.Length <= _currentWaveIndex || waves[_currentWaveIndex].segments == null || waves[_currentWaveIndex].segments.Count <= _currentSegmentIndex)
            return null;
            
        return waves[_currentWaveIndex].segments[_currentSegmentIndex];
    }
>>>>>>> Stashed changes

    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;

=======

    // --- Spawn state ---
    private int _currentGroupIndex = 0;
    private int _spawnedInGroup = 0;
    private float _spawnTimer = 0f;
    private float _groupDelayTimer = 0f;
    private bool _waitingBetweenGroups = false;

    // Tổng số quái đã spawn / đã bị loại bỏ trong wave hiện tại
    private int _totalSpawnedThisWave = 0;
    private int _enemiesRemoved = 0;

    // --- Timing ---
    public float _initialDelay = 25f;
    private float _initialTimer;
    private bool _isInitialDelay = true;

    public float _timeBetweenWaves = 15f;
    public float _waveCooldown;
    public bool _isBetweenWaves = false;

    private bool _isCountdownActive = false;
    [SerializeField] private TMP_Text countdownText;

    // --- Pools ---
>>>>>>> Stashed changes
    [SerializeField] private ObjectPooler SnakePool;
    [SerializeField] private ObjectPooler SpiderPool;
    [SerializeField] private ObjectPooler BearPool;
    [SerializeField] private ObjectPooler ShamanPool;
    [SerializeField] private ObjectPooler ThiefPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
<<<<<<< Updated upstream

    public float _timeBetweenWaves = 3f;
    public float _waveCooldown;
    public bool _isBetweenWaves = false;
<<<<<<< Updated upstream
=======
    [SerializeField] private TMP_Text countdownText;
    private bool _isCountdownActive = false;
    private int _currentSegmentIndex = 0;
    private int _enemiesSpawnedInSegment = 0;
=======
>>>>>>> Stashed changes

    // --- Public properties ---
    public int CurrentWaveIndex => _currentWaveIndex;
    public int TotalWaves => waves.Length;
    public int SpawnedEnemies => _totalSpawnedThisWave;
    public int DestroyedEnemies => _enemiesRemoved;
>>>>>>> Stashed changes

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
<<<<<<< Updated upstream
            { EnemyType.Snake, SnakePool},
            { EnemyType.Bear, BearPool},
            { EnemyType.Spider, SpiderPool},
            { EnemyType.Shaman, ShamanPool},
            { EnemyType.Thief, ThiefPool},
=======
            { EnemyType.Snake,           SnakePool          },
            { EnemyType.Bear,            BearPool           },
            { EnemyType.Spider,          SpiderPool         },
            { EnemyType.Shaman,          ShamanPool         },
            { EnemyType.Thief,           ThiefPool          },
            { EnemyType.HarpoonFish,     HarpoonFishPool    },
            { EnemyType.Lancer,          LancerPool         },
            { EnemyType.HarpoonFishBoss, HarpoonFishBossPool},
            { EnemyType.ShamanBoss,      ShamanBossPool     },
            { EnemyType.BearBoss,        BearBossPool       },
>>>>>>> Stashed changes
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
<<<<<<< Updated upstream
=======
        Enemy.OnEnemyDestroyed  += HandleEnemyDestroyed;
>>>>>>> Stashed changes
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
<<<<<<< Updated upstream
=======
        Enemy.OnEnemyDestroyed  -= HandleEnemyDestroyed;
>>>>>>> Stashed changes
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_currentWaveIndex);
<<<<<<< Updated upstream
    }
    void Update()
    {
=======

        if (countdownText != null)
            countdownText.gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateCountdownDisplay();

        // --- Initial delay ---
        if (_isInitialDelay)
        {
            _initialTimer -= Time.deltaTime;
            if (_initialTimer <= 0f)
            {
                _isInitialDelay = false;
                StartWave();
            }
            return;
        }

        // --- Cooldown giữa các wave ---
>>>>>>> Stashed changes
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if(_waveCooldown <= 0f)
            {
                int total = TotalWavesCount;
                if (total > 0) _currentWaveIndex = (_currentWaveIndex + 1) % total;
                
                _waveCounter++;
                OnWaveChanged?.Invoke(_currentWaveIndex);
<<<<<<< Updated upstream
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _currentSegmentIndex = 0;
                _enemiesSpawnedInSegment = 0;
=======
>>>>>>> Stashed changes
                _isBetweenWaves = false;
                StartWave();
            }
            return;
        }

        // --- Spawning ---
        SpawnUpdate();
    }

    /// <summary>Khởi tạo trạng thái đầu wave mới.</summary>
    private void StartWave()
    {
        _currentGroupIndex = 0;
        _spawnedInGroup = 0;
        _totalSpawnedThisWave = 0;
        _enemiesRemoved = 0;
        _spawnTimer = 0f;
        _groupDelayTimer = 0f;
        _waitingBetweenGroups = false;

        AudioManager.Instance?.PlayWaveStart();
    }

    /// <summary>Xử lý logic spawn từng group quái theo thứ tự.</summary>
    private void SpawnUpdate()
    {
        if (CurrentWave.enemyGroups == null || CurrentWave.enemyGroups.Length == 0)
        {
<<<<<<< Updated upstream

            _spawnTimer -= Time.deltaTime;
            int totalEnemiesInWave = TotalEnemiesInCurrentWave;

            if (_spawnTimer <= 0 && _spawnCounter < totalEnemiesInWave)
            {
                WaveSegment currentSegment = GetCurrentSegment();
                if (currentSegment == null)
                {
                    Debug.LogWarning("[Spawner] Missing Wave/Segment data! Please configure waves in the Spawner Inspector.");
                    _spawnTimer = 1f; // Retry in 1s
                    return;
                }
                
                _spawnTimer = currentSegment.spawnInterval;
                SpawnEnemy(currentSegment.enemyType);
                
                _spawnCounter++;
<<<<<<< Updated upstream

=======
                _enemiesSpawnedInSegment++;

                if (_enemiesSpawnedInSegment >= currentSegment.count)
                {
                    _currentSegmentIndex++;
                    _enemiesSpawnedInSegment = 0;
                }
>>>>>>> Stashed changes
            }
            else if (_spawnCounter >= totalEnemiesInWave && _enemiesRemoved >= totalEnemiesInWave)
            {

                _isBetweenWaves = true;
          _waveCooldown = _timeBetweenWaves;
=======
            EndWave();
            return;
        }

        // Đã hết tất cả groups
        if (_currentGroupIndex >= CurrentWave.enemyGroups.Length)
        {
            // Chờ cho đến khi tất cả quái bị loại bỏ rồi mới kết thúc wave
            if (_enemiesRemoved >= CurrentWave.TotalEnemies)
                EndWave();
            return;
        }

        EnemyGroup group = CurrentWave.enemyGroups[_currentGroupIndex];

        // --- Đang chờ delay giữa các group ---
        if (_waitingBetweenGroups)
        {
            _groupDelayTimer -= Time.deltaTime;
            if (_groupDelayTimer <= 0f)
                _waitingBetweenGroups = false;
            return;
        }

        // --- Spawn từng con trong group ---
        _spawnTimer -= Time.deltaTime;
        if (_spawnTimer <= 0f && _spawnedInGroup < group.count)
        {
            _spawnTimer = group.spawnInterval;
            SpawnEnemy(group.enemyType);
            _spawnedInGroup++;
        }

        // Group này đã spawn đủ → sang group tiếp theo
        if (_spawnedInGroup >= group.count)
        {
            _currentGroupIndex++;
            _spawnedInGroup = 0;
            _spawnTimer = 0f;

            if (_currentGroupIndex < CurrentWave.enemyGroups.Length)
            {
                _waitingBetweenGroups = true;
                _groupDelayTimer = CurrentWave.delayBetweenGroups;
>>>>>>> Stashed changes
            }
        }
    }

<<<<<<< Updated upstream
=======
    private void EndWave()
    {
        _isBetweenWaves = true;
        _waveCooldown = _timeBetweenWaves;
    }

    private void SpawnEnemy(EnemyType type)
    {
        if (!_poolDictionary.TryGetValue(type, out var pool)) return;

        GameObject obj = pool.GetPooledObject();
        if (obj == null) return;

        // Đặt z = 0 rõ ràng để tránh Z-fighting gây chớp nháy
        obj.transform.position = new Vector3(transform.position.x, transform.position.y, 0f);

        float healthMultiplier = 1f + (_waveCounter * 0.09f);
        Enemy enemy = obj.GetComponent<Enemy>();
        enemy.Initialize(healthMultiplier);
        obj.SetActive(true);

        _totalSpawnedThisWave++;
    }

    private void HandleEnemyReachedEnd(EnemyData data) => _enemiesRemoved++;
    private void HandleEnemyDestroyed(Enemy enemy) => _enemiesRemoved++;

    // --- Public API cho UIController ---
    public bool IsInitialDelayActive() => _isInitialDelay;
    public float GetRemainingInitialDelay() => Mathf.Max(0f, _initialTimer);
    public float GetRemainingWaveCooldown() => Mathf.Max(0f, _waveCooldown);
    public bool IsCountdownActive() => _isCountdownActive;

    private void UpdateCountdownDisplay()
    {
        if (countdownText == null) return;
>>>>>>> Stashed changes

<<<<<<< Updated upstream
    private void SpawnEnemy() 
=======
        if (_isInitialDelay)
        {
            _isCountdownActive = true;
            countdownText.text = $"Wave begins at: {Mathf.CeilToInt(_initialTimer)}s";
            countdownText.gameObject.SetActive(true);
        }
        else if (_isBetweenWaves)
        {
            _isCountdownActive = true;
            countdownText.text = $"Next wave at: {Mathf.CeilToInt(_waveCooldown)}s";
            countdownText.gameObject.SetActive(true);
        }
        else
        {
            if (_isCountdownActive)
            {
                _isCountdownActive = false;
                countdownText.gameObject.SetActive(false);
            }
        }
    }
<<<<<<< Updated upstream

    private void SpawnEnemy(EnemyType type)
>>>>>>> Stashed changes
    {
        if (_poolDictionary.TryGetValue(type, out var pool))
        {
            if (pool == null)
            {
                Debug.LogError($"[Spawner] Pool for {type} is NOT assigned in the Spawner's Inspector! Please go to the Spawner object and drag the corresponding ObjectPooler into the slot.");
                return;
            }

            GameObject spawnedObject = pool.GetPooledObject();
            if (spawnedObject == null) return;
            spawnedObject.transform.position = transform.position;
            spawnedObject.SetActive(true);

            Debug.Log($"<color=cyan>[Spawner] Wave {_currentWaveIndex + 1} (Seg {_currentSegmentIndex + 1}): {type} ({_spawnCounter + 1}/{TotalEnemiesInCurrentWave})</color>");
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }
=======
>>>>>>> Stashed changes
}
