using UnityEngine;

[CreateAssetMenu(fileName = "NeuerDialog", menuName = "Dialog/Daten-Paket")]
public class DialogueData : ScriptableObject
{
    public string npcName;
    public string companionID = "martin"; // Für GameState.I.AddCompanion

    [Header("Die Dialog-Schritte")]
    public DialogueStep[] steps;
}

[System.Serializable]
public class DialogueStep
{
    [TextArea(3, 10)]
    public string text;
    public Expression face;
    public bool isQuestion;

    [Header("Nur bei Fragen ausfüllen (immer 4 Werte)")]
    public string[] answers = new string[4];
    public string[] reactions = new string[4];

    // HIER lagen die Fehler - diese Listen müssen existieren:
    public int[] trustGains = new int[4];
    public int[] credibilityChanges = new int[4];
    public int[] energyChanges = new int[4];
    public int[] featherChanges = new int[4];
}

public enum Expression { Neutral, Happy, Sad, Angry }