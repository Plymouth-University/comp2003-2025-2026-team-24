using UnityEngine;

public class Tile : MonoBehaviour
{
    private Vector2Int gridPosition;
    private bool isPlaced = false;

    public void SetGridPosition(Vector2Int position)
    {
        gridPosition = position;
        isPlaced = true;
    }

    public Vector2Int GetGridPosition()
    {
        return gridPosition;
    }

    public bool IsPlaced()
    {
        return isPlaced;
    }

    public void ResetTile()
    {
        isPlaced = false;
    }

    // Optional: Drag and drop support
    void OnMouseDrag()
    {
        if (!isPlaced && Camera.main != null)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0;
            transform.position = mousePosition;
        }
    }
}