using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    private EnemyUI EnemyUI;
    private EnemyFeedback enemyFeedback;
    GameObject balldyseus;

    public float health = 3f;
    public float startingImpulse = 2f;
    public float endingImpulse = 5f;
    public bool isThisEnemyTurn;
    public bool isOnFire = false;
    bool isDefeated = false;    

    void Start(){
        balldyseus = GameObject.Find("Balldyseus");
        EnemyUI = gameObject.GetComponent<EnemyUI>();
        enemyFeedback = gameObject.GetComponent<EnemyFeedback>();
    }

    //Everything That Occurs when an enemy takes damage
    public void TakeDamage(float amount)
    {
        Debug.Log(health);
        health -= amount;

        //causes the fire effect when fire damage taken on enemy turn
        if(isOnFire && TurnManager.Instance.currentState != TurnManager.GameState.PlayerTurn){
            if(health == 0){
                enemyFeedback.DeathFire();
            }
            enemyFeedback.DamageFire();
        }
            
        if (health <= 0){
            isDefeated = true;
            enemyFeedback.DeathFlush();
        }

        if(amount == 1f){
            enemyFeedback.HealthTextBounce();
        }
        else if(amount==2f){
            enemyFeedback.BigHealthTextBounce();
        }
    }

/*------------------------------------------------------------------*/
    //Fire State Stuff
    public bool GetCurrentFireState(){
        return isOnFire;
    }
    
    public void SetOnFire(bool onFire){
        isOnFire = onFire;
    }
/*------------------------------------------------------------------*/
    void GetDestroyed(){
        Destroy(gameObject);
    }

    public bool IsDefeated(){
        return isDefeated;
    }
    
    public void ThisEnemyTurnBegins(){
        isThisEnemyTurn = true;
    }

    public void ThisEnemyTurnEnds(){
        isThisEnemyTurn = false;
    }

    private Vector2 GetDirectionToBalldyseus(){
        if (balldyseus == null){
            Debug.LogError("Balldyseus Not Found!");
            return Vector2.zero;
        }

        Vector2 balldyseusPosition = balldyseus.transform.position;
        Vector2 directionToBalldyseus = (balldyseusPosition - (Vector2)transform.position).normalized;

        return directionToBalldyseus;
    }

}
