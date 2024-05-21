using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePercentPublisher : MonoBehaviour
{
    public delegate void OnForcePercentChange(float newForcePercent);
    public static event OnForcePercentChange ForcePercentChange;

    public static void NotifyForcePercentChange(float newForcePercent)
    {
        ForcePercentChange?.Invoke(newForcePercent);
        Debug.Log("Notifying of new Force Percent: " + newForcePercent);
    }

    /*
    void OnEnable(){
        ForcePercentPublisher.ForcePercentChange += OnForcePercentChange;
    }
    void OnDisable(){
        ForcePercentPublisher.ForcePercentChange -= OnForcePercentChange;
    }

    void OnForcePercentChange(float newForcePercent){
        
    }
    */
}
