using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    [Header("Grid Settings")]
    public int rows = 20;
    public int columns = 20;
    public GameObject cellButtonPrefab;   // our tile button
    public Transform gridParent;          // panel with GridLayoutGroup

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text turnsText;
    public TMP_Dropdown tileDropdown;
    public int maxTurns = 40;

    private TileType[,] grid;
    private int currentScore = 0;
    private int turnsUsed = 0;

    void Start()
    {
        grid = new TileType[rows, columns];
        CreateGridUI();
        UpdateScoreUI();
        UpdateTurnsUI();
    }

    void CreateGridUI()
    {
        // remove old
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < columns; c++)
            {
                int row = r;
                int col = c;

                GameObject cellObj = Instantiate(cellButtonPrefab, gridParent);
                Button btn = cellObj.GetComponent<Button>();
                TMP_Text label = cellObj.GetComponentInChildren<TMP_Text>();

                label.text = "";

                btn.onClick.AddListener(() => OnCellClicked(row, col, label));
            }
        }
    }

    void OnCellClicked(int row, int col, TMP_Text label)
    {
        if (turnsUsed >= maxTurns) return;
        if (grid[row, col] != TileType.Empty) return;

        TileType selected = GetSelectedTileType();
        if (selected == TileType.Empty) return;

        grid[row, col] = selected;
        label.text = TileShortName(selected);

        switch (selected)
        {
            case TileType.Path:  label.color = Color.yellow; break;
            case TileType.Tree:  label.color = Color.green;  break;
            case TileType.Bench: label.color = Color.cyan;   break;
        }

        int gained = CalculateScoreForPlacement(row, col, selected);
        currentScore += gained;
        turnsUsed++;

        UpdateScoreUI();
        UpdateTurnsUI();

        if (turnsUsed >= maxTurns)
{
    LeaderboardManager.CurrentRunScore = currentScore;
    LeaderboardManager.SaveCurrentScore();
    SceneManager.LoadScene("LeaderboardMenu");   // <-- IMPORTANT NAME
}
    }

    TileType GetSelectedTileType()
    {
        // dropdown options: 0 = Path, 1 = Tree, 2 = Bench
        switch (tileDropdown.value)
        {
            case 0: return TileType.Path;
            case 1: return TileType.Tree;
            case 2: return TileType.Bench;
        }
        return TileType.Empty;
    }

    string TileShortName(TileType t)
    {
        switch (t)
        {
            case TileType.Path: return "P";
            case TileType.Tree: return "T";
            case TileType.Bench: return "B";
        }
        return "";
    }

    int CalculateScoreForPlacement(int row, int col, TileType type)
    {
        int score = 0;

        // base points
        if (type == TileType.Path)  score += 1;
        if (type == TileType.Tree)  score += 2;
        if (type == TileType.Bench) score += 3;

        // neighbours
        TileType[] neighbours = {
            GetTile(row - 1, col),
            GetTile(row + 1, col),
            GetTile(row, col - 1),
            GetTile(row, col + 1)
        };

        foreach (var n in neighbours)
        {
            if (n == TileType.Empty) continue;

            if (type == TileType.Path && n == TileType.Path)   score += 1;
            if (type == TileType.Bench && n == TileType.Path)  score += 1;
            if (type == TileType.Tree && n == TileType.Bench)  score += 1;
        }

        return score;
    }

    TileType GetTile(int row, int col)
    {
        if (row < 0 || row >= rows || col < 0 || col >= columns)
            return TileType.Empty;
        return grid[row, col];
    }

    void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = $"Score: {currentScore}";
    }

    void UpdateTurnsUI()
    {
        if (turnsText != null)
            turnsText.text = $"Turns: {turnsUsed}/{maxTurns}";
    }
}
