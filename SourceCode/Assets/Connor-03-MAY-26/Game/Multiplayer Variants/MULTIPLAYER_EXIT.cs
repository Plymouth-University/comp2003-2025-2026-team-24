using UnityEngine;
using UnityEngine.SceneManagement;

public class MPEXIT : MonoBehaviour
{
    public void ExitGAME()
    {
        Debug.Log("Tried to Exit Game!");
        SceneManager.LoadSceneAsync("MainMenu");
    }
}