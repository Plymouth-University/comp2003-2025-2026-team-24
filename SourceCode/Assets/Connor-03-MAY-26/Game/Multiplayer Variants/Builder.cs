using UnityEngine;

public class Builder_MP : ControllableUnit_MP
{
    private bool isOnCourse = false;
    private bool hasScored = false;
    private bool enteredFromStart = false;

    [SerializeField] private int courseCompletionScore = 5;

    protected override void Start()
    {
        base.Start();

        // REMOVE home tile override
        // DO NOT set position here anymore

        if (currentTile != null)
        {
            transform.position =
                currentTile.transform.position +
                new Vector3(0.25f, 0.25f, 0f);
        }
    }

    // NEW: Allow spawner to set tile properly
    public void SetStartingTile(Tile tile)
    {
        currentTile = tile;

        if (currentTile != null)
        {
            currentTile.SetGuest(this);

            transform.position =
                currentTile.transform.position +
                new Vector3(0.25f, 0.25f, 0f);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (currentTile == null)
            return;

        TileContentType type = currentTile.GetContentType();

        if (!isOnCourse && type == TileContentType.CourseStart)
        {
            isOnCourse = true;
            enteredFromStart = true;
            hasScored = false;
        }

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
    }

    protected override bool CanWalkOn(TileContentType type)
    {
        bool isCourseTile = IsCourseTile(type);

        if (isOnCourse)
            return isCourseTile;

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