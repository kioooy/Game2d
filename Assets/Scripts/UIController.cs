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
}