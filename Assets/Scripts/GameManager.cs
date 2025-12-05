using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnCoinRewardChanged;


    private int _lives = 20;
    private int _coins = 300;
    public int Coins => _coins;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    private void OnEnable()
    {
        Enemy.OnEnemyReachedEnd += HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }
    private void OnDisable()
    {
        Enemy.OnEnemyReachedEnd -= HandleEnemyReachedEnd;
        Enemy.OnEnemyDestroyed += HandleEnemyDestroyed;
    }

    private void Start()
    {
        OnLivesChanged?.Invoke(_lives);
        OnCoinRewardChanged?.Invoke(_coins);
    }

    private void HandleEnemyReachedEnd(EnemyData data)
    {
        _lives = Mathf.Max(0, _lives - data.damage);
        OnLivesChanged?.Invoke(_lives);
    }


    private void HandleEnemyDestroyed(Enemy enemy)
    {
        AddRewards(Mathf.RoundToInt(enemy.Data.coinReward));
    }

    private void AddRewards(int amount)
    {
        _coins += amount;
        OnCoinRewardChanged?.Invoke(_coins);
    }

    public void SetTimeScale(float scale)
    {
        Time.timeScale = scale;
    }

    public void SpendCoins(int amount)
    {
        if(_coins >= amount)
        {
            _coins -= amount;
            OnCoinRewardChanged?.Invoke(_coins);
        }
    }

}
