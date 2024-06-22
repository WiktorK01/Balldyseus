using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWipeCompletePublisher : MonoBehaviour
{
    public delegate void OnScreenWipeComplete();
    public static event OnScreenWipeComplete ScreenWipeComplete;

    public static void NotifyScreenWipeComplete()
    {
        ScreenWipeComplete?.Invoke();
    }

    /* 
    void OnEnable(){\
        ScreenWipeCompletePublisher.ScreenWipeComplete += OnScreenWipeComplete;
    }


    void OnDisable(){
        ScreenWipeCompletePublisher.ScreenWipeComplete -= OnScreenWipeComplete;
    }

    private void OnScreenWipeComplete()
    {
        //do something
    }
    */
}
