using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    public void StartGame()
    {
        // Loads the scene named GameScene
        Debug.Log("Game is loading...");
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Quits the game (only works in build)
        Debug.Log("Game is quitting...");
        Application.Quit();
    }

    [SerializeField] private GameObject settingsMenu;

    public void OpenSettings()
    {
        if (settingsMenu != null)
        {
            settingsMenu.SetActive(true);
        }
    }
}
