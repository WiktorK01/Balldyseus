using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] BallVisuals BallVisuals; 
    [SerializeField] BallCollision BallCollision;
    [SerializeField] BallProperties BallProperties;

    Rigidbody2D rb;

    bool isDragging = false;
    bool isMoving = false;
    bool hasMovedThisTurn = true;

    Vector2 startPoint;
    Vector2 originalVelocity;

    [Header("Drag vars")]
    [SerializeField] float stopThreshold = .1f;
    [SerializeField] float forceMultiplier = 1f;
    [SerializeField] float maxDragDistance = 5f;

    [Header("Velocity vars")]
    [SerializeField] float maxVelocity = 40f;
    [SerializeField] float dampingFactor = 0.95f;

    Coroutine resetHighSpeedCoroutine = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        CheckForPlayerInput();
        HandleDrag();

        BringToStopWhenTooSlow();
        KeepSpeedBelowMaxVelocity();
        CheckForHighSpeed();

        BallVisuals.SetSpriteColor(isShoveMode);
    }

    //this function looks for player input, whether or not the respective mode is gagged, and if the player has moved yet during its turn,
    //then performs the expected methods for those input

    private void CheckForPlayerInput(){
        bool isAttackGagged = BallProperties.isAttackGagged();
        bool isShoveGagged = BallProperties.isShoveGagged();
        bool isShoveMode = BallProperties.isShoveMode();

        if (!hasMovedThisTurn)
        {
            if ((Input.GetMouseButtonDown(0) && !isAttackGagged) || (Input.GetMouseButtonDown(1) && !isShoveGagged))
            {
                startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
                isShoveMode = Input.GetMouseButtonDown(1);

                BallVisuals.SetPullLineRendererState(true, isShoveMode ? Color.blue : Color.red, startPoint);
            }
        }
        else if (isMoving)
        {
            if (Input.GetMouseButtonDown(0) && !isAttackGagged)
                BallProperties.SetAttackMode();
            else if (Input.GetMouseButtonDown(1) && !isShoveGagged)
                BallProperties.SetShoveModeMode();
        }
    }

    private void HandleDrag(){
        if (isDragging)
        {
            Vector3 screenPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            screenPoint.z = 0;
            Vector2 currentPoint = new Vector2(screenPoint.x, screenPoint.y);
            Vector2 dragVector = startPoint - currentPoint;

            if (dragVector.magnitude > maxDragDistance)
            {
                dragVector = dragVector.normalized * maxDragDistance;
                currentPoint = startPoint - dragVector;
            }

            BallVisuals.UpdatePullLineRendererPosition(currentPoint);
            BallVisuals.UpdateTrajectory(startPoint, currentPoint, forceMultiplier);

            // Perform movement only if the released button corresponds to the current mode

            if (isShoveMode && Input.GetMouseButtonUp(1))
            {
                isDragging = false;
                PerformMovement(dragVector);
            }
            else if (!isShoveMode && Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                PerformMovement(dragVector);
            }
        }
    }

    public void PerformMovement(Vector2 dragVector)
    {
        Vector2 force = dragVector * forceMultiplier;
        rb.AddForce(force, ForceMode2D.Impulse);
        isDragging = false;
        isMoving = true;
        hasMovedThisTurn = true;

        DisableLineRenderers();
    }

    private void DisableLineRenderers(){
        BallVisuals.DisablePullLineRenderer();
        BallVisuals.ClearTrajectory();
    }

/*-------------------------------------------------------------------------------------------*/

    private void BringToStopWhenTooSlow(){
        if (isMoving && rb.velocity.magnitude < stopThreshold){
            rb.velocity *= dampingFactor;
        }
    }

    private void KeepSpeedBelowMaxVelocity(){
        if (rb.velocity.magnitude > maxVelocity)
        {
            rb.velocity = rb.velocity.normalized * maxVelocity;
        }
    }

    private void CheckForHighSpeed(){
        if(SpeedGreaterThanThreshold())
        {
            if (resetHighSpeedCoroutine != null)
            {
                StopCoroutine(resetHighSpeedCoroutine);
                resetHighSpeedCoroutine = null;
            }
            Debug.Log("HighSpeed");
            isHighSpeed = true;
        }

        //this will ensure that HighSpeed won't immediately disappear after going below the threshold, giving the player extra time to react 
        if(isHighSpeed && !SpeedGreaterThanThreshold() && resetHighSpeedCoroutine == null)
        {
            resetHighSpeedCoroutine = StartCoroutine(ResetHighSpeedAfterDelay(delayToTurnOffHighSpeed));
        }
    }

    bool SpeedGreaterThanThreshold(){
        return rb.velocity.magnitude > highSpeedThreshold;
    }

    private IEnumerator ResetHighSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isHighSpeed = false;
        resetHighSpeedCoroutine = null; // Reset the coroutine reference
    }

    public void ResetMovement()
    {
        hasMovedThisTurn = false;
    }

    public bool HasStopped()
    {   
        if (rb.velocity.magnitude < .001f && isMoving)
        {
            isMoving = false;
            BallCollision.CollisionEndOfTurnResetters();
            return true;
        }
        return false;
    }

/*--------------------------------------------------------------------------------------------------*/

    public bool IsShoveMode(){
        return isShoveMode;
    }
    public bool IsMoving(){
        return isMoving;
    }
    
    public void MultiplyVelocity(float multiplier){
        originalVelocity = rb.velocity; 
        rb.velocity *= multiplier; 
    }

    public bool IsHighSpeed(){
        return isHighSpeed;
    }

    public Vector2 GetCurrentVelocity(){
        return rb.velocity; 
    }
/*--------------------------------------------------------------------------------------------------*/

    public void GagShove(){
        isShoveGagged = true;
    }
    public void GagAttack(){
        isAttackGagged = true;
    }
    public bool ShoveGagged(){
        return isShoveGagged;
    }

}