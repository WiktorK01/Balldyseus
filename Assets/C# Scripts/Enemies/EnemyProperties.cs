using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProperties : MonoBehaviour
{
    public float health = 3f;
    public float startingImpulse = 2f;
    public float endingImpulse = 5f;
    bool isOnFire = false;
    bool isDefeated = false;

    float currentImpulse;
    [SerializeField]float maxHealth = 3f;
    
    public EnemyUI enemyUI;

    void Start(){
        currentImpulse = startingImpulse;
    }

    public void TakeDamage(float amount)
    {
        Debug.Log(health);
        health -= amount;

        if (health <= 0){
            isDefeated = true;
            GetDestroyed();
        }

        else{
            UpdateImpulse();
        }
    }

    void UpdateImpulse(){
        float healthRatio = (health - 1) / (maxHealth - 1); 
        currentImpulse = Mathf.Lerp(startingImpulse, endingImpulse, 1 - healthRatio);
        Debug.Log("Current Impulse: " + currentImpulse);
    }


    void GetDestroyed(){
        // Add destruction logic here
        enemyUI.DestroyUI();
        Destroy(gameObject);
    }

    public float GetCurrentImpulse(){
        return currentImpulse;
    }

    public bool IsDefeated(){
        return isDefeated;
    }

/*------------------------------------------------------------------*/
    //Fire State Stuff
    public bool GetCurrentFireState(){
        return isOnFire;
    }
    
    public void SetOnFire(bool onFire)
    {
        isOnFire = onFire;
    }
/*------------------------------------------------------------------*/

}
