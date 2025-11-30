using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelMenu : MonoBehaviour
{
    public void OpenLevel (int levelId)
    {
        string levelName = "Level " + levelId;
        SceneManager.LoadScene(levelName);
        // Add your level loading logic here
    }
}
