using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;

public class EnemyFeedback : MonoBehaviour
{
    private MMF_Player FeedbackHealthTextBounce;

    void Start(){
        FeedbackHealthTextBounce = gameObject.transform.Find("FeedbackHealthTextBounce").GetComponent<MMF_Player>();
    }

    public void EnemyHealthTextBounce()
    {
        FeedbackHealthTextBounce.PlayFeedbacks();
    }
}
