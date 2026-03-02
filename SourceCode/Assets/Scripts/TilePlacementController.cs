using UnityEngine;

public class TilePlacementController : MonoBehaviour
{
    public TileGridManager gridManager;
    private Tile selectedTile;

    void Update()
    {
        if (selectedTile != null)
        {
            HandlePreview();
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryPlace();
        }

        if (Input.GetMouseButtonDown(1))
        {
            TryRemove();
        }
    }

    void HandlePreview()
    {
        if (Camera.main == null)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2Int gridPos = gridManager.WorldToGrid(mouseWorldPos);

        if (gridManager.IsValidPosition(gridPos))
        {
            selectedTile.transform.position = new Vector3(
                gridPos.x * gridManager.tileSize + gridManager.tileSize / 2f,
                gridPos.y * gridManager.tileSize + gridManager.tileSize / 2f,
                0
            );
        }
    }

    void TryPlace()
    {
        if (selectedTile == null || Camera.main == null)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        bool placed = gridManager.TryPlaceTile(selectedTile, mouseWorldPos);

        if (placed)
        {
            selectedTile = null;
        }
    }

    void TryRemove()
    {
        if (Camera.main == null)
            return;

        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        Vector2Int gridPos = gridManager.WorldToGrid(mouseWorldPos);

        if (!gridManager.IsValidPosition(gridPos))
            return;

        gridManager.RemoveTile(gridPos);
    }

    public void SelectTile(Tile tile)
    {
        selectedTile = tile;
    }
}