using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid")]
    [SerializeField] private int _width = 10;
    [SerializeField] private int _height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI _scoreText;

    private Dictionary<Vector2Int, Tile> _tiles = new Dictionary<Vector2Int, Tile>();

    private int _score = 0;
    private int _nextCourseID = 1;

    private void Awake()
    {
        Instance = this;

    }

    private void Start()
    {
        GenerateGrid();
        UpdateScoreUI();
    }

    private void GenerateGrid()
    {
        _tiles.Clear();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = Instantiate(_tilePrefab, new Vector3(x, y, 0), Quaternion.identity);

                tile.name = $"Tile {x} {y}";
                tile.SetPosition(x, y);

                bool isOffset = (x + y) % 2 == 0;
                tile.Init(isOffset);

                _tiles.Add(new Vector2Int(x, y), tile);
            }
        }

        if (_cam != null)
        {
            _cam.position = new Vector3(
                _width / 2f - 0.5f,
                _height / 2f - 0.5f,
                -10f
            );
        }
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        if (_tiles.TryGetValue(pos, out Tile tile))
            return tile;

        return null;
    }

    public Tile GetHomeTile()
    {
        foreach (Tile tile in _tiles.Values)
        {
            if (tile != null && tile.GetContentType() == TileContentType.Home)
                return tile;
        }

        return null;
    }

    public List<Tile> GetWalkableNeighbours(Tile tile)
    {
        List<Tile> result = new List<Tile>();

        if (tile == null)
            return result;

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Vector2Int dir in dirs)
        {
            Tile neighbour = GetTileAt(tile.GridPosition + dir);

            if (neighbour == null)
                continue;

            TileContentType type = neighbour.GetContentType();

            if (type == TileContentType.Path ||
                type == TileContentType.Home ||
                type == TileContentType.CourseStart)
            {
                result.Add(neighbour);
            }
        }

        return result;
    }

    public void AddScore(int amount)
    {
        _score += amount;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (_scoreText != null)
            _scoreText.text = "Score: " + _score;
    }

    public int GetNextCourseID()
    {
        return _nextCourseID++;
    }

    public List<Tile> GetAllCourseStarts()
    {
        List<Tile> result = new List<Tile>();

        foreach (Tile tile in _tiles.Values)
        {
            if (tile == null)
                continue;

            if (tile.GetContentType() == TileContentType.CourseStart)
            {
                result.Add(tile);
            }
        }

        return result;
    }
   
    public int Score
    {
        get { return _score; }
    }

    public bool IsCourseFullyBuilt()
    {
        List<Tile> starts = GetAllCourseStarts();

        if (starts.Count == 0)
            return false;

        foreach (Tile startTile in starts)
        {
            if (IsCourseCompleteFromStart(startTile))
                return true; // at least one valid course exists
        }

        return false;
    }

    // BFS PATH VALIDATION
    private bool IsCourseCompleteFromStart(Tile startTile)
    {
        HashSet<Tile> visited = new HashSet<Tile>();
        Queue<Tile> queue = new Queue<Tile>();

        queue.Enqueue(startTile);
        visited.Add(startTile);

        while (queue.Count > 0)
        {
            Tile current = queue.Dequeue();

            // SUCCESS CONDITION
            if (current.GetContentType() == TileContentType.CourseEnd_Part2)
                return true;

            Vector2Int[] directions =
            {
                Vector2Int.up,
                Vector2Int.down,
                Vector2Int.left,
                Vector2Int.right
            };

            foreach (var dir in directions)
            {
                Tile neighbor = GetTileAt(current.GridPosition + dir);

                if (neighbor == null || visited.Contains(neighbor))
                    continue;

                TileContentType type = neighbor.GetContentType();

                // Only allow valid course tiles
                bool isValid =
                    type == TileContentType.Course ||
                    type == TileContentType.CourseEnd_Part1 ||
                    type == TileContentType.CourseEnd_Part2;

                if (!isValid)
                    continue;

                visited.Add(neighbor);
                queue.Enqueue(neighbor);
            }
        }

        // Path never reached the end
        return false;
    }
}