using UnityEngine;

public class Guest_MP : ControllableUnit_MP
{
    private bool isOnCourse = false;
    private bool hasScored = false;
    private bool enteredFromStart = false;

    [SerializeField] private int courseCompletionScore = 5;

    protected override void Start()
    {
        base.Start();
        currentTile = GameManager_MP.Instance.GetHomeTile();

        if (currentTile == null)
        {
            Destroy(gameObject);
            return;
        }

        transform.position = currentTile.transform.position;

        // DO NOT clear tile
        currentTile.SetGuest(this);
    }

    protected override void Update()
    {
        base.Update();

        if (currentTile == null)
            return;

        TileContentType type = currentTile.GetContentType();

        // =========================
        // ENTER COURSE (ONLY FROM START)
        // =========================
        if (!isOnCourse && type == TileContentType.CourseStart)
        {
            isOnCourse = true;
            enteredFromStart = true;
            hasScored = false;
        }

        // =========================
        // FINISH COURSE
        // =========================
        if (isOnCourse && type == TileContentType.CourseEnd_Part2)
        {
            if (enteredFromStart && !hasScored)
            {
                GameManager_MP.Instance.AddScore(
                    courseCompletionScore,
                    ownerPlayerIndex
                );

                hasScored = true;
            }

            isOnCourse = false;
            enteredFromStart = false;
        }

        // =========================
        // LEFT COURSE EARLY (RESET)
        // =========================
        if (isOnCourse && !IsCourseTile(type))
        {
            isOnCourse = false;
            enteredFromStart = false;
            hasScored = false;
        }
    }

    protected override bool CanWalkOn(TileContentType type)
    {
        bool isCourseTile = IsCourseTile(type);

        //  Block course if not fully built
        if (isCourseTile && !GameManager_MP.Instance.IsCourseFullyBuilt())
            return false;

        //  If already on course  stay on course only
        if (isOnCourse)
            return isCourseTile;

        //  Prevent entering mid-course
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