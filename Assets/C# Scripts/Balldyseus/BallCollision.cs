using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    BallProperties BallProperties;
    BallMovement BallMovement;
    Rigidbody2D rb;

    [SerializeField] private float shoveModeImpulseStrength = 12f;

    [SerializeField] private float remainingShoveCount = 5f;
    [SerializeField] private float referenceShoveCount = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        BallMovement = GetComponent<BallMovement>();
        BallProperties = GetComponent<BallProperties>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        bool isMoving = BallMovement.IsMoving();

        if (!isMoving) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision);
        }

        else if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision(collision);
        }
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        bool ShoveMode = BallProperties.ShoveMode;
        bool HighSpeed = BallProperties.HighSpeed;
        EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();

        if (enemyMovement == null) return;

        if (ShoveMode && remainingShoveCount > 0)
        {
            Vector2 contactPoint = collision.contacts[0].point;
            Vector2 center = collision.collider.bounds.center;
             
            //Shoving code
            float deltaX = Mathf.Abs(contactPoint.x - center.x);
            float deltaY = Mathf.Abs(contactPoint.y - center.y);
            
            if (deltaX > deltaY) {
                if (contactPoint.x > center.x)
                    enemyMovement.Shove(EnemyMovement.Direction.Left);
                else
                    enemyMovement.Shove(EnemyMovement.Direction.Right);
            }
            else {
                if (contactPoint.y > center.y)
                    enemyMovement.Shove(EnemyMovement.Direction.Down);
                else
                    enemyMovement.Shove(EnemyMovement.Direction.Up);
            }

            Vector2 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * shoveModeImpulseStrength, ForceMode2D.Impulse);

            DecrementShoveCount();

        }
        //IN ATTACK MODE
        else if (!ShoveMode)
        {
            EnemyProperties enemy = collision.gameObject.GetComponent<EnemyProperties>();
            if(HighSpeed){
                enemy.TakeDamage(2f);
            }
            else{
                enemy.TakeDamage(1f);
            }
        }  
    }

    private void HandleWallCollision(Collision2D collision)
    {
        if(BallProperties.ShoveMode && remainingShoveCount > 0)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * shoveModeImpulseStrength, ForceMode2D.Impulse);
            DecrementShoveCount();
        }
    }

    private void DecrementShoveCount()
    {
        if (remainingShoveCount > 0)
            remainingShoveCount--;
    }


    public float GetRemainingShoveCount(){
        return remainingShoveCount;
    }

    public void CollisionEndOfTurnResetters(){
        remainingShoveCount = referenceShoveCount;
    }
}
