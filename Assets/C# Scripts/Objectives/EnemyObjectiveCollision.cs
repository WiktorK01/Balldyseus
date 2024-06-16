using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObjectiveCollision : MonoBehaviour
{
    [SerializeField]Collider2D myColliderObject;
    bool objectiveEntered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Objective"))
        {
            objectiveEntered = true;
        }
    }

    public void CheckIfObjectiveEntered()
    {
        if (objectiveEntered)
        {
            EnemyEnteredObjectivePublisher.NotifyEnemyEnteredObjective(gameObject);
        }
    }
}
