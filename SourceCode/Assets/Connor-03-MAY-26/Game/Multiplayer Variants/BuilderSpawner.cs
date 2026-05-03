using UnityEngine;

public class BuilderSpawner_MP : MonoBehaviour
{
    public static BuilderSpawner_MP Instance;

    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private int maxBuildersPerPlayer = 2;

    private void Awake()
    {
        Instance = this;
    }

    // CALL THIS WHEN A COURSE START TILE IS PLACED
    public void TrySpawnBuilderAtTile(Tile tile)
    {
        if (tile == null)
            return;

        if (tile.GetContentType() != TileContentType.CourseStart)
            return;

        int currentPlayer = GameManager_MP.Instance.currentPlayerIndex;

        int builderCount = CountBuildersForPlayer(currentPlayer);

        if (builderCount >= maxBuildersPerPlayer)
        {
            Debug.Log("Max builders reached for player " + currentPlayer);
            return;
        }

        if (!tile.IsGuestFree())
        {
            Debug.Log("Tile occupied, cannot spawn builder.");
            return;
        }

        // Spawn directly on the tile
        GameObject obj = Instantiate(
            builderPrefab,
            tile.transform.position + new Vector3(0.25f, 0.25f, 0f),
            Quaternion.identity
        );

        Builder_MP builder = obj.GetComponent<Builder_MP>();

        if (builder == null)
        {
            Debug.LogError("Builder prefab missing Builder_MP!");
            return;
        }

        // Assign correct owner
        builder.ownerPlayerIndex = currentPlayer;

        Debug.Log($"Spawned Builder for Player {currentPlayer} at {tile.name}");
    }

    private int CountBuildersForPlayer(int playerIndex)
    {
        Builder_MP[] allBuilders = FindObjectsOfType<Builder_MP>();

        int count = 0;

        foreach (var builder in allBuilders)
        {
            if (builder.ownerPlayerIndex == playerIndex)
                count++;
        }

        return count;
    }
}