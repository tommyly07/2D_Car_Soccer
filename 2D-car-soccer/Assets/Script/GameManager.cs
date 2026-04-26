using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Player References")]
    public GameObject player1;
    public GameObject player2;

    [Header("Spawn Positions")]
    public Vector2 player1SpawnPos = new Vector2(-3f, 0f);
    public Vector2 player2SpawnPos = new Vector2(3f, 0f);

    [Header("References")]
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
        // Spieler 1 startet mit dem Ball
        ball?.PlaceBallAtCenter(player1);
    }

    public void OnGoalScored(int scoringPlayer)
    {
        if (scoringPlayer == 1)
            ScorePlayer1++;
        else
            ScorePlayer2++;

        OnScoreChanged?.Invoke();

        ResetPlayers();

        // Der Verlierer bekommt den Ball
        GameObject losingPlayer = scoringPlayer == 1 ? player2 : player1;
        ball?.PlaceBallAtCenter(losingPlayer);
    }

    private void ResetPlayers()
    {
        ResetPlayer(player1, player1SpawnPos);
        ResetPlayer(player2, player2SpawnPos);
    }

    private void ResetPlayer(GameObject player, Vector2 spawnPos)
    {
        if (player == null) return;
        player.transform.position = spawnPos;
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}