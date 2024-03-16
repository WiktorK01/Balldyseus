using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallMovement : MonoBehaviour
{
    BallVisuals BallVisuals; 
    BallCollision BallCollision;
    BallProperties BallProperties;
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

    Vector2 dragVector;

    private float forcePercentage;

    void Start()
    {
        BallVisuals = GetComponent<BallVisuals>(); 
        BallCollision = GetComponent<BallCollision>();
        BallProperties = GetComponent<BallProperties>();

        rb = GetComponent<Rigidbody2D>();

    }

    void Update()
    {
        if(BallExists()){
            CheckForPlayerInput();
            HandleDrag();

            BringToStopWhenTooSlow();
            KeepSpeedBelowMaxVelocity();
            CheckForHighSpeed();
        }
    }

    //this function looks for player input, whether or not the respective mode is gagged, and if the player has moved yet during its turn,
    //then performs the expected methods for those input

    private void CheckForPlayerInput(){

        if(!hasMovedThisTurn||isMoving){
            if (Input.GetMouseButtonDown(0))
                BallProperties.SetAttackMode();

            else if (Input.GetMouseButtonDown(1))
                BallProperties.SetShoveMode();
        }

        if (!hasMovedThisTurn)
        {
            if (IsMouseOverBalldyseus())
            {
                if (Input.GetMouseButtonDown(0) && !BallProperties.AttackGagged || Input.GetMouseButtonDown(1) && !BallProperties.ShoveGagged)
                {
                    isDragging = true;
                    startPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                    BallVisuals.SetPullLineRendererState(true, BallProperties.ShoveMode ? Color.blue : Color.red, startPoint);
                }
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

            // Perform movement only if the released button corresponds to the current mode
            if (BallProperties.ShoveMode && Input.GetMouseButtonUp(1))
            {
                Debug.Log(forcePercentage);
                isDragging = false;
                BallVisuals.DisablePullLineRenderer();
                //PerformMovement(dragVector);
            }
            else if (!BallProperties.ShoveMode && Input.GetMouseButtonUp(0))
            {
                Debug.Log(forcePercentage);
                isDragging = false;
                BallVisuals.DisablePullLineRenderer();
                //PerformMovement(dragVector);
            }
        }
    }

    public void PerformMovementButtonWrapper()
    {
        PerformMovement(dragVector);
    }

    public void PerformMovement(Vector2 dragVector)
    {
        rb.WakeUp();
        Debug.Log($"Attempting to perform movement with vector: {dragVector}");
        Vector2 force = dragVector * forceMultiplier;
        rb.AddForce(force, ForceMode2D.Impulse);
        Debug.Log($"Force applied: {force}");

        DisableLineRenderers();

        isDragging = false;
        isMoving = true;
        hasMovedThisTurn = true;

        this.dragVector = Vector2.zero;
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

            BallProperties.EnableHighSpeed();
        }

        //this will ensure that HighSpeed won't immediately disappear after going below the threshold, giving the player extra time to react 
        if(BallProperties.HighSpeed && !SpeedGreaterThanThreshold() && resetHighSpeedCoroutine == null)
        {
            resetHighSpeedCoroutine = StartCoroutine(ResetHighSpeedAfterDelay(BallProperties.delayToTurnOffHighSpeed));
        }
    }

    bool SpeedGreaterThanThreshold(){
        return rb.velocity.magnitude > BallProperties.highSpeedThreshold;
    }

    private IEnumerator ResetHighSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        BallProperties.DisableHighSpeed();
        resetHighSpeedCoroutine = null; // Reset the coroutine reference
    }

    public void ResetMovement()
    {
        hasMovedThisTurn = false;
        isMoving = false; 
        isDragging = false; 
        rb.velocity = Vector2.zero; 
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

    private bool IsMouseOverBalldyseus()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePosition, Vector2.zero);

        return hit.collider != null && hit.collider.gameObject == gameObject;
    }

/*--------------------------------------------------------------------------------------------------*/

    public bool IsMoving(){
        return isMoving;
    }
    
    public void MultiplyVelocity(float multiplier){
        originalVelocity = rb.velocity; 
        rb.velocity *= multiplier; 
    }

    public Vector2 GetCurrentVelocity(){
        return rb.velocity; 
    }

    public float GetForcePercentage()
    {
        return forcePercentage;
    }

    public Vector2 GetDragVector(){
        return dragVector;
    }

    public bool BallExists(){
        if(!gameObject.activeSelf || gameObject==null)
            return false;
        else return true;
    }
}