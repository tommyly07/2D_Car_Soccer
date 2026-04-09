using UnityEngine;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreText;       // z.B. "2 : 1" oben Mitte
    [SerializeField] private TextMeshProUGUI goalBannerText;  // "TOR!" kurz einblenden (optional)
    [SerializeField] private GameObject goalBanner;           // Panel das kurz aufpoppt

    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null) return;

        gameManager.OnScoreChanged += UpdateScore;

        UpdateScore();
        if (goalBanner != null) goalBanner.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gameManager == null) return;
        gameManager.OnScoreChanged -= UpdateScore;
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