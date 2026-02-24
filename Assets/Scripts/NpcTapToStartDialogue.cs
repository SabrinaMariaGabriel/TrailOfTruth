using UnityEngine;
using UnityEngine.SceneManagement;

public class NpcTapToStartDialogue : MonoBehaviour
{
    public string dialogueSceneName = "Dialogue_Martin";
    public Transform player;
    public float interactDistance = 0.8f;

    void OnMouseDown()
    {
        if (player == null)
        {
            Debug.LogError("Player not set on NpcTapToStartDialogue!");
            return;
        }

        float dist = Vector2.Distance(player.position, transform.position);
        Debug.Log("Tap NPC - distance: " + dist);

        if (dist <= interactDistance)
        {
            SceneManager.LoadScene(dialogueSceneName);
        }
        else
        {
            Debug.Log("Too far away -> walk closer first.");
        }
    }
}