using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoldierController : MonoBehaviour
{
    private Animator animator;
    private EnemyMovement enemyMovement;

    void Start()
    {
        animator = GetComponent<Animator>();
        enemyMovement = GetComponentInParent<EnemyMovement>();

        if (enemyMovement.moveMoney < 2){
            animator.Play("SlowSoldier");
        }
        else if (enemyMovement.moveMoney > 2){
            animator.Play("FastSoldier");
        }
        else{
            animator.Play("RegularSoldier");
        }
    }
    //for now this will work, but if we ever add a mechanic that alters an enemy's moveMoney in the middle of battle, this will need an observer
}
