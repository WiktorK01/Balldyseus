using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class BallFeedback : MonoBehaviour
{
    BallCollision ballCollision;

    [SerializeField] MMF_Player squashDown;
    [SerializeField] MMF_Player squashUp;
    [SerializeField] MMF_Player squashLeft;
    [SerializeField] MMF_Player squashRight;
    [SerializeField] MMF_Player bigSquashDown;
    [SerializeField] MMF_Player bigSquashUp;
    [SerializeField] MMF_Player bigSquashLeft;
    [SerializeField] MMF_Player bigSquashRight;
    [SerializeField] MMF_Player changeModes;
    [SerializeField] MMF_Player floatingBounceNumber;

    void Awake(){
        ballCollision = GetComponent<BallCollision>();
    }

    public void SquashDown(){
        squashDown.Initialization();
        squashDown.PlayFeedbacks();
    }

    public void SquashUp(){
        squashUp.Initialization();
        squashUp.PlayFeedbacks();
    }

    public void SquashLeft(){
        squashLeft.Initialization();
        squashLeft.PlayFeedbacks();
    }

    public void SquashRight(){
        squashRight.Initialization();
        squashRight.PlayFeedbacks();
    }

    public void BigSquashDown(){
        bigSquashDown.Initialization();
        bigSquashDown.PlayFeedbacks();
    }

    public void BigSquashUp(){
        bigSquashUp.Initialization();
        bigSquashUp.PlayFeedbacks();
    }

    public void BigSquashLeft(){
        bigSquashLeft.Initialization();
        bigSquashLeft.PlayFeedbacks();
    }

    public void BigSquashRight(){
        bigSquashRight.Initialization();
        bigSquashRight.PlayFeedbacks();
    }

    public void ChangeModes(){
        changeModes.Initialization();
        changeModes.PlayFeedbacks();
    }

    public void FloatingBounceNumber(){
        MMF_FloatingText floatingText = floatingBounceNumber.GetFeedbackOfType<MMF_FloatingText>();
        floatingText.Value = ballCollision.GetRemainingShoveCount().ToString();
        floatingBounceNumber.Initialization();
        floatingBounceNumber.PlayFeedbacks();
    }
}