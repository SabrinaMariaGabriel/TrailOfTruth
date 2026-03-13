using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("Slot-System")]
    public int currentSlot = 1; // Standard-Slot

    [Header("Daten")]
    private HashSet<string> companions = new HashSet<string>();
    public List<string> currentParty = new List<string>();
    public Vector3 lastPlayerPosition;
    public bool hasSavedPosition = false;
    public bool martinTalked = false;
    public string selectedCharacterId = "male";

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

    // PRÜFEN: Existiert ein Spielstand? (Für das Ausgrauen im Menü)
    public bool HasSave(int slot)
    {
        return PlayerPrefs.HasKey("Slot" + slot + "_CharId");
    }

    // BEGLEITER LOGIK
    public void AddCompanion(string id)
    {
        companions.Add(id);
        if (!currentParty.Contains(id)) currentParty.Add(id);
        RefreshParty();
    }

    public void RemoveCompanion(string id)
    {
        companions.Remove(id);
        currentParty.Remove(id);
        RefreshParty();
    }

    public bool HasCompanion(string id) => companions.Contains(id);

    public List<string> GetCurrentParty()
    {
        if (currentParty.Count == 0 && HasCompanion("martin"))
            currentParty.Add("martin");
        return currentParty;
    }

    // SPEICHERN
    // SPEICHERN
    public void SaveGame()
    {
        string p = "Slot" + currentSlot + "_";

        PlayerPrefs.SetString(p + "CharId", selectedCharacterId);
        PlayerPrefs.SetFloat(p + "PosX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat(p + "PosY", lastPlayerPosition.y);
        PlayerPrefs.SetInt(p + "MartinTalked", martinTalked ? 1 : 0);

        // Begleiter-Liste als Text speichern (z.B. "martin,robin")
        string partyData = string.Join(",", currentParty);
        PlayerPrefs.SetString(p + "Party", partyData);

        if (GameManager.I != null)
        {
            PlayerPrefs.SetInt(p + "Energy", GameManager.I.energy);
            PlayerPrefs.SetInt(p + "Credibility", GameManager.I.credibility);
            PlayerPrefs.SetInt(p + "Feather", GameManager.I.feather);
        }

        // Datum UND Uhrzeit für die Slot-Anzeige
        string timestamp = System.DateTime.Now.ToString("dd.MM.yyyy HH:mm"); // z.B. 13.03.2026 09:15
        PlayerPrefs.SetString(p + "SaveDate", timestamp);

        PlayerPrefs.Save();
        Debug.Log($"<color=orange>In Slot {currentSlot} gespeichert um {timestamp}!</color>");
    }

    // LADEN
    public void LoadGame(int slot)
    {
        currentSlot = slot;
        string p = "Slot" + slot + "_";

        if (HasSave(slot))
        {
            selectedCharacterId = PlayerPrefs.GetString(p + "CharId");
            lastPlayerPosition = new Vector3(PlayerPrefs.GetFloat(p + "PosX"), PlayerPrefs.GetFloat(p + "PosY"), 0);
            martinTalked = PlayerPrefs.GetInt(p + "MartinTalked") == 1;

            // Stats laden
            if (GameManager.I != null)
            {
                GameManager.I.energy = PlayerPrefs.GetInt(p + "Energy", 0);
                GameManager.I.credibility = PlayerPrefs.GetInt(p + "Credibility", 0);
                GameManager.I.feather = PlayerPrefs.GetInt(p + "Feather", 0);
                GameManager.I.RefreshUI();
            }

            // Party/Begleiter laden
            string partyString = PlayerPrefs.GetString(p + "Party", "");
            currentParty = new List<string>(partyString.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries));

            // HashSet für die HasCompanion-Logik füllen
            companions.Clear();
            foreach (string id in currentParty) companions.Add(id);

            hasSavedPosition = true;
            Debug.Log($"<color=green>Slot {slot} geladen!</color>");
        }
    }

    // LÖSCHEN
    public void DeleteSave(int slot)
    {
        string p = "Slot" + slot + "_";
        PlayerPrefs.DeleteKey(p + "CharId");
        PlayerPrefs.DeleteKey(p + "PosX");
        PlayerPrefs.DeleteKey(p + "PosY");
        PlayerPrefs.DeleteKey(p + "MartinTalked");
        PlayerPrefs.DeleteKey(p + "Energy");
        PlayerPrefs.DeleteKey(p + "Credibility");
        PlayerPrefs.DeleteKey(p + "Feather");
        PlayerPrefs.DeleteKey(p + "Party");
        PlayerPrefs.DeleteKey(p + "SaveDate");
        PlayerPrefs.Save();
        Debug.Log($"<color=red>Slot {slot} gelöscht!</color>");
    }

    public void RefreshParty()
    {
        FollowPlayer[] followers = Object.FindObjectsByType<FollowPlayer>(FindObjectsSortMode.None);
        foreach (var follower in followers) follower.UpdateChainTarget();
    }
}