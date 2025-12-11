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

    // ===== THÊM CODE MỚI Ở ĐÂY - Khai báo biến countdown =====
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private GameObject countdownPanel;

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
    }

    private void Start()
    {
        speed1Button.onClick.AddListener(() => SetGameSpeed(0.2f));
        speed2Button.onClick.AddListener(() => SetGameSpeed(1f));
        speed3Button.onClick.AddListener(() => SetGameSpeed(2f));
        HighlightSelectedSpeedButton(GameManager.Instance.GameSpeed);
        
        // ===== THÊM CODE MỚI Ở ĐÂY - Khởi tạo countdown =====
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    private void UpdateWaveText(int currentWave)
    {
        waveText.text = $"Wave: {currentWave + 1}";
        
        // ===== THÊM CODE MỚI Ở ĐÂY - Ẩn countdown khi wave bắt đầu =====
        if (countdownPanel != null && countdownPanel.activeSelf)
        {
            countdownPanel.SetActive(false);
        }
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
        GameManager.Instance.SetTimeScale(0f);
        PopulateTowerCards();
    }

    public void HideTowerPanel()
    {
        towerPanel.SetActive(false);
        Platform.towerPanelOpen = false;
        GameManager.Instance.SetTimeScale(1f);
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
        if(GameManager.Instance.Coins >= towerData.cost)
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
        yield return new WaitForSecondsRealtime(3f);
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

    // ===== THÊM CODE MỚI Ở ĐÂY - Update() để cập nhật countdown =====
    private void Update()
    {
        UpdateCountdownDisplay();
    }

    // ===== THÊM CODE MỚI Ở ĐÂY - Phương thức hiển thị countdown =====
    private void UpdateCountdownDisplay()
    {
        if (countdownText == null) return;
        
        Spawner spawner = FindObjectOfType<Spawner>();
        if (spawner != null && spawner.IsInWaitingPhase())
        {
            float remainingTime = spawner.GetRemainingWaitTime();
            int waitType = spawner.GetCurrentWaitType();
            
            // Hiển thị panel countdown
            if (countdownPanel != null && !countdownPanel.activeSelf)
            {
                countdownPanel.SetActive(true);
            }
            
            // Cập nhật text countdown
            if (waitType == 1) // Initial Delay
            {
                countdownText.text = $"Wave starts at: {Mathf.Ceil(remainingTime)}s";
            }
            else if (waitType == 2) // Between Waves
            {
                countdownText.text = $"Next wave at: {Mathf.Ceil(remainingTime)}s";
            }
            
            // Cập nhật trên MenuPanel
            UpdateMenuPanelCountdown(remainingTime, waitType);
        }
        else
        {
            // Ẩn panel khi không trong thời gian chờ
            if (countdownPanel != null && countdownPanel.activeSelf)
            {
                countdownPanel.SetActive(false);
            }
        }
    }
    
    // ===== THÊM CODE MỚI Ở ĐÂY - Cập nhật countdown trên MenuPanel =====
    private void UpdateMenuPanelCountdown(float remainingTime, int waitType)
    {
        GameObject menuPanel = GameObject.Find("MenuPanel");
        if (menuPanel != null && menuPanel.activeSelf)
        {
            // Tìm tất cả TMP_Text trong MenuPanel
            TMP_Text[] menuTexts = menuPanel.GetComponentsInChildren<TMP_Text>(true);
            
            // Tìm text có tên chứa "Countdown", "Timer" hoặc "Time"
            foreach (TMP_Text text in menuTexts)
            {
                if (text.gameObject.name.Contains("Countdown") || 
                    text.gameObject.name.Contains("Timer") ||
                    text.gameObject.name.Contains("Time"))
                {
                    if (waitType == 1)
                    {
                        text.text = $"Wave đầu bắt đầu sau: {Mathf.Ceil(remainingTime)}s";
                    }
                    else if (waitType == 2)
                    {
                        text.text = $"Wave tiếp theo sau: {Mathf.Ceil(remainingTime)}s";
                    }
                    break;
                }
            }
            
            // Nếu không tìm thấy text đặc biệt, tìm text đầu tiên không phải là Lives hoặc Coin
            if (menuTexts.Length > 0)
            {
                bool found = false;
                foreach (TMP_Text text in menuTexts)
                {
                    string textName = text.gameObject.name.ToLower();
                    if (!textName.Contains("lives") && !textName.Contains("coin") && 
                        !textName.Contains("wave") && !textName.Contains("menu"))
                    {
                        if (waitType == 1)
                        {
                            text.text = $"Wave đầu bắt đầu sau: {Mathf.Ceil(remainingTime)}s";
                        }
                        else if (waitType == 2)
                        {
                            text.text = $"Wave tiếp theo sau: {Mathf.Ceil(remainingTime)}s";
                        }
                        found = true;
                        break;
                    }
                }
                
                // Nếu vẫn không tìm thấy, sử dụng text đầu tiên
                if (!found && menuTexts.Length > 0)
                {
                    if (waitType == 1)
                    {
                        menuTexts[0].text = $"Wave đầu bắt đầu sau: {Mathf.Ceil(remainingTime)}s";
                    }
                    else if (waitType == 2)
                    {
                        menuTexts[0].text = $"Wave tiếp theo sau: {Mathf.Ceil(remainingTime)}s";
                    }
                }
            }
        }
    }
}