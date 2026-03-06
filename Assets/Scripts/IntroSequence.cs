using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro; // WICHTIG: Damit das Skript TextMeshPro versteht

public class IntroSequence : MonoBehaviour
{
    [Header("Die Schauspieler")]
    public Transform bird;
    public GameObject playerGroup; // NEU: Hier ziehen wir die "Player_Cutscene_Group" rein
    private Transform player;      // Wird automatisch befüllt
    private Animator playerAnimator; // Wird automatisch befüllt

    public Image fadeScreen;
    public Animator birdAnimator;

    [Header("Requisiten")]
    public TextMeshProUGUI skipText; // Hier ziehen wir das Text-Objekt rein
    public GameObject exclamationMark;

    [Header("Die Einstellungen")]
    public float birdSpeed = 6f;
    public string nextSceneName = "DialogScene";

    void Start()
    {
        SetupSelectedCharacter();
        StartCoroutine(PlayIntroFilm());
    }

    void SetupSelectedCharacter()
    {
        string chosenId = GameState.I.selectedCharacterId;
        bool found = false;

        foreach (Transform child in playerGroup.transform)
        {
            if (child.name == chosenId)
            {
                child.gameObject.SetActive(true);
                player = child;
                playerAnimator = child.GetComponent<Animator>();
                found = true;
            }
            else
            {
                child.gameObject.SetActive(false);
            }
        }

        if (!found) Debug.LogError("IntroSequence: Charakter '" + chosenId + "' nicht in der Gruppe gefunden!");
    }

    IEnumerator PlayIntroFilm()
    {
        // --- VORBEREITUNG ---
        // Text am Anfang unsichtbar machen (Alpha auf 0)
        if (skipText != null) skipText.color = new Color(skipText.color.r, skipText.color.g, skipText.color.b, 0);
        if (exclamationMark != null) exclamationMark.SetActive(false);

        // 1. SZENE WIRD HELL (Fade In)
        fadeScreen.color = new Color(0, 0, 0, 1);
        while (fadeScreen.color.a > 0)
        {
            float newAlpha = fadeScreen.color.a - (Time.deltaTime / 2.5f);
            fadeScreen.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        yield return new WaitForSeconds(1f);

        // --- TEXT SANFT EINFADEN ---
        if (skipText != null)
        {
            float t = 0;
            while (t < 1)
            {
                t += Time.deltaTime / 1f; // Text braucht 1 Sek zum Erscheinen
                skipText.color = new Color(skipText.color.r, skipText.color.g, skipText.color.b, t);
                yield return null;
            }
        }

        yield return new WaitForSeconds(1f);

        // 2. VOGEL STÜRZT AUS DEM HIMMEL AUF DEN BODEN
        Vector3 landingPos = new Vector3(bird.position.x, player.position.y, 0f);

        if (birdAnimator != null) birdAnimator.Play("Bird_Fly");

        while (Vector3.Distance(bird.position, landingPos) > 0.1f)
        {
            bird.position = Vector3.MoveTowards(bird.position, landingPos, birdSpeed * Time.deltaTime);
            yield return null;
        }

        if (birdAnimator != null) birdAnimator.Play("Bird_Idle");
        yield return new WaitForSeconds(0.6f);

        // 3. VOGEL HÜPFT ZUM SPIELER
        Vector3 targetPos = player.position + new Vector3(-1.0f, 0.7f, 0f);

        while (Vector3.Distance(bird.position, targetPos) > 0.1f)
        {
            if (birdAnimator != null) birdAnimator.Play("Bird_Fly");

            Vector3 startPos = bird.position;
            Vector3 nextHopPos = Vector3.MoveTowards(startPos, targetPos, 0.8f);

            float hopDuration = 0.8f / birdSpeed;
            float timer = 0f;

            while (timer < hopDuration)
            {
                timer += Time.deltaTime;
                float percent = timer / hopDuration;

                Vector3 currentPos = Vector3.Lerp(startPos, nextHopPos, percent);
                currentPos.y += Mathf.Sin(percent * Mathf.PI) * 0.6f;

                bird.position = currentPos;
                yield return null;
            }

            bird.position = nextHopPos;

            if (birdAnimator != null) birdAnimator.Play("Bird_Idle");
            yield return new WaitForSeconds(0.2f);
        }

        // 4. DRAMATISCHE PAUSE
        yield return new WaitForSeconds(1.5f);

        // 5. DER SCHRECK-MOMENT!
        if (playerAnimator != null) playerAnimator.Play("Player_WakeUp");

        if (exclamationMark != null) exclamationMark.SetActive(true);
        yield return new WaitForSeconds(0.8f);
        if (exclamationMark != null) exclamationMark.SetActive(false);


        // 6. VOGEL FLIEHT PANISCH NACH LINKS!
        Vector3 fleePos = bird.position + new Vector3(-3f, 0f, 0f);
        if (birdAnimator != null) birdAnimator.Play("Bird_Fly");

        while (Vector3.Distance(bird.position, fleePos) > 0.1f)
        {
            bird.position = Vector3.MoveTowards(bird.position, fleePos, (birdSpeed * 2) * Time.deltaTime);

            float offset = Mathf.Sin(Time.time * 20f) * 0.2f;
            bird.position = new Vector3(bird.position.x, fleePos.y + Mathf.Abs(offset), bird.position.z);

            yield return null;
        }

        // Text wieder ausblenden, damit er beim schwarzen Schirm nicht stört
        if (skipText != null) skipText.gameObject.SetActive(false);

        // 7. SZENE WIRD WIEDER DUNKEL (Fade Out)
        while (fadeScreen.color.a < 1)
        {
            float newAlpha = fadeScreen.color.a + (Time.deltaTime / 0.3f);
            fadeScreen.color = new Color(0, 0, 0, newAlpha);
            yield return null;
        }

        // 8. SCHNITT! 
        SceneManager.LoadScene(nextSceneName);
    }

    public void SkipCutscene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}