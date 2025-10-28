using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private WaveData[] waves;
    private int _currentWaveIndex = 0;
    private WaveData CurrentWave => waves[_currentWaveIndex];

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
    void Update()
    {
        if (_isBetweenWaves)
        {
            _waveCooldown -= Time.deltaTime;
            if(_waveCooldown <= 0f)
            {
                _currentWaveIndex = (_currentWaveIndex + 1) % waves.Length;
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
            spawnedObject.SetActive(true);
        }
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _enemiesRemoved++;
    }
}
