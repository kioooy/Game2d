using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour
{
    [SerializeField] private TMP_Text waveText;
<<<<<<< Updated upstream
=======
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text coinRewardText;
    [SerializeField] private TMP_Text notEnoughCoinsText;
    [SerializeField] private GameObject towerPanel;
    [SerializeField] private TowerCard towerCardPrefab;
    [SerializeField] private Transform cardsContainer;
    [SerializeField] private TowerData[] towers;
    private List<GameObject> activeCards = new List<GameObject>();
    private Platform _currentPlatform;
    private Tower _currentTower;
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
    private int _lastScreenWidth;
    private int _lastScreenHeight;
    private int _lastLives = -1;

    // --- Tower Action Menu ---
    private GameObject _towerActionMenu;
    private Button _sellButton;
    private Button _upgradeButton;
    private TMP_Text _sellText;
    private TMP_Text _upgradeText;
    private TMP_Text _towerInfoText;
>>>>>>> Stashed changes

    private void OnEnable()
    {
        Spawner.OnWaveChanged += UpdateWaveText;
<<<<<<< Updated upstream

=======
        GameManager.OnLivesChanged += UpdateLivesText;
        GameManager.OnCoinRewardChanged += UpdateCoinRewardText;
        Platform.OnPlatformClicked += handlePlatformClicked;
        TowerCard.OnTowerSelected += handleTowerSelected;
        Enemy.OnEnemyDestroyed += OnEnemyDestroyed;
        Tower.OnTowerClicked += handleTowerClicked;
>>>>>>> Stashed changes
    }

    private void OnDisable()
    {
        Spawner.OnWaveChanged -= UpdateWaveText;
<<<<<<< Updated upstream
    }

=======
        GameManager.OnLivesChanged -= UpdateLivesText;
        GameManager.OnCoinRewardChanged -= UpdateCoinRewardText;
        Platform.OnPlatformClicked -= handlePlatformClicked;
        TowerCard.OnTowerSelected -= handleTowerSelected;
        Enemy.OnEnemyDestroyed -= OnEnemyDestroyed;
        Tower.OnTowerClicked -= handleTowerClicked;

        if (Time.timeScale == 0f)
        {
            Time.timeScale = 1f;
        }
    }

    private void Start()
    {
        if (speed1Button != null) speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        if (speed2Button != null) speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        if (speed3Button != null) speed3Button.onClick.AddListener(() => SetGameSpeed(2f));
        if (GameManager.Instance != null)
            HighlightSelectedSpeedButton(GameManager.Instance.GameSpeed);

        _spawner = FindFirstObjectByType<Spawner>();

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
            completedBackToMapButton.onClick.AddListener(LoadNextLevelOrBackToMap);
        }

        if (gameoverPanel != null)
        {
            gameoverPanel.SetActive(false);
            Button[] gameoverButtons = gameoverPanel.GetComponentsInChildren<Button>(true);
            if (gameoverButtons != null && gameoverButtons.Length >= 1)
                gameoverButtons[0].onClick.AddListener(RestartLevel);
            if (gameoverButtons != null && gameoverButtons.Length >= 2)
                gameoverButtons[1].onClick.AddListener(BackToMap);
        }

        _lastScreenWidth = Screen.width;
        _lastScreenHeight = Screen.height;

        // Ensure UI Canvas is on top of gameplay but potentially behind other HUDs if needed
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
        {
            canvas.sortingOrder = 10; // High enough for towers/bars, low enough for HUDs
        }
    }

    private void Update()
    {
        if (Screen.width != _lastScreenWidth || Screen.height != _lastScreenHeight)
        {
            _lastScreenWidth = Screen.width;
            _lastScreenHeight = Screen.height;
            EnsureUIVisibleWhenResized();
        }

        UpdateMenuCountdownText();

        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePause();
        }

        // Kiểm tra hoàn thành level
        if (!_levelCompleted && _spawner != null)
        {
            CheckLevelCompletion();
        }

        // --- Centralized click detection for Platform & Tower ---
        HandleWorldClick();
    }

    private void HandleWorldClick()
    {
        if (Platform.towerPanelOpen || Time.timeScale == 0f) return;
        if (Mouse.current == null || !Mouse.current.leftButton.wasPressedThisFrame) return;

        Vector3 mousePos = Mouse.current.position.ReadValue();
        mousePos.z = Camera.main.nearClipPlane;
        Vector2 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);

        // Find all colliders at the exact click point
        Collider2D[] hits = Physics2D.OverlapPointAll(worldPoint);

        // Priority 1: check for Platform
        foreach (var hit in hits)
        {
            Platform platform = hit.GetComponent<Platform>();
            if (platform != null)
            {
                Platform.InvokePlatformClicked(platform);
                return;
            }
        }

        // Priority 2: check for Tower (find the closest one to click point)
        Tower closestTower = null;
        float closestDist = float.MaxValue;
        foreach (var hit in hits)
        {
            Tower tower = hit.GetComponent<Tower>();
            if (tower != null)
            {
                float dist = Vector2.Distance(worldPoint, tower.PlacedPosition);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    closestTower = tower;
                }
            }
        }

        if (closestTower != null)
        {
            Tower.InvokeTowerClicked(closestTower);
        }
    }
>>>>>>> Stashed changes

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
        if (AudioManager.Instance != null) AudioManager.Instance.PlayWaveStart();
    }
<<<<<<< Updated upstream
}
=======

    private void UpdateLivesText(int currentLives)
    {
        livesText.text = $" {currentLives}";
        
        // Play life lost sound if lives decreased
        if (_lastLives != -1 && currentLives < _lastLives)
        {
            if (AudioManager.Instance != null) AudioManager.Instance.PlayLifeLost();
        }
        _lastLives = currentLives;

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
        _currentTower = null;
        HideTowerActionMenu();
        ShowTowerPanel();
    }

    private void handleTowerClicked(Tower tower)
    {
        _currentTower = tower;
        _currentPlatform = null;
        HideTowerPanel();
        ShowTowerActionMenu(tower);
    }

    private void ShowTowerPanel()
    {
        towerPanel.SetActive(true);
        Platform.towerPanelOpen = true;
        PopulateTowerCards();

        if (AudioManager.Instance != null) AudioManager.Instance.PlayMenuOpen();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayMenuClose();
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
        // --- Normal placement from Platform ---
        if (_currentPlatform != null)
        {
            if (GameManager.Instance.Coins >= towerData.cost)
            {
                GameManager.Instance.SpendCoins(towerData.cost);
                _currentPlatform.PlaceTower(towerData);
                HideTowerPanel();

                if (AudioManager.Instance != null) AudioManager.Instance.PlayBuyTower();
            }
            else
            {
                StartCoroutine(ShowNotEnoughCoinsText());

                if (AudioManager.Instance != null) AudioManager.Instance.PlayNotEnoughCoins();
            }
        }
    }

    // ========== TOWER ACTION MENU ==========

    private void CreateTowerActionMenu()
    {
        Canvas canvas = GetComponent<Canvas>();
        if (canvas == null) canvas = GetComponentInParent<Canvas>();

        // Root panel
        _towerActionMenu = new GameObject("TowerActionMenu");
        _towerActionMenu.transform.SetParent(canvas.transform, false);
        RectTransform menuRt = _towerActionMenu.AddComponent<RectTransform>();
        menuRt.anchorMin = new Vector2(0.5f, 0.5f);
        menuRt.anchorMax = new Vector2(0.5f, 0.5f);
        menuRt.sizeDelta = new Vector2(320f, 220f);
        menuRt.anchoredPosition = Vector2.zero;

        Image menuBg = _towerActionMenu.AddComponent<Image>();
        menuBg.color = new Color(0f, 0f, 0f, 0.85f);

        // vertical layout
        VerticalLayoutGroup vlg = _towerActionMenu.AddComponent<VerticalLayoutGroup>();
        vlg.padding = new RectOffset(15, 15, 15, 15);
        vlg.spacing = 10f;
        vlg.childAlignment = TextAnchor.MiddleCenter;
        vlg.childControlWidth = true;
        vlg.childControlHeight = true;
        vlg.childForceExpandWidth = true;
        vlg.childForceExpandHeight = false;

        // Title / Info text
        _towerInfoText = CreateActionMenuText(_towerActionMenu.transform, "Tower Info", 18);
        LayoutElement infoLe = _towerInfoText.gameObject.AddComponent<LayoutElement>();
        infoLe.preferredHeight = 40f;

        // Upgrade button
        _upgradeButton = CreateActionMenuButton(_towerActionMenu.transform, "Upgrade", new Color(0.2f, 0.7f, 0.2f), out _upgradeText);
        _upgradeButton.onClick.AddListener(OnUpgradeClicked);

        // Sell button
        _sellButton = CreateActionMenuButton(_towerActionMenu.transform, "Sell", new Color(0.8f, 0.2f, 0.2f), out _sellText);
        _sellButton.onClick.AddListener(OnSellClicked);

        // Close button
        Button closeBtn = CreateActionMenuButton(_towerActionMenu.transform, "Close", new Color(0.4f, 0.4f, 0.4f), out _);
        closeBtn.onClick.AddListener(HideTowerActionMenu);

        _towerActionMenu.SetActive(false);
    }

    private Button CreateActionMenuButton(Transform parent, string label, Color color, out TMP_Text btnText)
    {
        GameObject btnObj = new GameObject(label + "Button");
        btnObj.transform.SetParent(parent, false);
        RectTransform btnRt = btnObj.AddComponent<RectTransform>();
        LayoutElement le = btnObj.AddComponent<LayoutElement>();
        le.preferredHeight = 40f;

        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = color;
        Button btn = btnObj.AddComponent<Button>();
        btn.targetGraphic = btnImg;

        // Text child
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(btnObj.transform, false);
        RectTransform textRt = textObj.AddComponent<RectTransform>();
        textRt.anchorMin = Vector2.zero;
        textRt.anchorMax = Vector2.one;
        textRt.sizeDelta = Vector2.zero;
        textRt.offsetMin = Vector2.zero;
        textRt.offsetMax = Vector2.zero;

        btnText = textObj.AddComponent<TextMeshProUGUI>();
        btnText.text = label;
        btnText.fontSize = 16;
        btnText.alignment = TextAlignmentOptions.Center;
        btnText.color = Color.white;

        return btn;
    }

    private TMP_Text CreateActionMenuText(Transform parent, string content, float fontSize)
    {
        GameObject textObj = new GameObject("InfoText");
        textObj.transform.SetParent(parent, false);
        textObj.AddComponent<RectTransform>();
        TMP_Text text = textObj.AddComponent<TextMeshProUGUI>();
        text.text = content;
        text.fontSize = fontSize;
        text.alignment = TextAlignmentOptions.Center;
        text.color = Color.white;
        return text;
    }

    private void ShowTowerActionMenu(Tower tower)
    {
        if (_towerActionMenu == null) CreateTowerActionMenu();
        Platform.towerPanelOpen = true;
        _towerActionMenu.SetActive(true);
        _towerActionMenu.transform.SetAsLastSibling();
        RefreshActionMenuInfo(tower);

        tower.ShowRange(true);

        if (AudioManager.Instance != null) AudioManager.Instance.PlayMenuOpen();
    }

    private void HideTowerActionMenu()
    {
        if (_towerActionMenu != null) _towerActionMenu.SetActive(false);
        Platform.towerPanelOpen = false;
        
        if (_currentTower != null) _currentTower.ShowRange(false);
        _currentTower = null;

        if (AudioManager.Instance != null) AudioManager.Instance.PlayMenuClose();
    }

    private void RefreshActionMenuInfo(Tower tower)
    {
        if (tower == null) return;
        _towerInfoText.text = $"Lv.{tower.UpgradeLevel}  DMG: {tower.GetCurrentDamage():F0}";
        _sellText.text = $"Sell  (+{tower.GetSellRefund()} coins)";
        if (tower.CanUpgrade())
        {
            _upgradeText.text = $"Upgrade  (-{tower.GetUpgradeCost()} coins)";
            _upgradeButton.interactable = true;
        }
        else
        {
            _upgradeText.text = "MAX LEVEL";
            _upgradeButton.interactable = false;
        }
    }

    private void OnSellClicked()
    {
        if (_currentTower == null) return;
        int refund = _currentTower.GetSellRefund();
        GameManager.Instance.AddCoins(refund);

        // Restore the original platform
        if (_currentTower.OriginalPlatform != null)
        {
            _currentTower.OriginalPlatform.SetActive(true);
        }

        Destroy(_currentTower.gameObject);
        HideTowerActionMenu();

        if (AudioManager.Instance != null) AudioManager.Instance.PlaySellTower();
    }

    private void OnUpgradeClicked()
    {
        if (_currentTower == null || !_currentTower.CanUpgrade()) return;
        int cost = _currentTower.GetUpgradeCost();
        if (GameManager.Instance.Coins >= cost)
        {
            GameManager.Instance.SpendCoins(cost);
            _currentTower.Upgrade();
            RefreshActionMenuInfo(_currentTower);

            if (AudioManager.Instance != null) AudioManager.Instance.PlayUpgradeTower();
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
        if (GameManager.Instance != null)
            GameManager.Instance.SetGameSpeed(timeScale);
    }

    private void UpdateButtonVisual(Button button, bool isSelected)
    {
        if (button == null) return;
        button.image.color = isSelected ? selectedButtonColor : normalButtonColor;
        TMP_Text text = button.GetComponentInChildren<TMP_Text>();
        if (text != null)
        {
            text.color = isSelected ? selectedTextColor : normalTextColor;
        }
    }

    private void HighlightSelectedSpeedButton(float selectedSpeed)
    {
        if (speed1Button != null) UpdateButtonVisual(speed1Button, selectedSpeed == 0.2f);
        if (speed2Button != null) UpdateButtonVisual(speed2Button, selectedSpeed == 1f);
        if (speed3Button != null) UpdateButtonVisual(speed3Button, selectedSpeed == 2f);
    }

    public void TogglePause()
    {
        if (pausePanel == null || GameManager.Instance == null) return;
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
        if (GameManager.Instance != null) GameManager.Instance.SetTimeScale(1f);
        SceneManager.LoadScene("LevelSelect");
    }

    /// <summary>Chuyển sang level tiếp theo; nếu đang ở level 15 thì về LevelSelect.</summary>
    public void LoadNextLevelOrBackToMap()
    {
        if (GameManager.Instance != null) GameManager.Instance.SetTimeScale(1f);
        string currentSceneName = SceneManager.GetActiveScene().name;
        if (currentSceneName.StartsWith("Level "))
        {
            string levelNumberStr = currentSceneName.Replace("Level ", "").Trim();
            if (int.TryParse(levelNumberStr, out int currentLevel))
            {
                int nextLevel = currentLevel + 1;
                if (nextLevel <= 15)
                {
                    SceneManager.LoadScene($"Level {nextLevel}");
                    return;
                }
            }
        }
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

        if (AudioManager.Instance != null) AudioManager.Instance.PlayGameOver();
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
                Enemy[] enemies = FindObjectsByType<Enemy>(FindObjectsSortMode.None);
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

            if (GameManager.Instance != null)
                GameManager.Instance.SetTimeScale(0f);

            // Hiển thị panel và đảm bảo thấy trong Game view (scale parent = 1, đưa lên trên cùng)
            completedPanel.SetActive(true);
            if (completedPanel.transform.parent != null)
            {
                completedPanel.transform.parent.localScale = Vector3.one;
                completedPanel.transform.SetAsLastSibling();
            }
            // Ép toàn bộ hierarchy panel: scale = 1, ButtonGroup size 800x150, nút có size tối thiểu
            SetRecursiveScale(completedPanel.transform, Vector3.one);
            for (int i = 0; i < completedPanel.transform.childCount; i++)
            {
                Transform child = completedPanel.transform.GetChild(i);
                if (child.name == "ButtonGroup")
                {
                    if (child is RectTransform groupRt)
                    {
                        groupRt.sizeDelta = new Vector2(800f, 150f);
                        groupRt.anchoredPosition = new Vector2(0f, -61.5f);
                    }
                    for (int j = 0; j < child.childCount; j++)
                    {
                        Transform btn = child.GetChild(j);
                        btn.localScale = Vector3.one;
                        btn.gameObject.SetActive(true);
                        if (btn is RectTransform btnRt)
                        {
                            if (btnRt.sizeDelta.x < 100f || btnRt.sizeDelta.y < 30f)
                                btnRt.sizeDelta = new Vector2(250f, 60f);
                        }
                    }
                    break;
                }
            }
            // Ép hai nút tham chiếu luôn bật và có kích thước
            if (completedPlayAgainButton != null)
            {
                completedPlayAgainButton.gameObject.SetActive(true);
                completedPlayAgainButton.transform.localScale = Vector3.one;
                if (completedPlayAgainButton.transform is RectTransform r) r.sizeDelta = new Vector2(Mathf.Max(r.sizeDelta.x, 200f), Mathf.Max(r.sizeDelta.y, 50f));
            }
            if (completedBackToMapButton != null)
            {
                completedBackToMapButton.gameObject.SetActive(true);
                completedBackToMapButton.transform.localScale = Vector3.one;
                if (completedBackToMapButton.transform is RectTransform r) r.sizeDelta = new Vector2(Mathf.Max(r.sizeDelta.x, 200f), Mathf.Max(r.sizeDelta.y, 50f));
            }
            Canvas.ForceUpdateCanvases();

            // Mở khóa level tiếp theo
            UnlockNextLevel();

            // Đổi chữ nút: "Next Level" nếu còn level tiếp theo, "Back to Map" nếu là level 15
            if (completedBackToMapButton != null)
            {
                TMP_Text btnText = completedBackToMapButton.GetComponentInChildren<TMP_Text>();
                if (btnText != null)
                {
                    string sceneName = SceneManager.GetActiveScene().name;
                    int currentLevel = 0;
                    if (sceneName.StartsWith("Level ") && int.TryParse(sceneName.Replace("Level ", "").Trim(), out currentLevel))
                        btnText.text = currentLevel < 15 ? "Next Level" : "Back to Map";
                    else
                        btnText.text = "Back to Map";
                }
            }
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

    private static void SetRecursiveScale(Transform t, Vector3 scale)
    {
        if (t == null) return;
        t.localScale = scale;
        for (int i = 0; i < t.childCount; i++)
            SetRecursiveScale(t.GetChild(i), scale);
    }

    /// <summary>
    /// Gọi khi kích thước cửa sổ thay đổi (vd. phóng to Game tab).
    /// Ép scale Canvas và toàn bộ UI = 1 cố định, để thanh UI không bị thu nhỏ/theo cửa sổ và mất khi phóng to.
    /// Không gọi ở Start() để thanh UI hiện bình thường khi mới Play.
    /// </summary>
    private void EnsureUIVisibleWhenResized()
    {
        Transform root = waveText != null ? waveText.transform.root
            : livesText != null ? livesText.transform.root
            : coinRewardText != null ? coinRewardText.transform.root
            : null;
        if (root == null) return;

        Canvas can = root.GetComponent<Canvas>();
        if (can == null) can = root.GetComponentInParent<Canvas>();
        if (can == null) return;

        SetRecursiveScale(can.transform, Vector3.one);
        if (can.transform.parent != null)
            can.transform.parent.localScale = Vector3.one;
    }
}
>>>>>>> Stashed changes
