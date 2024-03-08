using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Text impulseCountText;
    public Text impulseCountNumber;
    public Text speedNumberText;
    public Image highSpeedImage;
    public Text TurnNumberText;
    public Button launchingUI;
    public Text forcePercentageText;

    BallCollision BallCollision;
    BallMovement BallMovement;
    BallProperties BallProperties;

    TurnManager turnManager;

    private Color grayColor = Color.gray;

    bool ShoveMode = false;

    void Start()
    {
        BallCollision = FindObjectOfType<BallCollision>();
        BallMovement = FindObjectOfType<BallMovement>();
        BallProperties = FindObjectOfType<BallProperties>();
        turnManager = FindObjectOfType<TurnManager>();

        // Turn off the highSpeedImage at the start
        if (highSpeedImage != null)
        {
            highSpeedImage.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (BallProperties != null)
        {
            ShoveMode = BallProperties.ShoveMode;

            if (impulseCountNumber != null)
            {
                if(BallProperties.ShoveGagged){
                    impulseCountNumber.gameObject.SetActive(false);
                    impulseCountText.gameObject.SetActive(false);
                }
                else{
                    float remainingImpulses = BallCollision.GetRemainingShoveCount();
                    impulseCountNumber.text = remainingImpulses.ToString();
                    if(remainingImpulses == 0){
                        impulseCountNumber.color = grayColor;
                    }
                }

            }

            if (speedNumberText != null)
            {
                float currentSpeed = BallMovement.GetCurrentVelocity().magnitude;
                speedNumberText.text = currentSpeed.ToString("F2");
            }

            if (BallProperties.HighSpeed)
            {
                if (highSpeedImage != null && !highSpeedImage.gameObject.activeSelf)
                {
                    highSpeedImage.gameObject.SetActive(true);
                }
            }
            else
            {
                if (highSpeedImage != null && highSpeedImage.gameObject.activeSelf)
                {
                    highSpeedImage.gameObject.SetActive(false);
                }
            }
        }

        if(turnManager.currentState == TurnManager.GameState.PlayerTurn && BallMovement.IsMoving() == false){
            launchingUI.gameObject.SetActive(true);
        }
        else{
            launchingUI.gameObject.SetActive(false);
        }

        float currentTurnNumber = turnManager.GetTurnNumber();
        if(TurnNumberText != null)
        {
            TurnNumberText.text = "It took you " + currentTurnNumber + " turns!";
        }

        float forcePercentageNumber = BallMovement.GetForcePercentage();
        if(forcePercentageText != null){
            forcePercentageText.text = forcePercentageNumber.ToString() + '%';
        }
    }
}
