using UnityEngine;
using System.Collections.Generic;

public class FollowPlayer : MonoBehaviour
{
    [Header("Identität")]
    public string characterId = "martin";

    [Header("Einstellungen")]
    public Transform target;
    public float followDistance = 1.2f;
    public float stopDistance = 0.8f;
    public float speed = 2.5f;
    public float characterScale = 0.5f;

    private Animator anim;
    private bool isActive = false;
    private bool wasWalking = false; // Für saubere Logs

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        UpdateChainTarget();

        if (isActive && target != null)
        {
            transform.position = target.position - new Vector3(0.5f, 0, 0);
        }
    }

    public void UpdateChainTarget()
    {
        if (GameState.I == null) return;

        List<string> party = GameState.I.GetCurrentParty();
        int myIndex = party.IndexOf(characterId);

        // Wenn nicht in der Party: AUSSCHALTEN
        if (myIndex == -1)
        {
            isActive = false;
            gameObject.SetActive(false);
            return;
        }

        // Wenn in der Party: EINSCHALTEN
        isActive = true;
        gameObject.SetActive(true);

        if (myIndex == 0)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) target = player.transform;
        }
        else
        {
            string leaderId = party[myIndex - 1];
            GameObject leaderObj = GameObject.Find(leaderId);
            if (leaderObj != null) target = leaderObj.transform;
        }
    }

    void Update()
    {
        if (target == null || !isActive) return;

        float distance = Vector2.Distance(transform.position, target.position);

        if (distance > followDistance)
        {
            float multiplier = Mathf.Clamp01((distance - followDistance) / 1.0f);
            float finalMultiplier = Mathf.Max(0.2f, multiplier);

            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * (speed * finalMultiplier) * Time.deltaTime;
            transform.position = new Vector3(transform.position.x, transform.position.y, 0);

            if (anim != null) anim.SetFloat("Speed", finalMultiplier);

            // LOG NUR BEI ÄNDERUNG
            if (!wasWalking)
            {
                Debug.Log($"<color=green>{characterId} startet Bewegung.</color> Ziel: {target.name}");
                wasWalking = true;
            }

            float flip = (target.position.x > transform.position.x) ? 1f : -1f;
            transform.localScale = new Vector3(flip * characterScale, characterScale, 1f);
        }
        else
        {
            if (anim != null) anim.SetFloat("Speed", 0f);

            // LOG NUR BEI ÄNDERUNG
            if (wasWalking && distance < followDistance + 0.1f)
            {
                Debug.Log($"<color=red>{characterId} gestoppt.</color> Distanz: {distance:F2}");
                wasWalking = false;
            }
        }
    }
}