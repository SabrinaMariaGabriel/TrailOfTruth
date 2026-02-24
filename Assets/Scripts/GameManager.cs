using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Stats")]
    public int credibility = 50;
    public int energy = 80;
    public int feather = 30;

    [Header("Max Values")]
    public int credibilityMax = 100;
    public int energyMax = 100;
    public int featherMax = 100;

    [Header("UI References")]
    public StatBarUI credibilityBar;
    public StatBarUI energyBar;
    public StatBarUI featherBar;

    [Header("Popups")]
    public StatPopupSpawner popupSpawner;
    public Transform credibilityPopupAnchor;
    public Transform energyPopupAnchor;
    public Transform featherPopupAnchor;


    void Start()
    {
        credibilityBar.maxValue = credibilityMax;
        energyBar.maxValue = energyMax;
        featherBar.maxValue = featherMax;

        RefreshUI();
    }

    public void AddCredibility(int amount)
    {
        credibility += amount;
        credibility = Mathf.Clamp(credibility, 0, credibilityMax);

        popupSpawner?.Show(credibilityPopupAnchor, amount);
        RefreshUI();
    }

    public void AddEnergy(int amount)
    {
        energy += amount;
        energy = Mathf.Clamp(energy, 0, energyMax);

        popupSpawner?.Show(energyPopupAnchor, amount);
        RefreshUI();
    }

    public void AddFeather(int amount)
    {
        feather += amount;
        feather = Mathf.Clamp(feather, 0, featherMax);

        popupSpawner?.Show(featherPopupAnchor, amount);
        RefreshUI();
    }

    void RefreshUI()
    {
        credibilityBar.SetValue(credibility);
        energyBar.SetValue(energy);
        featherBar.SetValue(feather);
    }
}
