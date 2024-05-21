using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementStatePublisher : MonoBehaviour
{
    public delegate void OnMovementStateChange(BallMovement.MovementState newMovementState);
    public static event OnMovementStateChange MovementStateChange;

    public static void NotifyMovementStateChange(BallMovement.MovementState newMovementState)
    {
        MovementStateChange?.Invoke(newMovementState);
        Debug.Log("NEW MOVEMENT STATE AS: " + newMovementState);
    }

    /*     
        void OnEnable(){
            MovementStatePublisher.MovementStateChange += OnMovementStateChange;
        }
        void OnDisable(){
            MovementStatePublisher.MovementStateChange -= OnMovementStateChange;
        }

        private void OnMovementStateChange(string newMovementState){
            switch (stateString){
                case "Null":
                    break;

                case "HasNotMoved":
                    break;

                case "IsMoving":
                    break;

                case "HasCompletedMovement":
                    break;
        }
    */
}
