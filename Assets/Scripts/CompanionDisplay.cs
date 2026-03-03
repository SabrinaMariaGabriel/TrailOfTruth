using UnityEngine;

public class CompanionDisplay : MonoBehaviour
{
    [Tooltip("Die ID muss exakt so geschrieben sein wie im AddCompanion-Befehl!")]
    public string companionID = "martin";

    void Start()
    {
        // Wir prüfen beim Start der Wald-Szene, ob Martin im GameState ist
        if (GameState.I != null && GameState.I.HasCompanion(companionID))
        {
            // Er ist dabei! Icon einschalten.
            gameObject.SetActive(true);
            Debug.Log(companionID + " wurde im Wald gefunden und wird angezeigt.");
        }
        else
        {
            // Er ist nicht dabei! Icon ausschalten.
            gameObject.SetActive(false);
        }
    }
}