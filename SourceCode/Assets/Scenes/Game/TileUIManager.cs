using UnityEngine;
using UnityEngine.UI;   
using System.Collections.Generic;

public class TileUIManager : MonoBehaviour
{
    public List<Image> slots;

    private TileHandSystem hand;
    private int selected = 0;

    public void Init(TileHandSystem h)
    {
        hand = h;
        Refresh();
    }

    public int GetSelectedSlot()
    {
        return selected;
    }

    public void Select(int index)
    {
        selected = index;
        Refresh();
    }

    public void Refresh()
    {
        if (hand == null) return;

        for (int i = 0; i < slots.Count; i++)
        {
            var tile = hand.GetTile(i);

            if (tile == null)
            {
                slots[i].enabled = false;
            }
            else
            {
                slots[i].enabled = true;
                slots[i].sprite = tile.sprite;
            }

            slots[i].color = (i == selected)
                ? Color.white
                : new Color(1, 1, 1, 0.5f);
        }
    }

    // THESE MUST BE HERE (outside other methods)

    public void Select0()
    {
        Select(0);
    }

    public void Select1()
    {
        Select(1);
    }

    public void Select2()
    {
        Select(2);
    }
}