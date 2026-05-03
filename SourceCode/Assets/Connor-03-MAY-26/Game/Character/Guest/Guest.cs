using UnityEngine;

public class Guest : ControllableUnit
{
    private bool isOnCourse = false;
    private bool hasScored = false;
    private bool enteredFromStart = false;

    [SerializeField] private int courseCompletionScore = 5;

    protected override void Start()
    {
        base.Start();

        currentTile = GameManager.Instance.GetHomeTile();

        if (currentTile == null)
        {
            Destroy(gameObject);
            return;
        }

        currentTile.SetGuest(null);

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

        // ENTERING COURSE (ONLY via CourseStart)
        if (!isOnCourse && type == TileContentType.CourseStart)
        {
            isOnCourse = true;
            enteredFromStart = true;
            hasScored = false;
        }

        // FINISHING COURSE
        if (isOnCourse && type == TileContentType.CourseEnd_Part2)
        {
            // Only reward if they entered properly
            if (enteredFromStart && !hasScored)
            {
                GameManager.Instance.AddScore(courseCompletionScore);
                hasScored = true;
            }

            // Unlock movement
            isOnCourse = false;
            enteredFromStart = false;
        }
    }

    protected override bool CanWalkOn(TileContentType type)
    {
        bool isCourseTile = IsCourseTile(type);

        // Block all course tiles if no valid course exists
        if (isCourseTile && !GameManager.Instance.IsCourseFullyBuilt())
            return false;

        // If currently on course = ONLY allow course tiles
        if (isOnCourse)
            return isCourseTile;

        // Prevent entering mid-course (must use CourseStart)
        if (!isOnCourse && isCourseTile && type != TileContentType.CourseStart)
            return false;

        return base.CanWalkOn(type);
    }

    private bool IsCourseTile(TileContentType type)
    {
        return type == TileContentType.CourseStart ||
               type == TileContentType.Course ||
               type == TileContentType.CourseEnd_Part1 ||
               type == TileContentType.CourseEnd_Part2;
    }
}