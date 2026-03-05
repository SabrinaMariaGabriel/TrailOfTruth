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

    [Header("Buttons")]
    public Button loadGameButton; // Damit wir ihn ausgrauen können, wenn es kein Savegame gibt

    // Hier merken wir uns, auf welchen Charakter in der Liste geklickt wurde
    private string pendingCharacterClass = "";

    void Start()
    {
        ShowMainMenu();

        // Prüfen, ob ein Spielstand existiert. Wenn nicht, den "Laden"-Button deaktivieren.
        if (loadGameButton != null)
        {
            loadGameButton.interactable = PlayerPrefs.HasKey("Save_CharId");
        }
    }

    // --- Panel Navigation ---

    public void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        startGamePanel.SetActive(false);
        characterSelectPanel.SetActive(false);
    }

    public void ShowStartGameMenu()
    {
        mainMenuPanel.SetActive(false);
        startGamePanel.SetActive(true);
        characterSelectPanel.SetActive(false);
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
        // 1. Dem GameState den Charakter sagen
        if (GameState.I != null)
        {
            GameState.I.selectedCharacterId = characterId;
            GameState.I.hasSavedPosition = false; // Wichtig: Beim neuen Spiel starten wir am Standard-Punkt!
        }

        // 2. Szene laden (Cutscene)
        SceneManager.LoadScene("StartSequence");
    }

    public void LoadSavedGame()
    {
        // 1. Spielstand in den GameState laden
        if (GameState.I != null)
        {
            GameState.I.LoadGame();
        }

        // 2. Szene laden (der Spieler wird dann automatisch an die geladene Position gesetzt)
        SceneManager.LoadScene("ForestScene");
    }
}