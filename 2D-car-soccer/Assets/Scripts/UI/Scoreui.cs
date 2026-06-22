using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;       
    [SerializeField] private TextMeshProUGUI goalBannerText;  
    [SerializeField] private GameObject goalBanner;           

    [Header("Timer & Game Over References")]
    [SerializeField] private TextMeshProUGUI timerText;       
    [SerializeField] private GameObject gameOverPanel;        

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null) return;

        gameManager.OnScoreChanged += HandleScoreChanged; 

        UpdateScore();
        if (goalBanner != null) goalBanner.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false); 
    }

    private void Update()
    {
        if (gameManager == null) return;

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(gameManager.CurrentTime / 60);
            int seconds = Mathf.FloorToInt(gameManager.CurrentTime % 60);
            
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        if (gameManager.IsGameOver && gameOverPanel != null && !gameOverPanel.activeSelf)
        {
            gameOverPanel.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        if (gameManager == null) return;
        gameManager.OnScoreChanged -= HandleScoreChanged; 
    }

    private void HandleScoreChanged(int scoringPlayer)
    {
        UpdateScore();              
        ShowGoalBanner(scoringPlayer); 
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