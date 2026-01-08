using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayMenu : MonoBehaviour
{
    public void LoadSinglePlayer()
    {
        SceneManager.LoadSceneAsync("Game");
    }

    public void LoadMultiplayerPlayer()
    {
        SceneManager.LoadSceneAsync("Multiplayer");
    }

    public void LoadMain()
    {
        SceneManager.LoadSceneAsync("MainMenu");
    }
}
