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

    public System.Action<int> OnScoreChanged;

    [Header("Timer Settings")]
    [SerializeField] private float gameDuration = 300f; // 5 minutes in seconds
    private float timeRemaining;
    public bool IsGameOver { get; private set; } = false;

    public System.Action<float> OnTimerChanged;
    public System.Action OnGameOver;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        timeRemaining = gameDuration;
    }

    private void Start()
    {
        ball?.PlaceBallAtCenter(player1);
    }

    private void Update()
    {
        if (!IsGameOver)
        {
            timeRemaining -= Time.deltaTime;
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame();
            }
            OnTimerChanged?.Invoke(timeRemaining);
        }
    }

    private void EndGame()
    {
        IsGameOver = true;
        Time.timeScale = 0f; // Pause game physics and updates
        OnGameOver?.Invoke();
    }

    public void OnGoalScored(int scoringPlayer)
    {
        if (IsGameOver) return; // Do not score goals after game is over

        if (scoringPlayer == 1)
            ScorePlayer1++;
        else
            ScorePlayer2++;

        OnScoreChanged?.Invoke(scoringPlayer);

        GameObject losingPlayer = scoringPlayer == 1 ? player2 : player1;
        ball?.PlaceBallAtCenter(losingPlayer);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time is unfrozen
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}