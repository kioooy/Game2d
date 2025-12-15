using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

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

    // THÊM: Tham chiếu đến Spawner để lấy thông tin countdown
    private Spawner _spawner;

    // THÊM: Text để hiển thị countdown trên MenuPanel
    [SerializeField] private TMP_Text menuCountdownText;

    // THÊM: Biến cho chức năng Pause
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private Button pauseButton;
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button backToMapButton;

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnCoinRewardChanged += UpdateCoinRewardText;
        Platform.OnPlatformClicked += handlePlatformClicked;
        TowerCard.OnTowerSelected += handleTowerSelected;
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnCoinRewardChanged -= UpdateCoinRewardText;
        Platform.OnPlatformClicked -= handlePlatformClicked;
        TowerCard.OnTowerSelected -= handleTowerSelected;

        // THÊM: Khôi phục Time.timeScale khi disable
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

        // THÊM: Khởi tạo Pause Panel
        if (pausePanel != null)
        {
            pausePanel.SetActive(false);
        }

        // THÊM: Thiết lập sự kiện cho nút pause
        if (pauseButton != null)
        {
            pauseButton.onClick.AddListener(TogglePausePanel);
        }

        // THÊM: Thiết lập sự kiện cho các nút trong pause panel
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (restartButton != null)
        {
            restartButton.onClick.AddListener(RestartGame);
        }

        if (backToMapButton != null)
        {
            backToMapButton.onClick.AddListener(BackToMap);
        }

        // THÊM: Tìm Spawner trong scene
        _spawner = FindObjectOfType<Spawner>();

        // THÊM: Ẩn menuCountdownText ban đầu
        if (menuCountdownText != null)
        {
            menuCountdownText.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateMenuCountdownText();
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
    }

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $" {currentLives}";
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

    private void handlePlatformClickedOutside(Platform platform)
    {
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

    // THÊM: Cập nhật countdown text trên MenuPanel
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

    // THÊM: Các phương thức cho chức năng Pause
    private void TogglePausePanel()
    {
        bool isActive = !pausePanel.activeSelf;
        pausePanel.SetActive(isActive);

        // Dừng hoặc tiếp tục game
        if (isActive)
        {
            Time.timeScale = 0f; // Dừng game
        }
        else
        {
            // Khôi phục tốc độ game trước khi pause
            Time.timeScale = GameManager.Instance.GameSpeed;
        }

        // Cập nhật trạng thái của PauseButton
        if (pauseButton != null)
        {
            pauseButton.interactable = !isActive;
        }
    }

    private void ResumeGame()
    {
        pausePanel.SetActive(false);
        Time.timeScale = GameManager.Instance.GameSpeed; // Khôi phục tốc độ game

        if (pauseButton != null)
        {
            pauseButton.interactable = true;
        }
    }

    private void RestartGame()
    {
        // Tải lại scene hiện tại
        string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        UnityEngine.SceneManagement.SceneManager.LoadScene(currentSceneName);

        // Khôi phục time scale khi restart
        Time.timeScale = 1f;
    }

    private void BackToMap()
    {
        // Tải scene LevelSelect (giả sử tên scene là "LevelSelect")
        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelSelect");

        // Khôi phục time scale khi chuyển scene
        Time.timeScale = 1f;
    }
}