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

    [Header("Lösch-Warnung")]
    public GameObject confirmDeletePanel; // Ein neues Panel im Inspector
    private int slotToDelete;

    void OnEnable() { RefreshSlots(); }

    public void RefreshSlots()
    {
        for (int i = 0; i < 3; i++)
        {
            int slotNum = i + 1;
            bool exists = PlayerPrefs.HasKey("Slot" + slotNum + "_CharId");

            // TEXT ANZEIGEN
            if (statusTexts[i] != null)
            {
                statusTexts[i].text = exists ? "Gespeichert: " + PlayerPrefs.GetString("Slot" + slotNum + "_SaveDate") : "Leer";
            }

            // BUTTONS INTERAKTIV MACHEN
            if (currentMode == MenuMode.NewGame)
            {
                // Im "Neues Spiel" Modus muss man IMMER klicken können, 
                // egal ob der Slot leer ist oder nicht (zum Überschreiben)
                slotButtons[i].interactable = true;
            }
            else // Load Modus
            {
                // Laden kann man nur, wenn auch was da ist
                slotButtons[i].interactable = exists;
            }

            // Mülltonne nur zeigen, wenn ein Spielstand da ist
            deleteButtons[i].gameObject.SetActive(exists);

            // Die braunen Laden-Buttons (falls vorhanden)
            if (loadButtons[i] != null) loadButtons[i].SetActive(exists && currentMode == MenuMode.Load);
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

    // Wird von der Mülltonne aufgerufen
    public void DeleteSlot(int slot)
    {
        slotToDelete = slot;
        confirmDeletePanel.SetActive(true); // Pop-up öffnen: "Willst du wirklich löschen?"
    }

    // Wird vom "JA"-Button im Lösch-Pop-up aufgerufen
    public void ConfirmDelete()
    {
        GameState.I.DeleteSave(slotToDelete);
        confirmDeletePanel.SetActive(false);
        RefreshSlots();

        // WICHTIG: Den MainMenuController informieren, damit der "Laden"-Button 
        // im Hauptmenü ausgegraut wird, falls jetzt alle Slots leer sind.
        if (mainMenu != null) mainMenu.Start();
    }

    public void CancelDelete() => confirmDeletePanel.SetActive(false);
}