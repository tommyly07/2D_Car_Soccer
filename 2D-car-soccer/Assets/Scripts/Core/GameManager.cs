using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player References")]
    public GameObject player1;
    public GameObject player2;
    public BallController ball;

    [Header("Match Settings")]
    public float matchLength = 300f; 

    public int ScorePlayer1 { get; private set; } = 0;
    public int ScorePlayer2 { get; private set; } = 0;

    public float CurrentTime { get; private set; }
    public bool IsGameOver { get; private set; } = false;

    public System.Action<int> OnScoreChanged;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void Start()
    {
        CurrentTime = matchLength; 
        ball?.PlaceBallAtCenter(player1);
    }

    private void Update()
    {
        if (IsGameOver) return; 

        CurrentTime -= Time.deltaTime; 

        if (CurrentTime <= 0)
        {
            CurrentTime = 0;
            EndGame();
        }
    }

    public void OnGoalScored(int scoringPlayer)
    {
        if (IsGameOver) return; 

        if (scoringPlayer == 1)
            ScorePlayer1++;
        else
            ScorePlayer2++;

        OnScoreChanged?.Invoke(scoringPlayer);

        GameObject losingPlayer = scoringPlayer == 1 ? player2 : player1;
        ball?.PlaceBallAtCenter(losingPlayer);
    }

    private void EndGame()
    {
        IsGameOver = true;
        Debug.Log("Zeit abgelaufen! Spiel beendet.");
        
        // Friere den Ball ein, damit er nicht mehr weiterrollt
        if (ball != null && ball.GetComponent<Rigidbody2D>() != null)
        {
            ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
            ball.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}