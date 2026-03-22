using UnityEngine;

public class TileUIButton : MonoBehaviour
{
    public int index;
    public TileUIManager ui;

    public void Click()
    {
        ui.Select(index);
    }
}