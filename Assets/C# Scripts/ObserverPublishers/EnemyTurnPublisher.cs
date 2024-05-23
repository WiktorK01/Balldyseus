using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurnPublisher : MonoBehaviour
{
    public delegate void OnEnemyTurnChange(GameObject enemyObject);
    public static event OnEnemyTurnChange EnemyTurnChange;

    public static void NotifyEnemyTurnChange(GameObject enemyObject)
    {
        EnemyTurnChange?.Invoke(enemyObject);
        Debug.Log("New Turn for: " + enemyObject.ToString());
    }
    /*
    void OnEnable(){
        EnemyTurnPublisher.EnemyTurnChange += OnEnemyTurnChange;
    }
    void OnDisable(){
        EnemyTurnPublisher.EnemyTurnChange -= OnEnemyTurnChange;
    }

    void OnEnemyTurnChange(GameObject enemyWhoseTurnItIs){
    }*/
}
