using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Dropdown : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    [SerializeField] private List<GameObject> placeablePrefabs;

    private void Start()
    {
        dropdown.ClearOptions();

        List<string> names = new List<string>();

        foreach (var prefab in placeablePrefabs)
        {
            names.Add(prefab.name);
        }

        dropdown.AddOptions(names);
        dropdown.onValueChanged.AddListener(OnSelectionChanged);

        OnSelectionChanged(0);
    }

    private void OnSelectionChanged(int index)
    {
        BuildManager.Instance.SetSelectedPrefab(placeablePrefabs[index]);
    }
}