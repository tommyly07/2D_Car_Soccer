using UnityEngine;

public class BallController : MonoBehaviour
{
    [Header("Ball Settings")]
    [SerializeField] private float kickForce = 8f;

    [Header("Goal Line X Positions")]
    [SerializeField] private float leftGoalX = -8f;
    [SerializeField] private float rightGoalX = 8f;

    private Rigidbody2D rb;
    private bool goalScored = false;
    private bool waitingForTouch = false;
    private GameObject allowedPlayer = null;

    private GameManager gameManager;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        gameManager = FindFirstObjectByType<GameManager>();
    }

    private void Update()
    {
        if (goalScored) return;

        // Ball eingefroren halten bis richtiger Spieler berührt
        if (waitingForTouch)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            transform.position = Vector2.zero;
            return;
        }

        if (transform.position.x >= rightGoalX)
            OnGoalScored(1);
        else if (transform.position.x <= leftGoalX)
            OnGoalScored(2);
    }

    private void OnGoalScored(int scoringPlayer)
    {
        if (goalScored) return;
        goalScored = true;
        gameManager?.OnGoalScored(scoringPlayer);
    }

    public void PlaceBallAtCenter(GameObject losingPlayer)
    {
        goalScored = false;
        waitingForTouch = true;
        allowedPlayer = losingPlayer;
        rb.linearVelocity = Vector2.zero;
        rb.angularVelocity = 0f;
        transform.position = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (!col.gameObject.CompareTag("Player")) return;

        if (waitingForTouch)
        {
            if (col.gameObject == allowedPlayer)
            {
                waitingForTouch = false;
                allowedPlayer = null;
                // Kleiner Kick damit Ball sofort reagiert
                Vector2 dir = (transform.position - col.transform.position).normalized;
                rb.AddForce(dir * kickForce * 0.5f, ForceMode2D.Impulse);
            }
            return;
        }

        // Normaler Kick
        Vector2 bounceDir = (transform.position - col.transform.position).normalized;
        rb.AddForce(bounceDir * kickForce, ForceMode2D.Impulse);
    }
}