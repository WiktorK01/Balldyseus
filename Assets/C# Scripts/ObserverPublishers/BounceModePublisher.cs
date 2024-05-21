using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceModePublisher : MonoBehaviour
{
    public delegate void OnBounceModeChange(bool bounceMode);
    public static event OnBounceModeChange BounceModeChange;

    public static void NotifyBounceModeChange(bool bounceMode)
    {
        BounceModeChange?.Invoke(bounceMode);
    }

    /*
    void OnEnable(){
        BounceModePublisher.BounceModeChange += OnBounceModeChange;
    }
    void OnDisable(){
        BounceModePublisher.BounceModeChange -= OnBounceModeChange;
    }

    void OnBounceModeChange(bool bounceMode){

    }
    */
}
