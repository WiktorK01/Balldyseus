using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManagerCaller : MonoBehaviour
{
    public void StartPlayerTurn(){
        TurnManager.Instance.StartPlayerTurn();
    }
    public void StartWinState(){
        TurnManager.Instance.ChangeGameState(GameState.Win);
    }
    public void StartLossState(){
        TurnManager.Instance.ChangeGameState(GameState.Loss);
    }
}
