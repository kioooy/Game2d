using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuLoader : MonoBehaviour
{
    public void LoadScene()
    {
        SceneManager.LoadScene("LevelSelect");
    }
}
