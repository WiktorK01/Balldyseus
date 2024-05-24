using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;
using MoreMountains.Tools;

public class FeedbackEnemyMovementCompletePublisher : MonoBehaviour
{
    public delegate void OnFeedbackEnemyMovementComplete();
    public static event OnFeedbackEnemyMovementComplete FeedbackEnemyMovementComplete;

    public static void NotifyFeedbackEnemyMovementComplete()
    {
        Debug.Log("NOTIFYING OF ENEMY MOVEMENT COMPLETION");
        FeedbackEnemyMovementComplete?.Invoke();
    }

    // Method to be called by Unity Event
    public void OnFeedbackComplete()
    {
        NotifyFeedbackEnemyMovementComplete();
    }
}