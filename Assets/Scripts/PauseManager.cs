using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class PauseManager : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMapButton;

    private bool isPaused = false;

    private void Start()
    {
        // Ensure panel is hidden at start
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Assign button listeners
        if (pauseButton != null)
            pauseButton.onClick.AddListener(TogglePause);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(ResumeGame);

        if (restartButton != null)
            restartButton.onClick.AddListener(RestartLevel);

        if (backToMapButton != null)
            backToMapButton.onClick.AddListener(BackToMap);
    }

    private void Update()
    {
        // Optional: Allow ESC key to toggle pause
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    private void PauseGame()
    {
        Time.timeScale = 0f;
        if (pausePanel != null)
            pausePanel.SetActive(true);

        // Also pause tower panel interactions if needed
        Platform.towerPanelOpen = true;
    }

    private void ResumeGame()
    {
        Time.timeScale = GameManager.Instance?.GameSpeed ?? 1f;
        if (pausePanel != null)
            pausePanel.SetActive(false);

        // Re-enable tower panel interactions
        Platform.towerPanelOpen = false;
    }

    private void RestartLevel()
    {
        Time.timeScale = 1f;
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    private void BackToMap()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MapScene"); // Replace with your map scene name
    }
}