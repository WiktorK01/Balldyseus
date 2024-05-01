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
    [SerializeField] MMF_Player centerSprite;
    [SerializeField] MMF_Player changeModes;
    [SerializeField] MMF_Player floatingBounceNumber;


    void Awake(){
        ballCollision = GetComponent<BallCollision>();
    }

    public void SquashDown(){
        squashDown.Initialization();
        squashDown.PlayFeedbacks();
        CenterSprite();
    }

    public void SquashUp(){
        squashUp.Initialization();
        squashUp.PlayFeedbacks();
        CenterSprite();
    }

    public void SquashLeft(){
        squashLeft.Initialization();
        squashLeft.PlayFeedbacks();
        CenterSprite();
    }

    public void SquashRight(){
        squashRight.Initialization();
        squashRight.PlayFeedbacks();
        CenterSprite();
    }

    public void BigSquashDown(){
        bigSquashDown.Initialization();
        bigSquashDown.PlayFeedbacks();
        CenterSprite();
    }

    public void BigSquashUp(){
        bigSquashUp.Initialization();
        bigSquashUp.PlayFeedbacks();
        CenterSprite();
    }

    public void BigSquashLeft(){
        bigSquashLeft.Initialization();
        bigSquashLeft.PlayFeedbacks();
        CenterSprite();
    }

    public void BigSquashRight(){
        bigSquashRight.Initialization();
        bigSquashRight.PlayFeedbacks();
        CenterSprite();
    }

    public void CenterSprite(){
        centerSprite.Initialization();
        centerSprite.PlayFeedbacks();
    }

    public void ChangeModes(){
        changeModes.Initialization();
        changeModes.PlayFeedbacks();
    }

    public void FloatingBounceNumber(){
        if(TurnManager.Instance.currentState == TurnManager.GameState.PlayerTurn){
            MMF_FloatingText floatingText = floatingBounceNumber.GetFeedbackOfType<MMF_FloatingText>();
            floatingText.Value = ballCollision.GetRemainingShoveCount().ToString();
            floatingBounceNumber.Initialization();
            floatingBounceNumber.PlayFeedbacks();
        }
    }


}