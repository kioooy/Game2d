using UnityEngine;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private SaveLoadController saveLoadController;

    public void NewGame()
    {
        if (saveLoadController != null)
        {
            saveLoadController.ShowLoadingMenuForNewGame();
        }
        else
        {
            // Fallback: tạo game mới ngay lập tức
            SaveLoadManager.Instance.CreateNewGame(0);
        }
    }

    public void LoadGame()
    {
        if (saveLoadController != null)
        {
            saveLoadController.ShowLoadingMenuForLoad();
        }
        else
        {
            Debug.LogError("SaveLoadController not assigned!");
        }
    }

    public void Setting()
    {
        // Mở menu setting (giữ nguyên)
    }

    public void QuitGame()
    {
        Application.Quit();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}