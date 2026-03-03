using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UniversalDialogueController : MonoBehaviour
{
    [Header("Daten")]
    public DialogueData currentDialogue; // Hier kommt das Paket rein

    [Header("UI Referenzen")]
    public TMP_Text npcNameText;
    public TMP_Text npcLineText;
    public GameObject answersGrid;
    public Button[] answerButtons;
    public TMP_Text[] answerButtonTexts;

    [Header("NPC & Effekte")]
    public NpcExpressionController npcExpression;
    public GameObject companionIcon;

    [Header("Flow")]
    public string nextScene = "ForestScene";

    private int stepIndex = 0;
    private bool isWaiting = false;
    private int trust = 0;

    void Start()
    {
        if (answersGrid != null) answersGrid.SetActive(false);
        if (companionIcon != null) companionIcon.SetActive(false);

        if (currentDialogue != null)
        {
            npcNameText.text = currentDialogue.npcName;
            ShowStep();
        }
    }

    public void Advance() // Wird vom Panel-Button aufgerufen
    {
        if (isWaiting) return;

        stepIndex++;
        if (stepIndex >= currentDialogue.steps.Length)
        {
            EndDialogue();
        }
        else
        {
            ShowStep();
        }
    }

    void ShowStep()
    {
        var s = currentDialogue.steps[stepIndex];
        npcLineText.text = s.text;
        SetFace(s.face);

        if (s.isQuestion)
        {
            SetupQuestion(s);
        }
    }

    void SetupQuestion(DialogueStep s)
    {
        isWaiting = true;
        answersGrid.SetActive(true);
        for (int i = 0; i < answerButtons.Length; i++)
        {
            int j = i;
            answerButtonTexts[i].text = s.answers[i];
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => HandleChoice(j, s));
        }
    }

    void HandleChoice(int choiceIndex, DialogueStep s)
    {
        answersGrid.SetActive(false);
        isWaiting = false;

        trust += s.trustGains[choiceIndex];
        npcLineText.text = s.reactions[choiceIndex];

        // Stats im GameState oder GameManager anpassen
        if (GameState.I != null)
        {
            // Hier könntest du Stats ändern, falls gewünscht
        }
    }

    void EndDialogue()
    {
        if (trust >= 3 && !string.IsNullOrEmpty(currentDialogue.companionID))
        {
            GameState.I.AddCompanion(currentDialogue.companionID);
            if (companionIcon != null) companionIcon.SetActive(true);
        }
        StartCoroutine(ExitRoutine());
    }

    IEnumerator ExitRoutine()
    {
        yield return new WaitForSeconds(2.5f);
        SceneManager.LoadScene(nextScene);
    }

    void SetFace(Expression e)
    {
        if (npcExpression == null) return;
        switch (e)
        {
            case Expression.Neutral: npcExpression.SetNeutral(); break;
            case Expression.Happy: npcExpression.SetHappy(); break;
            case Expression.Sad: npcExpression.SetSad(); break;
            case Expression.Angry: npcExpression.SetAngry(); break;
        }
    }
}