using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveSegment
{
    public EnemyType enemyType;
    public int count;
    public float spawnInterval;
}

[CreateAssetMenu(fileName = "WaveData", menuName = "Scriptable Objects/WaveData")]
public class WaveData : ScriptableObject
{
    public List<WaveSegment> segments;

    public int GetTotalEnemies()
    {
        int total = 0;
        if (segments == null) return 0;
        foreach (var s in segments)
        {
            if (s != null) total += s.count;
        }
        return total;
    }
}
