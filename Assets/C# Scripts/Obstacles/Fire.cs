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
            BallMovement ballMovement = collision.gameObject.GetComponentInParent<BallMovement>();
            ballMovement.MultiplyVelocity(velocityMultiplier);
        }
        if (collision.gameObject.CompareTag("EnemyCollider"))
        {
            EnemyProperties enemy = collision.gameObject.GetComponentInParent<EnemyProperties>();
            enemy.SetOnFire(true);
        }
    }

}
