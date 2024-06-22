using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
//MOVEMENT STATE MACHINE******************************************
    public enum MovementState
    {
        Null,
        HasNotMoved,
        IsMoving,
        HasCompletedMovement
    }
    public MovementState currentMovementState = MovementState.HasNotMoved;

    public void ChangeMovementState(MovementState newState){
        if (newState == currentMovementState) return;
        
        switch (newState){
            case MovementState.HasNotMoved:
                currentMovementState = MovementState.HasNotMoved;
                break;

            case MovementState.IsMoving:
                currentMovementState = MovementState.IsMoving;
                break;

            case MovementState.HasCompletedMovement:
                currentMovementState = MovementState.HasCompletedMovement;
                break;
        }

        MovementStatePublisher.NotifyMovementStateChange(newState);
    }

//**********************************************************************



//**********************************************************************

    BallVisuals BallVisuals; 
    BallProperties BallProperties;
    Rigidbody2D rb;

    Vector2 startPoint;
    Vector2 originalVelocity; //this stores the previous velocity from before MultiplyVelocity(), useless rn but may be needed later

    [Header("Drag vars")]
    [SerializeField] float stopThreshold = .1f;
    [SerializeField] float forceMultiplier = 1f;
    [SerializeField] float maxDragDistance = 5f;

    [Header("Velocity vars")]
    [SerializeField] float maxVelocity = 40f;
    [SerializeField] float dampingFactor = 0.95f;

    Vector2 dragVector;
    private float forcePercentage;

    bool isDragging = false;
    bool hasLaunchedThisRound = true;

    void Awake()
    {
        BallVisuals = GetComponent<BallVisuals>(); 
        BallProperties = GetComponent<BallProperties>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckForPlayerInput();
        HandleDrag();

        BringToStopWhenTooSlow();
        KeepSpeedBelowMaxVelocity();

        CheckIfBallStoppedAfterMoving();
    }

    //this function looks for player input, whether or not the respective mode is gagged, and if the player has moved yet during its turn,
    //then performs the expected methods for those input

    private void CheckForPlayerInput(){

        if(currentMovementState != MovementState.HasCompletedMovement){
            if (Input.GetMouseButtonDown(0))
                BallProperties.SetAttackMode();

            else if (Input.GetMouseButtonDown(1))
                BallProperties.SetBounceMode();
        }

        if (!hasLaunchedThisRound)
        {
            if (IsMouseOverBalldyseus() && (Input.GetMouseButtonDown(0) && !BallProperties.attackGagged || Input.GetMouseButtonDown(1) && !BallProperties.bounceGagged)){
                isDragging = true;
                startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                BallVisuals.SetPullLineRendererState(true, BallProperties.bounceMode ? Color.blue : Color.red, startPoint);
            }
        }
    }

    private void HandleDrag(){
        if (isDragging)
        {
            Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPoint.z = 0;
            Vector2 currentPoint = new Vector2(screenPoint.x, screenPoint.y);
            dragVector = startPoint - currentPoint;

            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
                
            }
            currentPoint = startPoint - dragVector;

            forcePercentage = Mathf.Round(Mathf.Clamp01(dragVector.magnitude / maxDragDistance) * 100f);

            BallVisuals.UpdatePullLineRendererPosition(currentPoint);
            BallVisuals.UpdateTrajectory(startPoint, currentPoint, forceMultiplier);

            if (BallProperties.bounceMode && Input.GetMouseButtonUp(1))
            {
                isDragging = false;
                BallVisuals.DisablePullLineRenderer();
            }
            else if (!BallProperties.bounceMode && Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                BallVisuals.DisablePullLineRenderer();
            }

            //OBSERVER NOTIFIER OF FORCE PERCENT CHANGE
            ForcePercentPublisher.NotifyForcePercentChange(forcePercentage);
        }
    }

    public void PerformMovementButtonWrapper()
    {
        PerformMovement(dragVector);
    }

    public void PerformMovement(Vector2 dragVector)
    {
        rb.WakeUp();
        Vector2 force = dragVector * forceMultiplier;
        rb.AddForce(force, ForceMode2D.Impulse);

        DisableLineRenderers();

        isDragging = false;
        ChangeMovementState(MovementState.IsMoving);
        hasLaunchedThisRound = true;

        this.dragVector = Vector2.zero;
    }

    private void DisableLineRenderers(){
        BallVisuals.DisablePullLineRenderer();
        BallVisuals.ClearTrajectory();
    }

/*-------------------------------------------------------------------------------------------*/

    private void BringToStopWhenTooSlow(){
        if (currentMovementState == MovementState.IsMoving && rb.velocity.magnitude < stopThreshold){
            rb.velocity *= dampingFactor;
        }
    }

    private void KeepSpeedBelowMaxVelocity(){
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    private void ResetMovement()
    {
        ChangeMovementState(MovementState.HasNotMoved);
        hasLaunchedThisRound = false;
        isDragging = false; 
        rb.velocity = Vector2.zero;

    }

    private void CheckIfBallStoppedAfterMoving()
    {   
        if (rb.velocity.magnitude < .001f && currentMovementState == MovementState.IsMoving)
        {
            ChangeMovementState(MovementState.HasCompletedMovement);
        }
    }

    private bool IsMouseOverBalldyseus()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

/*--------------------------------------------------------------------------------------------------*/

    //multiplies Balldyseus' velocity by a certain amount. used for obstacles like Fire
    public void MultiplyVelocity(float multiplier){
        originalVelocity = rb.velocity; 
        rb.velocity *= multiplier; 
    }


/*--------------------------------------------------------------------------------------------------*/

    /*public Vector2 GetCurrentVelocity(){
        return rb.velocity; 
    }*/

    public Vector2 GetDragVector(){
        return dragVector;
    }
    
    private void ResetForcePercentage(){
        forcePercentage = 0;
        ForcePercentPublisher.NotifyForcePercentChange(forcePercentage);
    }

/*-----------------------------------OBSERVERS---------------------------------------------------------------*/


    void OnEnable(){
        GameStatePublisher.GameStateChange += OnGameStateChange;
    }
    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(TurnManager.GameState newState){
        if(newState == TurnManager.GameState.PlayerTurn){
            ResetForcePercentage();
            ResetMovement();
        }

        else if(newState == TurnManager.GameState.EnemyTurn){

        }
    }
}