using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    private EnemyUI EnemyUI;

    public float health = 3f;
    public float startingImpulse = 2f;
    public float endingImpulse = 5f;
    public bool isThisEnemyTurn;
    bool isOnFire = false;
    bool isDefeated = false;

    [SerializeField]float maxHealth = 3f;
    
    public EnemyUI enemyUI;

    void Start(){
        EnemyUI = gameObject.GetComponent<EnemyUI>();
    }

    //Everything That Occurs when an enemy takes damage
    public void TakeDamage(float amount)
    {
        Debug.Log(health);
        health -= amount;

        if (health <= 0){
            isDefeated = true;
            GetDestroyed();
        }

        else{
            enemyUI.PerformAttackActionsUI();
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
        enemyUI.DestroyUI();
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

}
