using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndEnemyTurnPublisher : MonoBehaviour
{
    public delegate void OnEndEnemyTurn(GameObject enemyObject);
    public static event OnEndEnemyTurn EndEnemyTurn;

    public static void NotifyEndEnemyTurn(GameObject enemyObject)
    {
        EndEnemyTurn?.Invoke(enemyObject);
    }
    /*
    void OnEnable(){
        EndEnemyTurnPublisher.EndEnemyTurn += OnEndEnemyTurn;
    }
    void OnDisable(){
        EndEnemyTurnPublisher.EndEnemyTurn -= OnEndEnemyTurn;
    }

    void OnEndEnemyTurn(GameObject enemyWhoseTurnEnded){
    }*/
}
