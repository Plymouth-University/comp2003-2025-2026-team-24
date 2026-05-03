using UnityEngine;

public class ControllableUnit_MP : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 4f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip courseEndSFX;
    [SerializeField] private AudioClip walkSFX;

    protected SpriteRenderer sr;

    private Color normalColor = Color.white;
    private Color dimColor = new Color(.8f, .8f, .8f, 0.3f);
    private Color selectedColor = Color.green;

    [Header("Multiplayer")]
    public int ownerPlayerIndex = -1; // default = unassigned

    protected Tile currentTile;
    protected Tile targetTile;

    protected bool moving = false;
    private int defaultSortingOrder;
    private int selectedSortingOrder = 250;

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sr != null)
            defaultSortingOrder = sr.sortingOrder;

        if (UnitManager_MP.Instance != null)
        {
            UnitManager_MP.Instance.RegisterUnit(this);
            Debug.Log("REGISTERED: " + name);
        }

        // ONLY assign if not already set (THIS FIXES YOUR BUG)
        if (ownerPlayerIndex == -1 && GameManager_MP.Instance != null)
        {
            ownerPlayerIndex = GameManager_MP.Instance.currentPlayerIndex;
        }

        Debug.Log(name + " OWNER: " + ownerPlayerIndex);
    }

    protected virtual void Update()
    {
        if (moving)
            MoveToTarget();
    }

    public virtual void SetSelected(bool selected)
    {
        if (sr == null) return;

        if (selected)
        {
            sr.color = Color.yellow;
            sr.sortingOrder = selectedSortingOrder;
        }
        else
        {
            sr.color = Color.white;
            transform.localScale = Vector3.one;
            sr.sortingOrder = defaultSortingOrder;
        }
    }

    public void MoveUp() => TryMove(Vector2Int.up);
    public void MoveDown() => TryMove(Vector2Int.down);
    public void MoveLeft() => TryMove(Vector2Int.left);
    public void MoveRight() => TryMove(Vector2Int.right);

    public Tile GetCurrentTile()
    {
        return currentTile;
    }

    private void OnMouseDown()
    {
        if (UnitController_MP.Instance != null)
        {
            UnitController_MP.Instance.SelectUnit(this);
        }
    }

    public void UpdateVisual()
    {
        if (sr == null) return;

        int currentPlayer = GameManager_MP.Instance.currentPlayerIndex;

        if (ownerPlayerIndex == currentPlayer)
        {
            sr.color = normalColor;
        }
        else
        {
            sr.color = dimColor;
        }
    }

    protected virtual bool CanWalkOn(TileContentType type)
    {
        return type == TileContentType.Path ||
               type == TileContentType.Home ||
               type == TileContentType.CourseStart ||
               type == TileContentType.Course ||
               type == TileContentType.CourseEnd_Part1 ||
               type == TileContentType.CourseEnd_Part2;
    }

    private void TryMove(Vector2Int dir)
    {
        Debug.Log("TryMove called");

        if (ownerPlayerIndex != GameManager_MP.Instance.currentPlayerIndex)
        {
            Debug.Log("Wrong player turn");
            return;
        }

        if (!GameManager_MP.Instance.CanMoveUnit(this))
        {
            Debug.Log("Unit already moved this turn");
            return;
        }

        if (moving)
        {
            Debug.Log("Already moving");
            return;
        }

        if (currentTile == null)
        {
            Debug.Log("No current tile");
            return;
        }

        Tile next = GameManager_MP.Instance.GetTileAt(
            currentTile.GridPosition + dir);

        if (next == null)
        {
            Debug.Log("Next tile is null");
            return;
        }

        if (!CanWalkOn(next.GetContentType()))
        {
            Debug.Log("Cannot walk on tile: " + next.GetContentType());
            return;
        }

        if (!next.IsGuestFree())
        {
            Debug.Log("Tile occupied");
            return;
        }

        Debug.Log("MOVING SUCCESS");

        targetTile = next;
        moving = true;

        GameManager_MP.Instance.MarkUnitMoved(this);
    }

    private void PlayCourseEndSFX()
    {
        if (audioSource != null && courseEndSFX != null)
            audioSource.PlayOneShot(courseEndSFX);
    }

    private void PlayWalkSFX()
    {
        if (audioSource != null && walkSFX != null)
            audioSource.PlayOneShot(walkSFX);
    }

    private void MoveToTarget()
    {
        Vector3 targetPos =
            targetTile.transform.position +
            new Vector3(0.25f, 0.25f, 0f);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            moveSpeed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.02f)
        {
            currentTile = targetTile;
            targetTile = null;
            moving = false;
        }
    }
}