using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    public float health = 3f;
    public bool isOnFire = false;
    bool isDefeated = false;    

    public enum DamageType
    {
        BallImpact,
        BallImpactCritical,
        BallBounce,
        GotShovedInto,
        FireDamage,
    }

    //Everything That Occurs when an enemy takes damage
    private void TakeDamage(float amountLost, DamageType damageType)
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

    void OnEnable(){
        TakeDamage(0f, DamageType.BallImpact);
        EnemyDamagePublisher.EnemyDamage += OnEnemyDamage;
    }
    void OnDisable(){
        EnemyDamagePublisher.EnemyDamage -= OnEnemyDamage;
    }

    void OnEnemyDamage(GameObject enemyWhoGotHurt, Vector3 balldyseusLocation, DamageType damageType){
        if(gameObject == enemyWhoGotHurt){
            if(damageType == DamageType.BallImpactCritical){
                TakeDamage(2f, damageType);
            }
            else TakeDamage(1f, damageType);
        }
    }
}
