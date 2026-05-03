using System.Collections.Generic;
using UnityEngine;

public class UnitManager_MP : MonoBehaviour
{
    public static UnitManager_MP Instance;

    private List<ControllableUnit_MP> _allUnits = new List<ControllableUnit_MP>();

    private void Awake()
    {
        Instance = this;
    }

    // =========================
    // REGISTER
    // =========================

    public void RegisterUnit(ControllableUnit_MP unit)
    {
        if (!_allUnits.Contains(unit))
        {
            _allUnits.Add(unit);
        }
    }

    public void UnregisterUnit(ControllableUnit_MP unit)
    {
        if (_allUnits.Contains(unit))
        {
            _allUnits.Remove(unit);
        }
    }

    // =========================
    // GET UNITS
    // =========================

    public List<ControllableUnit_MP> GetUnitsForPlayer(int playerIndex)
    {
        List<ControllableUnit_MP> result = new List<ControllableUnit_MP>();

        foreach (var unit in _allUnits)
        {
            if (unit.ownerPlayerIndex == playerIndex)
            {
                result.Add(unit);
            }
        }

        return result;
    }

    public List<ControllableUnit_MP> GetAllUnits()
    {
        return _allUnits;
    }
}