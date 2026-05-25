using UnityEngine;

public class GoalManager : MonoBehaviour
{
    public Transform ball;
    public Transform player1;
    public Transform player2;

    private Vector3 ballStartPos;
    private Vector3 p1StartPos;
    private Vector3 p2StartPos;

    void Start()
    {
        ballStartPos = ball.position;
        p1StartPos = player1.position;
        p2StartPos = player2.position;
    }

    public void ScoreGoal(int scoringPlayer)
    {
        Debug.Log("Tor geschossen!");
        GameManager.Instance.OnGoalScored(scoringPlayer);
        ResetMatch();
    }

    void ResetMatch()
    {
        ball.position = ballStartPos;
        player1.position = p1StartPos;
        player2.position = p2StartPos;

        player1.rotation = Quaternion.Euler(0f, 0f, 0f);   
        player2.rotation = Quaternion.Euler(0f, 0f, 180f); 

        ball.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        ball.GetComponent<Rigidbody2D>().angularVelocity = 0f;

        player1.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player1.GetComponent<Rigidbody2D>().angularVelocity = 0f;
        player2.GetComponent<Rigidbody2D>().linearVelocity = Vector2.zero;
        player2.GetComponent<Rigidbody2D>().angularVelocity = 0f;
    }
}