using UnityEngine;

public class PartyDebugger : MonoBehaviour
{
    void Update()
    {
        // Taste M für Martin (zum Testen, falls er noch nicht da ist)
        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("<color=cyan>Debug: Martin tritt der Gruppe bei!</color>");
            GameState.I.AddCompanion("martin");
        }

        // Taste A für Antonia (simuliert den gewonnenen Dialog)
        if (Input.GetKeyDown(KeyCode.A))
        {
            Debug.Log("<color=magenta>Debug: Antonia tritt der Gruppe bei!</color>");
            GameState.I.AddCompanion("antonia");
        }

        // Taste R für Reset (wirft alle raus, zum Testen von Lücken)
        if (Input.GetKeyDown(KeyCode.R))
        {
            Debug.Log("<color=yellow>Debug: Gruppe geleert!</color>");
            GameState.I.currentParty.Clear();
            GameState.I.RefreshParty();
        }
    }
}