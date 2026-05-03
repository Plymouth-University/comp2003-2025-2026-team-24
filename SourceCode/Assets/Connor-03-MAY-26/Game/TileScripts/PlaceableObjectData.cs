using UnityEngine;

[System.Flags]
public enum Direction
{
    None = 0,
    Up = 1,
    Down = 2,
    Left = 4,
    Right = 8
}

public enum TileContentType
{
    None,
    Path,
    Course,
    CourseStart,
    CourseEnd_Part1,
    CourseEnd_Part2,
    Home
}

public class PlaceableObjectData : MonoBehaviour
{
    [Header("Tile Type")]
    public TileContentType type;

    [Header("Connections (UNROTATED)")]
    [Tooltip("Define which directions this tile connects BEFORE rotation")]
    public Direction connections;

    // =========================
    // CONNECTION LOGIC
    // =========================

    /// <summary>
    /// Gets connections after rotation (Z axis)
    /// </summary>
    public Direction GetRotatedConnections(float rotation)
    {
        int steps = Mathf.RoundToInt(rotation / 90f);

        // Normalize to 0–3 (handles negatives + overflow)
        steps = ((steps % 4) + 4) % 4;

        Direction result = connections;

        for (int i = 0; i < steps; i++)
        {
            result = Rotate90(result);
        }

        return result;
    }

    /// <summary>
    /// Check if this tile connects in a direction after rotation
    /// </summary>
    public bool HasConnection(Direction dir, float rotation)
    {
        return GetRotatedConnections(rotation).HasFlag(dir);
    }

    private Direction Rotate90(Direction dir)
    {
        Direction result = Direction.None;

        if (dir.HasFlag(Direction.Up)) result |= Direction.Right;
        if (dir.HasFlag(Direction.Right)) result |= Direction.Down;
        if (dir.HasFlag(Direction.Down)) result |= Direction.Left;
        if (dir.HasFlag(Direction.Left)) result |= Direction.Up;

        return result;
    }

    // =========================
    // STATIC HELPERS
    // =========================

    public static Direction GetOpposite(Direction dir)
    {
        switch (dir)
        {
            case Direction.Up: return Direction.Down;
            case Direction.Down: return Direction.Up;
            case Direction.Left: return Direction.Right;
            case Direction.Right: return Direction.Left;
            default: return Direction.None;
        }
    }

    public static Direction VectorToDirection(Vector2Int dir)
    {
        if (dir == Vector2Int.up) return Direction.Up;
        if (dir == Vector2Int.down) return Direction.Down;
        if (dir == Vector2Int.left) return Direction.Left;
        if (dir == Vector2Int.right) return Direction.Right;

        return Direction.None;
    }
}