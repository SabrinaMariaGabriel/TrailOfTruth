using UnityEngine;

public class StatPopupSpawner : MonoBehaviour
{
    public StatPopup popupPrefab;

    [Header("Audio")]
    public AudioClip positiveSfx;
    public AudioClip negativeSfx;

    public void Show(Transform anchor, int amount)
    {
        if (popupPrefab == null || anchor == null) return;

        var popup = Instantiate(popupPrefab, anchor);
        popup.transform.localPosition = Vector3.zero;

        string sign = amount >= 0 ? "+" : "";
        popup.Play($"{sign}{amount}", amount, positiveSfx, negativeSfx);
    }
}
