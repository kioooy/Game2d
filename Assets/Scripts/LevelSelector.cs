using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelSelector : MonoBehaviour
{
    public string level;
    private UnityEngine.UI.Button button;
    private TMP_Text levelText;

    void Start()
    {
        button = GetComponent<UnityEngine.UI.Button>();
        TMP_Text[] texts = GetComponentsInChildren<TMP_Text>();
        foreach (TMP_Text text in texts)
        {
            if (text.name == "LevelText" || text.text == level.ToString())
            {
                levelText = text;
                break;
            }
        }
        if (levelText == null && texts.Length > 0)
        {
            levelText = texts[0];
        }
        UpdateLevelState();
    }

    public void OpenScene()
    {
        if (IsLevelUnlocked(int.Parse(level)))
        {
            // TỰ ĐỘNG LƯU TRƯỚC KHI VÀO LEVEL
            SaveLoadManager.Instance.AutoSaveToCurrentSlot();
            SceneManager.LoadScene("Level " + level.ToString());
        }
    }

    private void UpdateLevelState()
    {
        bool isUnlocked = IsLevelUnlocked(int.Parse(level));
        if (button != null)
        {
            button.interactable = isUnlocked;
        }
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Lock"))
            {
                child.gameObject.SetActive(!isUnlocked);
                break;
            }
        }
        if (levelText != null)
        {
            levelText.text = level.ToString();
            levelText.gameObject.SetActive(true);
        }
    }

    private bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1) return true;
        return PlayerPrefs.GetInt("LevelUnlocked_" + levelNumber, 0) == 1;
    }
}