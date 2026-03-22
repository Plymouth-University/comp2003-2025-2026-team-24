using UnityEngine;

[CreateAssetMenu(menuName = "Game/Tile")]
public class TileData : ScriptableObject
{
    public string tileName;
    public Sprite sprite;
    public GameObject prefab;

    public int scoreValue = 1;
    public int weight = 10;

    public TileType type;
}
