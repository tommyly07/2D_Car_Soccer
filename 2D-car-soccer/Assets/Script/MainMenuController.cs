using UnityEngine;
using UnityEngine.SceneManagement; // Wichtig für den Szenenwechsel!

public class MenuLogic : MonoBehaviour
{
    public void StartGame()
    {
        // Lädt die Szene mit dem Namen GameScene
        Debug.Log("Spiel wird geladen...");
        SceneManager.LoadScene("GameScene");
    }

    public void QuitGame()
    {
        // Beendet das Spiel (funktioniert nur in der fertigen .exe)
        Debug.Log("Spiel wird beendet...");
        Application.Quit();
    }
}