using System;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public static event Action<int> OnWaveChanged;
    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private int _waveCounter = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];
    private float _spawnTimer;
    private float _spawnCounter;
    private int _enemiesRemoved;
    // NEW: Add initial delay before first wave
    // thoi gian choi game truoc khi bat dau wave dau tien khoang x giay
    public float _initialDelay = 25f;
    private float _initialTimer;
    private bool _isInitialDelay = true;
    [SerializeField] private ObjectPooler SnakePool;
    [SerializeField] private ObjectPooler SpiderPool;
    [SerializeField] private ObjectPooler BearPool;
    [SerializeField] private ObjectPooler ShamanPool;
    [SerializeField] private ObjectPooler ThiefPool;
    [SerializeField] private ObjectPooler HarpoonFishPool;
    [SerializeField] private ObjectPooler LancerPool;
    private Dictionary<EnemyType, ObjectPooler> _poolDictionary;
    public float _timeBetweenWaves = 15f;
    public float _waveCooldown;
    public bool _isBetweenWaves = false;

    private void Awake()
    {
        _poolDictionary = new Dictionary<EnemyType, ObjectPooler>()
        {
            { EnemyType.Snake, SnakePool},
            { EnemyType.Bear, BearPool},
            { EnemyType.Spider, SpiderPool},
            { EnemyType.Shaman, ShamanPool},
            { EnemyType.Thief, ThiefPool},
            { EnemyType.HarpoonFish, HarpoonFishPool},
            { EnemyType.Lancer, LancerPool},
        };
    }

    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed -= HandleEnemyDestroyed;
    }

    private void Start()
    {
        // Start with initial delay
        _initialTimer = _initialDelay;
        _isInitialDelay = true;
        OnWaveChanged?.Invoke(_currentWaveIndex);
    }

    void Update()
    {
        // NEW: Handle initial delay
        if (_isInitialDelay)
        {
            _initialTimer -= Time.deltaTime;
            if (_initialTimer <= 0f)
            {
                _isInitialDelay = false;
                // Start first wave immediately after delay
                _isBetweenWaves = false;
            }
            return; // Skip wave spawning during initial delay
        }

        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if (_waveCooldown <= 0f)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
                _waveCounter++;
                OnWaveChanged?.Invoke(_currentWaveIndex);
                _spawnCounter = 0;
                _enemiesRemoved = 0;
                _spawnTimer = 0f;
                _isBetweenWaves = false;
            }
        }
        else
        {
            _spawnTimer -= Time.deltaTime;
            if (_spawnTimer <= 0 && _spawnCounter < CurrentWave.enemiesPerWave)
            {
                _spawnTimer = CurrentWave.spawnInterval;
                SpawnEnemy();
                _spawnCounter++;
            }
            else if (_spawnCounter >= CurrentWave.enemiesPerWave && _enemiesRemoved >= CurrentWave.enemiesPerWave)
            {
                _isBetweenWaves = true;
                _waveCooldown = _timeBetweenWaves;
            }
        }
    }

    private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            float healthMultiplication = 1f + (_waveCounter * 0.09f); // +9% sinh luc cho ke dich moi wave
            Enemy enemy = spawnedObject.GetComponent<Enemy>();
            enemy.Initialize(healthMultiplication);
            spawnedObject.SetActive(true);
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }

    private void HandleEnemyDestroyed(Enemy enemy)
    {
        _enemiesRemoved++;
    }

    // NEW: Optional public method to check if initial delay is active
    public bool IsInitialDelayActive()
    {
        return _isInitialDelay;
    }

    // NEW: Optional public method to get remaining initial delay time
    public float GetRemainingInitialDelay()
    {
        return Mathf.Max(0f, _initialTimer);
    }

    // ===== THÊM CODE MỚI Ở ĐÂY =====
    // Thêm phương thức lấy thời gian chờ còn lại
    public float GetRemainingWaitTime()
    {
        if (_isInitialDelay)
        {
            return _initialTimer;
        }
        else if (_isBetweenWaves)
        {
            return _waveCooldown;
        }
        else
        {
            return 0f;
        }
    }

    // Thêm phương thức kiểm tra đang trong thời gian chờ
    public bool IsInWaitingPhase()
    {
        return _isInitialDelay || _isBetweenWaves;
    }

    // Thêm phương thức lấy loại thời gian chờ (1: Initial, 2: Between Waves)
    public int GetCurrentWaitType()
    {
        if (_isInitialDelay) return 1;
        else if (_isBetweenWaves) return 2;
        else return 0;
    }
    // ===== KẾT THÚC CODE MỚI =====
}