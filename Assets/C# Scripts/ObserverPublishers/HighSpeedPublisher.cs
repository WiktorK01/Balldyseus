using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeedStatePublisher : MonoBehaviour
{
    public delegate void OnSpeedStateChange(BallProperties.SpeedState newSpeedState);
    public static event OnSpeedStateChange SpeedStateChange;

    public static void NotifySpeedStateChange(BallProperties.SpeedState newSpeedState)
    {
        SpeedStateChange?.Invoke(newSpeedState);
        Debug.Log("NEW SPEED STATE AS:" + newSpeedState.ToString());
    }

    /* 
        void OnEnable(){
            SpeedStatePublisher.SpeedStateChange += OnSpeedStateChange;
        }
        void OnDisable(){
            SpeedStatePublisher.SpeedStateChange -= OnSpeedStateChange;
        }

        private void OnSpeedStateChange(string newSpeedState){
            switch (stateString){
                case "Low":
                    break;

                case "Normal":
                    break;

                case "High":
                    break;
        }
    */
}
