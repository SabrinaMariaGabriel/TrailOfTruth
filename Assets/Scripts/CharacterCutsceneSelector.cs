using UnityEngine;

public class CharacterCutsceneSelector : MonoBehaviour
{
    void Start()
    {
        // Wir fragen den GameState: "Wer wurde gewählt?"
        string chosen = GameState.I.selectedCharacterId;
        Debug.Log("Cutscene lädt Charakter: " + chosen);

        bool found = false;

        foreach (Transform child in transform)
        {
            if (child.name == chosen)
            {
                child.gameObject.SetActive(true);
                found = true;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        if (!found) Debug.LogError("Charakter '" + chosen + "' wurde in der Gruppe nicht gefunden!");
    }
}