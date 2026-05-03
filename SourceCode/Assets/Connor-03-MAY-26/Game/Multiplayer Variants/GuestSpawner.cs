using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GuestSpawner_MP : MonoBehaviour
{
    public GameObject guestPrefab;

    public int guestsPerPlayer = 5;
    public int totalPlayers = 4;

    private bool hasSpawned = false;

    private void Start()
    {
        StartCoroutine(WaitAndSpawn());
    }

    private IEnumerator WaitAndSpawn()
    {
        yield return new WaitUntil(() =>
            GameManager_MP.Instance != null &&
            GameManager_MP.Instance.GetHomeTile() != null
        );

        SpawnGuests();
    }
    public void SpawnGuests()
    {
        if (hasSpawned)
            return;

        hasSpawned = true;

        if (GameManager_MP.Instance == null)
        {
            Debug.LogError("GameManager_MP missing!");
            return;
        }

        Tile home = GameManager_MP.Instance.GetHomeTile();

        if (home == null)
        {
            Debug.LogError("No Home tile found!");
            return;
        }

        for (int player = 0; player < totalPlayers; player++)
        {
            for (int i = 0; i < guestsPerPlayer; i++)
            {
                GameObject obj = Instantiate(
                    guestPrefab,
                    home.transform.position,
                    Quaternion.identity
                );

                Guest_MP guest = obj.GetComponent<Guest_MP>();

                if (guest == null)
                {
                    Debug.LogError("Guest prefab missing Guest_MP!");
                    continue;
                }

                guest.ownerPlayerIndex = player;
            }
        }
    }
}