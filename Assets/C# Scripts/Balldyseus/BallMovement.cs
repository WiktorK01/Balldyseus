using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    [SerializeField] private BallVisuals BallVisuals; 
    [SerializeField] private BallCollision BallCollision;

    private Rigidbody2D rb;

    private bool isDragging = false;
    private bool isMoving = false;
    private bool hasMovedThisTurn = true;
    private bool isShoveMode = false;
    private bool isHighSpeed;

    private bool isShoveGagged = false;
    private bool isAttackGagged = false;

    private Vector2 startPoint;
    private Vector2 originalVelocity;

    [Header("Drag vars")]
    [SerializeField] private float stopThreshold = .1f;
    [SerializeField] private float forceMultiplier = 1f;
    [SerializeField] private float maxDragDistance = 5f;

    [Header("Velocity vars")]
    [SerializeField] private float maxVelocity = 40f;
    [SerializeField] private float dampingFactor = 0.95f;

    [Header("HighSpeed vars")]
    [SerializeField] private float highSpeedThreshold = 10f;
    [SerializeField] private float delayToTurnOffHighSpeed = 0.7f;

    private Coroutine resetHighSpeedCoroutine = null;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
//Line
    void Update()
    {
        CheckForPlayerInput();
        HandleDrag();
        BringToStopWhenTooSlow();
        KeepSpeedBelowMaxVelocity();
        CheckForHighSpeed();
    }

    private void CheckForPlayerInput(){
        if (!hasMovedThisTurn)
        {
            if ((Input.GetMouseButtonDown(0) && !isAttackGagged) || (Input.GetMouseButtonDown(1) && !isShoveGagged))
            {
                startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                isDragging = true;
                isShoveMode = Input.GetMouseButtonDown(1);

                BallVisuals.SetSpriteColor(isShoveMode); 
                BallVisuals.SetPullLineRendererState(true, isShoveMode ? Color.blue : Color.red, startPoint);
            }
        }
        else if (isMoving)
        {
            if (Input.GetMouseButtonDown(0) && !isAttackGagged)
                isShoveMode = false;
            else if (Input.GetMouseButtonDown(1) && !isShoveGagged)
                isShoveMode = true;

            BallVisuals.SetSpriteColor(isShoveMode);
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
                PerformMovement(dragVector);
            }
            else if (!isShoveMode && Input.GetMouseButtonUp(0))
            {
                PerformMovement(dragVector);
            }
        }
        else if (!isMoving){
            BallVisuals.ClearTrajectory(); 
        }
    }
    
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

    private void PerformMovement(Vector2 dragVector)
    {
        Vector2 force = dragVector * forceMultiplier;
        rb.AddForce(force, ForceMode2D.Impulse);
        isDragging = false;
        isMoving = true;
        hasMovedThisTurn = true;
        BallVisuals.DisablePullLineRenderer();
        BallVisuals.ClearTrajectory();
    }

    private IEnumerator ResetHighSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        isHighSpeed = false;
        resetHighSpeedCoroutine = null; // Reset the coroutine reference
    }

    bool SpeedGreaterThanThreshold(){
        return rb.velocity.magnitude > highSpeedThreshold;
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

