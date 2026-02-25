using UnityEngine;
using UnityEngine.EventSystems;

public class TapToMove2D : MonoBehaviour
{
    public Camera cam;
    public LayerMask walkableMask; // WalkArea
    public LayerMask npcMask;      // NPC Layer
    public float speed = 3f;

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
        }
    }

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