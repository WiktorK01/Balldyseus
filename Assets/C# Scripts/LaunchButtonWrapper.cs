using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaunchButtonWrapper : MonoBehaviour
{
    public void PerformMovementWrapper()
    {
        GameObject ballGameObject = GameObject.FindGameObjectWithTag("Player");
        Vector2 dragVector = ballGameObject.GetComponent<BallMovement>().GetDragVector();

        ballGameObject.GetComponent<BallMovement>().PerformMovement(dragVector);
    }
}
