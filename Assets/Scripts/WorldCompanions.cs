using UnityEngine;

public class WorldCompanions : MonoBehaviour
{
    [Header("World Objects")]
    public GameObject martinPixelCompanion; // dein Pixel-Martin der mitläuft (oder erstmal nur sichtbar)

    [Header("UI")]
    public GameObject martinUiIcon; // optional: Icon oben rechts

    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        bool hasMartin = (GameState.I != null) && GameState.I.HasCompanion("martin");

        if (martinPixelCompanion != null) martinPixelCompanion.SetActive(hasMartin);
        if (martinUiIcon != null) martinUiIcon.SetActive(hasMartin);

        Debug.Log("WorldCompanions Refresh -> hasMartin=" + hasMartin);
    }
}