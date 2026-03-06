using UnityEngine;

public class NpcVisibilityManager : MonoBehaviour
{
    void Start()
    {
        // Sobald der Wald lädt, prüft Martin: "Haben wir schon geredet?"
        if (GameState.I != null && GameState.I.martinTalked)
        {
            Debug.Log("Martin wurde bereits angesprochen und verschwindet nun aus der Szene.");
            gameObject.SetActive(false);
        }
    }
}