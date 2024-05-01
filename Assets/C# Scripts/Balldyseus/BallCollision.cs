using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    BallProperties ballProperties;
    BallMovement ballMovement;
    BallFeedback ballFeedback;
    Rigidbody2D rb;

    CameraFeedback cameraFeedback;

    [SerializeField] private float shoveModeImpulseStrength = 12f;

    [SerializeField] private float remainingShoveCount = 5f;
    [SerializeField] private float referenceShoveCount = 5f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballMovement = GetComponent<BallMovement>();
        ballProperties = GetComponent<BallProperties>();
        ballFeedback = GetComponent<BallFeedback>();

        cameraFeedback = FindObjectOfType<CameraFeedback>();
    }

    //handles what different things happen when Balldyseus collides with something while moving or not moving
    void OnCollisionEnter2D(Collision2D collision)
    {
        bool isMoving = ballMovement.IsMoving();

        //currently, nothing happens when balldyseus interacts with an object while not moving
        if (!isMoving) return;

        if (collision.gameObject.CompareTag("Enemy"))
        {
            HandleBallSquashFeedback(collision);
            HandleEnemyCollision(collision);
        }

        else if (collision.gameObject.CompareTag("Wall"))
        {
            HandleBallSquashFeedback(collision);
            HandleWallCollision(collision);
        }
    }

    private void HandleEnemyCollision(Collision2D collision)
    {
        bool ShoveMode = ballProperties.ShoveMode;
        bool HighSpeed = ballProperties.HighSpeed;
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
            HandleEnemyDamageFeedback(collision);

            if(HighSpeed)enemy.TakeDamage(2f);
            else enemy.TakeDamage(1f);
        }  
    }

    private void HandleWallCollision(Collision2D collision)
    {
        if(ballProperties.ShoveMode && remainingShoveCount > 0)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * shoveModeImpulseStrength, ForceMode2D.Impulse);
            DecrementShoveCount();
        }
    }

    //gets the point of contact, checks it's relative direction to Balldyseus, then does the appropriate squash effect
    private void HandleBallSquashFeedback(Collision2D collision){
        Vector2 myPosition = transform.position;
        Vector2 collisionPosition = collision.contacts[0].point;

        Vector2 direction = myPosition - collisionPosition;

        if(ballProperties.LowSpeed) return;

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
            if(direction.x > 0){
                if(ballProperties.HighSpeed){
                    ballFeedback.BigSquashLeft();
                    cameraFeedback.CameraShakeHorizontal();
                }
                else 
                    ballFeedback.SquashLeft();
            }
            else{
                if(ballProperties.HighSpeed){
                    ballFeedback.BigSquashRight();
                    cameraFeedback.CameraShakeHorizontal();
                }
                else 
                    ballFeedback.SquashRight();
            }
        }
        else{
            if(direction.y > 0){
                if(ballProperties.HighSpeed){
                    ballFeedback.BigSquashDown();
                    cameraFeedback.CameraShakeVertical();
                }
                else 
                    ballFeedback.SquashDown();
            }
            else{
                if(ballProperties.HighSpeed){
                    ballFeedback.BigSquashUp();
                    cameraFeedback.CameraShakeVertical();
                }
                else 
                    ballFeedback.SquashUp();
            }
        }
    }

    //performs the enemy's damage feedback when balldyseus hits them
    private void HandleEnemyDamageFeedback(Collision2D collision){
        GameObject enemy = collision.gameObject;
        Vector2 myPosition = transform.position;
        Vector2 enemyPosition = enemy.transform.position;

        EnemyFeedback enemyFeedback = enemy.GetComponent<EnemyFeedback>();

        Vector2 direction = myPosition - enemyPosition;

        if(ballProperties.LowSpeed) return;

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
            if(direction.x > 0){
                enemyFeedback.DamageRight();
            }
            else{
                enemyFeedback.DamageLeft();
            }
        }
        else{
            if(direction.y > 0){
                enemyFeedback.DamageUp();
            }
            else{
                enemyFeedback.DamageDown();
            }
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
