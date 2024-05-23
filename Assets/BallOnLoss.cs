using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallOnLoss : MonoBehaviour
{
    void OnEnable(){
        GameStatePublisher.GameStateChange += OnGameStateChange;
    }

    void OnDisable(){
        GameStatePublisher.GameStateChange -= OnGameStateChange;
    }

    void OnGameStateChange(TurnManager.GameState newState){
        if(newState == TurnManager.GameState.Loss){
            gameObject.SetActive(false);
        }
    }
}
