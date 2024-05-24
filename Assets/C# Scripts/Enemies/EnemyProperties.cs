using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    public float health = 3f;
    public float startingImpulse = 2f;
    public float endingImpulse = 5f;
    public bool isOnFire = false;
    bool isDefeated = false;    

    public enum DamageType
    {
        BallImpact,
        BallImpactCritical,
        BallBounce,
        FireDamage,
    }

    void OnEnable(){
        TakeDamage(0f, DamageType.BallImpact);
    }

    //Everything That Occurs when an enemy takes damage
    public void TakeDamage(float amountLost, DamageType damageType)
    {
        health -= amountLost;
        if (health <= 0) isDefeated = true;
        EnemyHealthChangePublisher.NotifyEnemyHealthChange(gameObject, health, amountLost, damageType);
    }

/*------------------------------------------------------------------*/
    //Fire State Stuff
    public bool GetCurrentFireState(){
        return isOnFire;
    }
    
    public void SetOnFire(bool onFire){
        isOnFire = onFire;
    }
    
    public void ApplyFireDamageIfOnFire()
    {
        if (isOnFire)
        {
            TakeDamage(Fire.fireDamage, DamageType.FireDamage);
        }
    }
/*------------------------------------------------------------------*/
    public bool IsDefeated(){
        return isDefeated;
    }

/****************OBSERVERS*************/

//this is for if I want a bool that checks if if it's the enemy's turn at the moment. not sure i need this atm, wont delete yet bc we'll see
    /*void OnEnable(){
        EnemyTurnPublisher.EnemyTurnChange += OnEnemyTurnChange;
        GameStatePublisher.GameStateChange += OnGameStateChange;
    }
    void OnDisable(){
        EnemyTurnPublisher.EnemyTurnChange -= OnEnemyTurnChange;
        GameStatePublisher.GameStateChange -= OnGameStateChange;
    }

    void OnEnemyTurnChange(GameObject enemyWhoseTurnItIs){
        if(gameObject == enemyWhoseTurnItIs){
            isMyTurn = true;
        }
        else isMyTurn = false;
    }

    void OnGameStateChange(TurnManager.GameState newGameState){
        isMyTurn = false;
    }*/
}
