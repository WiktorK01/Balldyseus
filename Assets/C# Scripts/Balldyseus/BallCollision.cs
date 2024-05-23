using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    BallFeedback ballFeedback;
    Rigidbody2D rb;

    CameraFeedback cameraFeedback;
    [SerializeField] private float shoveModeImpulseStrength = 12f;
    private float remainingBounceCount = 5f;
    public static float  referenceBounceCount = 5f;

    bool isMoving = false;
    bool bounceMode = false;
    BallProperties.SpeedState currentSpeedState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ballFeedback = GetComponent<BallFeedback>();
        cameraFeedback = FindObjectOfType<CameraFeedback>();
    }

    //handles what different things happen when Balldyseus collides with something while moving or not moving
    void OnCollisionEnter2D(Collision2D collision)
    {
        //currently, nothing happens when balldyseus interacts with an object while not moving
        if (!isMoving) return;

        if (collision.gameObject.CompareTag("EnemyCollider"))
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
        EnemyMovement enemyMovement = collision.gameObject.GetComponentInParent<EnemyMovement>();

        if (enemyMovement == null) return;

        if (bounceMode && remainingBounceCount > 0)
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
        else if (!bounceMode)
        {
            EnemyProperties enemy = collision.gameObject.GetComponentInParent<EnemyProperties>();
            HandleEnemyDamageFeedback(collision);

            if(HighSpeed())enemy.TakeDamage(2f, EnemyProperties.DamageType.BallImpactCritical);
            else enemy.TakeDamage(1f, EnemyProperties.DamageType.BallImpact);
        }  
    }

    private void HandleWallCollision(Collision2D collision)
    {
        if(bounceMode && remainingBounceCount > 0)
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

        if(LowSpeed()) return;

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
            if(direction.x > 0){
                if(HighSpeed()){
                    ballFeedback.BigSquashLeft();
                    cameraFeedback.CameraShakeHorizontal();
                }
                else 
                    ballFeedback.SquashLeft();
            }
            else{
                if(HighSpeed()){
                    ballFeedback.BigSquashRight();
                    cameraFeedback.CameraShakeHorizontal();
                }
                else 
                    ballFeedback.SquashRight();
            }
        }
        else{
            if(direction.y > 0){
                if(HighSpeed()){
                    ballFeedback.BigSquashDown();
                    cameraFeedback.CameraShakeVertical();
                }
                else 
                    ballFeedback.SquashDown();
            }
            else{
                if(HighSpeed()){
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

        EnemyFeedback enemyFeedback = enemy.GetComponentInParent<EnemyFeedback>();

        Vector2 direction = myPosition - enemyPosition;

        if(LowSpeed()) return;

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
        if (remainingBounceCount > 0){
            remainingBounceCount--;
            Debug.Log("New Bounce Count is " + remainingBounceCount);
            BounceCountPublisher.NotifyBounceCountChange(remainingBounceCount);
        }

        if (remainingBounceCount == 0) BounceCountPublisher.NotifyBounceCountChange(remainingBounceCount);
    }

    public float GetRemainingShoveCount(){
        return remainingBounceCount;
    }

    private void EndOfTurnResetters(){
        remainingBounceCount = referenceBounceCount;
        BounceCountPublisher.NotifyBounceCountChange(referenceBounceCount);
    }

    bool HighSpeed(){
        return currentSpeedState == BallProperties.SpeedState.High;
    }
    bool LowSpeed(){
        return currentSpeedState == BallProperties.SpeedState.Low;
    }

//*****************************OBSERVERS******************************************
    void OnEnable(){
        GameStatePublisher.GameStateChange += OnGameStateChange;
        MovementStatePublisher.MovementStateChange += OnMovementStateChange;
        SpeedStatePublisher.SpeedStateChange += OnSpeedStateChange;
        BounceModePublisher.BounceModeChange += OnBounceModeChange;
    }
    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
        MovementStatePublisher.MovementStateChange -= OnMovementStateChange;
        SpeedStatePublisher.SpeedStateChange -= OnSpeedStateChange;
        BounceModePublisher.BounceModeChange -= OnBounceModeChange;
    }

    void OnGameStateChange(TurnManager.GameState newState){
        if(newState == TurnManager.GameState.PlayerTurn){
            BounceCountPublisher.NotifyBounceCountChange(remainingBounceCount);
            ResolveCollisionsWithEnemies();
        }
        if (newState == TurnManager.GameState.EnemyTurn) {
            EndOfTurnResetters();
        }
    }

    private void OnMovementStateChange(BallMovement.MovementState newState)
    {
        if(newState == BallMovement.MovementState.IsMoving) isMoving = true;
        else isMoving = false;
    }

    void OnBounceModeChange(bool newBounceModeSetting){
        bounceMode = newBounceModeSetting;
    }

    void OnSpeedStateChange(BallProperties.SpeedState newSpeedState){
        currentSpeedState = newSpeedState;
    }

    private void ResolveCollisionsWithEnemies()
    {
        var balldyseusCollider = GetComponent<Collider2D>();
        var hitColliders = Physics2D.OverlapCircleAll(transform.position, balldyseusCollider.bounds.extents.x);

        foreach (var hitCollider in hitColliders)
        {
            if (hitCollider.gameObject.CompareTag("Enemy"))
            {
                Vector3 directionToEnemy = hitCollider.transform.position - transform.position;
                transform.position -= directionToEnemy.normalized * 0.05f;
                break;
            }
        }
    }
}
