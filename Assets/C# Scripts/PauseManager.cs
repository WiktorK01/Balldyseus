using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    bool isGamePaused = false;
    bool isMoving = false;
    bool canPause = true;

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && !isMoving && canPause){
            Debug.Log("PAUSING");
            if (!isGamePaused) 
                PauseGame();
            else 
                ResumeGame();
        }
    }

    void PauseGame(){
        PausePublisher.NotifyPauseChange(true);
        isGamePaused = true;
    }

    public void ResumeGame(){
        PausePublisher.NotifyPauseChange(false);
        isGamePaused = false;
    }



//****************OBSERVERS***************

    void OnEnable(){
        GameStatePublisher.GameStateChange += OnGameStateChange;
        MovementStatePublisher.MovementStateChange += OnMovementStateChange;
    }

    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
        MovementStatePublisher.MovementStateChange -= OnMovementStateChange;
    }

    private void OnGameStateChange(TurnManager.GameState newState) {
        if (isGamePaused && newState == TurnManager.GameState.PlayerTurn) {
            ResumeGame();
        }
        if (newState == TurnManager.GameState.Loss || newState == TurnManager.GameState.Win) {
            canPause = false;
        }
        else {
            canPause = true;
        }
    }

    private void OnMovementStateChange(BallMovement.MovementState newState)
    {
        if(newState == BallMovement.MovementState.IsMoving) 
            isMoving = true;
        else 
            isMoving = false;
    }
}
