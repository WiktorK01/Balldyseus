using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BounceCountPublisher : MonoBehaviour
{
    public delegate void OnBounceCountChange(float newBounceCount);
    public static event OnBounceCountChange BounceCountChange;

    public static void NotifyBounceCountChange(float newBounceCount)
    {
        BounceCountChange?.Invoke(newBounceCount);
    }

    /*
    void OnEnable(){
        BounceCountPublisher.BounceCountChange += OnBounceCountChange;
    }
    void OnDisable(){
        BounceCountPublisher.BounceCountChange -= OnBounceCountChange;
    }

    void OnBounceCountChange(float newBounceCount){

    }
    */
}
