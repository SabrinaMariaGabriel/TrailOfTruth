using System.Collections;
using TMPro;
using UnityEngine;

public class StatPopup : MonoBehaviour
{
    public TMP_Text text;
    public float lifeTime = 0.6f;
    public float riseDistance = 60f;

    [Header("Effects")]
    public bool shakeOnNegative = true;
    public float shakeStrength = 8f;
    public float shakeDuration = 0.2f;

    CanvasGroup cg;
    RectTransform rt;

    AudioSource audioSource;

    void Awake()
    {
        rt = GetComponent<RectTransform>();
        cg = gameObject.AddComponent<CanvasGroup>();
        audioSource = gameObject.AddComponent<AudioSource>();

        if (text == null)
            text = GetComponent<TMP_Text>();
    }

    public void Play(string msg, int amount, AudioClip positiveSfx, AudioClip negativeSfx)
    {
        text.text = msg;
        text.color = amount >= 0 ? Color.green : Color.red;

        // SICHERHEITS-CHECK FÜR SOUND
        if (audioSource != null)
        {
            if (amount >= 0 && positiveSfx != null)
                audioSource.PlayOneShot(positiveSfx);
            else if (amount < 0 && negativeSfx != null)
                audioSource.PlayOneShot(negativeSfx);
        }

        StartCoroutine(Anim(amount));
    }

    IEnumerator Anim(int amount)
    {
        Vector2 start = rt.anchoredPosition;
        Vector2 end = start + Vector2.up * riseDistance;

        float t = 0f;
        cg.alpha = 1f;

        while (t < 1f)
        {
            t += Time.deltaTime / lifeTime;

            // Float nach oben
            rt.anchoredPosition = Vector2.Lerp(start, end, t);
            cg.alpha = 1f - t;

            // Shake wenn negativ
            if (amount < 0 && shakeOnNegative)
            {
                float shake = Mathf.Sin(Time.time * 80f) * shakeStrength;
                rt.anchoredPosition += Vector2.right * shake;
            }

            yield return null;
        }

        Destroy(gameObject);
    }
}
