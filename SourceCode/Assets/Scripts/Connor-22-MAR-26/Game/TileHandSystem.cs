using UnityEngine;
using System.Collections.Generic;

public class TileHandSystem : MonoBehaviour
{
    public TileDatabase database;

    public List<TileData> hand = new List<TileData>();
    public int maxHand = 3;

    public void Init()
    {
        hand.Clear();

        for (int i = 0; i < maxHand; i++)
            Draw();
    }

    public void Draw()
    {
        TileData tile = database.GetRandomTile();
        hand.Add(tile);
    }

    public TileData GetTile(int index)
    {
        if (index >= hand.Count) return null;
        return hand[index];
    }

    public void UseTile(int index)
    {
        if (index >= hand.Count) return;

        hand.RemoveAt(index);
        Draw();
    }
}
