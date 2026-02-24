using UnityEngine;
using UnityEngine.SceneManagement;

public class NpcApproachAndTalk : MonoBehaviour
{
    [Header("NPC Setup")]
    public string dialogueSceneName = "Dialogue_Martin";
    public float interactDistance = 0.8f;

    [Header("Refs")]
    public Transform player;                 // Player Transform
    public Vector3 approachOffset = new Vector3(-0.4f, 0f, 0f);

    bool isApproaching = false;
    TapToMove2D playerMove;

    // Wird von TapToMove2D aufgerufen
    public void OnTapped(TapToMove2D mover)
    {
        if (player == null)
        {
            Debug.LogError("NpcApproachAndTalk: Player Transform fehlt!");
            return;
        }

        playerMove = mover;
        if (playerMove == null)
        {
            Debug.LogError("NpcApproachAndTalk: TapToMove2D mover ist null!");
            return;
        }

        Vector3 target = transform.position + approachOffset;
        playerMove.SetTarget(target);
        isApproaching = true;

        Debug.Log("NPC tapped -> approaching. Target: " + target);
    }

    void Update()
    {
        if (!isApproaching || player == null) return;

        float dist = Vector2.Distance(player.position, transform.position);

        //if (dist <= interactDistance)
        //{
        //   isApproaching = false;
        //   playerMove.Stop();
        //  Debug.Log("In range -> load dialogue: " + dialogueSceneName);
        // SceneManager.LoadScene(dialogueSceneName);
        // }

        if (dist <= interactDistance)
        {
            isApproaching = false;
            playerMove.Stop();
            Debug.Log("In range -> load dialogue: " + dialogueSceneName);

            // NEU: Position speichern, bevor die Szene wechselt!
            if (GameState.I != null)
            {
                GameState.I.lastPlayerPosition = player.position;
                GameState.I.hasSavedPosition = true;
            }

            SceneManager.LoadScene(dialogueSceneName);
        }
    }
}