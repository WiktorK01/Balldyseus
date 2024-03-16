using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.UI;

public class DynamicUIManager : MonoBehaviour
{
    //This Manager controls UI elements that change dynamically based on the current state of the game
    //This generally does not control instantiation of UI Canvases.

    BallCollision BallCollision;
    BallMovement BallMovement;
    BallProperties BallProperties;
    TurnManager turnManager;

    //bool ShoveMode;

    void Start()
    {
        BallCollision = FindObjectOfType<BallCollision>();
        BallMovement = FindObjectOfType<BallMovement>();
        BallProperties = FindObjectOfType<BallProperties>();
        turnManager = FindObjectOfType<TurnManager>();
    }

    void Update()
    {

        //ShoveMode = BallProperties.ShoveMode;
        if(BallProperties !=null){
            //Remove the Impulse Count UI when the shove is gagged
            if(turnManager.currentState == TurnManager.GameState.PlayerTurn){
                if(BallProperties.ShoveGagged){
                    UIManager2.Instance.HideUIElement("ImpulseCountUI");
                }
                //edit the Impulse Count
                else{
                    float remainingImpulses = BallCollision.GetRemainingShoveCount();
                    string impulseCountString = remainingImpulses.ToString();
                    UIManager2.Instance.SetTextValueInUIElement("ImpulseCountUI", "ImpulseCountNumber", impulseCountString);
                }
            }


            //HighSpeed Instantiator
            if (BallProperties.HighSpeed && BallMovement.IsMoving())
            {
                UIManager2.Instance.ShowUIElement("HighSpeedUI");
            }
            else
            {
                UIManager2.Instance.HideUIElement("HighSpeedUI");
            }
        }

        if(BallMovement != null){
            //Hides the LaunchUI while the player is moving on their turn
            if(turnManager.currentState == TurnManager.GameState.PlayerTurn && BallMovement.IsMoving() == false){
                UIManager2.Instance.ShowUIElement("LaunchUI");
            }
            else{
                UIManager2.Instance.HideUIElement("LaunchUI");
            }
            
            //set the changing forcePercentage number on the PlayerTurn for the LaunchUI
            if(turnManager.currentState == TurnManager.GameState.PlayerTurn){
                float forcePercentageNumber = BallMovement.GetForcePercentage();
                string forcePercentageText = forcePercentageNumber.ToString() + '%';
                UIManager2.Instance.SetTextValueInUIElement("LaunchUI", "ForcePercentageNumber", forcePercentageText);
            }
        }



        //edits the winning text to display the total amount of turns needed to win
        if(turnManager.currentState == TurnManager.GameState.Win){
            float currentTurnNumber = turnManager.GetTurnNumber();   
            string winText = "It took you " + currentTurnNumber + " turns!";
            UIManager2.Instance.SetTextValueInUIElement("WinUI", "TurnNumberText", winText);
        }




        /*if (speedNumberText != null)
        {
            float currentSpeed = BallMovement.GetCurrentVelocity().magnitude;
            speedNumberText.text = currentSpeed.ToString("F2");
        }*/
    }
}
