using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static event Action<int> OnLivesChanged;
    public static event Action<int> OnCoinRewardChanged;

    private int _lives = 20;
    private int _coins = 300;

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



}
