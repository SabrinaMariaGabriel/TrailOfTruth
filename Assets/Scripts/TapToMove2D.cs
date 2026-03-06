using UnityEngine;
using UnityEngine.EventSystems;

public class TapToMove2D : MonoBehaviour
{
    public Camera cam;
    public LayerMask walkableMask; // WalkArea
    public LayerMask npcMask;      // NPC Layer
    public float speed = 3f;
    private Animator anim;
    public float characterScale = 0.5f; // Hier trägst du deine Wunschgröße ein (z.B. 0.5)

    [Header("UI")]
    public bool blockWhenPointerOverUI = true;

    private Vector2 target; // Auf Vector2 geändert (für 2D-Physik sauberer)
    public bool IsMoving { get; private set; }

    private Rigidbody2D rb; // NEU: Unser Physik-Motor

    void Awake()
    {
        if (cam == null) cam = Camera.main;

        // NEU: Wir schnappen uns den Rigidbody von unserem Spieler
        rb = GetComponent<Rigidbody2D>();
        target = transform.position;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        if (GameState.I != null && GameState.I.hasSavedPosition)
        {
            transform.position = GameState.I.lastPlayerPosition;
            target = transform.position;
        }
    }

    void Update()
    {
        if (PointerPressedThisFrame())
        {
            if (blockWhenPointerOverUI && IsPointerOverUI())
                return;

            Vector2 world = cam.ScreenToWorldPoint(GetPointerPosition());

            // 1) NPC?
            Collider2D npcHit = Physics2D.OverlapPoint(world, npcMask);
            if (npcHit != null)
            {
                NpcApproachAndTalk npc = npcHit.GetComponent<NpcApproachAndTalk>();
                if (npc != null)
                {
                    npc.OnTapped(this);
                }
                return;
            }

            // 2) Walkable?
            Collider2D walkHit = Physics2D.OverlapPoint(world, walkableMask);
            if (walkHit != null)
            {
                SetTarget(world);
            }
        }

        // NEU: Wir prüfen hier nur noch, ob er angekommen ist, um den Motor abzustellen
        if (IsMoving)
        {
            if (Vector2.Distance(transform.position, target) < 0.02f)
            {
                IsMoving = false;
            }
            else
            {
                // --- NEU: SPIEGELN (FLIP) ---
                if (target.x > transform.position.x)
                {
                    transform.localScale = new Vector3(characterScale, characterScale, 1); // Nach Rechts
                }
                else if (target.x < transform.position.x)
                {
                    transform.localScale = new Vector3(-characterScale, characterScale, 1); // Nach Links
                }
            }
        }

        // AM ENDE DER UPDATE: Animator aktualisieren
        if (anim != null)
        {
            // Wenn IsMoving true ist, setzen wir "Speed" auf 1, sonst auf 0
            anim.SetFloat("Speed", IsMoving ? 1f : 0f);
        }
    }

    /*
    // NEU: Physikalische Bewegungen gehören IMMER in die FixedUpdate!
    void FixedUpdate()
    {
        if (IsMoving)
        {
            // Wir schieben den Körper jetzt physikalisch, statt ihn zu teleportieren!
            // Time.fixedDeltaTime ist wichtig für flüssige Physik.
            Vector2 newPos = Vector2.MoveTowards(rb.position, target, speed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
        }
    }
    */
    void FixedUpdate()
    {
        if (IsMoving)
        {
            Vector2 currentPos = rb.position;
            Vector2 direction = (target - currentPos).normalized;
            float moveDistance = speed * Time.fixedDeltaTime;

            // --- DER REPARIERTE PHYSIK-CHECK ---
            ContactFilter2D filter = new ContactFilter2D();
            filter.useLayerMask = true;
            // Wir sagen dem Filter: Achte NUR auf Walkable und NPCs (oder alles außer Player)
            // WICHTIG: Ersetze 'walkableMask' durch die Masken, die blockieren sollen
            filter.layerMask = npcMask;
            filter.useTriggers = false; // Trigger ignorieren (wie Durchgänge)

            RaycastHit2D[] hits = new RaycastHit2D[1];

            // rb.Cast sucht jetzt nur nach Dingen im npcMask Layer
            int hitCount = rb.Cast(direction, filter, hits, moveDistance + 0.05f);

            if (hitCount > 0)
            {
                // Wir sind gegen einen NPC oder ein Hindernis gelaufen
                Stop();
                Debug.Log("Blockiert durch: " + hits[0].collider.name);
            }
            else
            {
                // Weg ist frei
                Vector2 newPos = Vector2.MoveTowards(currentPos, target, moveDistance);
                rb.MovePosition(newPos);
            }
        }
    }


    bool IsPointerOverUI()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (Input.touchCount > 0)
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        return false;
#else
        return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
#endif
    }

    public void SetTarget(Vector2 worldPos)
    {
        target = worldPos;
        IsMoving = true;
    }

    public void Stop()
    {
        IsMoving = false;
        target = transform.position;
    }

    bool PointerPressedThisFrame()
    {
        if (Input.GetMouseButtonDown(0)) return true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) return true;
        return false;
    }

    Vector2 GetPointerPosition()
    {
        if (Input.touchCount > 0) return Input.GetTouch(0).position;
        return Input.mousePosition;
    }
}