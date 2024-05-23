using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectiveCollision : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnemyCollider"))
        {
            Debug.Log("ENEMY COLLISION WITH OBJECTIVE DETECTED");
            TurnManager.Instance.OnEnemyReachedObjective();
        }
    }
}
