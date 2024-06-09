using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AchillesTurnRoutine : MonoBehaviour
{
    EnemyMovement enemyMovement;
    EnemyProperties enemyProperties;

    //In case future enemies may have their own unique turn routine, i have decoupled this from the turn manager
    IEnumerator TurnRoutine(){
        if (enemyProperties.isOnFire)
        {
            enemyProperties.ApplyFireDamageIfOnFire();
            yield return new WaitForSeconds(.4f); //this should be changed, I think I should avoid directly waiting for seconds
        }
        

        if (enemyProperties.IsDefeated()){
            EndEnemyTurnPublisher.NotifyEndEnemyTurn(gameObject);
            yield break;
        } 

        if (gameObject != null)
        {
            enemyMovement.Move();
            yield return new WaitUntil(() => enemyMovement.HasMoved());

            TurnManager.Instance.UpdateEnemyLocations();
            EndEnemyTurnPublisher.NotifyEndEnemyTurn(gameObject);
        }
    }

//-----------OBSERVERS---------------------------

    void OnEnable(){
        enemyMovement = GetComponent<EnemyMovement>();
        enemyProperties = GetComponent<EnemyProperties>();
        EnemyTurnPublisher.EnemyTurnChange += OnEnemyTurnChange;
    }
    void OnDisable(){
        EnemyTurnPublisher.EnemyTurnChange -= OnEnemyTurnChange;
    }

    void OnEnemyTurnChange(GameObject enemyWhoseTurnItIs){
        if(gameObject == enemyWhoseTurnItIs){
            StartCoroutine(TurnRoutine());
        }
    }
}
