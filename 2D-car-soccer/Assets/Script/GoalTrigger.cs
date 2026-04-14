using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public GoalManager goalManager;
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Ball")) 
        {
            goalManager.ScoreGoal();
        }
    }
}