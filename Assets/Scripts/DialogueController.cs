using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    [Header("NPC")]
    public NpcExpressionController npcExpression;

    [Header("Scene Flow")]
    public string returnSceneName = "ForestScene";

    [Header("UI")]
    public TMP_Text npcLineText;
    public Button dialogPanelButton;   // Button auf dem DialogPanel (Tap to advance)
    public GameObject answersGrid;     // Parent der Buttons

    public Button[] answerButtons;       // 4 Buttons
    public TMP_Text[] answerButtonTexts; // 4 TMP Texte auf den Buttons

    [Header("UI - Spezial")]
    public GameObject companionIcon; // Das Bild oben rechts für Martin

    int stepIndex = 0;
    bool waitingForAnswer = false;
    bool dialogueFinished = false;

    // “eine fiese kann alles ruinieren”
    int martinTrust = 0;

    void Start()
    {
        if (answersGrid != null) answersGrid.SetActive(false);

        // Sicherstellen, dass das HUD die aktuellen Werte zeigt
        if (GameManager.I != null) GameManager.I.RefreshUI();

        if (dialogPanelButton != null)
        {
            dialogPanelButton.onClick.RemoveAllListeners();
            dialogPanelButton.onClick.AddListener(Advance);
        }

        stepIndex = 0;
        ShowCurrentStep();
    }

    public void Advance()
    {
        if (waitingForAnswer) return;

        // Wenn der Dialog beendet ist, ignorieren wir weitere Klicks, 
        // weil die Coroutine (WaitAndExit) den Szenenwechsel übernimmt.
        if (dialogueFinished) return;

        stepIndex++;
        ShowCurrentStep();
    }

    void ShowCurrentStep()
    {
        // Intro / normaler Dialog
        if (stepIndex == 0)
        {
            npcExpression?.SetNeutral();
            SetLine("Ich bin’s, Bruder Martin. Wie geht es dir?");
            return;
        }

        if (stepIndex == 1)
        {
            npcExpression?.SetHappy();
            SetLine("Schön, dich zu sehen. Du wirkst… irgendwie verloren.");
            return;
        }

        if (stepIndex == 2)
        {
            npcExpression?.SetNeutral();
            SetLine("Ich begleite hier viele Reisende… aber es ist nicht immer leicht.");
            return;
        }

        if (stepIndex == 3)
        {
            npcExpression?.SetSad();
            SetLine("Manchmal bin ich selbst müde.");
            return;
        }

        if (stepIndex == 4)
        {
            npcExpression?.SetSad();
            SetLine("Ich muss dir etwas gestehen...");
            return;
        }

        // 3 Fragen
        if (stepIndex == 5) { ShowQuestion1(); return; }
        if (stepIndex == 6) { ShowQuestion2(); return; }
        if (stepIndex == 7) { ShowQuestion3(); return; }

        // Ergebnis
        if (stepIndex == 8)
        {
            if (answersGrid != null) answersGrid.SetActive(false);

            // NEU: Wir markieren Martin IMMER als "gesprochen", 
            // damit er nicht mehr im Wald als NPC herumsteht.
            if (GameState.I != null)
            {
                GameState.I.martinTalked = true;
                GameState.I.SaveGame(); // Speichern, damit es permanent bleibt
            }

            bool comesWithYou = martinTrust >= 3;

            if (comesWithYou)
            {
                npcExpression?.SetHappy();
                SetLine("Ich habe mich entschieden: Ich komme mit dir!");

                // DAS ICON EINSCHALTEN
                if (companionIcon != null) companionIcon.SetActive(true);
                StartCoroutine(PloppAnimation(companionIcon)); // Starte den Effekt

                if (GameState.I != null) GameState.I.AddCompanion("martin");
            }
            else
            {
                npcExpression?.SetSad();
                SetLine("Du setzt deine Reise allein fort.");
                if (GameState.I != null) GameState.I.RemoveCompanion("martin");
            }

            dialogueFinished = true;

            // NEU: Wir warten kurz, damit man das Icon/Text sieht, bevor die Szene wechselt
            StartCoroutine(WaitBeforeSceneChange());
            return;
        }
    }

    // -------------------------
    // QUESTIONS
    // -------------------------

    void ShowQuestion1()
    {
        waitingForAnswer = true;
        ShowAnswers(true);

        npcExpression?.SetSad();
        SetLine("Ich hab so viele Menschen begleitet…\n\naber manchmal frage ich mich: Was, wenn ich selbst gar nicht genug Glauben habe?");

        SetAnswers(
            "Danke für deine Ehrlichkeit.",
            "Aber du bist doch Pfarrer!",
            "Zweifel = geistlich schwach.",
            "Darf ich für dich beten?"
        );

        ClearButtonListeners();

        answerButtons[0].onClick.AddListener(() =>
            ChooseAnswer(reaction: "Bruder Martin lächelt erleichtert.", good: true, trustGain: +2, cred: +10, en: 0, fe: +2));

        answerButtons[1].onClick.AddListener(() =>
            ChooseAnswer(reaction: "Bruder Martin schaut weg.", good: false, trustGain: 0, cred: -3, en: 0, fe: -1));

        answerButtons[2].onClick.AddListener(() =>
            ChooseAnswer(reaction: "Bruder Martin wirkt verletzt.", good: false, trustGain: -3, cred: -10, en: -5, fe: -2)); // fies -> ruiniert

        answerButtons[3].onClick.AddListener(() =>
            ChooseAnswer(reaction: "Bruder Martin nickt dankbar.", good: true, trustGain: +2, cred: +8, en: +3, fe: +1));
    }

    void ShowQuestion2()
    {
        waitingForAnswer = true;
        ShowAnswers(true);

        npcExpression?.SetSad();
        SetLine("Weißt du…\n\nich habe Angst, dass ich selbst irgendwann aufgebe.");

        SetAnswers(
            "Du musst nicht perfekt sein.",
            "Dann bist du ungeeignet.",
            "Jesus geht mit dir, auch im Zweifel.",
            "Reiß dich zusammen."
        );

        ClearButtonListeners();

        answerButtons[0].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin atmet auf.", true, +1, +5, 0, +1));

        answerButtons[1].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin schaut dich entsetzt an.", false, -2, -10, -3, -2));

        answerButtons[2].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin wirkt berührt.", true, +2, +7, +2, +1));

        answerButtons[3].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin wird still.", false, -1, -5, -2, -1));
    }

    void ShowQuestion3()
    {
        waitingForAnswer = true;
        ShowAnswers(true);

        npcExpression?.SetNeutral();
        SetLine("Wenn ich mit dir weitergehe…\n\nwas ist, wenn wir scheitern?");

        SetAnswers(
            "Dann scheitern wir gemeinsam.",
            "Dann bleib lieber hier.",
            "Wir vertrauen Gott, nicht uns selbst.",
            "Scheitern ist keine Option."
        );

        ClearButtonListeners();

        answerButtons[0].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin lächelt warm.", true, +2, +6, +2, +1));

        answerButtons[1].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin wirkt traurig.", false, -2, -5, 0, -1));

        answerButtons[2].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin wirkt überzeugt.", true, +2, +8, +1, +2));

        answerButtons[3].onClick.AddListener(() =>
            ChooseAnswer("Bruder Martin wird angespannt.", false, -1, -4, -1, -1));
    }

    // -------------------------
    // HELPERS
    // -------------------------

    void ChooseAnswer(string reaction, bool good, int trustGain, int cred, int en, int fe)
    {
        DisableAnswerButtons();

        martinTrust += trustGain;

        if (good) npcExpression?.SetHappy();
        else npcExpression?.SetAngry();

        // Reaktionstext anzeigen
        SetLine(reaction);

        // --- ÄNDERUNG HIER: Nutze das Singleton GameManager.I ---
        if (GameManager.I != null)
        {
            GameManager.I.AddCredibility(cred);
            GameManager.I.AddEnergy(en);
            GameManager.I.AddFeather(fe);
        }
        else
        {
            Debug.LogWarning("Kein GameManager in der Szene gefunden!");
        }

        // Antwort-Buttons direkt ausblenden
        ShowAnswers(false);

        waitingForAnswer = false;
    }

    //IEnumerator AfterAnswerFlow()
    //{
    //     yield return new WaitForSeconds(0.35f);
    //    ShowAnswers(false);

    // Reaktion kurz stehen lassen, damit man sie lesen kann
    //     yield return new WaitForSeconds(0.9f);
    //
    //      stepIndex++;
    //      ShowCurrentStep();
    //  }




    void SetLine(string text)
    {
        if (npcLineText != null)
            npcLineText.text = text;
    }

    void SetAnswers(string a, string b, string c, string d)
    {
        answerButtonTexts[0].text = a;
        answerButtonTexts[1].text = b;
        answerButtonTexts[2].text = c;
        answerButtonTexts[3].text = d;
    }

    void ShowAnswers(bool show)
    {
        if (answersGrid != null)
            answersGrid.SetActive(show);
    }

    void DisableAnswerButtons()
    {
        foreach (var b in answerButtons)
            b.interactable = false;
    }

    void ClearButtonListeners()
    {
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].interactable = true;
        }
    }

    IEnumerator WaitAndExit()
    {
        // Warte 3 Sekunden, damit man das Icon und den Text sieht
        yield return new WaitForSeconds(3f);

        // Erst JETZT laden wir die Wald-Szene
        SceneManager.LoadScene(returnSceneName);
    }

    IEnumerator WaitBeforeSceneChange()
    {
        // Warte 3 Sekunden (Zeit für das Icon zum Glänzen!)
        yield return new WaitForSeconds(3f);

        // Erst jetzt wird die Szene gewechselt
        SceneManager.LoadScene(returnSceneName);
    }

    IEnumerator PloppAnimation(GameObject icon)
    {
        RectTransform rt = icon.GetComponent<RectTransform>();
        Vector3 originalScale = Vector3.one; // Zielgröße (1,1,1)

        // 1. Ganz klein starten
        rt.localScale = Vector3.zero;

        // 2. Größer werden als normal (Overshoot für den "Plopp")
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f; // Geschwindigkeit
            rt.localScale = Vector3.Lerp(Vector3.zero, originalScale * 1.2f, t);
            yield return null;
        }

        // 3. Auf Normalgröße zurückschwingen
        t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 5f;
            rt.localScale = Vector3.Lerp(originalScale * 1.2f, originalScale, t);
            yield return null;
        }
    }
}