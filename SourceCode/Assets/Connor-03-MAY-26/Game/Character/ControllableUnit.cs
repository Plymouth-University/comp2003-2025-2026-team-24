using UnityEngine;

public class ControllableUnit : MonoBehaviour
{
    [SerializeField] protected float moveSpeed = 4f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip courseEndSFX;
    [SerializeField] private AudioClip WalkSFX;

    protected Tile currentTile;
    protected Tile targetTile;

    protected bool moving = false;

    protected SpriteRenderer sr;

    protected virtual void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (UnitManager.Instance != null)
            UnitManager.Instance.RegisterUnit(this);
    }

    protected virtual void Update()
    {
        if (moving)
            MoveToTarget();
    }

    public virtual void SetSelected(bool selected)
    {
        if (sr != null)
            sr.color = selected ? Color.green : Color.white;
    }

    public void MoveUp() => TryMove(Vector2Int.up);
    public void MoveDown() => TryMove(Vector2Int.down);
    public void MoveLeft() => TryMove(Vector2Int.left);
    public void MoveRight() => TryMove(Vector2Int.right);

    public Tile GetCurrentTile()
    {
        return currentTile;
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
        if (moving)
            return;

        if (currentTile == null)
            return;

        Tile next = GameManager.Instance.GetTileAt(
            currentTile.GridPosition + dir);

        if (next == null)
            return;

        if (!CanWalkOn(next.GetContentType()))
            return;

        if (!next.IsGuestFree())
            return;

        if (currentTile.GetContentType() == TileContentType.CourseEnd_Part2)
        {
            PlayCourseEndSFX();
        }
        else
        {
            PlayWalkSFX();
        }

        targetTile = next;
        moving = true;
    }

    private void PlayCourseEndSFX()
    {
        if (audioSource != null && courseEndSFX != null)
        {
            audioSource.PlayOneShot(courseEndSFX);
        }
    }

    private void PlayWalkSFX()
    {
        if (audioSource != null && WalkSFX != null)
        {
            audioSource.PlayOneShot(WalkSFX);
        }
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