using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SaveLoadController saveLoadController;

    public void NewGame()
    {
        int emptySlot = FindEmptySlot();
        if (emptySlot >= 0)
        {
            SaveLoadManager.Instance.CreateNewGame(emptySlot);
        }
        else
        {
            SaveLoadManager.Instance.CreateNewGame(0);
        }
    }

    public void LoadGame()
    {
        if (saveLoadController != null)
        {
            saveLoadController.ShowLoadingMenu();
        }
        else
        {
            Debug.LogError("SaveLoadController not assigned!");
        }
    }

    public void Setting()
    {
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < 3; i++)
        {
            string key = $"SaveSlot_{i}";
            string json = PlayerPrefs.GetString(key, "");
            if (string.IsNullOrEmpty(json))
            {
                return i;
            }
        }
        return -1;
    }
}
