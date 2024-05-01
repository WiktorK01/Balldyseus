using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Feedbacks;


public class LaunchUIFeedback : MonoBehaviour
{


    bool firstTurn = true;
    bool launchButtonClicked = false;

    //subscribing our OnGameStateChanged() function to the GameStateEventPublisher which tracks when the gamestate changes in TurnManager
    void OnEnable(){
        GameStateEventPublisher.GameStateChanged += OnGameStateChanged;
    }
    void OnDisable(){
        GameStateEventPublisher.GameStateChanged -= OnGameStateChanged;
    }

    //since it's subscribed to GameStateEventPublisher, this will run everytime a GameState changes
    private void OnGameStateChanged(TurnManager.GameState newState){
        if(newState == TurnManager.GameState.PlayerTurn){
            if(!firstTurn)EnemyTurnExit();
            launchButtonClicked = false; 
            ButtonEntrance();
            PercentageEntrance();
            PlayerTurnEnter();
            PercentageScale();
        }
        else if(newState == TurnManager.GameState.EnemyTurn){
            firstTurn = false;
            PlayerTurnExit();
            EnemyTurnEnter();
        }
    }

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

    //most of these are for updating force percentage values
    MMF_Scale percentageScaleFeedback;
    MMF_TMPText percentageTextFeedback;
    MMF_RotationShake percentageRotationFeedback;
    BallMovement ballMovement;
    MMF_SquashAndStretch percentageExitSquash;
    float forcePercentage;
    float scaledForcePercentage;
    float startingZeroForPercentExit = 0f;

    void Start(){
        ballMovement = FindObjectOfType<BallMovement>();
        percentageScaleFeedback = percentageScale.GetFeedbackOfType<MMF_Scale>();
        percentageTextFeedback = percentageScale.GetFeedbackOfType<MMF_TMPText>();
        percentageRotationFeedback = percentageScale.GetFeedbackOfType<MMF_RotationShake>();
        percentageExitSquash = percentageExit.GetFeedbackOfType<MMF_SquashAndStretch>();
    }

    void Update(){
        //updating force percentage values. this would probably work better with an observer, dont care for now tho
        forcePercentage = ballMovement.GetForcePercentage();
        scaledForcePercentage = forcePercentage * 0.01f + 1;

        percentageScaleFeedback.DestinationScale = new Vector3 (scaledForcePercentage, scaledForcePercentage, 0);
        percentageTextFeedback.NewText = forcePercentage.ToString() + "%";
        percentageRotationFeedback.ShakeSpeed = .25f * forcePercentage;
        percentageRotationFeedback.ShakeRange = .15f * forcePercentage;

        percentageExitSquash.RemapCurveZero = scaledForcePercentage;
        percentageExitSquash.RemapCurveOne = scaledForcePercentage + .6f;
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

    //We have the launch button clicked check to ensure that the force percentage does not correctly go it's set destination for 
    //hover-enter or hover-exit after the button gets clicked, preventing it's exit animation from running correctly
    public void OnClick(){
        launchButtonClicked = true;
    }
}