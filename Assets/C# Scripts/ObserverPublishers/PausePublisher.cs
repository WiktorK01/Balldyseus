using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PausePublisher : MonoBehaviour
{
    public delegate void OnPauseChange(bool isGamePaused);
    public static event OnPauseChange PauseChange;

    public static void NotifyPauseChange(bool isGamePaused)
    {
        PauseChange?.Invoke(isGamePaused);
    }

    /* 
        void OnEnable(){\
            PausePublisher.PauseChange += OnPauseChange;
        }
        void OnDisable(){
            PausePublisher.PauseChange -= OnPauseChange;
        }

        private void OnPauseChange(bool isGamePaused){
            if (isGamePaused){
                //do something
            }
            else{
                //do something else
            }
        }
    */
}
