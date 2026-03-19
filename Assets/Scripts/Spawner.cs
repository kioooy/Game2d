using System;
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

    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];
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

    [SerializeField] private ObjectPooler SnakePool;
    [SerializeField] private ObjectPooler SpiderPool;
    [SerializeField] private ObjectPooler BearPool;
    [SerializeField] private ObjectPooler ShamanPool;
    [SerializeField] private ObjectPooler ThiefPool;

    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;

    public float _timeBetweenWaves = 3f;
    public float _waveCooldown;
    public bool _isBetweenWaves = false;
<<<<<<< Updated upstream
=======
    [SerializeField] private TMP_Text countdownText;
    private bool _isCountdownActive = false;
    private int _currentSegmentIndex = 0;
    private int _enemiesSpawnedInSegment = 0;

    // Public properties để kiểm tra
    public int CurrentWaveIndex => _currentWaveIndex;
    public int TotalWaves => waves.Length;
    public int SpawnedEnemies => Mathf.RoundToInt(_spawnCounter);
    public int DestroyedEnemies => _enemiesRemoved;
>>>>>>> Stashed changes

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Snake, SnakePool},
            { EnemyType.Bear, BearPool},
            { EnemyType.Spider, SpiderPool},
            { EnemyType.Shaman, ShamanPool},
            { EnemyType.Thief, ThiefPool},
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
    }

    private void Start()
    {
        OnWaveChanged?.Invoke(_currentWaveIndex);
    }
    void Update()
    {
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if(_waveCooldown <= 0f)
            {
                int total = TotalWavesCount;
                if (total > 0) _currentWaveIndex = (_currentWaveIndex + 1) % total;
                
                _waveCounter++;
                OnWaveChanged?.Invoke(_currentWaveIndex);
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _currentSegmentIndex = 0;
                _enemiesSpawnedInSegment = 0;
                _isBetweenWaves = false;
            }
        }
        else
        {

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
            }
        }
    }


<<<<<<< Updated upstream
    private void SpawnEnemy() 
=======
        if (_isInitialDelay)
        {
            _isCountdownActive = true;
            int seconds = Mathf.CeilToInt(_initialTimer);
            countdownText.text = $"Wave begins at: {seconds}s";
            countdownText.gameObject.SetActive(true);
        }
        else if (_isBetweenWaves)
        {
            _isCountdownActive = true;
            int seconds = Mathf.CeilToInt(_waveCooldown);
            countdownText.text = $"Next wave at: {seconds}s";
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
}
