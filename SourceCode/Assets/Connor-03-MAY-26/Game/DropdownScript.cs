using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropdownScript : MonoBehaviour
{
    public static DropdownScript Instance;

    [Header("References")]
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private List<GameObject> placeablePrefabs;

    private const int HAND_SIZE = 3;
    private List<GameObject> currentHand = new List<GameObject>();

    // =========================
    // 🔁 MANAGER HELPERS
    // =========================

    private bool IsMP => GameManager_MP.Instance != null;

    private GameManager GM_SP => GameManager.Instance;
    private GameManager_MP GM_MP => GameManager_MP.Instance;

    private BuildManager BM_SP => BuildManager.Instance;
    private BuildManager_MP BM_MP => BuildManager_MP.Instance;

    // =====================================================
    // SETUP
    // =====================================================

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        dropdown.onValueChanged.AddListener(OnSelectionChanged);
        GenerateStartingHand();
    }

    // =====================================================
    // START HAND
    // =====================================================

    private void GenerateStartingHand()
    {
        currentHand.Clear();

        while (currentHand.Count < HAND_SIZE)
            currentHand.Add(GetSmartRandomTile());

        EnforceRules();
        RefreshDropdown();

        dropdown.SetValueWithoutNotify(0);
        OnSelectionChanged(0);
    }

    // =====================================================
    // PLAY TILE
    // =====================================================

    public void PlaySelectedTile()
    {
        int playedIndex = dropdown.value;

        if (playedIndex < 0 || playedIndex >= currentHand.Count)
            playedIndex = 0;

        currentHand[playedIndex] = GetSmartRandomTile();

        EnforceRules();
        RefreshDropdown();

        dropdown.SetValueWithoutNotify(playedIndex);
        dropdown.RefreshShownValue();

        OnSelectionChanged(playedIndex);
    }

    // =====================================================
    // RULES
    // =====================================================

    private void EnforceRules()
    {
        EnsurePathExists();

        if (!IsHomePlaced())
            EnsureCardExists(TileContentType.Home);

        if (IsHomePlaced() && !BoardHas(TileContentType.CourseStart))
            EnsureCardExists(TileContentType.CourseStart);
    }

    private bool IsHomePlaced()
    {
        return IsMP ? BM_MP.IsHomePlaced() : BM_SP.IsHomePlaced();
    }

    private void EnsurePathExists()
    {
        if (HandContains(TileContentType.Path))
            return;

        ReplaceRandomSlot(GetPrefab(TileContentType.Path));
    }

    private void EnsureCardExists(TileContentType type)
    {
        if (HandContains(type))
            return;

        GameObject prefab = GetPrefab(type);
        if (prefab == null) return;

        ReplaceRandomSlot(prefab);
    }

    private void ReplaceRandomSlot(GameObject prefab)
    {
        if (prefab == null) return;

        int randomIndex = Random.Range(0, currentHand.Count);
        currentHand[randomIndex] = prefab;
    }

    private bool HandContains(TileContentType type)
    {
        foreach (GameObject obj in currentHand)
        {
            PlaceableObjectData data = obj.GetComponent<PlaceableObjectData>();

            if (data != null && data.type == type)
                return true;
        }

        return false;
    }

    private GameObject GetPrefab(TileContentType type)
    {
        foreach (GameObject prefab in placeablePrefabs)
        {
            PlaceableObjectData data = prefab.GetComponent<PlaceableObjectData>();

            if (data != null && data.type == type)
                return prefab;
        }

        return null;
    }

    // =====================================================
    // SMART RANDOM
    // =====================================================

    private GameObject GetSmartRandomTile()
    {
        List<GameObject> valid = new List<GameObject>();

        foreach (GameObject prefab in placeablePrefabs)
        {
            PlaceableObjectData data = prefab.GetComponent<PlaceableObjectData>();
            if (data == null) continue;

            if (CanCurrentlyUse(data.type))
                valid.Add(prefab);
        }

        if (valid.Count == 0)
            return placeablePrefabs[0];

        return valid[Random.Range(0, valid.Count)];
    }

    private bool CanCurrentlyUse(TileContentType type)
    {
        if (type == TileContentType.Path)
            return true;

        if (type == TileContentType.Home)
            return !IsHomePlaced();

        if (!IsHomePlaced())
            return false;

        if (type == TileContentType.CourseStart)
            return HasBuildableNeighbour(TileContentType.Path) ||
                   HasBuildableNeighbour(TileContentType.Home);

        if (type == TileContentType.Course)
            return HasBuildableNeighbour(TileContentType.CourseStart) ||
                   HasBuildableNeighbour(TileContentType.Course);

        if (type == TileContentType.CourseEnd_Part1)
            return HasBuildableNeighbour(TileContentType.Course);

        if (type == TileContentType.CourseEnd_Part2)
            return HasBuildableNeighbour(TileContentType.CourseEnd_Part1);

        return true;
    }

    // =====================================================
    // BOARD CHECKS
    // =====================================================

    private bool BoardHas(TileContentType wanted)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        foreach (Tile tile in allTiles)
        {
            if (tile.GetContentType() == wanted)
                return true;
        }

        return false;
    }

    private bool HasBuildableNeighbour(TileContentType wanted)
    {
        Tile[] allTiles = FindObjectsOfType<Tile>();

        Vector2Int[] dirs =
        {
            Vector2Int.up,
            Vector2Int.down,
            Vector2Int.left,
            Vector2Int.right
        };

        foreach (Tile tile in allTiles)
        {
            if (tile.GetContentType() != wanted)
                continue;

            foreach (Vector2Int dir in dirs)
            {
                Tile neighbour = IsMP
                    ? GM_MP.GetTileAt(tile.GridPosition + dir)
                    : GM_SP.GetTileAt(tile.GridPosition + dir);

                if (neighbour == null)
                    continue;

                if (!neighbour.IsOccupied())
                    return true;
            }
        }

        return false;
    }

    // =====================================================
    // UI
    // =====================================================

    private void RefreshDropdown()
    {
        dropdown.ClearOptions();

        List<string> names = new List<string>();

        foreach (GameObject obj in currentHand)
            names.Add(obj.name);

        dropdown.AddOptions(names);
        dropdown.RefreshShownValue();
    }

    private void OnSelectionChanged(int index)
    {
        if (index < 0 || index >= currentHand.Count)
            return;

        if (IsMP)
            BM_MP.SetSelectedPrefab(currentHand[index]);
        else
            BM_SP.SetSelectedPrefab(currentHand[index]);
    }
}