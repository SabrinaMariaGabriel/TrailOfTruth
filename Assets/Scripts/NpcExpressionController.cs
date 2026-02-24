using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class NpcExpressionController : MonoBehaviour
{
    public Image portraitImage;

    [Header("Expressions")]
    public Sprite MartinNeutral;
    public Sprite MartinHappy;
    public Sprite MartinSad;
    public Sprite MartinAngry;

    [Header("Expression FX")]
    public bool playFxOnChange = true;
    public float shakeDuration = 0.15f;
    public float shakeStrength = 6f;   // Pixel-ish (UI)
    public float bounceScale = 1.03f;  // subtil

    Coroutine fxRoutine;

    void Awake()
    {
        if (portraitImage == null)
            portraitImage = GetComponent<Image>();
    }

    void Start()
    {
        // Immer neutral am Anfang
        Debug.Log("Setting neutral sprite now");
        SetNeutral();
    }

    public void SetNeutral() => SetSprite(MartinNeutral);
    public void SetHappy() => SetSprite(MartinHappy);
    public void SetSad() => SetSprite(MartinSad);
    public void SetAngry() => SetSprite(MartinAngry);

    void SetSprite(Sprite s)
    {
        if (portraitImage == null || s == null) return;

        portraitImage.sprite = s;

        if (playFxOnChange)
        {
            if (fxRoutine != null) StopCoroutine(fxRoutine);
            fxRoutine = StartCoroutine(ShakeAndBounce());
        }
    }

    IEnumerator ShakeAndBounce()
    {
        RectTransform rt = (RectTransform)transform;
        Vector2 startPos = rt.anchoredPosition;
        Vector3 startScale = rt.localScale;
        Vector3 upScale = startScale * bounceScale;

        float t = 0f;
        while (t < shakeDuration)
        {
            t += Time.deltaTime;

            float x = Random.Range(-shakeStrength, shakeStrength);
            float y = Random.Range(-shakeStrength, shakeStrength) * 0.4f;

            rt.anchoredPosition = startPos + new Vector2(x, y);
            rt.localScale = Vector3.Lerp(startScale, upScale, t / shakeDuration);

            yield return null;
        }

        // zurücksetzen
        rt.anchoredPosition = startPos;
        rt.localScale = startScale;
    }
}
