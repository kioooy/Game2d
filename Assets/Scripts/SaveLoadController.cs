using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SaveLoadController : MonoBehaviour
{
    [SerializeField] private GameObject loadingMenu;
    [SerializeField] private SaveLoadSlot[] saveSlots;
    [SerializeField] private Button loadButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button backButton;
    [SerializeField] private TMP_Text statusText;

    [SerializeField] private GameObject confirmDialog;
    [SerializeField] private TMP_Text confirmMessage;
    [SerializeField] private Button confirmYesButton;
    [SerializeField] private Button confirmNoButton;

    private SaveLoadSlot selectedSlot;
    private bool isNewGameMode = false;

    private void Start()
    {
        InitializeSlots();
        SetupButtons();
        HideConfirmationDialog();
    }

    private void InitializeSlots()
    {
        for (int i = 0; i < saveSlots.Length; i++)
        {
            saveSlots[i].Initialize(i);
            saveSlots[i].OnSlotSelected += OnSlotSelected;
        }
    }

    private void SetupButtons()
    {
        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadClicked);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (backButton != null)
            backButton.onClick.AddListener(OnBackClicked);

        if (confirmYesButton != null)
            confirmYesButton.onClick.AddListener(OnConfirmYes);

        if (confirmNoButton != null)
            confirmNoButton.onClick.AddListener(HideConfirmationDialog);
    }

    private void OnSlotSelected(SaveLoadSlot slot)
    {
        if (selectedSlot != null && selectedSlot != slot)
        {
            selectedSlot.DeselectSlot();
        }

        selectedSlot = slot;
        selectedSlot.SelectSlot();
        UpdateLoadButtonState();
    }

    private void UpdateLoadButtonState()
    {
        if (loadButton == null) return;

        if (isNewGameMode && selectedSlot != null)
        {
            loadButton.interactable = true;
            loadButton.GetComponentInChildren<TMP_Text>().text = "SAVE NEW GAME";
            statusText.text = $"New game will be saved to Slot {selectedSlot.SlotIndex + 1}";
        }
        else if (selectedSlot != null && !selectedSlot.IsEmpty)
        {
            loadButton.interactable = true;
            loadButton.GetComponentInChildren<TMP_Text>().text = "LOAD";
            statusText.text = $"Selected: Slot {selectedSlot.SlotIndex + 1}";
        }
        else
        {
            loadButton.interactable = false;
            statusText.text = "Select a save slot";
        }
    }

    private void OnNewGameClicked()
    {
        isNewGameMode = true;
        statusText.text = "Select a slot for your new game";

        if (loadButton != null)
        {
            loadButton.GetComponentInChildren<TMP_Text>().text = "SAVE NEW GAME";
        }
    }

    private void OnLoadClicked()
    {
        if (selectedSlot == null)
        {
            statusText.text = "Please select a slot first!";
            return;
        }

        if (isNewGameMode)
        {
            SaveNewGameToSlot();
        }
        else
        {
            if (!selectedSlot.IsEmpty)
            {
                selectedSlot.LoadData();
            }
            else
            {
                statusText.text = "This slot is empty!";
            }
        }
    }

    private void SaveNewGameToSlot()
    {
        if (selectedSlot == null) return;

        if (!selectedSlot.IsEmpty)
        {
            ShowConfirmationDialog(
                $"Slot {selectedSlot.SlotIndex + 1} already has saved data.\nOverwrite?"
            );
        }
        else
        {
            CreateNewGameNow();
        }
    }

    private void CreateNewGameNow()
    {
        SaveLoadManager.Instance.CreateNewGame(selectedSlot.SlotIndex);

        if (loadingMenu != null)
            loadingMenu.SetActive(false);
    }

    private void OnBackClicked()
    {
        if (loadingMenu != null)
            loadingMenu.SetActive(false);

        isNewGameMode = false;
        if (selectedSlot != null)
        {
            selectedSlot.DeselectSlot();
            selectedSlot = null;
        }

        if (loadButton != null)
        {
            loadButton.GetComponentInChildren<TMP_Text>().text = "LOAD";
            loadButton.interactable = false;
        }

        statusText.text = "";
    }

    private void ShowConfirmationDialog(string message)
    {
        if (confirmDialog != null)
        {
            confirmDialog.SetActive(true);
            confirmMessage.text = message;
        }
    }

    private void HideConfirmationDialog()
    {
        if (confirmDialog != null)
            confirmDialog.SetActive(false);
    }

    private void OnConfirmYes()
    {
        CreateNewGameNow();
        HideConfirmationDialog();
    }

    public void ShowLoadingMenu()
    {
        if (loadingMenu != null)
        {
            loadingMenu.SetActive(true);

            foreach (var slot in saveSlots)
            {
                slot.LoadSlotData();
            }

            isNewGameMode = false;
            selectedSlot = null;

            if (loadButton != null)
            {
                loadButton.GetComponentInChildren<TMP_Text>().text = "LOAD";
                loadButton.interactable = false;
            }

            statusText.text = "Select a save slot";
        }
    }
}
