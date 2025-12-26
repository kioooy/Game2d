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

        // Kiểm tra nếu đang chạy từ Level scene, không destroy GameManager đang có
        if (GameManager.Instance != null && GameManager.Instance != this)
        {
            // Không làm gì, để GameManager tồn tại
        }
    }

    public void CreateNewGame(int slotIndex = -1)
    {
        // Reset tất cả level về trạng thái khóa (chỉ mở level 1)
        ResetAllLevels();

        currentGameData = new SaveLoadData();
        currentGameData.SetDefaultNewGame();

        if (slotIndex >= 0 && slotIndex < TOTAL_SLOTS)
        {
            SaveToSlot(slotIndex);
        }

        // Load Level Select Scene
        SceneManager.LoadScene("LevelSelect");
    }

    public void LoadGame(SaveLoadData data)
    {
        if (data == null) return;

        currentGameData = data;
        ApplyUnlockedLevels(data);

        // Load scene tương ứng
        if (data.currentLevel >= 1)
        {
            SceneManager.LoadScene($"Level {data.currentLevel}");
        }
        else
        {
            SceneManager.LoadScene("LevelSelect");
        }
    }

    private void ResetAllLevels()
    {
        // Khóa tất cả level trừ level 1
        for (int i = 1; i <= 15; i++)
        {
            if (i == 1)
                PlayerPrefs.SetInt($"LevelUnlocked_{i}", 1);
            else
                PlayerPrefs.SetInt($"LevelUnlocked_{i}", 0);
        }
        PlayerPrefs.Save();
    }

    private void ApplyUnlockedLevels(SaveLoadData data)
    {
        if (data == null || data.unlockedLevels == null) return;

        // Reset tất cả về khóa
        for (int i = 1; i <= 15; i++)
        {
            PlayerPrefs.SetInt($"LevelUnlocked_{i}", 0);
        }

        // Mở khóa các level trong data
        foreach (int level in data.unlockedLevels)
        {
            if (level >= 1 && level <= 15)
            {
                PlayerPrefs.SetInt($"LevelUnlocked_{level}", 1);
            }
        }
        PlayerPrefs.Save();
    }

    public void SaveToSlot(int slotIndex)
    {
        if (currentGameData == null)
        {
            currentGameData = new SaveLoadData();
            currentGameData.SetDefaultNewGame();
        }

        currentGameData.saveSlotIndex = slotIndex;
        currentGameData.saveTime = System.DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // Cập nhật level hiện tại từ scene
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName.StartsWith("Level "))
        {
            string levelStr = sceneName.Replace("Level ", "");
            if (int.TryParse(levelStr, out int level))
            {
                currentGameData.currentLevel = level;
            }
        }

        string json = JsonUtility.ToJson(currentGameData, true);
        PlayerPrefs.SetString($"SaveSlot_{slotIndex}", json);
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

                // Lưu lại progress
                SaveToSlot(0);
            }
        }
    }

    public bool HasAnySave()
    {
        for (int i = 0; i < TOTAL_SLOTS; i++)
        {
            string key = $"SaveSlot_{i}";
            if (PlayerPrefs.HasKey(key))
            {
                string json = PlayerPrefs.GetString(key, "");
                if (!string.IsNullOrEmpty(json))
                {
                    return true;
                }
            }
        }
        return false;
    }
}