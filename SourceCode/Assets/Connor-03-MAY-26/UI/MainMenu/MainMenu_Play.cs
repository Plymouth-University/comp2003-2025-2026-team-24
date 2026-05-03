using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu_Play : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync("PlayMenu");
    }
}
