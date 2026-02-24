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

    private Vector3 target;
    public bool IsMoving { get; private set; }

    void Awake()
    {
        if (cam == null) cam = Camera.main;
        target = transform.position;
    }

    // NEU: Beim Start der Szene prüfen, ob wir eine Position gespeichert haben
    void Start()
    {
        if (GameState.I != null && GameState.I.hasSavedPosition)
        {
            transform.position = GameState.I.lastPlayerPosition;
            target = transform.position; // Wichtig, damit er nicht sofort wieder wegläuft
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
                // Sag dem NPC, dass er angetippt wurde und überib dieses Skript (this)
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

        if (IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
            if (Vector3.Distance(transform.position, target) < 0.02f)
                IsMoving = false;
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
        target = new Vector3(worldPos.x, worldPos.y, transform.position.z);
        IsMoving = true;
    }

    public void Stop()
    {
        IsMoving = false;
        target = transform.position;
    }

    bool PointerPressedThisFrame()
    {
        // Reagiert auf Maus (Editor) ODER Touch (Handy)
        if (Input.GetMouseButtonDown(0)) return true;
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) return true;
        return false;
    }

    Vector2 GetPointerPosition()
    {
        // Wenn ein Finger da ist, nimm den, sonst die Maus
        if (Input.touchCount > 0) return Input.GetTouch(0).position;
        return Input.mousePosition;
    }
}