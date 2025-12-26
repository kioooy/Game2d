using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text coinRewardText;
    [SerializeField] private TMP_Text notEnoughCoinsText;
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private TowerCard towerCardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards = new List<GameObject>();
    private Platform _currentPlatform;
    [SerializeField] private Button speed1Button;
    [SerializeField] private Button speed2Button;
    [SerializeField] private Button speed3Button;
    [SerializeField] private Color normalButtonColor = Color.white;
    [SerializeField] private Color selectedButtonColor = Color.blue;
    [SerializeField] private Color normalTextColor = Color.black;
    [SerializeField] private Color selectedTextColor = Color.white;
    [SerializeField] private GameObject pausePanel;
    private bool _isGamePaused = false;
    [SerializeField] private GameObject gameoverPanel;
    private Spawner _spawner;
    [SerializeField] private TMP_Text menuCountdownText;
    [SerializeField] private GameObject completedPanel;
    [SerializeField] private Button completedPlayAgainButton;
    [SerializeField] private Button completedBackToMapButton;
    private bool _levelCompleted = false;

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnCoinRewardChanged += UpdateCoinRewardText;
        Platform.OnPlatformClicked += handlePlatformClicked;
        TowerCard.OnTowerSelected += handleTowerSelected;
        Enemy.OnEnemyDestroyed += OnEnemyDestroyed;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnCoinRewardChanged -= UpdateCoinRewardText;
        Platform.OnPlatformClicked -= handlePlatformClicked;
        TowerCard.OnTowerSelected -= handleTowerSelected;
        Enemy.OnEnemyDestroyed -= OnEnemyDestroyed;

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

    private void Start()
    {
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));
        HighlightSelectedSpeedButton(GameManager.Instance.GameSpeed);

        _spawner = FindObjectOfType<Spawner>();

        if (menuCountdownText != null)
        {
            menuCountdownText.gameObject.SetActive(false);
        }

        if (completedPanel != null)
        {
            completedPanel.SetActive(false);
        }

        if (completedPlayAgainButton != null)
        {
            completedPlayAgainButton.onClick.AddListener(RestartLevel);
        }

        if (completedBackToMapButton != null)
        {
            completedBackToMapButton.onClick.AddListener(BackToMap);
        }
    }

    private void Update()
    {
        UpdateMenuCountdownText();

        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        // Kiểm tra hoàn thành level
        if (!_levelCompleted && _spawner != null)
        {
            CheckLevelCompletion();
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $" {currentLives}";
        if (currentLives == 0)
        {
            ShowGameOver();
        }
    }

    private void UpdateCoinRewardText(int currentCoinRewards)
    {
        coinRewardText.text = $"{currentCoinRewards}";
    }

    private void handlePlatformClicked(Platform platform)
    {
        _currentPlatform = platform;
        ShowTowerPanel();
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        Platform.towerPanelOpen = true;
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;
    }

    private void PopulateTowerCards()
    {
        foreach (var card in activeCards)
        {
            Destroy(card);
        }
        activeCards.Clear();

        foreach (var data in towers)
        {
            GameObject cardGameObject = Instantiate(towerCardPrefab, cardsContainer).gameObject;
            TowerCard card = cardGameObject.GetComponent<TowerCard>();
            card.Initialize(data);
            activeCards.Add(cardGameObject);
        }
    }

    private void handleTowerSelected(TowerData towerData)
    {
        if (GameManager.Instance.Coins >= towerData.cost)
        {
            GameManager.Instance.SpendCoins(towerData.cost);
            _currentPlatform.PlaceTower(towerData);
            HideTowerPanel();
        }
        else
        {
            StartCoroutine(ShowNotEnoughCoinsText());
        }
    }

    private IEnumerator ShowNotEnoughCoinsText()
    {
        notEnoughCoinsText.gameObject.SetActive(true);
        yield return new WaitForSeconds(3f);
        notEnoughCoinsText.gameObject.SetActive(false);
    }

    private void SetGameSpeed(float timeScale)
    {
        HighlightSelectedSpeedButton(timeScale);
        GameManager.Instance.SetGameSpeed(timeScale);
    }

    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }

    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        UpdateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        UpdateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (_isGamePaused)
        {
            pausePanel.SetActive(false);
            _isGamePaused = false;
            GameManager.Instance.SetTimeScale(GameManager.Instance.GameSpeed);
        }
        else
        {
            pausePanel.SetActive(true);
            _isGamePaused = true;
            GameManager.Instance.SetTimeScale(0f);
        }
    }

    public void RestartLevel()
    {
        GameManager.Instance.SetTimeScale(1f);
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    public void BackToMap()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("LevelSelect");
    }

    public void MainMenu()
    {
        GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("MainMenu");
    }

    public void ShowGameOver()
    {
        GameManager.Instance.SetTimeScale(0.05f);
        gameoverPanel.SetActive(true);
    }

    private void UpdateMenuCountdownText()
    {
        if (_spawner == null || menuCountdownText == null) return;

        if (_spawner.IsInitialDelayActive())
        {
            float remainingTime = _spawner.GetRemainingInitialDelay();
            int seconds = Mathf.CeilToInt(remainingTime);
            menuCountdownText.text = $"Wave begins at: {seconds}";
            menuCountdownText.gameObject.SetActive(true);
        }
        else if (_spawner._isBetweenWaves)
        {
            float remainingTime = _spawner.GetRemainingWaveCooldown();
            int seconds = Mathf.CeilToInt(remainingTime);
            menuCountdownText.text = $"Next wave at: {seconds}";
            menuCountdownText.gameObject.SetActive(true);
        }
        else
        {
            menuCountdownText.gameObject.SetActive(false);
        }
    }

    private void OnEnemyDestroyed(Enemy enemy)
    {
        CheckLevelCompletion();
    }

    private void CheckLevelCompletion()
    {
        if (_levelCompleted || _spawner == null) return;

        // Kiểm tra xem có phải wave cuối không
        if (_spawner.CurrentWaveIndex == _spawner.TotalWaves - 1)
        {
            // Kiểm tra xem đã spawn đủ enemy và tiêu diệt đủ chưa
            if (_spawner.SpawnedEnemies >= 1 && _spawner.DestroyedEnemies >= 1)
            {
                // Kiểm tra xem còn enemy nào active không
                Enemy[] enemies = FindObjectsOfType<Enemy>();
                bool allEnemiesDefeated = true;

                foreach (Enemy enemy in enemies)
                {
                    if (enemy.gameObject.activeInHierarchy)
                    {
                        allEnemiesDefeated = false;
                        break;
                    }
                }

                if (allEnemiesDefeated)
                {
                    ShowLevelCompleted();
                }
            }
        }
    }

    public void ShowLevelCompleted()
    {
        if (completedPanel != null && !_levelCompleted)
        {
            _levelCompleted = true;

            // Tạm dừng game
            GameManager.Instance.SetTimeScale(0f);

            // Hiển thị panel
            completedPanel.SetActive(true);

            // Mở khóa level tiếp theo
            UnlockNextLevel();
        }
    }

    private void UnlockNextLevel()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.StartsWith("Level "))
        {
            string levelNumberStr = currentSceneName.Replace("Level ", "");
            if (int.TryParse(levelNumberStr, out int currentLevel))
            {
                int nextLevel = currentLevel + 1;
                if (nextLevel <= 15)
                {
                    PlayerPrefs.SetInt("LevelUnlocked_" + nextLevel, 1);
                    PlayerPrefs.Save();
                }
            }
        }
    }
}
