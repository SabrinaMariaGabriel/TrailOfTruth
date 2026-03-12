using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveSlotMenu : MonoBehaviour
{
    public enum MenuMode { Load, NewGame }
    public MenuMode currentMode = MenuMode.Load;

    [Header("UI Elemente")]
    public Button[] slotButtons;    // Die 3 großen goldenen Rahmen
    public TMP_Text[] statusTexts;  // Hier kommt Datum/Zeit oder "Leer" rein
    public Button[] deleteButtons;  // Die 3 roten Mülltonnen
    public GameObject[] loadButtons; // NEU: Die braunen Buttons (GameObject reicht zum Ein/Ausblenden)

    [Header("Warn-Panel")]
    public GameObject confirmOverwritePanel;
    private int selectedSlotAfterWarning;

    [Header("Referenzen")]
    public MainMenuController mainMenu;

    void OnEnable() { RefreshSlots(); }

    public void RefreshSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            int slotNum = i + 1;
            string p = "Slot" + slotNum + "_";
            bool exists = PlayerPrefs.HasKey(p + "CharId");

            // 1. Text-Anzeige: Datum oder "Leer"
            if (statusTexts[i] != null)
            {
                statusTexts[i].text = exists ? "Gespeichert am: " + PlayerPrefs.GetString(p + "SaveDate", System.DateTime.Now.ToShortDateString()) : "Leer";
            }

            // 2. Sichtbarkeit der braunen Laden-Buttons
            if (loadButtons[i] != null)
            {
                // Zeige den Button nur, wenn ein Save existiert
                loadButtons[i].SetActive(exists);
            }

            // 3. Interaktion der Rahmen
            if (currentMode == MenuMode.Load)
            {
                slotButtons[i].interactable = exists;
                deleteButtons[i].gameObject.SetActive(exists);
            }
            else
            {
                slotButtons[i].interactable = true;
                deleteButtons[i].gameObject.SetActive(exists);
            }
        }
    }

    public void OnSlotSelected(int slot)
    {
        selectedSlotAfterWarning = slot;
        if (currentMode == MenuMode.Load) { ConfirmSelection(); }
        else
        {
            if (PlayerPrefs.HasKey("Slot" + slot + "_CharId")) { confirmOverwritePanel.SetActive(true); }
            else { ConfirmSelection(); }
        }
    }

    public void ConfirmSelection()
    {
        GameState.I.currentSlot = selectedSlotAfterWarning;
        confirmOverwritePanel.SetActive(false);
        if (currentMode == MenuMode.Load)
        {
            GameState.I.LoadGame(selectedSlotAfterWarning);
            UnityEngine.SceneManagement.SceneManager.LoadScene("ForestScene");
        }
        else
        {
            mainMenu.ShowCharacterSelect();
            gameObject.SetActive(false);
        }
    }

    public void CancelWarning() => confirmOverwritePanel.SetActive(false);

    public void DeleteSlot(int slot)
    {
        GameState.I.DeleteSave(slot);
        RefreshSlots();
    }
}