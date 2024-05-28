using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveMoneyPublisher : MonoBehaviour
{
    public delegate void OnEnemyMoveMoneyChange(GameObject enemyWhoLostMoveMoney, float newMoveMoneyCount);
    public static event OnEnemyMoveMoneyChange EnemyMoveMoneyChange;

    public static void NotifyEnemyMoveMoneyChange(GameObject enemyWhoLostMoveMoney, float newMoveMoneyCount){
        EnemyMoveMoneyChange?.Invoke(enemyWhoLostMoveMoney, newMoveMoneyCount);
    }

    /*
    void OnEnable(){
        EnemyMoveMoneyPublisher.EnemyMoveMoneyChange += OnEnemyMoveMoneyChange;
    }

    void OnDisable(){
        EnemyMoveMoneyPublisher.EnemyMoveMoneyChange -= OnEnemyMoveMoneyChange;
    }

    void OnEnemyMoveMoneyChange(GameObject enemyWhoLostMoveMoney, float newMoveMoneyCount){
    }
    */
}
