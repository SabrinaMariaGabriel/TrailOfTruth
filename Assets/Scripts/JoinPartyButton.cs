using UnityEngine;

public class JoinPartyButton : MonoBehaviour
{
    [Header("Wer soll beitreten?")]
    public GameObject characterObject; // Hier Antonia aus der Hierarchy reinziehen
    public string characterId = "antonia";

    public void AddToParty()
    {
        if (GameState.I == null || characterObject == null) return;

        // 1. Logisch zur Party hinzufügen
        GameState.I.AddCompanion(characterId);

        // 2. Das Objekt EINSCHALTEN (Wacht aus dem SetActive(false) auf)
        characterObject.SetActive(true);

        // 3. Dem Skript sagen, dass es jetzt loslegen kann
        FollowPlayer fp = characterObject.GetComponent<FollowPlayer>();
        if (fp != null)
        {
            fp.UpdateChainTarget();
        }

        // 4. Den Rest der Gruppe informieren
        GameState.I.RefreshParty();

        Debug.Log($"<color=magenta>Button geklickt: {characterId} ist jetzt dabei!</color>");
    }
}