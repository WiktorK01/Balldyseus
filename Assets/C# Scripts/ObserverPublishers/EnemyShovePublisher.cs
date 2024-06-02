using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShovePublisher : MonoBehaviour
{
    public delegate void OnEnemyShove(GameObject enemyWhoGotShoved, EnemyMovement.Direction direction, bool canShoveHere);
    public static event OnEnemyShove EnemyShove;

    public static void NotifyEnemyShove(GameObject enemyWhoGotShoved, EnemyMovement.Direction direction, bool canShoveHere)
    {
        EnemyShove?.Invoke(enemyWhoGotShoved, direction, canShoveHere);
    }

    /*
    void OnEnable(){
        EnemyShovePublisher.EnemyShove += OnEnemyShove;
    }
    void OnDisable(){
        EnemyShovePublisher.EnemyShove -= OnEnemyShove;
    }

    void OnEnemyShove(GameObject enemyWhoGotShoved, EnemyShove.Direction direction){

    }
    */
}
