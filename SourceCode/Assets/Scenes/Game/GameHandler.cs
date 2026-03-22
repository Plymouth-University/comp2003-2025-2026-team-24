using UnityEngine;

public class GameHandler : MonoBehaviour
{
    public static GameHandler Instance;

    public GameManager gameManager;
    public TileHandSystem hand;

    public int score;

    float timer;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        gameManager.Init();
        hand.Init();
    }

    void Update()
    {
        HandleInput();
    }

    void HandleInput()
    {
        if (!Input.GetMouseButtonDown(0)) return;

        Vector3 world = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int x = Mathf.FloorToInt(world.x);
        int y = Mathf.FloorToInt(world.y);

        TileData data = hand.GetTile(0);
        if (data == null) return;

        gameManager.PlaceTile(x, y, data);

        Tile t = gameManager.GetTile(x, y);

        if (t != null)
        {
            score += 5 + t.scoreValue;
            hand.UseTile(0);
        }
    }
}
