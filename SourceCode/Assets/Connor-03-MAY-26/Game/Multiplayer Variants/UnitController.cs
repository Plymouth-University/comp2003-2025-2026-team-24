using System.Collections.Generic;
using UnityEngine;

public class UnitController_MP : MonoBehaviour
{
    public static UnitController_MP Instance;

    private ControllableUnit_MP selectedUnit;
    private int currentIndex = -1;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        HandleQSelection();
        HandleMovement();
    }

    // =========================
    // Q SELECTION (FINAL)
    // =========================

    private void HandleQSelection()
    {
        if (!Input.GetKeyDown(KeyCode.Q))
            return;

        if (UnitManager_MP.Instance == null)
        {
            Debug.LogError("UnitManager_MP missing!");
            return;
        }

        if (GameManager_MP.Instance == null)
        {
            Debug.LogError("GameManager_MP missing!");
            return;
        }

        Debug.Log("Q PRESSED");

        var units = UnitManager_MP.Instance.GetUnitsForPlayer(
            GameManager_MP.Instance.currentPlayerIndex
        );

        // SAFETY: No units
        if (units == null || units.Count == 0)
        {
            Debug.LogWarning("No units available for player: " + GameManager_MP.Instance.currentPlayerIndex);
            currentIndex = -1;
            SelectUnit(null);
            return;
        }

        Debug.Log("Units found: " + units.Count);

        // Reset index if it's out of bounds (prevents crash after unit count changes)
        if (currentIndex >= units.Count)
            currentIndex = -1;

        // Cycle forward
        currentIndex++;

        // Extra safety (not strictly needed but bulletproof)
        if (currentIndex >= units.Count)
            currentIndex = 0;

        // Select unit
        SelectUnit(units[currentIndex]);
    }

    // =========================
    // SELECT LOGIC
    // =========================

    public void SelectUnit(ControllableUnit_MP unit)
    {
        if (selectedUnit != null)
            selectedUnit.SetSelected(false);

        selectedUnit = unit;

        if (selectedUnit != null)
        {
            selectedUnit.SetSelected(true);
            Debug.Log("Selected: " + selectedUnit.name);
        }
    }

    // =========================
    // MOVEMENT (WASD + BUTTONS)
    // =========================

    private void HandleMovement()
    {
        if (selectedUnit == null)
            return;

        if (Input.GetKeyDown(KeyCode.W))
        {
            selectedUnit.MoveUp();
            Debug.Log("Moving Up!");
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            selectedUnit.MoveDown();
            Debug.Log("Moving Down!");
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            selectedUnit.MoveLeft();
            Debug.Log("Moving Left!");
        }

        if (Input.GetKeyDown(KeyCode.D))
        {
            selectedUnit.MoveRight();
            Debug.Log("Moving Right!");
        }
    }

    public void MoveUp() => selectedUnit?.MoveUp();
    public void MoveDown() => selectedUnit?.MoveDown();
    public void MoveLeft() => selectedUnit?.MoveLeft();
    public void MoveRight() => selectedUnit?.MoveRight();
}