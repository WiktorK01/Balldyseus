using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    UIManager uiManager;

    bool isGamePaused = false;
    bool isMoving = false;

    void Start(){
        uiManager = FindObjectOfType<UIManager>();
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && !isMoving){
            Debug.Log("PAUSING");
            if (!isGamePaused) 
                PauseGame();
            else 
                ResumeGame();
        }
    }

    void PauseGame(){
        uiManager.ShowUIElement("PauseMenuUI");
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        uiManager.HideUIElement("PauseMenuUI");
        isGamePaused = false;
        Time.timeScale = 1f;
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
    }

    private void OnMovementStateChange(BallMovement.MovementState newState)
    {
        if(newState == BallMovement.MovementState.IsMoving) isMoving = true;
        else isMoving = false;
    }
}
