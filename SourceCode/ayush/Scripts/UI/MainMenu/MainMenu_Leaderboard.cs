using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Leaderboard : MonoBehaviour
{
    public void LoadLeaderboard()
    {
        SceneManager.LoadSceneAsync("LeaderboardMenu");
    }
}
