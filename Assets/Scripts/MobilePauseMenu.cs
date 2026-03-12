using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

public class MobilePauseMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject pausePanel;
    public GameObject quitConfirmPanel;
    public GameObject joystickHUD; // Zieh hier dein Joystick/Interface rein

    [Header("Feedback")]
    public GameObject feedbackObject; // Ein UI-Element, das den Text anzeigt (z.B. ein Panel mit TextMeshPro)

    void Start()
    {
        pausePanel.SetActive(false);
        quitConfirmPanel.SetActive(false);

        // Sicherstellen, dass die Speicher-Anzeige am Anfang unsichtbar ist
        if (feedbackObject != null)
        {
            feedbackObject.SetActive(false);
            // Wir setzen den Alpha-Wert der Canvas Group sicherheitshalber auf 0
            CanvasGroup cg = feedbackObject.GetComponent<CanvasGroup>();
            if (cg != null) cg.alpha = 0;
        }
    }

    void Update()
    {
        // Android Back-Button / ESC
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleBackAction();
        }
    }

    public void HandleBackAction()
    {
        if (quitConfirmPanel.activeSelf)
        {
            quitConfirmPanel.SetActive(false);
        }
        else if (pausePanel.activeSelf)
        {
            ClosePauseMenu();
        }
        else
        {
            OpenPauseMenu();
        }
    }

    public void OpenPauseMenu()
    {
        pausePanel.SetActive(true);
        // Nur ausblenden, wenn auch wirklich was im Slot liegt!
        if (joystickHUD != null) joystickHUD.SetActive(false);
        Time.timeScale = 0f;
    }

    public void ClosePauseMenu()
    {
        pausePanel.SetActive(false);
        // Nur einblenden, wenn auch wirklich was im Slot liegt!
        if (joystickHUD != null) joystickHUD.SetActive(true);
        Time.timeScale = 1f;
    }

    public void SaveGame()
    {
        if (GameState.I != null)
        {
            GameState.I.SaveGame();
            StartCoroutine(ShowFeedback("Spielstand gesichert!"));
        }
    }

    public void SaveAndQuit()
    {
        if (GameState.I != null)
        {
            GameState.I.SaveGame();
            // Wir starten das Feedback und warten kurz, bevor wir die Szene wechseln
            StartCoroutine(SaveAndQuitSequence());
        }
    }

    IEnumerator SaveAndQuitSequence()
    {
        // Zeige den Text an
        yield return StartCoroutine(ShowFeedback("Erfolgreich gespeichert! Kehre zurück..."));

        // Kurze extra Pause für das Auge
        yield return new WaitForSecondsRealtime(0.5f);

        // Jetzt erst beenden
        ConfirmQuit();
    }

    public void ConfirmQuit()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    IEnumerator ShowFeedback(string msg)
    {
        // Wir holen uns den Text, der irgendwo im Objekt steckt
        TMP_Text t = feedbackObject.GetComponentInChildren<TMP_Text>();
        if (t != null) t.text = msg;

        CanvasGroup cg = feedbackObject.GetComponent<CanvasGroup>();
        if (cg == null) cg = feedbackObject.AddComponent<CanvasGroup>();

        feedbackObject.SetActive(true);

        // Fade In
        float duration = 0.5f;
        float currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(0, 1, currentTime / duration);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(1.5f);

        // Fade Out
        currentTime = 0f;
        while (currentTime < duration)
        {
            currentTime += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(1, 0, currentTime / duration);
            yield return null;
        }
        feedbackObject.SetActive(false);
    }
}