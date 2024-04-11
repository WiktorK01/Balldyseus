using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    private EnemyUI EnemyUI;

    private EnemyFeedback EnemyFeedback;

    public float health = 3f;
    public float startingImpulse = 2f;
    public float endingImpulse = 5f;
    public bool isThisEnemyTurn;
    bool isOnFire = false;
    bool isDefeated = false;

    [SerializeField]float maxHealth = 3f;
    

    void Start(){
        EnemyUI = gameObject.GetComponent<EnemyUI>();
        EnemyFeedback = gameObject.GetComponent<EnemyFeedback>();
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
            EnemyFeedback.EnemyHealthTextBounce();
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

}
