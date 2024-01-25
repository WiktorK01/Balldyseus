using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    [SerializeField] private BallMovement ballMovement;
    private Rigidbody2D rb;

    [SerializeField] private float shoveModeAgainstWallImpulseStrength = 12f;

    [SerializeField] private float remainingShoveCount = 5f;
    [SerializeField] private float referenceShoveCount = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballMovement = GetComponent<BallMovement>();
        if (ballMovement == null)
        {
            Debug.LogError("BallMovement script not found on " + gameObject.name);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        bool isShoveMode = ballMovement.IsShoveMode();
        bool isMoving = ballMovement.IsMoving();
        bool isHighSpeed = ballMovement.IsHighSpeed();

        if (!isMoving) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleEnemyCollision(collision, isShoveMode, isHighSpeed);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            HandleWallCollision(collision, isShoveMode);
        }
    }

    private void HandleEnemyCollision(Collision2D collision, bool isShoveMode, bool isHighSpeed)
    {
        EnemyMovement enemyMovement = collision.gameObject.GetComponent<EnemyMovement>();
        if (enemyMovement == null) return;

        if (isShoveMode && remainingShoveCount>0)
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

            //impulse code
            EnemyProperties enemy = collision.gameObject.GetComponent<EnemyProperties>();

            Vector2 impulseDirection = (transform.position - collision.transform.position).normalized;
            float impulseStrength = enemy.GetCurrentImpulse();
            rb.AddForce(impulseDirection * impulseStrength, ForceMode2D.Impulse);

            DecrementShoveCount();

        }
        //IN ATTACK MODE
        else if (!isShoveMode)
        {
            EnemyProperties enemy = collision.gameObject.GetComponent<EnemyProperties>();
            if(isHighSpeed){
                enemy.TakeDamage(2f);
            }
            else{
                enemy.TakeDamage(1f);
            }
        }  
    }

    private void HandleWallCollision(Collision2D collision, bool isShoveMode)
    {
        if(isShoveMode && remainingShoveCount > 0)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * shoveModeAgainstWallImpulseStrength, ForceMode2D.Impulse);
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
