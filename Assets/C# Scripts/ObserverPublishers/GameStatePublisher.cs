using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStatePublisher
{
    public delegate void OnGameStateChange(TurnManager.GameState newGameState);
    public static event OnGameStateChange GameStateChange;

    public static void NotifyGameStateChange(TurnManager.GameState newGameState)
    {
        GameStateChange?.Invoke(newGameState);
    }

    /* 
        void OnEnable(){
            GameStatePublisher.GameStateChange += OnGameStateChange;
        }
        void OnDisable(){
            GameStatePublisher.GameStateChange -= OnGameStateChange;
        }

        private void OnGameStateChange(string newGameState){
            switch (stateString){
                case "PlayerTurn":
                    break;

                case "EnemyTurn":
                    break;

                case "Win":
                    break;

                case "Loss":
                    break;
        }
    */
}