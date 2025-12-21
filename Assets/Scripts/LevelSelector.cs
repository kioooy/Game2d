using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour
{
    public string level;

    void Start()
    {
        UpdateLevelState();
    }

    public void OpenScene()
    {
        if (IsLevelUnlocked(int.Parse(level)))
        {
            SceneManager.LoadScene("Level " + level.ToString());
        }
    }

    private void UpdateLevelState()
    {
        bool isUnlocked = IsLevelUnlocked(int.Parse(level));

        UnityEngine.UI.Button button = GetComponent<UnityEngine.UI.Button>();
        if (button != null)
        {
            button.interactable = isUnlocked;
        }

        // Tìm lock icon
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Lock"))
            {
                child.gameObject.SetActive(!isUnlocked);
                break;
            }
        }

        // Tìm text level
        TMPro.TMP_Text[] texts = GetComponentsInChildren<TMPro.TMP_Text>();
        foreach (TMPro.TMP_Text text in texts)
        {
            if (text.text == level.ToString())
            {
                text.gameObject.SetActive(isUnlocked);
                break;
            }
        }
    }

    private bool IsLevelUnlocked(int levelNumber)
    {
        if (levelNumber == 1) return true;
        return PlayerPrefs.GetInt("LevelUnlocked_" + levelNumber, 0) == 1;
    }
}