using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    BallProperties BallProperties;
    
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private LineRenderer pullLineRenderer;
    [SerializeField] private GameObject contactPointCirclePrefab; // Reference to the circle prefab

    private GameObject currentContactPointCircle = null; // To keep track of the instantiated prefab


    private Color attackModeColor = Color.red; 
    private Color shoveModeColor = Color.blue;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
        if (ShoveMode)
        {
            spriteRenderer.color = shoveModeColor;
        }
        else
        {
            spriteRenderer.color = attackModeColor;
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
    public void UpdateTrajectory(Vector2 start, Vector2 end, float forceMultiplier)
    {
        Vector2 direction = start - end;
        Vector2 force = direction * forceMultiplier;
        int numPoints = 11;
        List<Vector3> points = new List<Vector3>();

        bool hitDetected = false;
        Vector2 lastPosition = transform.position; // Start from the GameObject's current position

        points.Add(lastPosition); // Ensure the first point is the GameObject's position

        for (int i = 1; i < numPoints; i++) // Start loop from 1 since the first point is already added
        {
            float simulationTime = i * 0.1f;
            Vector2 displacement = force * simulationTime;
            Vector2 currentPosition = (Vector2)transform.position + displacement;
            RaycastHit2D hit = Physics2D.Linecast(lastPosition, currentPosition);

            if (hit.collider != null && !hit.collider.CompareTag("Player") && !hit.collider.CompareTag("Objective") && !hit.collider.CompareTag("PassableObstacle"))
            {
                points.Add(hit.point);
                if (currentContactPointCircle != null) Destroy(currentContactPointCircle); // Destroy existing circle if any
                currentContactPointCircle = Instantiate(contactPointCirclePrefab, hit.point, Quaternion.identity); // Instantiate new circle at hit point
                hitDetected = true;
                break;
            }

            points.Add(new Vector3(currentPosition.x, currentPosition.y, 0));
            lastPosition = currentPosition;
        }

        if (!hitDetected && currentContactPointCircle != null)
        {
            Destroy(currentContactPointCircle); // Destroy the circle if no collision is detected
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