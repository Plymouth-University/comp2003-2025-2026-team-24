using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaderboardMenu_Back : MonoBehaviour
{
    public void OnBackPressed()
    {
        SceneManager.LoadScene("MainMenu");
    }
}
