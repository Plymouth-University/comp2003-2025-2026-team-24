using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour
{
    public GameManager gameManager;

    public void ExitLevel()
    {
        LeaderboardManager.CurrentRunScore = gameManager.Score;
        LeaderboardManager.SaveCurrentScore();

        SceneManager.LoadScene("MainMenu");
    }
}