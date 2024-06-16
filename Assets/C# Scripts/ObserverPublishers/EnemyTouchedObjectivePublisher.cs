using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyEnteredObjectivePublisher : MonoBehaviour
{
    public delegate void OnEnemyEnteredObjective(GameObject enemyWhoEnteredObjective);
    public static event OnEnemyEnteredObjective EnemyEnteredObjective;

    public static void NotifyEnemyEnteredObjective(GameObject enemyWhoEnteredObjective)
    {
        Debug.Log("EnemyEnteredObjectivePublisher");
        EnemyEnteredObjective?.Invoke(enemyWhoEnteredObjective);
    }

    /* 
        void OnEnable(){
            EnemyEnteredObjectivePublisher.EnemyEnteredObjective += OnEnemyEnteredObjective;
        }
        void OnDisable(){
            EnemyEnteredObjectivePublisher.EnemyEnteredObjective -= OnEnemyEnteredObjective;
        }

        private void OnEnemyEnteredObjective(GameObject enemyWhoEnteredObjective){
            //do something
        }
    */
}
