using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Game/Tile Database")]
public class TileDatabase : ScriptableObject
{
    public List<TileData> allTiles;

    public TileData GetRandomTile()
    {
        int totalWeight = 0;

        foreach (var t in allTiles)
            totalWeight += t.weight;

        int roll = Random.Range(0, totalWeight);

        int current = 0;

        foreach (var t in allTiles)
        {
            current += t.weight;
            if (roll < current)
                return t;
        }

        return allTiles[0];
    }
}
