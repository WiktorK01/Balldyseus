using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenWipeObserver : MonoBehaviour
{
    public void NotifyScreenWipeComplete()
    {
        Debug.Log("Screen Wipe Complete");
        ScreenWipeCompletePublisher.NotifyScreenWipeComplete();
    }
}
