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


    void Start()
    {
        BallCollision = FindObjectOfType<BallCollision>();
        BallMovement = FindObjectOfType<BallMovement>();
        BallProperties = FindObjectOfType<BallProperties>();
    }

    void Update()
    {

        //ShoveMode = BallProperties.ShoveMode;
        if(BallProperties !=null){
            //Remove the Impulse Count UI when the shove is gagged
            if(TurnManager.Instance.currentState == TurnManager.GameState.PlayerTurn){
                if(BallProperties.ShoveGagged){
                    UIManager.Instance.HideUIElement("ImpulseCountUI");
                }
                //edit the Impulse Count
                else{
                    float remainingImpulses = BallCollision.GetRemainingShoveCount();
                    string impulseCountString = remainingImpulses.ToString();
                    UIManager.Instance.SetTextValueInUIElement("ImpulseCountUI", "ImpulseCountNumber", impulseCountString);
                }
            }


            //HighSpeed Instantiator
            if (BallProperties.HighSpeed && BallMovement.IsMoving())
            {
                UIManager.Instance.ShowUIElement("HighSpeedUI");
            }
            else
            {
                UIManager.Instance.HideUIElement("HighSpeedUI");
            }
        }

        if(BallMovement != null){
            //set the changing forcePercentage number on the PlayerTurn for the LaunchUI
            if(TurnManager.Instance.currentState == TurnManager.GameState.PlayerTurn){
                float forcePercentageNumber = BallMovement.GetForcePercentage();
                string forcePercentageText = forcePercentageNumber.ToString() + '%';
                UIManager.Instance.SetTextValueInUIElement("LaunchUI", "ForcePercentageNumber", forcePercentageText);
            }
        }

        //edits the winning text to display the total amount of turns needed to win
        if(TurnManager.Instance.currentState == TurnManager.GameState.Win){
            float currentTurnNumber = TurnManager.Instance.GetTurnNumber();   
            string winText = "It took you " + currentTurnNumber + " turns!";
            UIManager.Instance.SetTextValueInUIElement("WinUI", "TurnNumberText", winText);
        }
    }
}
