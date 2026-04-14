using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private int _width = 10, _height = 10;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _cam;

    [SerializeField] private TextMeshProUGUI _scoreText;

    private Dictionary<Vector2Int, Tile> _tiles;

    private int _score = 0;
    private int _nextCourseID = 1;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        GenerateGrid();

        if (_scoreText != null)
        {
            _scoreText.text = "Score: 0";
        }
    }

    private void GenerateGrid()
    {
        _tiles = new Dictionary<Vector2Int, Tile>();

        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = Instantiate(_tilePrefab, new Vector3(x, y), Quaternion.identity);

                tile.name = $"Tile {x} {y}";
                tile.SetPosition(x, y);

                bool isOffset = (x + y) % 2 == 0;
                tile.Init(isOffset);

                _tiles[new Vector2Int(x, y)] = tile;
            }
        }

        _cam.transform.position = new Vector3(
            _width / 2f - 0.5f,
            _height / 2f - 0.5f,
            -10
        );
    }

    public Tile GetTileAt(Vector2Int pos)
    {
        return _tiles.ContainsKey(pos) ? _tiles[pos] : null;
    }

    public void AddScore(int amount)
    {
        _score += amount;

        if (_scoreText != null)
        {
            _scoreText.text = "Score: " + _score;
        }
    }

    public int GetNextCourseID()
    {
        return _nextCourseID++;
    }
}