using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance;

    private List<ControllableUnit> units =
        new List<ControllableUnit>();

    private int selectedIndex = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        Cleanup();

        if (units.Count == 0)
            return;

        if (Input.GetKeyDown(KeyCode.Q))
            SelectNextUnit();

        if (Input.GetKeyDown(KeyCode.W))
            MoveUp();

        if (Input.GetKeyDown(KeyCode.S))
            MoveDown();

        if (Input.GetKeyDown(KeyCode.A))
            MoveLeft();

        if (Input.GetKeyDown(KeyCode.D))
            MoveRight();
    }

    public void RegisterUnit(ControllableUnit unit)
    {
        if (!units.Contains(unit))
            units.Add(unit);

        HighlightSelected();
    }

    private void Cleanup()
    {
        units.RemoveAll(x => x == null);

        if (selectedIndex >= units.Count)
            selectedIndex = 0;
    }

    // ==================================
    // NEW BUTTON + Q FUNCTION
    // ==================================

    public void SelectNextUnit()
    {
        if (units.Count == 0)
            return;

        selectedIndex++;

        if (selectedIndex >= units.Count)
            selectedIndex = 0;

        HighlightSelected();
    }

    private void HighlightSelected()
    {
        for (int i = 0; i < units.Count; i++)
        {
            units[i].SetSelected(i == selectedIndex);
        }
    }

    // ==================================
    // MOVEMENT
    // ==================================

    public void MoveUp()
    {
        if (units.Count > 0)
            units[selectedIndex].MoveUp();
    }

    public void MoveDown()
    {
        if (units.Count > 0)
            units[selectedIndex].MoveDown();
    }

    public void MoveLeft()
    {
        if (units.Count > 0)
            units[selectedIndex].MoveLeft();
    }

    public void MoveRight()
    {
        if (units.Count > 0)
            units[selectedIndex].MoveRight();
    }
}