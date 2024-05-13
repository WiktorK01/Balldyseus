using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    [SerializeField]GameObject SpriteObject;

    SpriteRenderer spriteRenderer;
    BallProperties BallProperties;
    BallCollision BallCollision;
    
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private LineRenderer pullLineRenderer;
    [SerializeField] private GameObject contactPointCirclePrefab; // Reference to the circle prefab

    private GameObject currentContactPointCircle = null; // To keep track of the instantiated prefab

    [SerializeField] GameObject baseModeSprite; 
    [SerializeField] GameObject attackModeSprite; 
    [SerializeField] GameObject shoveModeSprite;

    void Start()
    {
        BallCollision = GetComponent<BallCollision>();
        spriteRenderer = SpriteObject.GetComponent<SpriteRenderer>();
        BallProperties = GetComponent<BallProperties>();

        if (pullLineRenderer == null)
        {
            pullLineRenderer = GetComponent<LineRenderer>();
            pullLineRenderer.positionCount = 2;
            pullLineRenderer.enabled = false;
        }

        if (trajectoryLineRenderer == null)
        {
            trajectoryLineRenderer = GetComponent<LineRenderer>();
            trajectoryLineRenderer.positionCount = 0;
        }
    }

    void Update(){

        SetSpriteColor(BallProperties.ShoveMode);
    }


    //PULL LINE
    /*-------------------------------------------------------------------------*/
    public void SetSpriteColor(bool ShoveMode)
    {
        if (ShoveMode && BallCollision.GetRemainingShoveCount() == 0){
            baseModeSprite.SetActive(true);
            shoveModeSprite.SetActive(false);
            attackModeSprite.SetActive(false);
        }
        else if (ShoveMode){
            shoveModeSprite.SetActive(true);
            baseModeSprite.SetActive(false);
            attackModeSprite.SetActive(false);
        }
        else{
            attackModeSprite.SetActive(true);
            baseModeSprite.SetActive(false);
            shoveModeSprite.SetActive(false);
        }    
    }

    public void SetPullLineRendererState(bool enabled, Color color, Vector2 startPoint)
    {
        pullLineRenderer.enabled = enabled;
        pullLineRenderer.startColor = color;
        pullLineRenderer.endColor = color;
        pullLineRenderer.SetPosition(0, startPoint);
    }

    public void UpdatePullLineRendererPosition(Vector2 position)
    {
        if (pullLineRenderer.enabled)
        {
            pullLineRenderer.SetPosition(1, position);
        }
    }

    public void DisablePullLineRenderer()
    {
        pullLineRenderer.enabled = false;
    }

    
    //TRAJECTORY LINE
    /*-------------------------------------------------------------------------*/
    public LayerMask collisionLayerMask;

    public void UpdateTrajectory(Vector2 start, Vector2 end, float forceMultiplier)
    {
        Vector2 direction = start - end;
        Vector2 force = direction * forceMultiplier;
        int numPoints = 11;
        List<Vector3> points = new List<Vector3>();

        bool hitDetected = false;
        Vector2 lastPosition = transform.position;
        points.Add(lastPosition);

        for (int i = 1; i < numPoints; i++)
        {
            float simulationTime = i * 0.1f;
            Vector2 displacement = force * simulationTime;
            Vector2 currentPosition = (Vector2)transform.position + displacement;

            // Perform the linecast with the specific layer mask
            RaycastHit2D hit = Physics2D.Linecast(lastPosition, currentPosition, collisionLayerMask);

            // Check if the hit object is not null and is not tagged as "Player"
            if (hit.collider != null && !hit.collider.CompareTag("Player"))
            {
                points.Add(hit.point);
                if (currentContactPointCircle != null) Destroy(currentContactPointCircle);
                currentContactPointCircle = Instantiate(contactPointCirclePrefab, hit.point, Quaternion.identity);
                hitDetected = true;
                break;
            }

            if (!hitDetected)
            {
                points.Add(new Vector3(currentPosition.x, currentPosition.y, 0));
            }
            else
            {
                break;
            }

            lastPosition = currentPosition;
        }

        if (!hitDetected && currentContactPointCircle != null)
        {
            Destroy(currentContactPointCircle);
        }

        trajectoryLineRenderer.positionCount = points.Count;
        trajectoryLineRenderer.SetPositions(points.ToArray());
    }

    public void ClearTrajectory()
    {
        trajectoryLineRenderer.positionCount = 0;
        if (currentContactPointCircle != null)
        {
            Destroy(currentContactPointCircle); // Ensure the circle is destroyed when clearing the trajectory
            currentContactPointCircle = null;
        }
    }
    
}