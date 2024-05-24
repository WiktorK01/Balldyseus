using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallCollision : MonoBehaviour
{
    Rigidbody2D rb;

    [SerializeField] private float bounceModeImpulseStrength = 12f;
    private float remainingBounceCount = 5f;
    public static float  referenceBounceCount = 5f;

    bool isMoving = false;
    bool bounceMode = false;
    BallProperties.SpeedState currentSpeedState;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    //handles what different things happen when Balldyseus collides with something while moving or not moving
    void OnCollisionEnter2D(Collision2D collision)
    {
        //currently, nothing happens when balldyseus interacts with an object while not moving
        if (!isMoving) return;

        BallCollisionPublisher.NotifyBallCollision(collision, transform.position, bounceMode, remainingBounceCount, currentSpeedState);
        HandleCollision(collision);
    }

    private void HandleCollision(Collision2D collision)
    {
        if(bounceMode && remainingBounceCount > 0)
        {
            Vector2 collisionNormal = collision.contacts[0].normal;
            rb.AddForce(collisionNormal * bounceModeImpulseStrength, ForceMode2D.Impulse);
            DecrementBounceCount();
        }
    }

    private void DecrementBounceCount()
    {
        if (remainingBounceCount > 0){
            remainingBounceCount--;
            BounceCountPublisher.NotifyBounceCountChange(remainingBounceCount);
        }
    }

    public float GetRemainingShoveCount(){
        return remainingBounceCount;
    }

    private void EndOfTurnResetters(){
        remainingBounceCount = referenceBounceCount;
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
