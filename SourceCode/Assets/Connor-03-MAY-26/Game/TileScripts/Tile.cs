using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [Header("Visuals")]
    [SerializeField] private Color _baseColor;
    [SerializeField] private Color _offsetColor;
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private GameObject _highlight;

    private bool _isOccupied = false;
    private GameObject _spawnedObject;
    private TileContentType _contentType = TileContentType.None;

    private Component occupyingGuest = null;
    private Component reservedGuest = null;

    private HashSet<int> _courseIDs = new HashSet<int>();

    public Vector2Int GridPosition { get; private set; }

    private bool IsMP => GameManager_MP.Instance != null;

    private GameManager GM_SP => GameManager.Instance;
    private GameManager_MP GM_MP => GameManager_MP.Instance;

    private BuildManager BM_SP => BuildManager.Instance;
    private BuildManager_MP BM_MP => BuildManager_MP.Instance;

    private static readonly Vector2Int[] Directions =
    {
        Vector2Int.up,
        Vector2Int.down,
        Vector2Int.left,
        Vector2Int.right
    };

    // =========================
    // SETUP
    // =========================

    public void SetPosition(int x, int y)
    {
        GridPosition = new Vector2Int(x, y);
    }

    public void Init(bool isOffset)
    {
        _renderer.color = isOffset ? _offsetColor : _baseColor;
    }

    // =========================
    // MOUSE
    // =========================

    private void OnMouseEnter()
    {
        if (_isOccupied) return;

        GameObject prefab = IsMP ? BM_MP.GetSelectedPrefab() : BM_SP.GetSelectedPrefab();
        if (prefab == null) return;

        bool valid = CanPlace(prefab);

        _highlight.GetComponent<SpriteRenderer>().color = valid ? Color.green : Color.red;

        if (IsMP) BM_MP.SetGhostValid(valid);
        else BM_SP.SetGhostValid(valid);

        _highlight.SetActive(true);
    }

    private void OnMouseExit()
    {
        _highlight.SetActive(false);
    }

    private void OnMouseDown()
    {
        if (_isOccupied) return;

        GameObject prefab = IsMP ? BM_MP.GetSelectedPrefab() : BM_SP.GetSelectedPrefab();
        if (prefab == null) return;

        if (IsMP && !BM_MP.CanPlace()) return;

        if (!CanPlace(prefab))
        {
            Debug.Log($"[Tile {GridPosition}] ❌ Placement failed");
            return;
        }

        Debug.Log($"[Tile {GridPosition}] ✅ Placed {prefab.name}");
        PlaceObject(prefab);
    }

    // =========================
    // BUILD RULES
    // =========================

    private bool CanPlace(GameObject prefab)
    {
        PlaceableObjectData data = prefab.GetComponent<PlaceableObjectData>();
        if (data == null) return false;

        TileContentType type = data.type;

        // Must place home first
        if (!(IsMP ? BM_MP.IsHomePlaced() : BM_SP.IsHomePlaced()))
            return type == TileContentType.Home;

        // =========================
        // PATH + END (connection system)
        // =========================
        if (type == TileContentType.Path ||
            type == TileContentType.CourseEnd_Part1 ||
            type == TileContentType.CourseEnd_Part2)
        {
            return HasValidConnection(prefab);
        }

        // =========================
        // COURSE SYSTEM (NO CONNECTION LOGIC HERE)
        // =========================

        if (type == TileContentType.CourseStart)
        {
            return HasAdjacent(TileContentType.Path);
        }

        if (type == TileContentType.Course)
        {
            // allow chaining OR starting
            bool validChain =
                HasAdjacent(TileContentType.Course) ||
                HasAdjacent(TileContentType.CourseStart);

            return validChain && HasAdjacentBuilder() ;
        }

        return false;
    }

    // =========================
    // FIXED CONNECTION SYSTEM
    // =========================

    private bool HasValidConnection(GameObject prefab)
    {
        PlaceableObjectData data = prefab.GetComponent<PlaceableObjectData>();
        if (data == null) return false;

        float rot = IsMP ? BM_MP.GetRotation() : BM_SP.GetRotation();
        Direction myConnections = data.GetRotatedConnections(rot);

        bool hasAtLeastOneValidConnection = false;

        foreach (Vector2Int dir in Directions)
        {
            Direction mySide = PlaceableObjectData.VectorToDirection(dir);

            // Only check sides this tile actually opens
            if (!myConnections.HasFlag(mySide))
                continue;

            Tile neighbour = GetNeighbour(dir);

            // If there's no neighbour, just ignore this side
            if (neighbour == null || !neighbour._isOccupied)
                continue;

            PlaceableObjectData neighbourData =
                neighbour._spawnedObject.GetComponent<PlaceableObjectData>();

            if (neighbourData == null)
                continue;

            float neighbourRot = Mathf.Round(neighbour._spawnedObject.transform.eulerAngles.z / 90f) * 90f;
            Direction neighbourConnections = neighbourData.GetRotatedConnections(neighbourRot);

            Direction opposite = PlaceableObjectData.GetOpposite(mySide);

            // If neighbour does NOT connect back → invalid
            if (!neighbourConnections.HasFlag(opposite))
                return false;

            // Valid connection found
            hasAtLeastOneValidConnection = true;
        }

        return hasAtLeastOneValidConnection;
    }

    private Tile GetNeighbour(Vector2Int dir)
    {
        return IsMP
            ? GM_MP.GetTileAt(GridPosition + dir)
            : GM_SP.GetTileAt(GridPosition + dir);
    }

    // =========================
    // ADJACENCY
    // =========================

    private bool HasAdjacent(TileContentType type)
    {
        foreach (Vector2Int dir in Directions)
        {
            Tile neighbour = GetNeighbour(dir);
            if (neighbour == null) continue;

            if (neighbour._contentType == type)
                return true;

            if (type == TileContentType.Path &&
                neighbour._contentType == TileContentType.Home)
                return true;
        }

        return false;
    }

    // =========================
    // BUILDER CHECK
    // =========================

    private bool HasAdjacentBuilder()
    {
        if (IsMP)
        {
            foreach (var builder in FindObjectsOfType<Builder_MP>())
            {
                Tile tile = builder.GetCurrentTile();
                if (tile == null) continue;

                if (IsAdjacent(tile.GridPosition - GridPosition))
                    return true;
            }
        }
        else
        {
            foreach (var builder in FindObjectsOfType<Builder>())
            {
                Tile tile = builder.GetCurrentTile();
                if (tile == null) continue;

                if (IsAdjacent(tile.GridPosition - GridPosition))
                    return true;
            }
        }

        return false;
    }

    private bool IsAdjacent(Vector2Int diff)
    {
        return diff == Vector2Int.up ||
               diff == Vector2Int.down ||
               diff == Vector2Int.left ||
               diff == Vector2Int.right;
    }

    // =========================
    // PLACE OBJECT
    // =========================

    private void PlaceObject(GameObject prefab)
    {
        float rot = IsMP ? BM_MP.GetRotation() : BM_SP.GetRotation();

        _spawnedObject = Instantiate(prefab, transform.position, Quaternion.Euler(0, 0, rot));
        _spawnedObject.transform.SetParent(transform);
        _spawnedObject.transform.position += new Vector3(0, 0, -0.1f);

        _isOccupied = true;

        PlaceableObjectData data = _spawnedObject.GetComponent<PlaceableObjectData>();
        if (data == null) return;

        _contentType = data.type;

        if (IsMP)
        {
            GM_MP.AddScore(1, GM_MP.currentPlayerIndex);
            GM_MP.MarkTilePlaced();
        }
        else
        {
            GM_SP.AddScore(1);
        }

        HandleCourseLogic(_contentType);

        if (_contentType == TileContentType.CourseStart)
        {
            if (IsMP)
                BuilderSpawner_MP.Instance?.TrySpawnBuilderAtTile(this);
            else
                BuilderSpawner.Instance?.TrySpawnBuilderAtTile(this);
        }

        if (_contentType == TileContentType.Home)
        {
            if (IsMP)
                BM_MP.SetHomePlaced();
            else
                BM_SP.SetHomePlaced();
        }

        _highlight.SetActive(false);

        if (DropdownScript.Instance != null)
            DropdownScript.Instance.PlaySelectedTile();
    }

    // =========================
    // COURSE LOGIC
    // =========================

    private void HandleCourseLogic(TileContentType type)
    {
        if (type == TileContentType.CourseStart)
        {
            int id = IsMP ? GM_MP.GetNextCourseID() : GM_SP.GetNextCourseID();
            _courseIDs.Add(id);
            return;
        }

        foreach (int id in GetAdjacentCourseIDs())
            _courseIDs.Add(id);
    }

    private HashSet<int> GetAdjacentCourseIDs()
    {
        HashSet<int> result = new HashSet<int>();

        foreach (Vector2Int dir in Directions)
        {
            Tile neighbour = GetNeighbour(dir);
            if (neighbour == null) continue;

            foreach (int id in neighbour._courseIDs)
                result.Add(id);
        }

        return result;
    }

    private void OnDrawGizmos()
    {
        if (_spawnedObject == null) return;

        PlaceableObjectData data = _spawnedObject.GetComponent<PlaceableObjectData>();
        if (data == null) return;

        Direction con = data.GetRotatedConnections(_spawnedObject.transform.eulerAngles.z);

        Gizmos.color = Color.blue;

        Vector3 pos = transform.position;

        if (con.HasFlag(Direction.Up))
            Gizmos.DrawLine(pos, pos + Vector3.up * 0.5f);

        if (con.HasFlag(Direction.Down))
            Gizmos.DrawLine(pos, pos + Vector3.down * 0.5f);

        if (con.HasFlag(Direction.Left))
            Gizmos.DrawLine(pos, pos + Vector3.left * 0.5f);

        if (con.HasFlag(Direction.Right))
            Gizmos.DrawLine(pos, pos + Vector3.right * 0.5f);
    }

    // =========================
    // GUEST
    // =========================

    public void SetGuest(Component guest)
    {
        occupyingGuest = guest;
        reservedGuest = null;
    }

    public bool IsGuestFree()
    {
        return occupyingGuest == null && reservedGuest == null;
    }

    public bool IsOccupied()
    {
        return _isOccupied;
    }

    public TileContentType GetContentType()
    {
        return _contentType;
    }
}