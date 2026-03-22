using UnityEngine;

public class Tile : MonoBehaviour
{
    public TileType tileType;
    public int scoreValue;

    public SpriteRenderer sr;

    public void Init(TileData data)
    {
        tileType = data.type;
        scoreValue = data.scoreValue;

        if (sr != null)
            sr.sprite = data.sprite;
    }
}
