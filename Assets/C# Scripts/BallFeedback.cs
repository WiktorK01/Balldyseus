using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class BallFeedback : MonoBehaviour
{
    [SerializeField] MMF_Player squashDown;
    [SerializeField] MMF_Player squashUp;
    [SerializeField] MMF_Player squashLeft;
    [SerializeField] MMF_Player squashRight;
    [SerializeField] MMF_Player bigSquashDown;
    [SerializeField] MMF_Player bigSquashUp;
    [SerializeField] MMF_Player bigSquashLeft;
    [SerializeField] MMF_Player bigSquashRight;
    [SerializeField] MMF_Player changeModes;
    [SerializeField] MMF_Player floatingBounceNumberBlue;
    [SerializeField] MMF_Player floatingBounceNumberGrey;
    [SerializeField] MMF_Player highSpeedMode;

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

    public void FloatingBounceNumberBlue(){
        floatingBounceNumberBlue.Initialization();
        floatingBounceNumberBlue.PlayFeedbacks();
    }

    public void FloatingBounceNumberGrey(){
        floatingBounceNumberGrey.Initialization();
        floatingBounceNumberGrey.PlayFeedbacks();
    }

    public void HighSpeedMode(){
        highSpeedMode.Initialization();
        highSpeedMode.PlayFeedbacks();
    }
    public void StopHighSpeedMode(){
        highSpeedMode.StopFeedbacks();
    }


//*********************************OBSERVERS****************************************
    void OnEnable(){
        BounceCountPublisher.BounceCountChange += OnBounceCountChange;
        BounceModePublisher.BounceModeChange += OnBounceModeChange;
        SpeedStatePublisher.SpeedStateChange += OnSpeedStateChange;
    }
    void OnDisable(){
        BounceCountPublisher.BounceCountChange -= OnBounceCountChange;
        BounceModePublisher.BounceModeChange -= OnBounceModeChange;
        SpeedStatePublisher.SpeedStateChange -= OnSpeedStateChange;
    }

    void OnBounceCountChange(float newBounceCount){
        if(TurnManager.Instance.currentState == TurnManager.GameState.PlayerTurn && newBounceCount != BallCollision.referenceBounceCount){
            if(newBounceCount == 0)
            {
                MMF_FloatingText floatingText = floatingBounceNumberGrey.GetFeedbackOfType<MMF_FloatingText>();
                floatingText.Value = newBounceCount.ToString();
                FloatingBounceNumberGrey();
            }
            else
            {
                MMF_FloatingText floatingText = floatingBounceNumberBlue.GetFeedbackOfType<MMF_FloatingText>();
                floatingText.Value = newBounceCount.ToString();
                FloatingBounceNumberBlue();
            }
        }
    }

    void OnBounceModeChange(bool bounceMode){
        ChangeModes();
    }

    void OnSpeedStateChange(BallProperties.SpeedState newSpeedState){
        if(newSpeedState == BallProperties.SpeedState.High){
            HighSpeedMode();
            Debug.Log("BEGINNING BALL FEEDBACK HIGH SPEED STATE");
        } 
        else StopHighSpeedMode();
    }

    
}

