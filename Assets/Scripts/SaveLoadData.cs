using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveLoadData
{
    public int saveSlotIndex;
    public string saveTime;
    public int currentLevel;
    public int playerCoins;
    public int playerLives;
    public float gameSpeed;
    public List<int> unlockedLevels;

    [Serializable]
    public class TowerData
    {
        public string towerType;
        public Vector3 position;
    }

    public List<TowerData> placedTowers;
    public int currentWaveIndex;
    public int waveCounter;
    public int enemiesDestroyed;
    public int enemiesSpawned;

    public SaveLoadData()
    {
        unlockedLevels = new List<int>();
        placedTowers = new List<TowerData>();
        saveTime = DateTime.Now.ToString("dd/MM/yyyy HH:mm");
    }

    public void SetDefaultNewGame()
    {
        currentLevel = 1;
        playerCoins = 200;
        playerLives = 20;
        gameSpeed = 1f;
        currentWaveIndex = 0;
        waveCounter = 0;
        enemiesDestroyed = 0;
        enemiesSpawned = 0;

        unlockedLevels.Clear();
        unlockedLevels.Add(1);

        placedTowers.Clear();
    }
}
