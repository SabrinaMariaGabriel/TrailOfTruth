using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    public Transform target;
    public float followDistance = 1.2f; // Etwas mehr Platz lassen
    public float stopDistance = 0.8f;   // Ab hier wird er langsamer/stoppt er
    public float speed = 2.5f;          // Ein klein wenig langsamer als der Player

    private Animator anim;
    private SpriteRenderer sr;

    void Awake()
    {
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        if (GameState.I != null && GameState.I.HasCompanion("martin"))
        {
            // Martin direkt hinter den Spieler setzen beim Start
            transform.position = target.position - new Vector3(0.5f, 0, 0);
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (target == null) return;

        float distance = Vector2.Distance(transform.position, target.position);

        // Nur bewegen, wenn der Player weit genug weg ist
        if (distance > followDistance)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            transform.position += direction * speed * Time.deltaTime;

            if (anim != null) anim.SetFloat("Speed", 1f);

            // Spiegeln
            if (target.position.x > transform.position.x)
                transform.localScale = new Vector3(1, 1, 1);
            else
                transform.localScale = new Vector3(-1, 1, 1);
        }
        else if (distance < stopDistance)
        {
            // Er ist nah genug dran
            if (anim != null) anim.SetFloat("Speed", 0f);
        }
    }
}