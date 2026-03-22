using UnityEngine;
using System.Collections.Generic;

public class TileHandSystem : MonoBehaviour
{
    public int maxHandSize = 3;

    public List<TileType> deck = new List<TileType>();
    public List<TileType> hand = new List<TileType>();

    private GameManager gm;

    void Start()
    {
        gm = GetComponent<GameManager>();

        GenerateStarterDeck();
        DrawStartingHand();
    }

    // ================= DECK SETUP =================

    void GenerateStarterDeck()
    {
        // You can tweak balance here
        deck.Add(TileType.Path);
        deck.Add(TileType.Path);
        deck.Add(TileType.TJunction);
        deck.Add(TileType.Cross);
        deck.Add(TileType.RopeSingle);
        deck.Add(TileType.RopeDouble);
        deck.Add(TileType.ZiplineStart);
        deck.Add(TileType.ZiplineEnd);

        Shuffle(deck);
    }

    void Shuffle(List<TileType> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            TileType temp = list[i];
            int rand = Random.Range(i, list.Count);
            list[i] = list[rand];
            list[rand] = temp;
        }
    }

    // ================= HAND =================

    void DrawStartingHand()
    {
        for (int i = 0; i < maxHandSize; i++)
        {
            DrawTile();
        }
    }

    public void DrawTile()
    {
        if (deck.Count == 0) return;
        if (hand.Count >= maxHandSize) return;

        TileType drawn = deck[0];
        deck.RemoveAt(0);
        hand.Add(drawn);
    }

    public TileType GetTile(int index)
    {
        if (index < 0 || index >= hand.Count) return TileType.Empty;
        return hand[index];
    }

    public void UseTile(int index)
    {
        if (index < 0 || index >= hand.Count) return;

        hand.RemoveAt(index);
        DrawTile();

        UpdateUI();
    }

    // ================= UI HOOK =================

    public void UpdateUI()
    {
        // Hook this into your UI later
        // For now just debug
        Debug.Log("Hand:");
        foreach (var tile in hand)
        {
            Debug.Log(tile);
        }
    }
}
