using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BirdTutorialController : MonoBehaviour
{
    [Header("Kamera & FX")]
    public Camera mainCamera;
    public float shakeMagnitude = 0.15f;

    [Header("UI Gruppen")]
    public GameObject hudTopLeft;
    public GameObject answersGrid;

    [Header("Dialog UI")]
    public TMP_Text npcLineText;
    public Button[] answerButtons;
    public TMP_Text[] answerButtonTexts;

    [Header("Icons für Erklärungen")]
    public GameObject iconEnergy;      // Das Herz
    public GameObject iconCredibility; // Der Fingerabdruck
    public GameObject iconFeather;     // Die Feder

    [Header("NPC & Werte")]
    public NpcExpressionController birdExpression;
    public GameManager gameManager;

    private int stepIndex = 0;
    private bool isWaiting = false;
    private GameObject currentPulsingIcon;

    void Start()
    {
        // Initialer Zustand
        hudTopLeft.SetActive(false);
        answersGrid.SetActive(false);

        // Schreck-Moment verzögert auslösen
        Invoke(nameof(ForceShock), 0.05f);
        SetLine("AAHH! Ein Mensch?! Mitten im tiefen Wald?!");
    }

    void ForceShock()
    {
        if (birdExpression != null) birdExpression.SetAngry();
        if (mainCamera != null) StartCoroutine(ScreenShake(0.4f, shakeMagnitude));
    }

    public void Advance()
    {
        if (isWaiting) return;
        StartCoroutine(ClickDelay());
        stepIndex++;

        switch (stepIndex)
        {
            case 1:
                birdExpression?.SetNeutral();
                SetLine("Puh... mein Herz. Du hast mich erschreckt! Du siehst aus, als wärst du gerade erst aus einem Albtraum erwacht.");
                break;
            case 2:
                birdExpression?.SetSad();
                SetLine("Dieser Wald hier... er ist tückisch. Er frisst dein Leben, wenn du nicht aufpasst.");
                break;
            case 3:
                // Hier fragen wir nach dem Profi-Status (Vogel schaut freundlich/happy)
                birdExpression?.SetNeutral();
                StartSkipAbfrage();
                break;
            case 4:
                // Falls "Nein" gewählt wurde
                birdExpression?.SetNeutral();
                SetLine("Na gut, dann hör mir gut zu. Da oben links... siehst du das?");
                StartCoroutine(ShowHUDOnly());
                break;
            case 5:
                SetLine("Dieses rote Herz dort oben ist deine Energie. Ohne sie klappst du im Unterholz einfach zusammen.");
                StartCoroutine(PulseIcon(iconEnergy));
                break;
            case 6:
                SetLine("Der Fingerabdruck zeigt deine Glaubwürdigkeit. Manche Wege öffnen sich nur denen, die einen reinen Namen haben.");
                StartCoroutine(PulseIcon(iconCredibility));
                break;
            case 7:
                SetLine("Und die Feder? Die steht für dein Geschick. Wer flink ist, kommt an Orte, die anderen verwehrt bleiben.");
                StartCoroutine(PulseIcon(iconFeather));
                break;
            case 8:
                // Pulsieren stoppen
                currentPulsingIcon = null;
                birdExpression?.SetHappy();
                SetLine("Ich hab dir mal was von meinem Vorrat gegeben. Schau!");
                break;
            case 9:
                // Werte erst nach der Erklärung vergeben
                StartCoroutine(FlashSequence());
                break;
            case 10:
                birdExpression?.SetSad();
                SetLine("Du wirst Freunde brauchen, Mensch. Der Wald kennt keine Gnade für Einsame.");
                break;
            case 11:
                birdExpression?.SetHappy();
                SetLine("Aber keine Sorge, ich behalte dich im Auge. Wir sehen uns zwischen den Bäumen!");
                break;
            case 12:
                SceneManager.LoadScene("ForestScene");
                break;
        }
    }

    // --- LOGIK FUNKTIONEN ---

    IEnumerator ShowHUDOnly()
    {
        hudTopLeft.SetActive(true);
        hudTopLeft.transform.localScale = Vector3.zero;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            hudTopLeft.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }
        hudTopLeft.transform.localScale = Vector3.one;
    }

    IEnumerator PulseIcon(GameObject icon)
    {
        // Altes Icon zurücksetzen
        if (currentPulsingIcon != null) currentPulsingIcon.transform.localScale = Vector3.one;

        currentPulsingIcon = icon;
        if (currentPulsingIcon == null) yield break;

        Vector3 origScale = Vector3.one;
        GameObject target = icon;

        while (currentPulsingIcon == target)
        {
            float s = 1f + Mathf.Sin(Time.time * 6f) * 0.15f;
            target.transform.localScale = origScale * s;
            yield return null;
        }
        target.transform.localScale = origScale;
    }

    IEnumerator FlashSequence()
    {
        isWaiting = true;

        // Herz (Energy)
        if (gameManager.energyBar != null)
        {
            StartCoroutine(FlashBar(gameManager.energyBar.GetComponentInChildren<Image>()));
            gameManager.AddEnergy(80);
        }
        yield return new WaitForSeconds(0.4f);

        // Fingerabdruck (Credibility)
        if (gameManager.credibilityBar != null)
        {
            StartCoroutine(FlashBar(gameManager.credibilityBar.GetComponentInChildren<Image>()));
            gameManager.AddCredibility(50);
        }
        yield return new WaitForSeconds(0.4f);

        // Feder (Agility)
        if (gameManager.featherBar != null)
        {
            StartCoroutine(FlashBar(gameManager.featherBar.GetComponentInChildren<Image>()));
            gameManager.AddFeather(30);
        }

        yield return new WaitForSeconds(0.5f);
        isWaiting = false;
        SetLine("Schon besser, oder? Damit lässt es sich arbeiten.");
    }

    // --- HILFSFUNKTIONEN ---

    void StartSkipAbfrage()
    {
        isWaiting = true;
        answersGrid.SetActive(true);
        SetLine("Sag mal... hast du sowas schon mal erlebt? Warst du schon mal hier?");
        SetupTwoButtons("Ja, bin Profi!", "Nein, erklär's mir.");

        answerButtons[0].onClick.RemoveAllListeners();
        answerButtons[0].onClick.AddListener(() => {
            SetLine("Ah, also ein Profi! Sicher, dass du keine Hilfe brauchst?");
            SetupTwoButtons("Ab in den Wald!", "Doch lieber Tutorial.");
            answerButtons[0].onClick.RemoveAllListeners();
            answerButtons[0].onClick.AddListener(() => SceneManager.LoadScene("ForestScene"));
            answerButtons[1].onClick.RemoveAllListeners();
            answerButtons[1].onClick.AddListener(() => ContinueTutorial());
        });

        answerButtons[1].onClick.RemoveAllListeners();
        answerButtons[1].onClick.AddListener(() => ContinueTutorial());
    }

    void SetupTwoButtons(string t0, string t1)
    {
        answerButtonTexts[0].text = t0; answerButtonTexts[1].text = t1;
        answerButtons[0].gameObject.SetActive(true); answerButtons[1].gameObject.SetActive(true);
        if (answerButtons.Length > 2) answerButtons[2].gameObject.SetActive(false);
        if (answerButtons.Length > 3) answerButtons[3].gameObject.SetActive(false);
    }

    void ContinueTutorial()
    {
        isWaiting = false;
        answersGrid.SetActive(false);
        Advance();
    }

    IEnumerator ClickDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(0.4f);
        if (!answersGrid.activeSelf) isWaiting = false;
    }

    void SetLine(string t) => npcLineText.text = t;

    IEnumerator ScreenShake(float duration, float magnitude)
    {
        Vector3 originalPos = mainCamera.transform.position;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            mainCamera.transform.position = new Vector3(originalPos.x + x, originalPos.y + y, originalPos.z);
            elapsed += Time.deltaTime;
            yield return null;
        }
        mainCamera.transform.position = originalPos;
    }

    IEnumerator FlashBar(Image barFill)
    {
        if (barFill == null) yield break;
        Color orig = barFill.color;
        float t = 0;
        while (t < 0.2f)
        {
            t += Time.deltaTime * 5f;
            barFill.color = Color.Lerp(orig, Color.white, t);
            yield return null;
        }
        t = 0;
        while (t < 1f)
        {
            t += Time.deltaTime * 2f;
            barFill.color = Color.Lerp(Color.white, orig, t);
            yield return null;
        }
        barFill.color = orig;
    }
}