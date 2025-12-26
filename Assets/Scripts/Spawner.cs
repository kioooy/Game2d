using System;
using System.Collections.Generic;
using TMPro;
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
    public float _initialDelay = 20f;
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
    [SerializeField] private TMP_Text countdownText;
    private bool _isCountdownActive = false;

    // Public properties để kiểm tra
    public int CurrentWaveIndex => _currentWaveIndex;
    public int TotalWaves => waves.Length;
    public int SpawnedEnemies => Mathf.RoundToInt(_spawnCounter);
    public int DestroyedEnemies => _enemiesRemoved;

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
        _initialTimer = _initialDelay;
        _isInitialDelay = true;
        OnWaveChanged?.Invoke(_currentWaveIndex);

        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        UpdateCountdownDisplay();

        if (_isInitialDelay)
        {
            _initialTimer -= Time.deltaTime;
            if (_initialTimer <= 0f)
            {
                _isInitialDelay = false;
                _isBetweenWaves = false;
            }
            return;
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

    private void UpdateCountdownDisplay()
    {
        if (countdownText == null) return;

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
            countdownText.text = $"Next Wave at: {seconds}s";
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

    private void SpawnEnemy()
    {
        if (_poolDictionary.TryGetValue(CurrentWave.enemyType, out var pool))
        {
            GameObject spawnedObject = pool.GetPooledObject();
            spawnedObject.transform.position = transform.position;
            float healthMultiplication = 1f + (_waveCounter * 0.09f);
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

    public bool IsInitialDelayActive()
    {
        return _isInitialDelay;
    }

    public float GetRemainingInitialDelay()
    {
        return Mathf.Max(0f, _initialTimer);
    }

    public float GetRemainingWaveCooldown()
    {
        return Mathf.Max(0f, _waveCooldown);
    }

    public bool IsCountdownActive()
    {
        return _isCountdownActive;
    }
}
