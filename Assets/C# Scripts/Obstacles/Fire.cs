using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public float velocityMultiplier = 0.5f; 
    public static float fireDamage = 1f; 

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            BallMovement ballMovement = collision.gameObject.GetComponent<BallMovement>();
            ballMovement.MultiplyVelocity(velocityMultiplier);
        }
        if (collision.gameObject.CompareTag("Enemy"))
        {
            EnemyProperties enemy = collision.gameObject.GetComponent<EnemyProperties>();
            enemy.SetOnFire(true);
        }
    }

    public static void ApplyFireDamageIfOnFire(EnemyProperties enemy)
    {
        if (enemy.GetCurrentFireState())
        {
            enemy.TakeDamage(fireDamage);
        }
        //this is called in "IEnumerator EnemyTurnRoutine()" in the TurnManager
    }
}
