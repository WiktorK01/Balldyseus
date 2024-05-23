using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class BallFeedback : MonoBehaviour
{
    CameraFeedback cameraFeedback;

    void Start(){
        cameraFeedback = FindObjectOfType<CameraFeedback>();
    }

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
        cameraFeedback.CameraShakeVertical();
    }

    public void BigSquashUp(){
        bigSquashUp.Initialization();
        bigSquashUp.PlayFeedbacks();
        cameraFeedback.CameraShakeVertical();
    }

    public void BigSquashLeft(){
        bigSquashLeft.Initialization();
        bigSquashLeft.PlayFeedbacks();
        cameraFeedback.CameraShakeHorizontal();
    }

    public void BigSquashRight(){
        bigSquashRight.Initialization();
        bigSquashRight.PlayFeedbacks();
        cameraFeedback.CameraShakeHorizontal();
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
    BallProperties.SpeedState currentSpeedState = BallProperties.SpeedState.Low;
    
    void OnEnable(){
        BounceCountPublisher.BounceCountChange += OnBounceCountChange;
        BounceModePublisher.BounceModeChange += OnBounceModeChange;
        SpeedStatePublisher.SpeedStateChange += OnSpeedStateChange;
        BallCollisionPublisher.BallCollision += OnBallCollision;
    }
    void OnDisable(){
        BounceCountPublisher.BounceCountChange -= OnBounceCountChange;
        BounceModePublisher.BounceModeChange -= OnBounceModeChange;
        SpeedStatePublisher.SpeedStateChange -= OnSpeedStateChange;
        BallCollisionPublisher.BallCollision -= OnBallCollision;
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
        currentSpeedState = newSpeedState;

        if(newSpeedState == BallProperties.SpeedState.High){
            HighSpeedMode();
            Debug.Log("BEGINNING BALL FEEDBACK HIGH SPEED STATE");
        } 
        else StopHighSpeedMode();
    }
    bool HighSpeed(){
        return currentSpeedState == BallProperties.SpeedState.High;
    }
    bool LowSpeed(){
        return currentSpeedState == BallProperties.SpeedState.Low;
    }

    void OnBallCollision(Collision2D collision, Vector2 myPosition, bool bounceMode, float remainingBounceCount, BallProperties.SpeedState currentSpeedState){
        Vector2 collisionPosition = collision.contacts[0].point;
        Vector2 direction = myPosition - collisionPosition;

        if(LowSpeed()) return;

        bool highSpeed = HighSpeed();

        if(Mathf.Abs(direction.x) > Mathf.Abs(direction.y)){
            if(direction.x > 0){
                if(highSpeed) BigSquashLeft();
                else SquashLeft();
            }
            else{
                if(highSpeed) BigSquashRight();
                else SquashRight();
            }
        }
        else{
            if(direction.y > 0){
                if(highSpeed) BigSquashDown();
                else SquashDown();
            }
            else{
                if(highSpeed) BigSquashUp();
                else SquashUp();
            }
        }
    }
    
}

