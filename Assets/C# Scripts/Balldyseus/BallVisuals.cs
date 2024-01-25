using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallVisuals : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private LineRenderer trajectoryLineRenderer;
    [SerializeField] private LineRenderer lineRenderer;

    private Color attackModeColor = Color.red; 
    private Color shoveModeColor = Color.blue;

    void Start()
    {
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
            lineRenderer.positionCount = 2;
            lineRenderer.enabled = false;
        }

        if (trajectoryLineRenderer == null)
        {
            trajectoryLineRenderer = GetComponent<LineRenderer>();
            trajectoryLineRenderer.positionCount = 0;
        }
    }

    public void SetSpriteColor(bool isShoveMode)
    {
        if (isShoveMode)
        {
            spriteRenderer.color = shoveModeColor;
        }
        else
        {
            spriteRenderer.color = attackModeColor;
        }    
    }

    public void SetLineRendererState(bool enabled, Color color, Vector2 startPoint)
    {
        lineRenderer.enabled = enabled;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
        lineRenderer.SetPosition(0, startPoint);
    }

    public void UpdateTrajectory(Vector2 start, Vector2 end, float forceMultiplier)
    {
        Vector2 direction = start - end;
        Vector2 force = direction * forceMultiplier;
        int numPoints = 11; 
        trajectoryLineRenderer.positionCount = numPoints;

        for (int i = 0; i < numPoints; i++)
        {
            float simulationTime = i * 0.1f;
            Vector2 displacement = force * simulationTime;
            Vector3 drawPoint = transform.position + new Vector3(displacement.x, displacement.y, 0); // Correctly add Vector2 to Vector3
            trajectoryLineRenderer.SetPosition(i, drawPoint);
        }
    }

    public void ClearTrajectory()
    {
        trajectoryLineRenderer.positionCount = 0;
    }

    public void UpdateLineRendererPosition(Vector2 position)
    {
        if (lineRenderer.enabled)
        {
            lineRenderer.SetPosition(1, position);
        }
    }

    public void DisableLineRenderer()
    {
        lineRenderer.enabled = false;
    }
}