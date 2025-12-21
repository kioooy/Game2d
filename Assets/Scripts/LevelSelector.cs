using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : MonoBehaviour

{
    public string level;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    public void OpenScene()
    {
        SceneManager.LoadScene("Level " + level.ToString());
    }
}
