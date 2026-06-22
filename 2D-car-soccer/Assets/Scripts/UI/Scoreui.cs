using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;       
    [SerializeField] private TextMeshProUGUI goalBannerText;  
    [SerializeField] private GameObject goalBanner;           

    [Header("Timer & Game Over References")]
    [SerializeField] private TextMeshProUGUI timerText;       // Displays "5:00"
    [SerializeField] private GameObject gameOverPanel;        // GameOver UI Panel
    [SerializeField] private TextMeshProUGUI winnerText;      // Displays who won
    [SerializeField] private UnityEngine.UI.Button mainMenuButton; // Returns to Main Menu

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null) return;

        gameManager.OnScoreChanged += HandleScoreChanged; 
        gameManager.OnTimerChanged += HandleTimerChanged;
        gameManager.OnGameOver += HandleGameOver;

        UpdateScore();
        if (goalBanner != null) goalBanner.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }
    }

    private void OnDestroy()
    {
        if (gameManager == null) return;
        gameManager.OnScoreChanged -= HandleScoreChanged; 
        gameManager.OnTimerChanged -= HandleTimerChanged;
        gameManager.OnGameOver -= HandleGameOver;
    }

    private void HandleScoreChanged(int scoringPlayer)
    {
        UpdateScore();              // 1. Punkte aktualisieren
        ShowGoalBanner(scoringPlayer); // 2. Banner zeigen
    }

    private void HandleTimerChanged(float timeRemaining)
    {
        if (timerText == null) return;
        int minutes = Mathf.FloorToInt(timeRemaining / 60F);
        int seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);
        timerText.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    private void HandleGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (winnerText != null)
        {
            if (gameManager.ScorePlayer1 > gameManager.ScorePlayer2)
            {
                winnerText.text = "SPIELER 1 GEWINNT!";
            }
            else if (gameManager.ScorePlayer2 > gameManager.ScorePlayer1)
            {
                winnerText.text = "SPIELER 2 GEWINNT!";
            }
            else
            {
                winnerText.text = "UNENTSCHIEDEN!";
            }
        }

        // Auto-focus the main menu button for keyboard/controller navigation
        if (mainMenuButton != null && UnityEngine.EventSystems.EventSystem.current != null)
        {
            UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(mainMenuButton.gameObject);
        }
    }

    private void LoadMainMenu()
    {
        Time.timeScale = 1f; // Ensure timescale is restored
        SceneManager.LoadScene("MainMenu");
    }

    private void UpdateScore()
    {
        if (scoreText == null || gameManager == null) return;
        scoreText.text = $"{gameManager.ScorePlayer1}  :  {gameManager.ScorePlayer2}";
    }

    private void ShowGoalBanner(int scoringPlayer)
    {
        if (goalBanner == null) return;
        if (goalBannerText != null)
            goalBannerText.text = $"TOR! Spieler {scoringPlayer}";
        StartCoroutine(BannerCoroutine());
    }

    private System.Collections.IEnumerator BannerCoroutine()
    {
        goalBanner.SetActive(true);
        yield return new WaitForSeconds(1.2f);
        goalBanner.SetActive(false);
    }
}