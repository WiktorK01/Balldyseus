using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameStateEventPublisher
{
    public delegate void OnGameStateChange(TurnManager.GameState newState);
    public static event OnGameStateChange GameStateChanged;

    public static void NotifyGameStateChange(TurnManager.GameState newState)
    {
        GameStateChanged?.Invoke(newState);
    }
}