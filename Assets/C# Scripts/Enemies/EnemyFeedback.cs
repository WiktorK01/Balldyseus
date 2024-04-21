using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyFeedback : MonoBehaviour
{
    [SerializeField] MMF_Player FeedbackHealthTextBounce;

    public void EnemyHealthTextBounce()
    {
        FeedbackHealthTextBounce.PlayFeedbacks();
    }
}
