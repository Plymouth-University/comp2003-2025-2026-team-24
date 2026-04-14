using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color _baseColor, _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    private bool _isOccupied = false;
    private GameObject _spawnedObject;
    private TileContentType _contentType = TileContentType.None;

    private HashSet<int> _courseIDs = new HashSet<int>();

    public Vector2Int GridPosition { get; private set; }

    public void SetPosition(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    void OnMouseEnter()
    {
        if (_isOccupied) return;

        GameObject prefab = BuildManager.Instance.GetSelectedPrefab();
        if (prefab == null) return;

        bool valid = CanPlace(prefab);

        var sr = _highlight.GetComponent<SpriteRenderer>();
        sr.color = valid ? Color.green : Color.red;

        BuildManager.Instance.SetGhostValid(valid);

        _highlight.SetActive(true);
    }

    void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    void OnMouseDown()
    {
        if (_isOccupied) return;

        GameObject prefab = BuildManager.Instance.GetSelectedPrefab();
        if (prefab == null) return;

        if (!CanPlace(prefab)) return;

        PlaceObject(prefab);
    }

    private bool CanPlace(GameObject prefab)
    {
        var data = prefab.GetComponent<PlaceableObjectData>();
        if (data == null) return false;

        var type = data.type;

        if (!BuildManager.Instance.IsHomePlaced())
        {
            return type == TileContentType.Home;
        }

        if (type == TileContentType.Path)
        {
            return HasConnectionToPath() || HasAdjacent(TileContentType.CourseEnd_Part2);
        }

        if (type == TileContentType.CourseStart)
        {
            return HasConnectionToPath();
        }

        if (type == TileContentType.Course)
        {
            return HasAdjacent(TileContentType.Course) ||
                   HasAdjacent(TileContentType.CourseStart);
        }

        if (type == TileContentType.CourseEnd_Part1)
        {
            return HasAdjacent(TileContentType.Course);
        }

        if (type == TileContentType.CourseEnd_Part2)
        {
            return HasAdjacent(TileContentType.CourseEnd_Part1);
        }

        return false;
    }

    private bool HasConnectionToPath()
    {
        return HasAdjacent(TileContentType.Path) ||
               HasAdjacent(TileContentType.Home);
    }

    private bool HasAdjacent(TileContentType type)
    {
        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Tile neighbour = GameManager.Instance.GetTileAt(GridPosition + dir);

            if (neighbour == null) continue;

            if (neighbour._contentType == type)
                return true;

            if (type == TileContentType.Path && neighbour._contentType == TileContentType.Home)
                return true;
        }

        return false;
    }

    private void PlaceObject(GameObject prefab)
    {
        _spawnedObject = Instantiate(
            prefab,
            transform.position,
            Quaternion.Euler(0, 0, BuildManager.Instance.GetRotation())
        );

        _spawnedObject.transform.SetParent(transform);
        _spawnedObject.transform.position += new Vector3(0, 0, -0.1f);

        _isOccupied = true;

        var data = _spawnedObject.GetComponent<PlaceableObjectData>();
        if (data == null) return;

        _contentType = data.type;

        GameManager.Instance.AddScore(1);

        HandleCourseLogic(_contentType);

        if (_contentType == TileContentType.Home)
        {
            BuildManager.Instance.SetHomePlaced();
        }

        _highlight.SetActive(false);
    }

    private void HandleCourseLogic(TileContentType type)
    {
        if (type == TileContentType.CourseStart)
        {
            int newID = GameManager.Instance.GetNextCourseID();
            _courseIDs.Add(newID);
            GameManager.Instance.AddScore(1);
            return;
        }

        if (type == TileContentType.Course ||
            type == TileContentType.CourseEnd_Part1 ||
            type == TileContentType.CourseEnd_Part2)
        {
            HashSet<int> connected = GetAdjacentCourseIDs();

            foreach (int id in connected)
            {
                if (!_courseIDs.Contains(id))
                {
                    _courseIDs.Add(id);
                    GameManager.Instance.AddScore(1);
                }
            }
        }
    }

    private HashSet<int> GetAdjacentCourseIDs()
    {
        HashSet<int> result = new HashSet<int>();

        Vector2Int[] directions =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (var dir in directions)
        {
            Tile neighbour = GameManager.Instance.GetTileAt(GridPosition + dir);

            if (neighbour == null) continue;

            foreach (int id in neighbour._courseIDs)
            {
                result.Add(id);
            }
        }

        return result;
    }
}