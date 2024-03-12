using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchButtonWrapper : MonoBehaviour
{
    public void PerformMovementWrapper()
    {
        GameObject ballGameObject = GameObject.FindGameObjectWithTag("Player");

        /*if (ballGameObject != null)
        {
            //Rigidbody2D rb = ballGameObject.GetComponent<Rigidbody2D>();
        }

        else
        {
            Debug.LogError("BallMovement: Failed to find the ball game object. Make sure it's tagged correctly.");
        }*/
        Vector2 dragVector = ballGameObject.GetComponent<BallMovement>().GetDragVector();

        ballGameObject.GetComponent<BallMovement>().PerformMovement(dragVector);
    }
}
