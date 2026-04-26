using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player References")]
    public GameObject player1;
    public GameObject player2;

    public BallController ball;

    public int ScorePlayer1 { get; private set; } = 0;
    public int ScorePlayer2 { get; private set; } = 0;

    public System.Action OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        ball?.PlaceBallAtCenter(player1);
    }

    public void OnGoalScored(int scoringPlayer)
    {
        if (scoringPlayer == 1)
            ScorePlayer1++;
        else
            ScorePlayer2++;

        OnScoreChanged?.Invoke();

        GameObject losingPlayer = scoringPlayer == 1 ? player2 : player1;
        ball?.PlaceBallAtCenter(losingPlayer);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}