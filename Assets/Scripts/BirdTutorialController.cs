using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class BirdTutorialController : MonoBehaviour
{
    [Header("UI Gruppen")]
    public GameObject hudTopLeft;
    public GameObject answersGrid;

    [Header("Dialog UI")]
    public TMP_Text npcLineText;
    public Button[] answerButtons;
    public TMP_Text[] answerButtonTexts;

    [Header("NPC & Werte")]
    public NpcExpressionController birdExpression;
    public GameManager gameManager;

    private int stepIndex = 0;
    private bool isWaiting = false;

    void Start()
    {
        hudTopLeft.SetActive(false);
        answersGrid.SetActive(false);

        // Wir rufen die Expression erst nach einem winzigen Moment auf
        Invoke(nameof(ForceShock), 0.05f);

        SetLine("AAHH! Ein Mensch?! Mitten im tiefen Wald?!");
    }

    void ForceShock()
    {
        if (birdExpression != null) birdExpression.SetAngry();
    }

    public void Advance()
    {
        if (isWaiting) return;

        // Kleine Sperre, damit man nicht zu schnell klickt
        StartCoroutine(ClickDelay());

        stepIndex++;

        switch (stepIndex)
        {
            case 1:
                birdExpression?.SetNeutral();
                SetLine("Puh... du hast mich erschreckt. Du siehst ja ganz schön verloren aus.");
                break;
            case 2:
                StartSkipAbfrage();
                break;
            case 3:
                StoryAndUI();
                break;
        }
    }

    IEnumerator ClickDelay()
    {
        isWaiting = true;
        yield return new WaitForSeconds(0.4f); // Kurze Pause zwischen den Klicks
        if (answersGrid.activeSelf == false) isWaiting = false;
    }

    void StartSkipAbfrage()
    {
        isWaiting = true;
        answersGrid.SetActive(true);
        SetLine("Sag mal... hast du sowas schon mal erlebt? Warst du schon mal hier?");

        // Wir nutzen nur Button 0 und 1, die anderen schalten wir aus
        SetupTwoButtons("Ja, bin Profi!", "Nein, erklär's mir.");

        // Button JA (Index 0)
        answerButtons[0].onClick.RemoveAllListeners();
        answerButtons[0].onClick.AddListener(() => {
            SetLine("Ah, also ein Profi! Sicher, dass du keine Hilfe brauchst?");
            SetupTwoButtons("Ab in den Wald!", "Doch lieber Tutorial.");

            answerButtons[0].onClick.RemoveAllListeners();
            answerButtons[0].onClick.AddListener(() => SceneManager.LoadScene("ForestScene"));

            answerButtons[1].onClick.RemoveAllListeners();
            answerButtons[1].onClick.AddListener(() => ContinueTutorial());
        });

        // Button NEIN (Index 1)
        answerButtons[1].onClick.RemoveAllListeners();
        answerButtons[1].onClick.AddListener(() => ContinueTutorial());
    }

    // Hilfsfunktion um nur 2 Buttons zu zeigen
    void SetupTwoButtons(string text0, string text1)
    {
        answerButtonTexts[0].text = text0;
        answerButtonTexts[1].text = text1;

        answerButtons[0].gameObject.SetActive(true);
        answerButtons[1].gameObject.SetActive(true);

        // Die anderen zwei Buttons einfach verstecken
        if (answerButtons.Length > 2) answerButtons[2].gameObject.SetActive(false);
        if (answerButtons.Length > 3) answerButtons[3].gameObject.SetActive(false);
    }

    void ContinueTutorial()
    {
        isWaiting = false;
        answersGrid.SetActive(false);
        // Wir setzen alle Buttons wieder auf aktiv für spätere Dialoge
        foreach (var b in answerButtons) b.gameObject.SetActive(true);
        Advance();
    }

    void StoryAndUI()
    {
        SetLine("In diesem Wald brauchst du Kraft... und Freunde.");
        StartCoroutine(ShowHUD());
    }

    IEnumerator ShowHUD()
    {
        yield return new WaitForSeconds(0.8f);
        hudTopLeft.SetActive(true);

        // Kleiner Animationseffekt
        hudTopLeft.transform.localScale = Vector3.zero;
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * 3f;
            hudTopLeft.transform.localScale = Vector3.Lerp(Vector3.zero, Vector3.one, t);
            yield return null;
        }

        birdExpression?.SetHappy();
        SetLine("Siehst du das? Das sind deine Werte. Ich geb dir einen Startvorrat!");

        yield return new WaitForSeconds(1.2f);
        gameManager.AddCredibility(50);
        gameManager.AddEnergy(80);
        gameManager.AddFeather(30);
    }

    void SetLine(string t) => npcLineText.text = t;
}