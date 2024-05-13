using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseManager : MonoBehaviour
{
    BallMovement ballMovement;
    bool isGamePaused = false;

    void Start(){
        ballMovement = FindObjectOfType<BallMovement>();
        if (ballMovement == null) Debug.LogError("BallMovement Not Found For PauseManager");
        GameStateEventPublisher.GameStateChanged += HandleGameStateChange;
    }

    void Update(){
        if (Input.GetKeyDown(KeyCode.Escape) && !ballMovement.IsMoving()){
            Debug.Log("PAUSING");
            if (!isGamePaused) 
                PauseGame();
            else 
                ResumeGame();
        }
    }

    void PauseGame(){
        UIManager.Instance.ShowUIElement("PauseMenuUI");
        isGamePaused = true;
        Time.timeScale = 0f;
    }

    public void ResumeGame()
    {
        UIManager.Instance.HideUIElement("PauseMenuUI");
        isGamePaused = false;
        Time.timeScale = 1f;
    }

    private void HandleGameStateChange(TurnManager.GameState newState) {
        if (isGamePaused && newState == TurnManager.GameState.PlayerTurn) {
            ResumeGame();
        }
    }
}
