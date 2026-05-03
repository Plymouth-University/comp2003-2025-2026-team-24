// ================================
// UPDATED GUESTSPAWNER.CS
// ================================

using System.Collections.Generic;
using UnityEngine;

public class GuestSpawner : MonoBehaviour
{
    [SerializeField] private GameObject guestPrefab;
    [SerializeField] private float spawnInterval = 4f;
    [SerializeField] private int maxGuests = 5;

    private List<GameObject> guests = new List<GameObject>();

    private void Start()
    {
        InvokeRepeating(nameof(TrySpawnGuest), 2f, spawnInterval);
    }

    private void TrySpawnGuest()
    {
        CleanupGuests();

        if (guests.Count >= maxGuests)
            return;

        Tile home = GameManager.Instance.GetHomeTile();

        if (home == null)
            return;

        if (!home.IsGuestFree())
            return;

        GameObject guest = Instantiate(
            guestPrefab,
            home.transform.position + new Vector3(0.25f, 0.25f, 0f),
            Quaternion.identity
        );

        guests.Add(guest);
    }

    private void CleanupGuests()
    {
        for (int i = guests.Count - 1; i >= 0; i--)
        {
            if (guests[i] == null)
                guests.RemoveAt(i);
        }
    }
}