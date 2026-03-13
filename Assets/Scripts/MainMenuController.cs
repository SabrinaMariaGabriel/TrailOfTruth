using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject startGamePanel;
    public GameObject characterSelectPanel;
    public GameObject characterDetailPanel; // Neues Panel für die Detail-Ansicht
    public GameObject saveSlotPanel; // Panel, das die Save Slots enthält

    [Header("Buttons")]
    public Button loadGameButton; // Damit wir ihn ausgrauen können, wenn es kein Savegame gibt

    // Hier merken wir uns, auf welchen Charakter in der Liste geklickt wurde
    private string pendingCharacterClass = "";


    public void Start()
    {
        ShowMainMenu();

        // Prüfen, ob überhaupt IRGENDWO ein Spielstand ist
        if (loadGameButton != null && GameState.I != null)
        {
            // Wenn Slot 1 ODER Slot 2 ODER Slot 3 existiert, darf man auf Laden klicken
            bool hasAnySave = GameState.I.HasSave(1) || GameState.I.HasSave(2) || GameState.I.HasSave(3);
            loadGameButton.interactable = hasAnySave;
        }
    }

    // --- Panel Navigation ---

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        startGamePanel.SetActive(false);
        characterSelectPanel.SetActive(false);
        saveSlotPanel.SetActive(false); // Sichergehen, dass es aus ist
    }

    public void ShowStartGameMenu()
    {
        mainMenuPanel.SetActive(false);
        startGamePanel.SetActive(true);
        characterSelectPanel.SetActive(false);
        saveSlotPanel.SetActive(false);
    }

    // NEU: Diese Methode öffnet das Slot-Panel für "Neues Spiel" oder "Laden"
    public void OpenSaveSlotPanel(bool isLoadingMode)
    {
        // 1. Zuerst die Referenz holen
        SaveSlotMenu slotMenu = saveSlotPanel.GetComponent<SaveSlotMenu>();

        if (slotMenu != null)
        {
            // 2. Den Modus setzen, BEVOR das Panel aktiv wird
            slotMenu.currentMode = isLoadingMode ? SaveSlotMenu.MenuMode.Load : SaveSlotMenu.MenuMode.NewGame;
        }

        // 3. Jetzt erst das Panel aktivieren
        saveSlotPanel.SetActive(true);
        startGamePanel.SetActive(false);

        // 4. Zur Sicherheit den Refresh nochmal anstoßen, damit die Buttons wirklich reagieren
        if (slotMenu != null)
        {
            slotMenu.RefreshSlots();
        }
    }

    public void ShowCharacterSelect()
    {
        mainMenuPanel.SetActive(false);
        startGamePanel.SetActive(false);
        characterSelectPanel.SetActive(true);
        characterDetailPanel.SetActive(false); // Detail-Panel ausblenden
    }

    // Wird aufgerufen, wenn man in der Liste auf z. B. "Krieger" klickt
    public void OpenCharacterDetails(string characterClass)
    {
        pendingCharacterClass = characterClass; // Wir merken uns die Klasse
        characterSelectPanel.SetActive(false);  // Liste ausblenden
        characterDetailPanel.SetActive(true);   // Detail-Ansicht einblenden
    }

    // Wird von den "Männlich" / "Weiblich" Buttons im Detail-Panel aufgerufen
    public void SelectGenderAndStart(string gender)
    {
        // Wir kombinieren Klasse und Geschlecht, z.B. "Krieger_male"
        string finalCharId = pendingCharacterClass + "_" + gender;

        StartNewGame(finalCharId); // Spiel starten!
    }
    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Spiel wird beendet!");
    }

    // --- Spiel starten ---

    public void StartNewGame(string characterId)
    {
        if (GameState.I != null)
        {
            GameState.I.currentParty.Clear(); // <--- DAS HIER FEHLT!
            GameState.I.selectedCharacterId = characterId;
            GameState.I.hasSavedPosition = false;

            // WICHTIG: Hier setzen wir die Standardwerte für ein frisches Spiel
            // Damit man nicht die Werte vom vorherigen Slot mitschleppt
            if (GameManager.I != null)
            {
                GameManager.I.energy = 0;
                GameManager.I.credibility = 0;
                GameManager.I.feather = 0;
            }
        }

        SceneManager.LoadScene("StartSequence");
    }

    // Diese Methode wird jetzt vom SaveSlotMenu aufgerufen, wenn man einen Slot zum Laden wählt
    public void LoadSavedGame(int slot)
    {
        if (GameState.I != null)
        {
            GameState.I.LoadGame(slot);
        }
        SceneManager.LoadScene("ForestScene");
    }

    public void CheckLoadButton()
    {
        if (loadGameButton != null && GameState.I != null)
        {
            // Prüft alle 3 Slots
            bool hasAnySave = GameState.I.HasSave(1) || GameState.I.HasSave(2) || GameState.I.HasSave(3);
            loadGameButton.interactable = hasAnySave;
        }
    }

    // Ruf das in der Start() auf und jedes Mal, wenn gelöscht wurde!
}