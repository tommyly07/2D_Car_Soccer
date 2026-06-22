using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static bool isPaused = false;

    [SerializeField] private GameObject pauseMenuUI;
    [SerializeField] private GameObject settingsMenuUI;
    [SerializeField] private GameObject pauseHUDButton;

    void Start()
    {
        // Ensure UI is hidden on start
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }
        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }
        
        // Only show the pause button in gameplay, hide it if we are on the MainMenu scene
        if (pauseHUDButton != null)
        {
            bool isMainMenu = SceneManager.GetActiveScene().name == "MainMenu";
            pauseHUDButton.SetActive(!isMainMenu);
        }

        isPaused = false;
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (settingsMenuUI != null && settingsMenuUI.activeSelf)
            {
                CloseSettings();
            }
            else if (isPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }

        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
        }

        Time.timeScale = 1f;
        isPaused = false;
    }

    public void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            FocusFirstButton(pauseMenuUI);
        }

        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }

        Time.timeScale = 0f;
        isPaused = true;
    }

    public void OpenSettings()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(true);
            FocusFirstButton(settingsMenuUI);
        }
    }

    public void CloseSettings()
    {
        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
            FocusFirstButton(pauseMenuUI);
        }
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    private void FocusFirstButton(GameObject panel)
    {
        if (panel == null || EventSystem.current == null)
        {
            return;
        }

        Button firstButton = panel.GetComponentInChildren<Button>(true);
        if (firstButton != null)
        {
            EventSystem.current.SetSelectedGameObject(firstButton.gameObject);
        }
    }
}
