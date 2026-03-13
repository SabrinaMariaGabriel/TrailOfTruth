using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
public class GameManager : MonoBehaviour
{
    // Das "I" erlaubt es dir, von überall mit GameManager.I.AddEnergy(10) zuzugreifen
    public static GameManager I;

    [Header("Stats")]
    public int credibility = 0; // Startet bei 0, Robin füllt es auf
    public int energy = 0;
    public int feather = 0;

    [Header("Max Values")]
    public int credibilityMax = 100;
    public int energyMax = 100;
    public int featherMax = 100;

    [Header("UI References (Auto-Assigned)")]
    public StatBarUI credibilityBar;
    public StatBarUI energyBar;
    public StatBarUI featherBar;

    [Header("Popups")]
    public StatPopupSpawner popupSpawner;
    public Transform credibilityPopupAnchor;
    public Transform energyPopupAnchor;
    public Transform featherPopupAnchor;

    void Awake()
    {
        // Singleton-Logik: Macht den Manager unsterblich
        if (I == null)
        {
            I = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnEnable()
    {
        // Registriert den Manager für Szenenwechsel
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        // Initialer Refresh für die allererste Szene
        FindUIReferences();
        RefreshUI();
    }

    // Läuft jedes Mal, wenn eine neue Szene (z.B. Wald) geladen wird
    // Diese Methode wird von Unity gerufen, wenn die Szene lädt
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Wir starten eine Coroutine, um der UI Zeit zum Laden zu geben
        StartCoroutine(LateSyncRoutine());
    }

    IEnumerator LateSyncRoutine()
    {
        // Wir warten genau einen Frame. Das reicht meistens völlig aus.
        yield return null;

        // Jetzt suchen wir die Balken in der neuen Szene
        FindUIReferences();

        // Und jetzt befüllen wir sie mit den aktuellen Werten
        RefreshUI();

        Debug.Log("UI erfolgreich synchronisiert!");
    }

    void FindUIReferences()
    {
        // Sucht die Balken im HUD-Prefab anhand ihrer Namen
        GameObject eObj = GameObject.Find("EnergyBar");
        if (eObj != null) energyBar = eObj.GetComponent<StatBarUI>();

        GameObject cObj = GameObject.Find("CredibilityBar");
        if (cObj != null) credibilityBar = cObj.GetComponent<StatBarUI>();

        GameObject fObj = GameObject.Find("FeatherBar");
        if (fObj != null) featherBar = fObj.GetComponent<StatBarUI>();

        // Popups suchen
        GameObject spawnerObj = GameObject.Find("StatPopupSpawner");
        if (spawnerObj != null) popupSpawner = spawnerObj.GetComponent<StatPopupSpawner>();

        // Anchors in den Objekten finden
        if (energyBar != null) energyPopupAnchor = energyBar.transform.Find("PopupAnchor");
        if (credibilityBar != null) credibilityPopupAnchor = credibilityBar.transform.Find("PopupAnchor");
        if (featherBar != null) featherPopupAnchor = featherBar.transform.Find("PopupAnchor");

        // Max-Werte an die neuen UI-Skripte übertragen
        if (energyBar) energyBar.maxValue = energyMax;
        if (credibilityBar) credibilityBar.maxValue = credibilityMax;
        if (featherBar) featherBar.maxValue = featherMax;
    }

    public void AddCredibility(int amount)
    {
        credibility = Mathf.Clamp(credibility + amount, 0, credibilityMax);
        popupSpawner?.Show(credibilityPopupAnchor, amount);
        RefreshUI();
    }

    public void AddEnergy(int amount)
    {
        energy = Mathf.Clamp(energy + amount, 0, energyMax);
        popupSpawner?.Show(energyPopupAnchor, amount);
        RefreshUI();
    }

    public void AddFeather(int amount)
    {
        feather = Mathf.Clamp(feather + amount, 0, featherMax);
        popupSpawner?.Show(featherPopupAnchor, amount);
        RefreshUI();
    }

    public void RefreshUI()
    {
        if (credibilityBar != null) credibilityBar.SetValue(credibility);
        if (energyBar != null) energyBar.SetValue(energy);
        if (featherBar != null) featherBar.SetValue(feather);
    }
}