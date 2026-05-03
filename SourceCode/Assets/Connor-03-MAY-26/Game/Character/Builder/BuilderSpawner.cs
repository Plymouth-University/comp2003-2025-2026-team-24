using UnityEngine;

public class BuilderSpawner : MonoBehaviour
{
    public static BuilderSpawner Instance;

    [SerializeField] private GameObject builderPrefab;
    [SerializeField] private int maxBuilders = 2;

    private void Awake()
    {
        Instance = this;
    }

    // Call this when a CourseStart tile is placed
    public void TrySpawnBuilderAtTile(Tile tile)
    {
        if (tile == null)
            return;

        if (tile.GetContentType() != TileContentType.CourseStart)
            return;

        int currentBuilders = FindObjectsOfType<Builder>().Length;

        if (currentBuilders >= maxBuilders)
        {
            Debug.Log("Maximum builders reached.");
            return;
        }

        if (!tile.IsGuestFree())
        {
            Debug.Log("Tile occupied.");
            return;
        }

        // Spawn directly on tile
        GameObject obj = Instantiate(
            builderPrefab,
            tile.transform.position + new Vector3(0.25f, 0.25f, 0f),
            Quaternion.identity
        );

        Builder builder = obj.GetComponent<Builder>();

        if (builder == null)
        {
            Debug.LogError("Builder prefab missing Builder script!");
            return;
        }

        // IMPORTANT: set tile so it doesn't snap to home
        builder.SetStartingTile(tile);

        Debug.Log("Builder spawned at: " + tile.name);
    }
}