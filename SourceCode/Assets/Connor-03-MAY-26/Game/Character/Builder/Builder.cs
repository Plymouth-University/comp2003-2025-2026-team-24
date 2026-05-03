using UnityEngine;

public class Builder : ControllableUnit
{
    private bool isOnCourse = false;
    private bool hasScored = false;
    private bool enteredFromStart = false;

    [SerializeField] private int courseCompletionScore = 5;

    protected override void Start()
    {
        base.Start();

        // If a tile was already assigned (from spawner), use it
        if (currentTile != null)
        {
            transform.position =
                currentTile.transform.position +
                new Vector3(0.25f, 0.25f, 0f);

            return;
        }

        // Fallback: only use Home if nothing was assigned
        currentTile = GameManager.Instance.GetHomeTile();

        if (currentTile == null)
        {
            Destroy(gameObject);
            return;
        }

        currentTile.SetGuest(this);

        transform.position =
            currentTile.transform.position +
            new Vector3(0.25f, 0.25f, 0f);
    }

    // allows spawner to place builder directly
    public void SetStartingTile(Tile tile)
    {
        if (tile == null)
            return;

        currentTile = tile;

        currentTile.SetGuest(this);

        transform.position =
            currentTile.transform.position +
            new Vector3(0.25f, 0.25f, 0f);
    }

    protected override void Update()
    {
        base.Update();

        if (currentTile == null)
            return;

        TileContentType type = currentTile.GetContentType();

        // ENTER COURSE (ONLY via CourseStart)
        if (!isOnCourse && type == TileContentType.CourseStart)
        {
            isOnCourse = true;
            enteredFromStart = true;
            hasScored = false;
        }

        // FINISH COURSE
        if (isOnCourse && type == TileContentType.CourseEnd_Part2)
        {
            if (enteredFromStart && !hasScored)
            {
                GameManager.Instance.AddScore(courseCompletionScore);
                hasScored = true;
            }

            isOnCourse = false;
            enteredFromStart = false;
        }
    }

    protected override bool CanWalkOn(TileContentType type)
    {
        bool isCourseTile = IsCourseTile(type);

        // If on course = only allow course tiles
        if (isOnCourse)
            return isCourseTile;

        // Prevent entering mid-course
        if (!isCourseTile)
            return base.CanWalkOn(type);

        return type == TileContentType.CourseStart;
    }

    private bool IsCourseTile(TileContentType type)
    {
        return type == TileContentType.CourseStart ||
               type == TileContentType.Course ||
               type == TileContentType.CourseEnd_Part1 ||
               type == TileContentType.CourseEnd_Part2;
    }
}