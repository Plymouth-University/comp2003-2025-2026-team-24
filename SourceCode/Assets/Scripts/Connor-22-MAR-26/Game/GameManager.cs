using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int width = 20;
    public int height = 20;

    public GameObject agentPrefab;

    private Tile[,] grid;

    public void Init()
    {
        grid = new Tile[width, height];
    }

    public void PlaceTile(int x, int y, TileData data)
    {
        if (!InBounds(x, y)) return;

        if (grid[x, y] != null)
            Destroy(grid[x, y].gameObject);

        GameObject obj = Instantiate(data.prefab, new Vector3(x, y, 0), Quaternion.identity);

        Tile tile = obj.GetComponent<Tile>();
        tile.Init(data);

        grid[x, y] = tile;
    }

    public Tile GetTile(int x, int y)
    {
        if (!InBounds(x, y)) return null;
        return grid[x, y];
    }

    bool InBounds(int x, int y)
    {
        return x >= 0 && y >= 0 && x < width && y < height;
    }
}
