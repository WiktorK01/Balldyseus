using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class LaunchUIFeedback : MonoBehaviour
{
    [SerializeField] MMF_Player buttonEntrance;
    [SerializeField] MMF_Player buttonExit;
    [SerializeField] MMF_Player buttonHover;
    [SerializeField] MMF_Player buttonHoverExit;
    [SerializeField] MMF_Player percentageEntrance;
    [SerializeField] MMF_Player percentageExit;
    [SerializeField] MMF_Player percentageScale;
    [SerializeField] MMF_Player playerTurnEnter;
    [SerializeField] MMF_Player playerTurnExit;
    [SerializeField] MMF_Player enemyTurnEnter;
    [SerializeField] MMF_Player enemyTurnExit;
    [SerializeField] MMF_Player bounceCountNumberBounce;
    [SerializeField] MMF_Player bounceCountNumberTextChange;
    [SerializeField] MMF_Player bounceCountNumberEntrance;
    [SerializeField] MMF_Player bounceCountNumberExit;
    [SerializeField] MMF_Player bounceCountTextEntrance;
    [SerializeField] MMF_Player bounceCountTextExit;
    [SerializeField] MMF_Player bounceCountColorToGrey;
    [SerializeField] MMF_Player bounceCountColorToBlue;


    //most of these are for updating force percentage values
    MMF_Scale percentageScaleFeedback;
    MMF_TMPText percentageTextValue;
    MMF_RotationShake percentageRotationFeedback;
    MMF_SquashAndStretch percentageExitSquash;


    void Awake(){
        percentageScaleFeedback = percentageScale.GetFeedbackOfType<MMF_Scale>();
        percentageTextValue = percentageScale.GetFeedbackOfType<MMF_TMPText>();
        percentageRotationFeedback = percentageScale.GetFeedbackOfType<MMF_RotationShake>();
        percentageExitSquash = percentageExit.GetFeedbackOfType<MMF_SquashAndStretch>();
    }

    public void ButtonEntrance(){
        buttonEntrance.Initialization();
        buttonEntrance.PlayFeedbacks();
    }
    public void ButtonExit(){
        buttonExit.Initialization();
        buttonExit.PlayFeedbacks();
    }

    public void ButtonHover(){
        if(!launchButtonClicked){
            buttonHover.Initialization();
            buttonHover.PlayFeedbacks();
        }
    }
    public void ButtonHoverExit(){
        buttonHover.StopFeedbacks();

        if(!launchButtonClicked){
            buttonHoverExit.Initialization();
            buttonHoverExit.PlayFeedbacks();
        }
    }
    
    public void PercentageEntrance(){
        percentageEntrance.Initialization();

        percentageEntrance.Initialization();
        percentageEntrance.PlayFeedbacks();
    }

    public void PercentageExit(){
        percentageExit.Initialization();
        percentageExit.PlayFeedbacks();
    }
    public void PercentageScale(){
        percentageScale.Initialization();
        percentageScale.PlayFeedbacks();
    }

    public void PlayerTurnEnter(){
        playerTurnEnter.Initialization();
        playerTurnEnter.PlayFeedbacks();
    }
    public void PlayerTurnExit(){
        playerTurnExit.Initialization();
        playerTurnExit.PlayFeedbacks();
    }

    public void EnemyTurnEnter(){
        enemyTurnEnter.Initialization();
        enemyTurnEnter.PlayFeedbacks();
    }
    public void EnemyTurnExit(){
        enemyTurnExit.Initialization();
        enemyTurnExit.PlayFeedbacks();
    }
    
    public void BounceCountNumberBounce(){
        bounceCountNumberBounce.Initialization();
        bounceCountNumberBounce.PlayFeedbacks();
    }
    public void BounceCountNumberTextChange(){
        bounceCountNumberTextChange.Initialization();
        bounceCountNumberTextChange.PlayFeedbacks();
    }
    public void BounceCountNumberEntrance(){
        bounceCountNumberEntrance.Initialization();
        bounceCountNumberEntrance.PlayFeedbacks();
    }
    public void BounceCountNumberExit(){
        bounceCountNumberExit.Initialization();
        bounceCountNumberExit.PlayFeedbacks();
    }
    public void BounceCountTextEntrance(){
        bounceCountTextEntrance.Initialization();
        bounceCountTextEntrance.PlayFeedbacks();
    }
    public void BounceCountTextExit(){
        bounceCountTextExit.Initialization();
        bounceCountTextExit.PlayFeedbacks();
    }
    public void BounceCountColorToGrey(){
        bounceCountColorToGrey.Initialization();
        bounceCountColorToGrey.PlayFeedbacks();
    }
    public void BounceCountColorToBlue(){
        bounceCountColorToBlue.Initialization();
        bounceCountColorToBlue.PlayFeedbacks();
    }

    //We have the launch button clicked check to ensure that the force percentage does not correctly go it's set destination for 
    //hover-enter or hover-exit after the button gets clicked, preventing it's exit animation from running correctly
    public void OnClick(){
        launchButtonClicked = true;
    }

    //******************************OBSERVERS*************************************************************


    bool firstTurn = true;
    bool launchButtonClicked = false;


    //subscribing our OnGameStateChanged() function to the GameStateEventPublisher which tracks when the gamestate changes in TurnManager
    void OnEnable(){
        Debug.Log("LaunchUIFeedback enabled");
        GameStatePublisher.GameStateChange += OnGameStateChange;
        BounceCountPublisher.BounceCountChange += OnBounceCountChange;
        ForcePercentPublisher.ForcePercentChange += OnForcePercentChange;
    }
    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
        BounceCountPublisher.BounceCountChange -= OnBounceCountChange;
        ForcePercentPublisher.ForcePercentChange -= OnForcePercentChange;
    }

    //GameState Observer
    private void OnGameStateChange(TurnManager.GameState newState){
        if(newState == TurnManager.GameState.PlayerTurn){
            if(!firstTurn)EnemyTurnExit();
            launchButtonClicked = false; 
            ButtonEntrance();
            PercentageEntrance();
            PlayerTurnEnter();
            PercentageScale();
            BounceCountNumberEntrance();
            BounceCountTextEntrance();
        }

        else if(newState == TurnManager.GameState.EnemyTurn){
            firstTurn = false;
            PlayerTurnExit();
            EnemyTurnEnter();
            BounceCountNumberExit();
            BounceCountTextExit();
        }
    }

    //Bounce Count Observer
    private void OnBounceCountChange(float newBounceCount){
        MMF_TMPText bounceCountNumberValue = bounceCountNumberTextChange.GetFeedbackOfType<MMF_TMPText>();
        bounceCountNumberValue.NewText = newBounceCount.ToString();
        BounceCountNumberTextChange();
        
        if(newBounceCount!=5)BounceCountNumberBounce();
        if(newBounceCount==0) BounceCountColorToGrey();
        else BounceCountColorToBlue();
    }

    private void OnForcePercentChange(float newForcePercent){
        float scaledForcePercent = newForcePercent * 0.01f + 1;

        //percent feedback assignments happened in Start()
        percentageScaleFeedback.DestinationScale = new Vector3 (scaledForcePercent, scaledForcePercent, 0);
        percentageTextValue.NewText = newForcePercent.ToString() + "%";

        percentageRotationFeedback.ShakeSpeed = .25f * newForcePercent;
        percentageRotationFeedback.ShakeRange = .15f * newForcePercent;

        percentageExitSquash.RemapCurveZero = scaledForcePercent;
        percentageExitSquash.RemapCurveOne = scaledForcePercent + .6f;
    }

}