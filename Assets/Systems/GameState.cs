using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    private HashSet<string> companions = new HashSet<string>();

    // NEU: Hier speichern wir die Position
    public Vector3 lastPlayerPosition;
    public bool hasSavedPosition = false;

    public bool martinTalked = false; // Hat der Spieler mit Martin gesprochen?


    void Awake()
    {
        if (I != null && I != this)
        {
            Destroy(gameObject);
            return;
        }
        I = this;
        DontDestroyOnLoad(gameObject);
    }

    public void AddCompanion(string id)
    {
        companions.Add(id);
        Debug.Log("AddCompanion: " + id);
    }

    public void RemoveCompanion(string id)
    {
        companions.Remove(id);
        Debug.Log("RemoveCompanion: " + id);
    }

    public bool HasCompanion(string id) => companions.Contains(id);

    // NEU: Welcher Charakter wurde im Menü gewählt?
    public string selectedCharacterId = "male"; // Standardwert

    // Speichert den aktuellen Stand fest auf dem Gerät
    public void SaveGame()
    {
        PlayerPrefs.SetString("Save_CharId", selectedCharacterId);
        PlayerPrefs.SetFloat("Save_PosX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat("Save_PosY", lastPlayerPosition.y);
        PlayerPrefs.SetInt("Save_MartinTalked", martinTalked ? 1 : 0);

        PlayerPrefs.Save(); // Speichervorgang abschließen
        Debug.Log("Spiel gespeichert!");
    }

    // Lädt den Stand vom Gerät in den GameState
    public void LoadGame()
    {
        if (PlayerPrefs.HasKey("Save_CharId"))
        {
            selectedCharacterId = PlayerPrefs.GetString("Save_CharId");

            float x = PlayerPrefs.GetFloat("Save_PosX");
            float y = PlayerPrefs.GetFloat("Save_PosY");
            lastPlayerPosition = new Vector3(x, y, 0);

            hasSavedPosition = true; // Damit der Spieler beim Laden dorthin teleportiert wird
            Debug.Log("Spiel geladen: " + selectedCharacterId + " an Position " + lastPlayerPosition);

            martinTalked = PlayerPrefs.GetInt("Save_MartinTalked", 0) == 1;
        }
    }
}