using System.Collections;
using UnityEngine;
using UnityEngine.UI; // Wichtig für das Fade-Bild
using UnityEngine.SceneManagement; // Wichtig für den Szenenwechsel

public class IntroSequence : MonoBehaviour
{
    [Header("Die Schauspieler")]
    public Transform bird;       // Unser Vogel
    public Transform player;     // Der liegende Held
    public Image fadeScreen;     // Das schwarze Bild im Canvas

    [Header("Die Einstellungen")]
    public float birdSpeed = 2f;
    public string nextSceneName = "DialogScene"; // WICHTIG: Hier den exakten Namen deiner nächsten Szene eintragen!

    void Start()
    {
        // Sobald die Szene startet, rufen wir "Action!"
        StartCoroutine(PlayIntroFilm());
    }

    IEnumerator PlayIntroFilm()
    {
        // 1. SZENE WIRD HELL (Fade In)
        fadeScreen.color = new Color(0, 0, 0, 1); // Startet komplett schwarz
        while (fadeScreen.color.a > 0)
        {
            float newAlpha = fadeScreen.color.a - (Time.deltaTime / 2f); // Dauert 2 Sekunden
            fadeScreen.color = new Color(0, 0, 0, newAlpha);
            yield return null; // Warte einen Frame
        }

        // 1 Sekunde Pause, damit der Spieler sich umsehen kann
        yield return new WaitForSeconds(1f);

        // 2. VOGEL FLIEGT ZUM SPIELER
        // Wir setzen das Ziel ein kleines Stück neben den Spieler, damit er nicht auf seinem Gesicht landet
        Vector3 targetPos = player.position + new Vector3(1f, 0.5f, 0f);

        while (Vector3.Distance(bird.position, targetPos) > 0.1f)
        {
            // Bewegt den Vogel Stück für Stück zum Ziel
            bird.position = Vector3.MoveTowards(bird.position, targetPos, birdSpeed * Time.deltaTime);
            yield return null;
        }

        // 3. DRAMATISCHE PAUSE (Vogel betrachtet den Spieler)
        yield return new WaitForSeconds(2f);

        // 4. SZENE WIRD WIEDER DUNKEL (Fade Out)
        while (fadeScreen.color.a < 1)
        {
            float newAlpha = fadeScreen.color.a + (Time.deltaTime / 1.5f);
            fadeScreen.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        // 5. SCHNITT! (Lade die nächste Szene)
        SceneManager.LoadScene(nextSceneName);
    }
}