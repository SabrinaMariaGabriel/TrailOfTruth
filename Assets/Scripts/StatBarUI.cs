using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBarUI : MonoBehaviour
{
    [Header("UI References")]
    public Image fillImage;     // Image Type = Filled
    public TMP_Text valueText;  // z.B. 45/100

    [Header("Config")]
    public int minValue = 0;
    public int maxValue = 100;
    public bool showMax = true;

    [Header("Animation")]
    [Tooltip("Wie schnell die Bar zum Zielwert gleitet. Höher = schneller.")]
    public float fillSpeed = 8f;

    [Tooltip("Wie schnell die Zahl zum Zielwert gleitet. Höher = schneller.")]
    public float numberSpeed = 12f;

    float targetFill = 0f;
    float currentFill = 0f;

    int targetValue = 0;
    float currentValueFloat = 0f;

    void Awake()
    {
        // Startwerte aus aktuellem UI übernehmen (falls gesetzt)
        currentFill = fillImage != null ? fillImage.fillAmount : 0f;
        targetFill = currentFill;

        currentValueFloat = minValue;
        targetValue = minValue;
        UpdateText((int)currentValueFloat);
    }

    public void SetValue(int value)
    {
        value = Mathf.Clamp(value, minValue, maxValue);

        targetValue = value;
        targetFill = (float)(value - minValue) / (maxValue - minValue);
    }

    void Update()
    {
        // Fill smooth
        currentFill = Mathf.Lerp(currentFill, targetFill, Time.deltaTime * fillSpeed);
        if (fillImage != null)
            fillImage.fillAmount = currentFill;

        // Number smooth
        currentValueFloat = Mathf.Lerp(currentValueFloat, targetValue, Time.deltaTime * numberSpeed);
        UpdateText(Mathf.RoundToInt(currentValueFloat));
    }

    void UpdateText(int value)
    {
        if (valueText == null) return;

        value = Mathf.Clamp(value, minValue, maxValue);
        valueText.text = showMax ? $"{value}/{maxValue}" : value.ToString();
    }
}
