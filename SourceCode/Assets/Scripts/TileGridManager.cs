using UnityEngine;

public class TileGridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float tileSize = 1f;

    private Tile[,] grid;

    void Start()
    {
        grid = new Tile[gridWidth, gridHeight];
    }

    // Convert world position to grid position
    public Vector2Int WorldToGrid(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x / tileSize);
        int y = Mathf.FloorToInt(worldPos.y / tileSize);

        return new Vector2Int(x, y);
    }

    // Try placing a tile at position
    public bool TryPlaceTile(Tile tile, Vector3 worldPosition)
    {
        Vector2Int gridPos = WorldToGrid(worldPosition);

        if (!IsValidPosition(gridPos))
            return false;

        if (grid[gridPos.x, gridPos.y] != null)
            return false; // Tile already exists

        grid[gridPos.x, gridPos.y] = tile;

        // Snap to CENTER of grid cell
        tile.transform.position = new Vector3(
            gridPos.x * tileSize + tileSize / 2f,
            gridPos.y * tileSize + tileSize / 2f,
            0
        );

        tile.SetGridPosition(gridPos);

        return true;
    }

    // Remove tile from grid
    public void RemoveTile(Vector2Int pos)
    {
        if (!IsValidPosition(pos))
            return;

        if (grid[pos.x, pos.y] != null)
        {
            grid[pos.x, pos.y] = null;
        }
    }

    public bool IsValidPosition(Vector2Int pos)
    {
        return pos.x >= 0 && pos.x < gridWidth &&
               pos.y >= 0 && pos.y < gridHeight;
    }

    // Draw grid in Scene view for debugging
    void OnDrawGizmos()
    {
        Gizmos.color = Color.gray;

        for (int x = 0; x <= gridWidth; x++)
        {
            Gizmos.DrawLine(
                new Vector3(x * tileSize, 0, 0),
                new Vector3(x * tileSize, gridHeight * tileSize, 0)
            );
        }

        for (int y = 0; y <= gridHeight; y++)
        {
            Gizmos.DrawLine(
                new Vector3(0, y * tileSize, 0),
                new Vector3(gridWidth * tileSize, y * tileSize, 0)
            );
        }
    }
}