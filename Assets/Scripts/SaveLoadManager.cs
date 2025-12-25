using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class SaveLoadManager : MonoBehaviour
{
    public static SaveLoadManager Instance { get; private set; }

    private SaveLoadData currentGameData;
    private const int TOTAL_SLOTS = 3;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        InitializeSaveSystem();
    }

    private void InitializeSaveSystem()
    {
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            string key = $"SaveSlot_{i}";
            if (!PlayerPrefs.HasKey(key))
            {
                PlayerPrefs.SetInt($"Slot_{i}_Initialized", 1);
            }
        }
        PlayerPrefs.Save();
    }

    public void CreateNewGame(int slotIndex = -1)
    {
        currentGameData = new SaveLoadData();
        currentGameData.SetDefaultNewGame();
        if (slotIndex >= 0 && slotIndex < TOTAL_SLOTS)
        {
            SaveToSlot(slotIndex);
            ApplyUnlockedLevels(currentGameData);
        }
        SceneManager.LoadScene("LevelSelect");
    }

    public void LoadGame(SaveLoadData data)
    {
        if (data == null) return;
        currentGameData = data;
        ApplyLoadedData();
        if (data.currentLevel > 0)
        {
            SceneManager.LoadScene($"Level {data.currentLevel}");
        }
        else
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    public void QuickSave(int slotIndex = 0)
    {
        if (currentGameData == null)
        {
            currentGameData = new SaveLoadData();
            currentGameData.SetDefaultNewGame();
        }
        UpdateCurrentGameData();
        SaveToSlot(slotIndex);
    }

    public void SaveToSlot(int slotIndex)
    {
        if (currentGameData == null) return;
        currentGameData.saveSlotIndex = slotIndex;
        currentGameData.saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");
        string json = JsonUtility.ToJson(currentGameData, true);
        PlayerPrefs.SetString($"SaveSlot_{slotIndex}", json);
        PlayerPrefs.Save();
    }

    private void UpdateCurrentGameData()
    {
        if (currentGameData == null) return;
        if (GameManager.Instance != null)
        {
            currentGameData.playerCoins = GameManager.Instance.Coins;
        }
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level "))
        {
            string levelStr = sceneName.Replace("Level ", "");
            if (int.TryParse(levelStr, out int level))
            {
                currentGameData.currentLevel = level;
            }
        }
    }

    private void ApplyLoadedData()
    {
        if (currentGameData == null) return;
        ApplyUnlockedLevels(currentGameData);
    }

    private void ApplyUnlockedLevels(SaveLoadData data)
    {
        if (data == null || data.unlockedLevels == null) return;
        for (int i = 1; i <= 15; i++)
        {
            PlayerPrefs.SetInt($"LevelUnlocked_{i}", 0);
        }
        foreach (int level in data.unlockedLevels)
        {
            if (level >= 1 && level <= 15)
            {
                PlayerPrefs.SetInt($"LevelUnlocked_{level}", 1);
            }
        }
        PlayerPrefs.Save();
    }

    public SaveLoadData GetSlotData(int slotIndex)
    {
        string key = $"SaveSlot_{slotIndex}";
        string json = PlayerPrefs.GetString(key, "");
        if (!string.IsNullOrEmpty(json))
        {
            return JsonUtility.FromJson<SaveLoadData>(json);
        }
        return null;
    }

    public void DeleteSlot(int slotIndex)
    {
        PlayerPrefs.DeleteKey($"SaveSlot_{slotIndex}");
        PlayerPrefs.Save();
    }

    public bool HasAnySave()
    {
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            if (PlayerPrefs.HasKey($"SaveSlot_{i}"))
            {
                string json = PlayerPrefs.GetString($"SaveSlot_{i}", "");
                if (!string.IsNullOrEmpty(json))
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void UnlockNextLevel(int currentLevel)
    {
        if (currentGameData == null)
        {
            currentGameData = new SaveLoadData();
            currentGameData.SetDefaultNewGame();
        }
        int nextLevel = currentLevel + 1;
        if (nextLevel <= 15)
        {
            if (!currentGameData.unlockedLevels.Contains(nextLevel))
            {
                currentGameData.unlockedLevels.Add(nextLevel);
                PlayerPrefs.SetInt($"LevelUnlocked_{nextLevel}", 1);
                PlayerPrefs.Save();
                QuickSave(0);
            }
        }
    }

    // THÊM PHẦN TỰ ĐỘNG LƯU
    public void AutoSaveToCurrentSlot()
    {
        if (currentGameData == null) return;

        // Tìm slot nào có dữ liệu hiện tại
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            string key = $"SaveSlot_{i}";
            string json = PlayerPrefs.GetString(key, "");
            if (!string.IsNullOrEmpty(json))
            {
                SaveLoadData data = JsonUtility.FromJson<SaveLoadData>(json);
                if (data != null && data.currentLevel == currentGameData.currentLevel)
                {
                    UpdateCurrentGameData();
                    SaveToSlot(i);
                    return;
                }
            }
        }

        // Nếu không tìm thấy, lưu vào slot đầu tiên
        UpdateCurrentGameData();
        SaveToSlot(0);
    }

    public void AutoSaveOnLevelComplete(int completedLevel)
    {
        if (currentGameData == null)
        {
            currentGameData = new SaveLoadData();
            currentGameData.SetDefaultNewGame();
        }

        currentGameData.currentLevel = completedLevel;
        currentGameData.saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // Mở khóa level tiếp theo
        int nextLevel = completedLevel + 1;
        if (nextLevel <= 15)
        {
            if (!currentGameData.unlockedLevels.Contains(nextLevel))
            {
                currentGameData.unlockedLevels.Add(nextLevel);
                PlayerPrefs.SetInt($"LevelUnlocked_{nextLevel}", 1);
                PlayerPrefs.Save();
            }
        }

        AutoSaveToCurrentSlot();
    }
}