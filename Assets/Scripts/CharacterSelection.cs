using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelection : MonoBehaviour
{
    [Header("Scene to load")]
    public string gameSceneName = "ForestScene";

    // Diese Funktion rufen die Buttons auf
    public void ChooseCharacter(string characterId)
    {
        // 1. Wahl im GameState speichern
        if (GameState.I != null)
        {
            GameState.I.selectedCharacterId = characterId;
        }

        Debug.Log("Charakter gewählt: " + characterId);

        // 2. Das Spiel starten!
        SceneManager.LoadScene(gameSceneName);
    }
}